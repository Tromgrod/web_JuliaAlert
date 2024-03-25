using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.EF;
using LIB.EF.Configurations;

namespace JuliaAlertLib.EF
{
    public class JuliaAlertContext : LIB.EF.JuliaAlertContext
    {
        public JuliaAlertContext() : this(nameof(JuliaAlertContext))
        {
            
        }

        protected JuliaAlertContext(string contextName) : base(contextName)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var configs = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IEntityConfiguration).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(x => x)
                .ToList();

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            configs.ForEach(c =>
                ((IEntityConfiguration)Activator.CreateInstance(c))
                .AddConfiguration(modelBuilder.Configurations));

            base.OnModelCreating(modelBuilder);
        }
    }
}
