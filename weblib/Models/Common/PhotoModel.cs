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
    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Models.Common;

    public class PhotoModel : iBaseControlModel
    {
        public string Caption { get; set; }

        public string Name { get; set; }

        public Graphic Value { get; set; }

        public string Class { get; set; }

        public ValidationTypes ValidationType { get; set; }

        public bool ReadOnly { get; set; }

        public bool Disabled { get; set; }

        public string RequiredMessage { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}