// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DB.cs" company="GalexStudio">
//   Copyright ©  2022
// </copyright>
// <summary>
//   Defines the DB type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;
    using System.Globalization;
    using System.Data;
    using LIB.Tools.BO;
    using System.Collections.Generic;

    /// <summary>
    /// The data base.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Db : PropertyItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Db"/> class.
        /// </summary>
        public Db()
        {
            this.ParamType = SqlDbType.Image;
            this.Sort = DbSortMode.None;
            this.Populate = true;
        }

        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// Gets or sets the parameter size.
        /// </summary>
        public int ParamSize { get; set; }

        /// <summary>
        /// Gets or sets the parameter type.
        /// </summary>
        public SqlDbType ParamType { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix { get; set; }

        public bool _Ignore
        {
            get
            {
                return Ignore == true;
            }
            set
            {
                Ignore = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ignore.
        /// </summary>
        public bool? Ignore { get; set; }

        public bool _AllowNull
        {
            get
            {
                return AllowNull == true;
            }
            set
            {
                AllowNull = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ignore.
        /// </summary>
        public bool? AllowNull { get; set; }

        public bool _Editable
        {
            get
            {
                return Editable == true;
            }
            set
            {
                Editable = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether editable.
        /// </summary>
        public bool? Editable { get; set; }

        public bool _Readable
        {
            get
            {
                return Readable == true;
            }
            set
            {
                Readable = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether readable.
        /// </summary>
        public bool? Readable { get; set; }

        public bool _Populate
        {
            get
            {
                return Populate == true;
            }
            set
            {
                Populate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether readable.
        /// </summary>
        public bool? Populate { get; set; }

        public bool _ReadableOnlyName
        {
            get
            {
                return ReadableOnlyName == true;
            }
            set
            {
                ReadableOnlyName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether editable.
        /// </summary>
        public bool? ReadableOnlyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if filed is sorted.
        /// </summary>
        public DbSortMode Sort { get; set; }

        public bool DeepFilter { get; set; }

        public bool ApplyFilter { get; set; }

        public string PopulateDropDown { get; set; }

        /// <summary>
        /// The copy user fields.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void CopyUserFields(PropertyItem item)
        {
            if (item != null)
            {
                if (string.IsNullOrEmpty(this.ParamName))
                {
                    this.ParamName = ((Db)item).ParamName;
                }
                if (this.ParamSize == 0)
                {
                    this.ParamSize = ((Db)item).ParamSize;
                }
                if (this.ParamType == SqlDbType.Image)
                {
                    this.ParamType = ((Db)item).ParamType;
                }
                if (this.Sort == DbSortMode.None)
                {
                    this.Sort = ((Db)item).Sort;
                }
                if (string.IsNullOrEmpty(this.Prefix))
                {
                    this.Prefix = ((Db)item).Prefix;
                }
                if (this.Ignore == null)
                {
                    this.Ignore = ((Db)item).Ignore;
                }
                if (this.AllowNull == null)
                {
                    this.AllowNull = ((Db)item).AllowNull;
                }
                if (this.Editable == null)
                {
                    this.Editable = ((Db)item).Editable;
                }
                if (this.Readable == null)
                {
                    this.Readable = ((Db)item).Readable;
                }
                if (this.Populate == null)
                {
                    this.Populate = ((Db)item).Populate;
                }
                if (this.ReadableOnlyName == null)
                {
                    this.ReadableOnlyName = ((Db)item).ReadableOnlyName;
                }
            }
        }
    }
}