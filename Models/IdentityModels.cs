using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using protean.Epicor;
using System;

namespace protean.Models
{

    /// <summary>
    /// You can add profile data for the user by adding more properties to your ApplicationUser class, please 
    /// visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {

        private SalesRep _salesRep;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [Display(Name = "IsEnabled")]
        public bool? IsEnabled { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public string BadgePath { get; set; }

        [Display(Name = "Work Force ID")]
        public string SalesRepCode { get; set; }

        public DateTime? LastAuth { get; set; }

        [NotMapped]
        [Display(Name = "Name")]
        public string FullName
        {
            get { return $"{ this.FirstName} {this.LastName}"; }
        }

        [NotMapped]
        public SalesRep SalesRep
        {
            get
            {
                if (_salesRep == null)
                {
                    _salesRep = new SalesRep { SalesRepCode = this.SalesRepCode };
                }
                return _salesRep;
            }
        }

        [NotMapped]
        public string Initials
        {
            get { return $"{this.FirstName.Substring(0, 1)}{this.LastName.Substring(0, 1)}"; }
        }


        /// <summary>
        /// Create function to get role names of this user
        /// </summary>
        /// <returns>List of roles names</returns>
        public IEnumerable<string> RoleNames()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Database.ActiveContext));
            return userManager.GetRoles(this.Id);

        }

    }

    /// <summary>
    /// Application Context
    /// This is used for all communication to the database.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<TestObject> TestData { get; set; }
    }

    /// <summary>
    /// Migration context
    /// Used to create and destroy in the database.  Only used for migrations.
    /// </summary>
    public class MigrationDbContext : IdentityDbContext<ApplicationUser>
    {
        public MigrationDbContext()
            : base("MigrationConnection", throwIfV1Schema: false)
        {
        }

        public static MigrationDbContext Create()
        {
            return new MigrationDbContext();
        }

        public DbSet<TestObject> TestData { get; set; }
    }

    public class TestObject
    {
        [Key]
        public string ID { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Value { get; set; }

    }
}