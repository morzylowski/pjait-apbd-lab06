namespace App.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Prescription
{
    [Key]
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }

    [ForeignKey("IdPatient")]
    public Patient Patient { get; set; }
    [ForeignKey("IdDoctor")]
    public Doctor Doctor { get; set; }
    public ICollection<Prescription_Medicament> Prescription_Medicaments { get; set; }
}