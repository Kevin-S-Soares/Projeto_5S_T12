using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebOdontologista.Models;

namespace WebOdontologista.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)  { }
        public DbSet<Dentist> Dentist { get; set; }
        public DbSet<Appointment> Appointment { get; set; }

    }
}
