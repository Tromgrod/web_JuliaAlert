using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Model
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Размеры моделей"
       , SingleName = "Размер модели"
       , DoCancel = true
       , LogRevisions = true)]
    public class ProductSize : ItemBase
    {
        #region Constructors
        public ProductSize()
            : base(0) { }

        public ProductSize(long id)
            : base(id) { }

        public ProductSize(string nameVal, SqlConnection conn = null)
            : base(nameVal, conn: conn) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Размер"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}