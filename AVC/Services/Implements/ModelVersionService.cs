using AutoMapper;
using AVC.Dtos.ModelDtos;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using AVC.Models;
using AVC.Constant;
using System.Transactions;
using AVC.Constants;
using System.Collections.Generic;
using AVC.Dtos.QueryFilter;
using AVC.Dtos.PagingDtos;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Implements
{
    public class ModelVersionService : BaseService, IModelVersionService
    {
        private readonly TrainModelConfig _trainModelConfig;


        public ModelVersionService(IUnitOfWork unit, IMapper mapper, IConfiguration config, TrainModelConfig trainModelConfig,
                                   IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
                                        : base(unit, mapper, config, urlHelper, httpContextAccessor)
        {
            _trainModelConfig = trainModelConfig;
        }

        public PagingResponseDto<ModelReadDto> GetModelList(ModelQueryFilter filter)
        {
            var page = filter.Page;
            var limit = filter.Limit;
            var searchValue = filter.SearchValue;

            var dto = _unit.ModelVersionRepository.GetAllWithOrderedDecs(page, limit, x => x.IsAvailable == true, x => x.CreatedAt, x => x.ModelStatus);

            if (filter.SuccessList)
            {
                dto.Result = dto.Result.Where(x => x.ModelStatusId == ModelState.SuccessedId);
            }
            else if (filter.QueuedList)
            {
                dto.Result = dto.Result.Where(x => x.ModelStatusId == ModelState.QueuedId);
            }
            else if (filter.FailedList)
            {
                dto.Result = dto.Result.Where(x => x.ModelStatusId == ModelState.FailedId);
            }
            else if (filter.TrainningList)
            {
                dto.Result = dto.Result.Where(x => x.ModelStatusId == ModelState.TrainningId);
            }

            var modelList = _mapper.Map<IEnumerable<ModelReadDto>>(dto.Result);

            var response = new PagingResponseDto<ModelReadDto>(modelList, page, limit);

            if (limit > 0)
            {
                if ((double)dto.Count / limit > page)
                {
                    response.NextPage = _urlHelper.Link(null, new { page = page + 1, limit, searchValue, filter.SuccessList, filter.QueuedList, filter.TrainningList, filter.FailedList });
                }

                if (page > 1)
                    response.PreviousPage = _urlHelper.Link(null, new { page = page - 1, limit, searchValue, filter.SuccessList, filter.QueuedList, filter.TrainningList, filter.FailedList });
            }

            return response;
        }

        public ModelReadDto GetModelDetail(int modelId)
        {
            var modelFromRepo = _unit.ModelVersionRepository.Get(x => x.Id == modelId, x => x.ModelStatus);

            if (modelFromRepo == null)
            {
                throw new NotFoundException("Model not found");
            }

            return _mapper.Map<ModelReadDto>(modelFromRepo);
        }

        public void UpdateModel(int modelId, ModelUpdateDto modelUpdateDto)
        {
            var modelFromRepo = _unit.ModelVersionRepository.Get(x => x.Id == modelId);

            if (modelFromRepo == null)
            {
                throw new NotFoundException("Model not found");
            }
            _mapper.Map(modelUpdateDto, modelFromRepo);

            _unit.SaveChanges();
        }

        public void ModelTrainSuccess(int modelId)
        {
            var model = _unit.ModelVersionRepository.Get(x => x.Id == modelId);

            if (model == null)
            {
                throw new NotFoundException("Model not found");
            }

            model.ModelStatusId = ModelState.SuccessedId;

            //TODO: create notification, send signalR

            _unit.SaveChanges();
        }

        public void ModelTraining(int modelId)
        {
            var model = _unit.ModelVersionRepository.Get(x => x.Id == modelId);

            if (model == null)
            {
                throw new NotFoundException("Model not found");
            }

            model.ModelStatusId = ModelState.TrainningId;

            //TODO: create notification, send signalR

            _unit.SaveChanges();
        }

        public void ModelTrainFailed(int modelId, string failedMessage)
        {
            var model = _unit.ModelVersionRepository.Get(x => x.Id == modelId);

            if (model == null)
            {
                throw new NotFoundException("Model not found");
            }

            model.ModelStatusId = ModelState.FailedId;

            //TODO: create notification, send signalR

            _unit.SaveChanges();
        }

        public ModelReadDto CreateNewModel(ModelCreateDto createDto)
        {
            ModelVersion model = _mapper.Map<ModelVersion>(createDto);

            model.ModelStatusId = ModelState.QueuedId;

            _unit.ModelVersionRepository.Add(model);
            _unit.SaveChanges();

            Task.WaitAll(PushRepo(createDto.zipFile, model.Id.ToString()));

            model = _unit.ModelVersionRepository.Get(x => x.Id == model.Id, x => x.ModelStatus);

            return _mapper.Map<ModelReadDto>(model);
        }

        private async Task PushRepo(IFormFile zip, string branchName)
        {
            if (zip.Length > 0)
            {
                var defaultBracnch = _trainModelConfig.DefaultBranch;

                string filePath = Path.Combine(_trainModelConfig.GitFolderPath, "traindata.zip");

                CloneOptions cloneOption = new CloneOptions
                {
                    CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials()
                            {
                                Username = _trainModelConfig.Username,
                                Password = _trainModelConfig.Password
                            }),

                    BranchName = defaultBracnch
                };

                var dir = new DirectoryInfo(_trainModelConfig.GitFolderPath);

                if (!dir.Exists)
                {
                    Repository.Clone(_trainModelConfig.GitURL, _trainModelConfig.GitFolderPath, cloneOption);
                }

                using (var repo = new Repository(_trainModelConfig.GitFolderPath))
                {
                    try
                    {
                        if (dir.Exists)
                        {
                            repo.RemoveUntrackedFiles();

                            LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions
                            {
                                FetchOptions = new FetchOptions
                                {
                                    CredentialsProvider = new CredentialsHandler(
                                (url, usernameFromUrl, types) =>
                                    new UsernamePasswordCredentials()
                                    {
                                        Username = _trainModelConfig.Username,
                                        Password = _trainModelConfig.Password
                                    })
                                }
                            };


                            // User information to create a merge commit
                            var signature = new LibGit2Sharp.Signature(
                                new Identity("Kay", _trainModelConfig.Username), DateTimeOffset.Now);

                            Commands.Pull(repo, signature, options);
                        }

                        Remote remote = repo.Network.Remotes["origin"];

                        var localBranch = repo.Branches[branchName];

                        if (localBranch == null)
                        {
                            localBranch = repo.CreateBranch(branchName);
                        }

                        Commands.Checkout(repo, localBranch);

                        repo.Branches.Update(localBranch, b => b.Remote = remote.Name, b => b.UpstreamBranch = localBranch.CanonicalName);

                        PushOptions pushOptions = new PushOptions
                        {
                            CredentialsProvider = new CredentialsHandler(
                             (url, usernameFromUrl, types) =>
                                    new UsernamePasswordCredentials
                                    {
                                        Username = _trainModelConfig.Username,
                                        Password = _trainModelConfig.Password
                                    })
                        };

                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            fileStream.Position = 0;
                            await zip.CopyToAsync(fileStream);
                            //fileStream.EndWrite();
                            await fileStream.FlushAsync();
                        }

                        Commands.Stage(repo, "*");

                        // Create the committer's signature and commit
                        Signature author = new Signature("Kay", _trainModelConfig.Username, DateTimeOffset.Now);
                        Signature committer = author;

                        // Commit to the repository
                        Commit commit = repo.Commit("Create new model", author, committer);

                        repo.Network.Push(localBranch, pushOptions);

                        Commands.Checkout(repo, defaultBracnch);

                        if (localBranch != null)
                        {
                            repo.Branches.Remove(localBranch);
                        }
                    }
                    catch(Exception e)
                    {
                        Commands.Checkout(repo, defaultBracnch);

                        var localBranch = repo.Branches[branchName];

                        if (localBranch != null)
                        {
                            repo.Branches.Remove(localBranch);
                        }

                        throw e;
                    }
                }
            }
        }
    }
}
