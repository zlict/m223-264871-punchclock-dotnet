using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace M223PunchclockDotnet.Model {

    public class DatabaseContext : DbContext 
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){

        }

        
        public DbSet<Entry> Entries {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Entry>().ToTable("Entry");
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseNpgsql("Host=db;Database=postgres;Username=postgres;Password=postgres");

        
    }
}