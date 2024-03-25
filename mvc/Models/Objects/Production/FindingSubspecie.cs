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
       , DisplayName = "Подвиды фунитуры"
       , SingleName = "Подвид фурнитуры"
       , DoCancel = true
       , LogRevisions = true)]
    public class FindingSubspecie : ItemBase
    {
        #region Constructors
        public FindingSubspecie()
            : base(0) { }

        public FindingSubspecie(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Название"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Код"), Template(Mode = Template.VisibleString)]
        public string Code { get; set; }

        [Common(DisplayName = "Вид фурнитуры"), Template(Mode = Template.ParentDropDown)]
        public FindingSpecie FindingSpecie { get; set; }
        #endregion
    }
}