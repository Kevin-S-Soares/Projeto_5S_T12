using System;

namespace WebOdontologista.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DurationInMinutes { get; set; }
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
