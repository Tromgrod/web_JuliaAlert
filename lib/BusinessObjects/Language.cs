// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Language.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Language type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;

    [Serializable]
    public class Language : ItemBase
    {        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        public Language()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Language(long id)
            : base(id)
        {
        }
        #endregion

        public override string GetCaption() => nameof(this.FullName);
      
        [Template(Mode = Template.Name)]
        public string ShortName { get; set; }

        [Template(Mode = Template.Name)]
        public string FullName { get; set; }

        [Common(_Searchable = true), Template(Mode = Template.Name)]
        public string Culture { get; set; }

        [Template(Mode = Template.Image)]
        public Graphic Image { get; set; }

        [Template(Mode = Template.CheckBox)]
        public bool Enabled { get; set; }
    }
 }