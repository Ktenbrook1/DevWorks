using System;
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
                Id = "23781239-0a44-4ab8-93c5-95ead3bc4db5",
                Name = "Developer",
                NormalizedName = "DEVELOPER",
                ConcurrencyStamp = "628985cd-3ff0-4c7b-abcd-eb7095f65ea7"
            }
             );
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>()
            .HasData(
            new IdentityRole
            {
                Id = "41ce790c-add9-4145-a486-a217804ab2e4",
                Name = "Employer",
                NormalizedName = "EMPLOYER",
                ConcurrencyStamp = "f94ac645-3fc0-4397-8062-b423c379fb35"
            }
            );

            //builder.Entity<Developer>().HasData(
            //    new Models.Developer
            //    {
            //        DeveloperId = 1,
            //        UserName = "Tommy",
            //        GitHubLink = "https://github.com/tommy351",
            //        ProfileImgURL = "https://upload.wikimedia.org/wikipedia/commons/d/db/Simon_Pryce_2014.jpg",
            //        Bio = "I'm the man for the job!",
            //        RatePerHr = 78.50,
            //        IsInContract = false
            //    });

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
        public DbSet<DevWorksCapstone.Models.Message> Message { get; set; }
    }
}
