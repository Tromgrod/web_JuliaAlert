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
    public class GraphicEntityConfiguration : BaseEntityConfiguration<Graphic>
    {
        public GraphicEntityConfiguration() : base(nameof(Graphic))
        {
            Property(x => x.Name).IsRequired().HasMaxLength(250);
            Property(x => x.Ext).IsRequired().HasMaxLength(4);
            Property(x => x.BOName).IsRequired().HasMaxLength(100);
        }
    }
}
