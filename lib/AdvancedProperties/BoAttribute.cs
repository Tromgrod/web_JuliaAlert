// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoAttribute.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the BOAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using LIB.Tools.AdminArea;
    using LIB.BusinessObjects;
    using System;

    /// <summary>
    /// The bo attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BoAttribute : PropertyItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoAttribute"/> class.
        /// </summary>
        public BoAttribute()
        {
            LoadFromDb = true;
            AllowDetails = true;
            AllowDelete = true;
            AllowEdit = true;
            AllowCreate = true;
            OpenInNewTab = true;
            ImportAccess = (int)BasePermissionenum.SuperAdmin;
            DefaultSimpleSearch = "";
            Icon = "circle-o";
            RecordsPerPage = 10;
        }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure.
        /// </summary>
        public bool UseCustomSp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure populate.
        /// </summary>
        public bool UseCustomSpPopulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure Report.
        /// </summary>
        public bool UseCustomSpReport { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure populate.
        /// </summary>
        public string SpPopulate { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure populate.
        /// </summary>
        public string SpPopulateReport { get; set; }

        /// <summary>
        /// Gets or sets the Default Filter.
        /// </summary>
        public string DefaultFilter { get; set; }

        /// <summary>
        /// Gets or sets the Default Filter.
        /// </summary>
        public string DefaultQuery { get; set; }

        /// <summary>
        /// Gets or sets the Default Filter.
        /// </summary>
        public string AfterPaginAdditionalQuery { get; set; }

        /// <summary>
        /// Gets or sets the Default Filter.
        /// </summary>
        public string AdditionalJoin { get; set; }

        /// <summary>
        /// Gets or sets the Default Filter.
        /// </summary>
        public string DefaultSimpleSearch { get; set; }

        /// <summary>
        /// Gets or sets the Default Filter.
        /// </summary>
        public string PreinitVar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure update.
        /// </summary>
        public bool UseCustomSpUpdate { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure update.
        /// </summary>
        public string SpUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure insert.
        /// </summary>
        public bool UseCustomSpInsert { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure insert.
        /// </summary>
        public string SpInsert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure delete.
        /// </summary>
        public bool UseCustomSpDelete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use custom Stored Procedure cancel.
        /// </summary>
        public bool DoCancel { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure delete.
        /// </summary>
        public string SpDelete { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure delete.
        /// </summary>
        public bool LoadFromDb { get; set; }

        /// <summary>
        /// Gets or sets the Stored Procedure delete.
        /// </summary>
        public bool LogRevisions { get; set; }

        public string ViewLink { get; set; }

        private string _DisplayName;
        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_DisplayName))
                {
                    return "";
                }
                return global::LIB.Tools.Utils.Translate.GetTranslatedValue(_DisplayName.Replace(" ", "_"), "BO", _DisplayName);
            }
            set { _DisplayName = value; }
        }

        private string _SingleName;
        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public string SingleName
        {
            get
            {
                if (string.IsNullOrEmpty(_SingleName))
                {
                    return "";
                }
                return global::LIB.Tools.Utils.Translate.GetTranslatedValue(_SingleName.Replace(" ", "_"), "BO", _SingleName);
            }
            set { _SingleName = value; }
        }

        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public AdminAreaGroupenum Group { get; set; }
        
        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long EditAccess { get; set; }
        /// <summary>
        /// Gets or sets a AllowEdit
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long CopyAccess { get; set; }
        /// <summary>
        /// Gets or sets a AllowCopy
        /// </summary>
        public bool AllowCopy { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long PrintAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long ExportAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long ImportAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long ReadAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long ReadAllAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long CreateAccess { get; set; }
        /// <summary>
        /// Gets or sets a AllowCreate
        /// </summary>
        public bool AllowCreate { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long DeleteAccess { get; set; }
        /// <summary>
        /// Gets or sets a AllowCreate
        /// </summary>
        public bool AllowDelete { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long DeleteAllAccess { get; set; }
        /// <summary>
        /// Gets or sets a AllowCreate
        /// </summary>
        public bool AllowDeleteAll { get; set; }
        /// <summary>
        /// Gets or sets a AllowDetails
        /// </summary>
        public bool AllowDetails { get; set; }

        /// <summary>
        /// Gets or sets a AllowDetails
        /// </summary>
        public bool AllowImport { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long RevisionsAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public long ModulesAccess { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public int RecordsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the CustomForm Tag.
        /// </summary>
        public string CustomFormTag { get; set; }

        /// <summary>
        /// Gets or sets the CustomForm Tag.
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Gets or sets the CustomForm Tag.
        /// </summary>
        public bool OpenInNewTab { get; set; }

        /// <summary>
        /// Gets or sets the Access.
        /// </summary>
        public short Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the MaxRecordsAllowed.
        /// </summary>
        public long MaxRecordsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the Display Name.
        /// </summary>
        public bool HideReportHeader{ get; set; }

        /// <summary>
        /// Gets or sets the Custom Page.
        /// </summary>
        public bool CustomPage { get; set; }
    }
}