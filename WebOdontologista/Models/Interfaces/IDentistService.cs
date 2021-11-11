using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface IDentistService
    {
        Task<Dentist> FindByIdAsync(int id);
        Task<List<Dentist>> FindAllAsync();
        Task InsertAsync(Dentist dentist);
        Task RemoveByIdAsync(int id);
        Task UpdateAsync(Dentist dentist);
    }
}
