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
using Microsoft.AspNetCore.Identity;
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

            if (doctors == null || !doctors.Any())
            {
                var error = BaseRespone<List<DoctorDto>>.ErrorResponse("Không tìm thấy bác sĩ nào.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var doctorDtos = doctors.Select(x => x.ToDoctorDto()).ToList();
            var response = BaseRespone<List<DoctorDto>>.SuccessResponse(doctorDtos, "Lấy danh sách thành công");
            return Ok(response);
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = BaseRespone<DoctorDto>.ErrorResponse(firstError, HttpStatusCode.BadRequest);
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
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = BaseRespone<DoctorDto>.ErrorResponse(firstError, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
            if (await _context.Accounts.AnyAsync(a => a.AccName == doctor.AccName))
            {
                return BadRequest(new BaseRespone<DoctorDto>(HttpStatusCode.BadRequest, "Tên tài khoản đã tồn tại"));
            }

            var account = new Account
            {
                AccName = doctor.AccName,
                RoleId = 4 
            };
            var passwordHasher = new PasswordHasher<Account>();
            account.Password = passwordHasher.HashPassword(account, doctor.Password);

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();


            var doctorModel = doctor.ToDoctorFromCreateDTO();
            doctorModel.AccId = account.AccId;

            await _doctorRepo.CreateDoctorAsync(doctorModel);


            var loadedDoctor = await _context.Doctors.Include(d => d.Acc).ThenInclude(acc => acc.Role).FirstOrDefaultAsync(d => d.DocId == doctorModel.DocId);

            if (loadedDoctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ sau khi tạo", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var response = BaseRespone<DoctorDto>.SuccessResponse(loadedDoctor.ToDoctorDto(), "Tạo bác sĩ thành công");
            return CreatedAtAction(nameof(GetDoctorById), new { id = loadedDoctor.DocId }, response);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] int id, [FromBody] UpdateDoctorRequestDto doctor)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = BaseRespone<DoctorDto>.ErrorResponse(firstError, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
            var existingDoctor = await _doctorRepo.GetDoctorByIdAsync(id);
            if (existingDoctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ nào để cập nhật.", HttpStatusCode.NotFound);
                return NotFound(error);
            }
            var updatedDoctor = await _doctorRepo.UpdateDoctorAsync(id, doctor);
            var response = BaseRespone<DoctorDto>.SuccessResponse(updatedDoctor.ToDoctorDto(), "Cập nhật thông tin bác sĩ thành công");
            return Ok(response);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = BaseRespone<DoctorDto>.ErrorResponse(firstError, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
            var checkExist = await _doctorRepo.GetDoctorByIdAsync(id);
            if (checkExist == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ nào để xóa.", HttpStatusCode.NotFound);
                return NotFound(error);
            }
            var doctor = await _doctorRepo.DeleteDoctorAsync(id);
            var response = BaseRespone<string>.SuccessResponse(doctor.ToString(), "Xóa bác sĩ thành công");
            return Ok(response);
        }
        [Authorize(Roles = "Doctor")]
        [HttpPost("RegisterSchedule")]
        public async Task<IActionResult> RegisterSchedule([FromBody] CreateDoctorScheduleDto doctorScheduleRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = BaseRespone<DoctorDto>.ErrorResponse(firstError, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.DocId == doctorScheduleRequest.DocId);
            if (doctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Bác sĩ không tồn tại", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var doctorSchedule = doctorScheduleRequest.ToDoctorScheduleFromCreateDTO();
            doctorSchedule.Doc = doctor;
            doctorSchedule.DsId = 0;
            var checkExist = await _doctorRepo.GetDoctorScheduleByIdAsync(doctorSchedule);
            if (checkExist != null)
            {
                return Conflict(BaseRespone<DoctorScheduleDto>.ErrorResponse("Bác sĩ đã có lịch hẹn", checkExist.ToDoctorScheduleDto(), HttpStatusCode.Conflict));
            }
            else
            {
                var doctorCreated = await _doctorRepo.RegisterDoctorSchedule(doctorSchedule);
                return Ok(BaseRespone<DoctorScheduleDto>.SuccessResponse(doctorCreated.ToDoctorScheduleDto(), "Đặt lịch thành công", HttpStatusCode.OK));
            }
        }
    }
}
