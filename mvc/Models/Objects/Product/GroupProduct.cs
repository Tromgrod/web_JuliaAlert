using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.ModelType
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Группы моделей"
       , SingleName = "Группу моделей"
       , DoCancel = true
       , LogRevisions = true)]
    public class GroupProduct : ItemBase
    {
        #region Constructors
        public GroupProduct()
            : base(0) { }

        public GroupProduct(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Название"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}