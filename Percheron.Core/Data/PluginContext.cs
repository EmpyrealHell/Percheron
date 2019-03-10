using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Core.Data
{
    public class PluginContext : DbContext
    {
        // public DbSet<>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This is basically how I did it before, need to figure out how to use sqlite in this context
            // optionsBuilder.UseSqlite("Data Source=data.db");
        }
    }
}
