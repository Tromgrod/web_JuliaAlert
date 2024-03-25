// ------------------------------------public --------------------------------------------------------------------------------
// <copyright file="TextboxModel.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the TextboxModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Weblib.Models.Common
{
    using LIB.Models.Common;
    using Weblib.Models.Common.Enums;

    /// <summary>
    /// The textbox model.
    /// </summary>
    public class CheckboxModel : iBaseControlModel
    {
        public CheckboxModel()
        {
            Value = "1";
        }

        public string Caption { get; set; }

        public string AdditionalControl { get; set; }

        public string Name { get; set; }

        private string _LabelName;
        public string LabelName 
        {
            get => string.IsNullOrEmpty(this._LabelName) ? this.Name : this._LabelName;
            set => this._LabelName = value;
        }

        public string Id { get; set; }

        public string Value { get; set; }

        public bool Checked { get; set; }

        public string Class { get; set; }

        public string OnClick { get; set; }

        public bool Disabled { get; set; }
    }
}
