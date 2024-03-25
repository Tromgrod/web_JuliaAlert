using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.BusinessObjects;
using LIB.Tools.BO;

namespace LIB.EF.Configurations
{
    public abstract class BaseEntityConfiguration<T> : EntityTypeConfiguration<T>, IEntityConfiguration where T : ItemBase
    {
        public BaseEntityConfiguration(string name)
        {
            ToTable(name);

            HasKey(x => x.Id, x => x.HasName($"{name}Id"));

            Property(x => x.Id).HasColumnName($"{name}Id");
            Property(x => x.DateCreated).IsRequired();

            Ignore(x => x.Category);

            HasRequired(x => x.CreatedBy).WithMany().Map(m =>
            {
                m.MapKey("CreatedBy");
            });
            HasOptional(x => x.DeletedBy).WithMany().Map(m =>
            {
                m.MapKey("DeletedBy");
            });
        }

        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
