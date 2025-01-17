﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebOdontologista.Models
{
    public class Dentist
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} requerido.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.")]
        [Display(Name = "Odontologista")]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "{0} requerido.")]
        [Display(Name = "Telefone")]
        [StringLength(15, MinimumLength = 14, ErrorMessage = "O campo {0} deve ser {2} ou {1} caracteres.")]
        public string TelephoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} requerido.")]
        [EmailAddress(ErrorMessage = "Entre um email válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public Dentist() { }

        public override bool Equals(object obj)
        {
            return obj is Dentist dentist &&
                   Id == dentist.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
        public int AppointmentsPerDay()
        {
            return 36;
        }
    }
}
