using AVC.Dtos.ModelDtos;
using AVC.Dtos.PagingDtos;
using AVC.Dtos.QueryFilter;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IModelVersionService _modelService;

        public ModelController(IModelVersionService modelService)
        {
            _modelService = modelService;
        }


        [Authorize]
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public ActionResult<ModelReadDto> Post([FromForm] ModelCreateDto createDto)
        {
            ModelReadDto model = _modelService.CreateNewModel(createDto);

            return CreatedAtAction("GetModelDetail", new { id = model.Id }, model);

        }

        [HttpGet]
        public ActionResult<PagingResponseDto<ModelReadDto>> GetModelList([FromQuery] ModelQueryFilter filter)
        {
            var response = _modelService.GetModelList(filter);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ModelReadDto> GetModelDetail(int id)
        {
            var respone = _modelService.GetModelDetail(id);

            return Ok(respone);

        }

        [HttpGet("applying")]
        public ActionResult<ModelReadDto> GetApplyingModel()
        {
            var respone = _modelService.GetApplyingModel();

            return Ok(respone);
        }

        [HttpGet("applyingId")]
        public ActionResult<int> GetApplyingModelId()
        {
            var respone = _modelService.GetApplyingModel().Id;

            return Ok(respone);
        }


        [HttpPut("{id}/applying")]
        public ActionResult ApplyModel(int id)
        {
            _modelService.ApplyModel(id);

            return Ok();
        }

        [HttpPost("{id}/succession")]
        public ActionResult SetModelSuccess(int id, IFormFile modelFile)
        {
            _modelService.ModelTrainSuccess(id, modelFile);

            return Ok();
        }

        [HttpPut("{id}/failure")]
        public ActionResult SetModelFail(int id, string failureMessage)
        {
            _modelService.ModelTrainFailed(id, failureMessage);

            return Ok();
        }

        [HttpPut("{id}/trainning")]
        public ActionResult SetModelTrainning(int id)
        {
            _modelService.ModelTraining(id);

            return Ok();
        }


    }
}
