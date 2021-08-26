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

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} requirido.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Paciente")]
        public string Patient { get; set; }

        [Display(Name = "Telefone")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(15, MinimumLength = 14, ErrorMessage = "O campo {0} deve ser {2} ou {1} caracteres.")]
        public string TelephoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} requirido.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Tipo de consulta")]
        public string AppointmentType { get; set; } // Provavelmente mudará, provavelmente será um enum

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} requirido.")]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "{0} requirido.")]
        [Display(Name = "Horário")]
        public TimeSpan Time { get; set; }

        [Display(Name = "Odontologista")]
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }
        public Appointment() { }
        public Appointment(int durationInMinutes, string appointmentType, DateTime date, TimeSpan time, Dentist dentist)
        {
            DurationInMinutes = durationInMinutes;
            AppointmentType = appointmentType;
            Date = date;
            Time = time;
            Dentist = dentist;
        }
        public DateTime DateAndTime()
        {
            return Date + Time;
        }
    }
}
