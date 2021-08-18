using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebOdontologista.Models;

namespace WebOdontologista.Data
{
    public class WebOdontologistaContext : DbContext
    {
        public WebOdontologistaContext (DbContextOptions<WebOdontologistaContext> options) : base(options) { }
        public DbSet<Dentist> Dentist { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
    }
}
