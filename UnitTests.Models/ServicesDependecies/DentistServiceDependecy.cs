using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;

namespace UnitTests.Models.ServicesDependecies
{
    public class DentistServiceDependecy : IDentistService
    {
        private readonly List<Dentist> _list = new List<Dentist>();
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
        public Task<List<Dentist>> FindAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task InsertAsync(Dentist dentist)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Dentist dentist)
        {
            throw new System.NotImplementedException();
        }

        private void GenerateDentists()
        {
            _list.AddRange(new Dentist[]
            {
                // TODO preencher com dentistas
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
