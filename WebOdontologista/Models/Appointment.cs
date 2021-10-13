using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebOdontologista.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Duração")]
        public int DurationInMinutes { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} requirido.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Paciente")]
        public string Patient { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "{0} requirido.")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(15, MinimumLength = 14, ErrorMessage = "O campo {0} deve ser {2} ou {1} caracteres.")]
        public string TelephoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} requirido.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Tipo de consulta")]
        public string AppointmentType { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} requirido.")]
        [Display(Name = "Data")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "{0} requirido.")]
        [Display(Name = "Horário")]
        [Column(TypeName = "time(0)")]
        public TimeSpan Time { get; set; }

        [Display(Name = "Odontologista")]
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }
        public Appointment() { }
        public DateTime DateAndTime()
        {
            return Date + Time;
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static Appointment Deserialize(string value)
        {

            return JsonConvert.DeserializeObject<Appointment>(value);
        }
        public override string ToString()
        {
            return "Id: " +
                Id +
                "\nPatient: " +
                Patient +
                "\nTelephoneNumber: " +
                TelephoneNumber +
                "\nDurationInMinutes: " +
                DurationInMinutes +
                "\nAppointmentType: " +
                AppointmentType +
                "\nDate: " +
                Date +
                "\nTime: " +
                Time +
                "\nDentistId: " +
                DentistId;
        }
        public override bool Equals(object obj)
        {
            return obj is Appointment appointment &&
                   Id == appointment.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
