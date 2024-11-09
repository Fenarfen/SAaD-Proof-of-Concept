using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuthAPI.Models;

namespace AuthAPI.Data
{
    public class AuthAPIContext : DbContext
    {
        public AuthAPIContext (DbContextOptions<AuthAPIContext> options)
            : base(options)
        {
        }

        public DbSet<AuthAPI.Models.Account> Account { get; set; } = default!;
    }
}
