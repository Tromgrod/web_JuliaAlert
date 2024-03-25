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
    public class SexEntityConfiguration : BaseEntityConfiguration<Sex>
    {
        public SexEntityConfiguration() : base(nameof(Sex))
        {
            Property(x => x.Name).IsRequired();
        }
    }
}
