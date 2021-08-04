using AVC.Dtos.ModelDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public interface IModelVersionService
    {
        void ApplyModel(int modelId);
        ModelReadDto CreateNewModel(ModelCreateDto createDto);
        ModelReadDto GetApplyingModel();
        ModelReadDto GetModelDetail(int modelId);
        PagingResponseDto<ModelReadDto> GetModelList(ModelQueryFilter filter);
        void ModelTrainFailed(int modelId, string failedMessage);
        void ModelTraining(int modelId);
        void ModelTrainSuccess(int modelId, IFormFile modelFile, IFormFile statisticFile);
        void UpdateModel(int modelId, ModelUpdateDto modelUpdateDto);
    }
}
