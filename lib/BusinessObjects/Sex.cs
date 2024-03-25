// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sex.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Sex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;

    [Serializable]
    public class Sex : ItemBase
    {
        #region Static Sex
        public static Sex Male => new Sex(1, "Male");

        public static Sex Female => new Sex(2, "Female");
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sex"/> class.
        /// </summary>
        public Sex()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sex"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Sex(long id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sex"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public Sex(int id, string name)
            : base(id)
        {
            this.Name = name;
        }

        #endregion

        #region Sex Properties
        [Template(Mode = Template.TranslatableName)]
        [Validation(ValidationType = ValidationTypes.Required),
         Access(DisplayMode = DisplayMode.Search | DisplayMode.Simple | DisplayMode.Advanced,
             EditableFor = (long)BasePermissionenum.SuperAdmin)]
        public string Name { get; set; }        
        #endregion                
    }
}