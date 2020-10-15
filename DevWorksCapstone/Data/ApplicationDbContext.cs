﻿using System;
using System.Collections.Generic;
using System.Text;
using DevWorksCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevWorksCapstone.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Ability> Abilities { get; set; }
        public DbSet<DeveloperAbilities> DeveloperAbilities { get; set; }
        public DbSet<EmployersWantedAbilities> EmployersWantedAbilities { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Team> Teams { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
            .HasData(
            new IdentityRole
            {
                Name = "Developer",
                NormalizedName = "DEVELOPER"
            }
             );
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>()
            .HasData(
            new IdentityRole
            {
                Name = "Employer",
                NormalizedName = "EMPLOYER"
            }
            );

            builder.Entity<Ability>().HasData(
               new Models.Ability
               {
                   AbilityId = 1,
                   AbilityName = "FrontEnd"
               },
               new Ability
               {
                   AbilityId = 2,
                   AbilityName = "Backend"
               },
               new Ability
               {
                   AbilityId = 3,
                   AbilityName = "App Developer"
               },
               new Ability
               {
                   AbilityId = 4,
                   AbilityName = "React"
               });

            builder.Entity<DeveloperAbilities>()
           .HasKey(bc => new { bc.DeveloperId, bc.AbilityId });
            builder.Entity<DeveloperAbilities>()
                .HasOne(bc => bc.Developer)
                .WithMany(b => b.DevAbilities)
                .HasForeignKey(bc => bc.DeveloperId);
            builder.Entity<DeveloperAbilities>()
                .HasOne(bc => bc.Ability)
                .WithMany(c => c.DeveloperAbilities)
                .HasForeignKey(bc => bc.AbilityId);

            builder.Entity<EmployersWantedAbilities>()
          .HasKey(bc => new { bc.ListingId, bc.AbilityId });
            builder.Entity<EmployersWantedAbilities>()
                .HasOne(bc => bc.Listing)
                .WithMany(b => b.EmployersWantedAbilities)
                .HasForeignKey(bc => bc.ListingId);
            builder.Entity<EmployersWantedAbilities>()
                .HasOne(bc => bc.Ability)
                .WithMany(c => c.EmployersWantedAbilities)
                .HasForeignKey(bc => bc.AbilityId);
        }
    }
}
