using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> FindAllAsync(Expression<Func<Appointment, bool>> expression);
        Task<Appointment> FindByIdAsync(int id);
        Task InsertAsync(Appointment appointment);
        Task RemoveByIdAsync(int id);
        Task UpdateAsync(Appointment appointment);
        Task<List<Appointment>> FindByDateAsync(DateTime? minDate, DateTime? maxDate);
        Task<List<IGrouping<Dentist, Appointment>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate);
    }
}
