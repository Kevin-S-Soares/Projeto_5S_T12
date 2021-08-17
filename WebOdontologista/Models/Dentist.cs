using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebOdontologista.Models
{
    public class Dentist
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new LinkedList<Appointment>();
        public Dentist() { }
        public Dentist(string name)
        {
            Name = name;
        }
    }
}
