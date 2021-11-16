using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.ServicesDependecies
{
    public class DentistServiceDependecy : IDentistService
    {
        private List<Dentist> _list = new List<Dentist>();
        public DentistServiceDependecy()
        {
            GenerateDentists();
        }
        public async Task<Dentist> FindByIdAsync(int id)
        {
            await Task.Delay(0);
            return _list.FirstOrDefault(obj => obj.Id == id);
        }
        public List<Dentist> FindAllDentists()
        {
            return _list;
        }
        public async Task<List<Dentist>> FindAllAsync()
        {
            await Task.Delay(0);
            return _list;
        }

        public async Task InsertAsync(Dentist dentist)
        {
            await Task.Delay(0);
            _list.Add(dentist);
        }

        public async Task RemoveByIdAsync(int id)
        {
            await Task.Delay(0);
            _list.RemoveAll(obj => obj.Id == id);
        }

        public async Task UpdateAsync(Dentist dentist)
        {
            await Task.Delay(0);
            Dentist editing = _list.FirstOrDefault(obj => obj.Id == dentist.Id);
            editing.Name = dentist.Name;
            editing.Email = dentist.Email;
            editing.TelephoneNumber = dentist.TelephoneNumber;
        }

        public void DeleteAll()
        {
            _list = new List<Dentist>();
        }

        private void GenerateDentists()
        {
            _list.AddRange(new Dentist[]
            {
                new Dentist()
                {
                    Id = 1,
                    Name = "Alex Blue",
                    TelephoneNumber = "(11) 11111-1111",
                    Email = "alex@gmail.com"
                },
                new Dentist()
                {
                    Id = 2,
                    Name = "Anna Green",
                    TelephoneNumber = "(22) 2222-2222",
                    Email = "anna@gmail.com"
                },
                new Dentist()
                {
                    Id = 3,
                    Name = "Bob Brown",
                    TelephoneNumber = "(33) 33333-3333",
                    Email = "bob@gmail.com"
                },
            });
        }
    }
}
