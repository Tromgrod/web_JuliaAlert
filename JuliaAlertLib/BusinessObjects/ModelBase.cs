using LIB.Tools.BO;
using System.Data.SqlClient;

namespace JuliaAlertLib.BusinessObjects
{
    public class ModelBase: ItemBase
    {
        public ModelBase()
            : base(0) { }

        public ModelBase(long id)
            : base(id) { }

        public ModelBase(string name, string nameProp = "Name", bool throwException = true, SqlConnection conn = null)
            : base(name, nameProp, throwException, conn) { }
    }
}