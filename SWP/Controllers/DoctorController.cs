using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Doctor;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using SWP.Repository;
using SWP.Data;
using System.Net;
using SWP.Dtos.Account;
namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly HIEM_MUONContext _context;
        private readonly IDoctor _doctorRepo;
        public DoctorController(HIEM_MUONContext context, IDoctor doctorRepo)
        {
            _context = context;
            _doctorRepo = doctorRepo;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            
            var doctors = await _doctorRepo.GetAllDoctorsAsync();
            if(doctors == null || !doctors.Any())
            {
                var error = BaseRespone<List<DoctorDto>>.ErrorResponse( "Không tìm thấy bác sĩ nào.", HttpStatusCode.NotFound);
                return NotFound(error);
            }
            var doctor = doctors.Select(x => new DoctorDto
            {
                DocId = x.DocId,
                DocName = x.DocName,
                Gender = x.Gender,
                Yob = x.Yob,
                Mail = x.Mail,
                Phone = x.Phone,
                Experience = x.Experience,
                Certification = x.Certification,
            }).ToList();
            var response = BaseRespone<List<DoctorDto>>.SuccessResponse(doctor, "Lấy danh sách thành công");
            return Ok(response);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = new BaseRespone<DoctorDto>(HttpStatusCode.BadRequest,firstError);
                return BadRequest(error);
            }
            var doctor = await _doctorRepo.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ nào.", HttpStatusCode.NotFound);
                return NotFound(error);
            }
            var response = BaseRespone<DoctorDto>.SuccessResponse(doctor.ToDoctorDto(), "Lấy thông tin bác sĩ thành công");
            return Ok(response);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDocotorRequestDto doctor)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = new BaseRespone<DoctorDto>(HttpStatusCode.BadRequest, firstError);
                return BadRequest(error);
            }
            
            
            var doctorModel = doctor.ToDoctorFromCreateDTO();
            await _doctorRepo.CreateDoctorAsync(doctorModel);
            // Trả về thông tin bác sĩ mới được tạo
            var response = BaseRespone<DoctorDto>.SuccessResponse(doctorModel.ToDoctorDto(), "Tạo bác sĩ thành công");
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctorModel.DocId }, response);
            //return CreatedAtAction(nameof(CreateDoctor), new { id = doctorModel.DocId }, doctorModel.ToDoctorDto());
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] int id,[FromBody] UpdateDoctorRequestDto doctor)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = new BaseRespone<DoctorDto>(HttpStatusCode.BadRequest, firstError);
                return BadRequest(error);
            }
            var existingDoctor = await _doctorRepo.GetDoctorByIdAsync(id);
            if(existingDoctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ nào để cập nhật.", HttpStatusCode.NotFound);
                return NotFound(error);
            }
            var updatedDoctor = await _doctorRepo.UpdateDoctorAsync(id, doctor);
            var response = BaseRespone<DoctorDto>.SuccessResponse(updatedDoctor.ToDoctorDto(), "Cập nhật thông tin bác sĩ thành công");
            return Ok(response);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = new BaseRespone<DoctorDto>(HttpStatusCode.BadRequest, firstError);
                return BadRequest(error);
            }
            var doctor = await _doctorRepo.DeleteDoctorAsync(id);
            if (doctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ nào để xóa.", HttpStatusCode.NotFound);
                return NotFound();
            }
            var response = BaseRespone<DoctorDto>.SuccessResponse(doctor.ToDoctorDto(), "Xóa bác sĩ thành công");
            return Ok(response);
        }
    }
}
