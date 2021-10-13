using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> FindAllAsync(Expression<Func<Appointment, bool>> expression);
        Task<Appointment> FindByIdAsync(int id);
    }
}
