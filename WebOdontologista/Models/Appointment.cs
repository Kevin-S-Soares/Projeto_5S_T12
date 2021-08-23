using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebOdontologista.Models
{
    public class Appointment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Duração")]
        public int DurationInMinutes { get; set; }
        [Display(Name = "Paciente")]
        [Required]
        public string Patient { get; set; }
        [Display(Name = "Telefone")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:(##) #####-####}")]
        public long TelephoneNumber { get; set; }
        [Display(Name = "Tipo de consulta")]
        [Required]
        public string AppointmentType { get; set; } // Provavelmente mudará, provavelmente será um enum
        [Display(Name = "Data")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Display(Name = "Odontologista")]
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }
        public Appointment() { }
        public Appointment(int durationInMinutes, string appointmentType, DateTime date, Dentist dentist)
        {
            DurationInMinutes = durationInMinutes;
            AppointmentType = appointmentType;
            Date = date;
            Dentist = dentist;
        }
    }
}
