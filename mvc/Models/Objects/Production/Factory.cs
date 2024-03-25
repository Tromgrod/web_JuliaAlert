using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Фабрики"
       , SingleName = "Фабрику"
       , DoCancel = true
       , LogRevisions = true)]
    public class Factory : ItemBase
    {
        #region Constructors
        public Factory()
            : base(0) { }
        public Factory(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Название фабрики"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion

        public override string GetName() => this.Name;
    }
}