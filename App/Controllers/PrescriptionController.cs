using App.Data;
using App.DTOs;
using App.Models;

namespace App.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly ApplicationContext _context;

    public PrescriptionController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDto prescriptionDto)
    {
        if (prescriptionDto.Medicaments.Count > 10)
        {
            return BadRequest("Prescription cannot contain more than 10 medicaments.");
        }

        var patient = await _context.Patients.FindAsync(prescriptionDto.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = prescriptionDto.IdPatient,
                FirstName = prescriptionDto.PatientFirstName,
                LastName = prescriptionDto.PatientLastName,
                Birthdate = prescriptionDto.PatientBirthdate
            };
            _context.Patients.Add(patient);
        }

        foreach (var med in prescriptionDto.Medicaments)
        {
            var medicament = await _context.Medicaments.FindAsync(med.IdMedicament);
            if (medicament == null)
            {
                return BadRequest($"Medicament with ID {med.IdMedicament} does not exist.");
            }
        }

        var doctor = await _context.Doctors.FindAsync(prescriptionDto.IdDoctor);
        if (doctor == null)
        {
            doctor = new Doctor
            {
                IdDoctor = prescriptionDto.IdDoctor,
                FirstName = prescriptionDto.DoctorFirstName,
                LastName = prescriptionDto.DoctorLastName,
                Email = prescriptionDto.DoctorEmail
            };
            _context.Doctors.Add(doctor);
        }

        var prescription = new Prescription
        {
            Date = prescriptionDto.Date,
            DueDate = prescriptionDto.DueDate,
            IdPatient = patient.IdPatient,
            IdDoctor = doctor.IdDoctor,
            Prescription_Medicaments = prescriptionDto.Medicaments.Select(m => new Prescription_Medicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Details
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Prescription_Medicaments)
                    .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync(p => p.IdPatient == id);

        if (patient == null)
        {
            return NotFound();
        }

        var patientDto = new
        {
            patient.IdPatient,
            patient.FirstName,
            patient.LastName,
            patient.Birthdate,
            Prescriptions = patient.Prescriptions.Select(pr => new
            {
                pr.IdPrescription,
                pr.Date,
                pr.DueDate,
                Doctor = new
                {
                    pr.Doctor.IdDoctor,
                    pr.Doctor.FirstName,
                    pr.Doctor.LastName,
                    pr.Doctor.Email
                },
                Medicaments = pr.Prescription_Medicaments.Select(pm => new
                {
                    pm.Medicament.IdMedicament,
                    pm.Medicament.Name,
                    pm.Medicament.Description,
                    pm.Medicament.Type,
                    pm.Dose,
                    pm.Details
                }).ToList()
            }).OrderBy(pr => pr.DueDate).ToList()
        };

        return Ok(patientDto);
    }
}
