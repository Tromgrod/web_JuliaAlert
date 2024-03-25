using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.BusinessObjects;

namespace LIB.EF.Configurations
{
    public class PersonEntityConfiguration : BaseEntityConfiguration<Person>
    {
        public PersonEntityConfiguration() : base(nameof(Person))
        {
            Property(x => x.FirstName).IsRequired();
            Property(x => x.LastName).IsRequired();

            HasRequired(x => x.Sex).WithMany().Map(m =>
            {
                m.MapKey("SexId");
            });
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
