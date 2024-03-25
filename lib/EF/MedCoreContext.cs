using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.BusinessObjects;

namespace LIB.EF
{
    public abstract class JuliaAlertContext : DbContext
    {
        public DbSet<Graphic> Graphic { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Sex> Sex { get; set; }


        protected JuliaAlertContext(string contextName) : base(contextName)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<JuliaAlertContext>());
        }
    }
}
