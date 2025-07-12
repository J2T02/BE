using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using SWP.Data;
using SWP.Dtos.Account;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using SWP.Repository;
using System.Globalization;
using System.Net;
using System.Security.Claims;
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

        [HttpGet("all")]
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
            if (await _context.Accounts.AnyAsync(a => a.Mail == doctor.Mail || a.Phone == doctor.Phone))
            {
                return BadRequest(new BaseRespone<DoctorDto>(HttpStatusCode.BadRequest, "Tên tài khoản đã tồn tại"));
            }

            var account = new Account
            {
                FullName = doctor.FullName,
                CreateAt = DateTime.Now,
                IsActive = true,
                Mail = doctor.Mail,
                Phone = doctor.Phone,
                Img = doctor.Img,
                RoleId = 5 // RoleId 5 là cho Doctor 
            };
            var passwordHasher = new PasswordHasher<Account>();
            account.Password = passwordHasher.HashPassword(account, doctor.Password);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var doctorModel = doctor.ToDoctorFromCreateDTO();
            doctorModel.AccId = account.AccId;
            await _doctorRepo.CreateDoctorAsync(doctorModel);
            await _context.SaveChangesAsync();

            //var loadedDoctor = await _context.Doctors.Include(d => d.Acc).ThenInclude(acc => acc.Role).FirstOrDefaultAsync(d => d.DocId == doctorModel.DocId);
            var loadedDoctor = await _doctorRepo.GetDoctorByIdAsync(doctorModel.DocId);
            if (loadedDoctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ sau khi tạo", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var response = BaseRespone<DoctorDto>.SuccessResponse(loadedDoctor.ToDoctorDto(), "Tạo bác sĩ thành công");
            return CreatedAtAction(nameof(GetDoctorById), new { id = loadedDoctor.DocId }, response);
        }

        [Authorize(Roles = "Admin,Manager,Doctor")]
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
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("RegisterSchedule/{docId}")]
        public async Task<IActionResult> RegisterSchedule([FromBody] CreateDoctorScheduleDto doctorScheduleRequest, [FromRoute] int docId)
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
            var checkSlot = await _doctorRepo.GetSlotById(doctorScheduleRequest.SlotId);
            if (doctorScheduleRequest.SlotId == 0 || checkSlot == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Khung giờ được chọn không hợp lệ", $"Slot id: {doctorScheduleRequest.SlotId}"));
            }
            var doctor = await _doctorRepo.GetDoctorByIdAsync(docId);
            if (doctor == null)
            {
                var error = BaseRespone<DoctorDto>.ErrorResponse("Bác sĩ không tồn tại", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var doctorSchedule = doctorScheduleRequest.ToDoctorScheduleFromCreateDTO(doctor.DocId);
            doctorSchedule.Doc = doctor;
            doctorSchedule.DsId = 0;
            var checkExist = await _doctorRepo.GetDoctorScheduleByIdAsync(doctorSchedule.DocId);
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

        [HttpGet("GetAllDoctorScheduleIsTrue/{id}")]
        public async Task<IActionResult> GetDoctorScheduleIsTrue([FromRoute] int id)
        {

            var doctorModel = await _doctorRepo.GetDoctorByIdAsync(id);
            if (doctorModel is null)
            {
                return BadRequest(BaseRespone<DoctorScheduleDto>.ErrorResponse("Không tìm thấy thông tin bác sĩ", doctorModel, HttpStatusCode.BadRequest));
            }
            var resultList = await _doctorRepo.GetDoctorScheduleIsTrue(doctorModel.DocId);
            var resultListDto = resultList.Select(x => x.ToDoctorScheduleDto()).ToList();
            if(resultListDto is null || !resultListDto.Any())
            {
                return Ok(BaseRespone<List<DoctorScheduleDto>>.SuccessResponse(resultListDto, "Danh sách lịch làm việc rỗng", HttpStatusCode.OK));
            }
            return Ok(BaseRespone<List<DoctorScheduleDto>>.SuccessResponse(resultListDto, "Lấy lịch làm việc thành công", HttpStatusCode.OK));
        }

        [HttpGet("GetAllDoctorScheduleByDoctorId/{id}")]
        public async Task<IActionResult> GetAllDoctorSchedule([FromRoute] int id)
        {

            var doctorModel = await _doctorRepo.GetDoctorByIdAsync(id);
            if (doctorModel is null)
            {
                return BadRequest(BaseRespone<DoctorScheduleDto>.ErrorResponse("Không tìm thấy thông tin bác sĩ", doctorModel, HttpStatusCode.BadRequest));
            }
            var resultList = await _doctorRepo.GetAllDoctorSchedule(doctorModel.DocId);
            var resultListDto = resultList.Select(x => x.ToDoctorScheduleDto()).ToList();
            if (resultListDto is null)
            {
                return Ok(BaseRespone<List<DoctorScheduleDto>>.SuccessResponse(resultListDto, "Danh sách lịch làm việc rỗng", HttpStatusCode.OK));
            }

            return Ok(BaseRespone<List<DoctorScheduleDto>>.SuccessResponse(resultListDto, "Lấy danh sách lịch làm việc thành công", HttpStatusCode.OK));
        }
        [HttpGet("GetAllDoctorScheduleByWorkDate/{workDate}")]
        public async Task<IActionResult> GetAllDoctorScheduleByWorkDate([FromRoute] string workDate)
        {
            bool isValid = DateOnly.TryParseExact(workDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly result);

            if (!isValid)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Ngày không đúng định dạng yyyy-MM-dd", workDate));
            }

            var resultList = await _doctorRepo.GetDoctorScheduleByDate(result);
            var resultListDto = resultList.Select(x => x.ToDoctorScheduleDto()).ToList();
            if (resultListDto is null || resultListDto.Count == 0)
            {
                return Ok(BaseRespone<List<DoctorScheduleDto>>.SuccessResponse(resultListDto, "Danh sách lịch làm việc rỗng", HttpStatusCode.OK));
            }

            return Ok(BaseRespone<List<DoctorScheduleDto>>.SuccessResponse(resultListDto, "Lấy danh sách lịch làm việc thành công", HttpStatusCode.OK));
        }

        [HttpGet("GetSlotByAccId/{id}")]
        public async Task<IActionResult> GetSlotByAccId([FromRoute] int id)
        {
            var doctor = await _doctorRepo.GetDoctorByAccountId(id);


            if (doctor == null)
            {
                return NotFound(BaseRespone<DoctorDto>.ErrorResponse("Không tìm thấy bác sĩ với tài khoản này", HttpStatusCode.NotFound));
            }

            return Ok(BaseRespone<Doctor>.SuccessResponse(doctor, "Lấy danh sách thành công", HttpStatusCode.OK));
        }
    }
}
