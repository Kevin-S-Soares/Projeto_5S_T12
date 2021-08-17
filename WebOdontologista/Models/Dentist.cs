namespace WebOdontologista.Models
{
    public class Dentist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dentist() { }
        public Dentist(string name)
        {
            Name = name;
        }
    }
}
