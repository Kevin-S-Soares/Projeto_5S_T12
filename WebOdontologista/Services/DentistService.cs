using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Services.Exceptions;

namespace WebOdontologista.Services
{
    public class DentistService : IDentistService
    {
        private readonly ApplicationDbContext _context;
        public DentistService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Dentist> FindAll()
        {
            return _context.Dentist.OrderBy(obj => obj.Name).ToList();
        }
        public async Task<List<Dentist>> FindAllAsync()
        {
            return await _context.Dentist.OrderBy(obj => obj.Name).ToListAsync();
        }
        public async Task InsertAsync(Dentist dentist)
        {
            _context.Add(dentist);
           await _context.SaveChangesAsync();
        }
        public async Task<Dentist> FindByIdAsync(int id)
        {
            return await _context.Dentist.FirstOrDefaultAsync(obj => obj.Id == id);
        }
        public async Task RemoveAsync(int id)
        {
            Dentist dentist = await _context.Dentist.FindAsync(id);
            _context.Dentist.Remove(dentist);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Dentist dentist)
        {
            bool hasAny = await _context.Dentist.AnyAsync(obj => obj.Id == dentist.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado!");
            }
            try
            {
                _context.Dentist.Update(dentist);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
