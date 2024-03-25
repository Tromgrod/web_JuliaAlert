// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the Person type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using LIB.Tools.AdminArea;
    using System.Data.SqlClient;
    using System.Data;

    /// <summary>
    /// The person.
    /// </summary>
    [Serializable]   
    public class Person : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        public Person()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public Person(long id)
            : base(id)
        {
        }
        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Common(Order = 0, ControlClass = CssClass.Large), Template(Mode = Template.Name)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Common(Order = 1, ControlClass = CssClass.Large), Template(Mode = Template.Name)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Common(Order = 2, ControlClass = CssClass.Large), Template(Mode = Template.Email),Access(DisplayMode = DisplayMode.Advanced)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user sex.
        /// </summary>
        [Common(Order = 3, _Sortable = true), Validation(ValidationType = ValidationTypes.Required), Access(DisplayMode = DisplayMode.Advanced|DisplayMode.Search)]
        [Template(Mode = Template.SearchDropDown)]
        public Sex Sex { get; set; }

        #endregion

        public override string GetName()
        {
            return LastName +(!string.IsNullOrEmpty(LastName) ? " " + FirstName: FirstName);
        }

        public override void SetName(object name)
        {
            LastName = name!=null?name.ToString():"";
        }

        public override string GetCaption()
        {
            return "LastName";
        }

        public bool CanDelete(Dictionary<long, ItemBase> dictionary, SqlConnection connection, out string reason)
        {
            reason = "";
            var itemIDs = dictionary.Values.Aggregate(string.Empty, (current, item) => current + (item.Id.ToString() + ";"));

            const string StrSql = "Person_CanDelete";

            var sqlCommand = new SqlCommand(StrSql, connection) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@ids", SqlDbType.NVarChar,-1) { Value = itemIDs };
            sqlCommand.Parameters.Add(param);
            
            using (var rdr = sqlCommand.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    if (!string.IsNullOrEmpty(reason))
                    {
                        reason += ", ";
                    }
                    reason += (string)rdr["Login"];
                }

                rdr.Close();
            }
            if (!string.IsNullOrEmpty(reason))
            {
                reason="Inscrierea nu poate fi eliminata deoarece este legata de utilizatorul(rii) " + reason;
            }
            return string.IsNullOrEmpty(reason);
        }
    }
}
