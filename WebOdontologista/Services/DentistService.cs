using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;

namespace WebOdontologista.Services
{
    public class DentistService
    {
        private readonly WebOdontologistaContext _context;
        public DentistService(WebOdontologistaContext context)
        {
            _context = context;
        }
        public List<Dentist> FindAll()
        {
            return _context.Dentist.OrderBy(x => x.Name).ToList();
        }
        public Dictionary<int, Dentist> PrimaryKey()
        {
            return _context.Dentist.ToDictionary<Dentist, int>(x => x.Id);
        }
    }
}
