using AVC.Dtos.ModelDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Services.Interfaces
{
    public interface IModelVersionService
    {
        ModelReadDto CreateNewModel(ModelCreateDto createDto);
        ModelReadDto GetModelDetail(int modelId);
        PagingResponseDto<ModelReadDto> GetModelList(ModelQueryFilter filter);
        void ModelTrainFailed(int modelId, string failedMessage);
        void ModelTraining(int modelId);
        void ModelTrainSuccess(int modelId);
        void UpdateModel(int modelId, ModelUpdateDto modelUpdateDto);
    }
}
