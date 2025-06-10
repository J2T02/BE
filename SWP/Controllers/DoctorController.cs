using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Doctor;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using SWP.Repository;

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
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorRepo.GetAllDoctorsAsync();
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
            return Ok(doctor);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var doctor = await _doctorRepo.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor.ToDoctorDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDocotorRequestDto doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (doctor == null)
            {
                return BadRequest();
            }
            var doctorModel = doctor.ToDoctorFromCreateDTO();
            await _doctorRepo.CreateDoctorAsync(doctorModel);
            return CreatedAtAction(nameof(CreateDoctor), new { id = doctorModel.DocId }, doctorModel.ToDoctorDto());
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] int id,[FromBody] UpdateDoctorRequestDto doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedDoctor = await _doctorRepo.UpdateDoctorAsync(id, doctor);
            if(updatedDoctor == null)
            {
                return NotFound();
            }
            return Ok(updatedDoctor.ToDoctorDto());
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var doctor = await _doctorRepo.DeleteDoctorAsync(id);
            if(doctor == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
