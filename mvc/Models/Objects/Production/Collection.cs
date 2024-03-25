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
       , DisplayName = "Коллекции"
       , SingleName = "Коллекцию"
       , DoCancel = false
       , LogRevisions = true)]
    public class Collection : ItemBase
    {
        #region Constructors
        public Collection()
            : base(0) { }

        public Collection(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Коллекция"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        #endregion
    }
}