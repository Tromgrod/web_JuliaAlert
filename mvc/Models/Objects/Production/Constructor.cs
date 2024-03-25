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
       , DisplayName = "Конструкторы"
       , SingleName = "Конструктора"
       , DoCancel = false
       , LogRevisions = true)]
    public class Constructor : ItemBase
    {
        #region Constructors
        public Constructor()
            : base(0) { }

        public Constructor(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Конструктор"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}