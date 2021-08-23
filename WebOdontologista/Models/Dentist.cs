﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebOdontologista.Models
{
    public class Dentist
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Odontologista")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Telefone")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:(##)#####-####}")]
        public string TelephoneNumber { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public Dentist() { }
        public Dentist(string name)
        {
            Name = name;
        }
    }
}
