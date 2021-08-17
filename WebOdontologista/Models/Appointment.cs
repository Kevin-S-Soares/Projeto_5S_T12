using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebOdontologista.Models
{
    public class Appointment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity]
        public int Id { get; set; }
        [Range(15,60)]
        public int DurationInMinutes { get; set; }
        public string Patient { get; set; }
        public string AppointmentType { get; set; }
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
