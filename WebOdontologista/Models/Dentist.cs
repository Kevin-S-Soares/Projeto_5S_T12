using System.Collections.Generic;
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
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1}.")]
        [Display(Name = "Odontologista")]
        public string Name { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "{0} requerido.")]
        [Display(Name = "Telefone")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:(##) #####-####}")]
        public long TelephoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} requerido.")]
        [EmailAddress(ErrorMessage = "Entre um email válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public Dentist() { }
        public Dentist(string name)
        {
            Name = name;
        }
    }
}
