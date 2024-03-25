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
    public class UserEntityConfiguration : BaseEntityConfiguration<User>
    {
        public UserEntityConfiguration() : base(nameof(User))
        {
            Property(x => x.Login).IsRequired().HasMaxLength(100);
            Property(x => x.Password).IsRequired().HasMaxLength(50);
            Property(x => x.Timeout).IsRequired();
            Property(x => x.Permission).IsRequired();
            Property(x => x.UniqueId).IsRequired();
            Property(x => x.Enabled).IsRequired();

            HasOptional(x => x.UpdatedBy).WithMany().Map(m =>
            {
                m.MapKey("UpdatedById");
            });
            HasOptional(x => x.Image).WithMany().Map(m =>
            {
                m.MapKey("ImageId");
            });
            HasOptional(x => x.Role).WithMany().Map(m =>
            {
                m.MapKey("RoleId");
            });
            HasOptional(x => x.Person).WithMany().Map(m =>
            {
                m.MapKey("PersonId");
            });
        }
    }
}
