using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Services;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Repository;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServices _serviceRepo;

        public ServicesController(IServices serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }
        [HttpGet("GetAllService")]
        public async Task<IActionResult> GetAllServices()
        {
            var listService = await _serviceRepo.GetAllServices();
            var listServiceDto = listService.Select(x=> x.ToServiceDto()).ToList();
            if(listServiceDto == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Lấy danh sách dịch vụ thất bại", ""));
            }
            return Ok(BaseRespone<List<ServiceDto>>.SuccessResponse(listServiceDto, "Lấy danh sách dịch vụ thành công"));
        }
        [HttpGet("GetServiceDetail/{id}")]
        public async Task<IActionResult> GetServiceById([FromRoute] int id)
        {
            var result = await _serviceRepo.GetServiceById(id);
            if (result == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Không tìm thấy thông tin dịch vụ", $"ServiceId: {id}"));
            }
            return Ok(BaseRespone<ServiceDto>.SuccessResponse(result.ToServiceDto(), "Lấy thông tin dịch vụ thành công"));
        }
    }
}
