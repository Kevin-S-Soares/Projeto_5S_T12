using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Services.Exceptions;

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
            return _context.Dentist.OrderBy(obj => obj.Name).ToList();
        }
        public void Insert(Dentist dentist)
        {
            _context.Add(dentist);
            _context.SaveChanges();
        }
        public Dentist FindById(int id)
        {
            return _context.Dentist.FirstOrDefault(obj => obj.Id == id);
        }
        public void Remove(int id)
        {
            Dentist dentist = _context.Dentist.Find(id);
            _context.Dentist.Remove(dentist);
            _context.SaveChanges();
        }
        public void Update(Dentist dentist)
        {
            if (!_context.Dentist.Any(obj => obj.Id == dentist.Id))
            {
                throw new NotFoundException("Id não encontrado!");
            }
            try
            {
                _context.Dentist.Update(dentist);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
