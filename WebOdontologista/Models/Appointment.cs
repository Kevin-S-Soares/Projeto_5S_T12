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
        public string Patient { get; set; }
        [Display(Name = "Tipo de consulta")]
        public string AppointmentType { get; set; }
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
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
