// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusinessObject.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the BusinessObject.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LIB.AdvancedProperties;

    public class BusinessObject
    {
        public Type Type { get; set; }
        public BoAttribute Properties { get; set; }
    }

 }
