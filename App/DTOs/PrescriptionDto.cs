namespace App.DTOs;

using System;
using System.Collections.Generic;

public class PrescriptionDto
{
    public int IdPatient { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public DateTime PatientBirthdate { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdDoctor { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorEmail { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
}