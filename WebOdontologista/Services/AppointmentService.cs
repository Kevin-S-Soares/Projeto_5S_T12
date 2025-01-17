﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebOdontologista.Data;
using WebOdontologista.Models;
using WebOdontologista.Models.Interfaces;
using WebOdontologista.Services.Exceptions;

namespace WebOdontologista.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> FindAllAsync(Expression<Func<Appointment, bool>> expression)
        {
            List<Appointment> result = await
            _context.Appointment
            .Include(obj => obj.Dentist)
            .Where(expression)
            .OrderBy(obj => obj.DateAndTime())
            .ThenBy(obj => obj.Dentist.Name)
            .ToListAsync();
            return result;
        }
        public async Task InsertAsync(Appointment appointment)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task<Appointment> FindByIdAsync(int id)
        {
            Appointment appointment = await
                _context.Appointment.Include(obj => obj.Dentist)
                .FirstOrDefaultAsync(obj => obj.Id == id);
            return appointment;
        }
        public async Task RemoveByIdAsync(int id)
        {
            Appointment appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Appointment appointment)
        {

            bool hasAny = await _context.Appointment.AnyAsync(obj => obj.Id == appointment.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado!");
            }
            try
            {
                Appointment entry = await _context.Appointment.FirstAsync(obj => obj.Id == appointment.Id);
                _context.Entry(entry).CurrentValues.SetValues(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
        public async Task<List<Appointment>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.Appointment select obj;
            if (minDate.HasValue)
            {
                result = result.Where(obj => obj.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(obj => obj.Date <= maxDate.Value);
            }
            return await result.Include(obj => obj.Dentist).OrderBy(obj => obj.Date).ToListAsync();
        }
        public async Task<List<IGrouping<Dentist, Appointment>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.Appointment select obj;
            if (minDate.HasValue)
            {
                result = result.Where(obj => obj.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(obj => obj.Date <= maxDate.Value);
            }
            return await result.Include(obj => obj.Dentist).OrderBy(obj => obj.Date).GroupBy(obj => obj.Dentist).ToListAsync();
        }
    }
}
