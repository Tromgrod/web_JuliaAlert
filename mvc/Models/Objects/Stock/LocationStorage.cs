using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Stock
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Места хранения"
       , SingleName = "Место хранение"
       , DoCancel = false
       , LogRevisions = true)]
    public class LocationStorage : ItemBase
    {
        #region Constructors
        public LocationStorage()
            : base(0) { }

        public LocationStorage(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Место хранения"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}