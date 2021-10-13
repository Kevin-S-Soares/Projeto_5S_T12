using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface IDentistService
    {
        Task<Dentist> FindByIdAsync(int id);
    }
}
