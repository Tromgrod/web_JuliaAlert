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
    public class RoleEntityConfiguration : BaseEntityConfiguration<Role>
    {
        public RoleEntityConfiguration() : base(nameof(Role))
        {
            Property(x => x.Name).IsRequired().HasMaxLength(250);
            Property(x => x.Permission).IsRequired();

            HasOptional(x => x.Avatar).WithMany().Map(m =>
            {
                m.MapKey("AvatarId");
            });
        }
    }
}
