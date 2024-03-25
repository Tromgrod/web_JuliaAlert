// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserBase.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the UserBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;
    using LIB.Tools.Security;

    [Serializable]
    public class User : ItemBase
    {
        #region Constructors
        public User()
            : this(0) { }

        public User(long id)
            : base(id)
        {
            this.Timeout = 1000;
            this.UniqueId = Guid.NewGuid();
        }

        public User(string login, string password)
            : base(0)
        {
            this.Login = login;
            this.Password = password;
        }
        #endregion

        #region Properties
        public Graphic _Image;
        [Template(Mode = Template.Image), Image(ThumbnailWidth = 160, ThumbnailHeight = 160),
         Access(DisplayMode = DisplayMode.Advanced)]
        public Graphic Image
        {
            get => (_Image == null || _Image.Id == 0) && Role != null ? Role.Avatar : _Image;
            set => _Image = value;
        }

        [Common(EditTemplate = EditTemplates.SimpleInput, _Sortable = true, _Searchable = true),
         Validation(ValidationType = ValidationTypes.Function, ValidationFunction = "ValidateUserName"),
         Access(DisplayMode = DisplayMode.Search | DisplayMode.Simple | DisplayMode.Advanced)]
        public string Login { get; set; }

        [Common(EditTemplate = EditTemplates.Password),
        Validation(ValidationType = ValidationTypes.Function, ValidationFunction = "ValidatePassword"),
        Access(DisplayMode = DisplayMode.Simple | DisplayMode.Advanced)]
        public string Password { get; set; }

        [Template(Mode = Template.Number),
        Validation(ValidationType = ValidationTypes.RegularExpressionRequired),
        Access(DisplayMode = DisplayMode.Advanced)]
        public int Timeout { get; set; }

        [Template(Mode = Template.PermissionsSelector),
        Access(EditableFor = (long)BasePermissionenum.SuperAdmin, VisibleFor = (long)BasePermissionenum.SuperAdmin, DisplayMode = DisplayMode.Advanced)]
        public long Permission { get; set; }

        [Common(ControlClass = CssClass.Large), Template(Mode = Template.SearchDropDown), LookUp(DefaultValue = true),
        Validation(ValidationType = ValidationTypes.Required)]
        public Role Role { get; set; }

        [Common(ControlClass = CssClass.Large), Template(Mode = Template.SearchSelectList), LookUp(DefaultValue = true),
        Access(DisplayMode = DisplayMode.Advanced)]
        public Person Person { get; set; }

        [Common(_Sortable = true), Template(Mode = Template.CheckBox)]
        public bool Enabled { get; set; }

        [Common(_Sortable = true, DisplayName = "Demonstrează eroarea", DisplayGroup = "Setări"), Template(Mode = Template.CheckBox)]
        public bool DisplayError { get; set; }

        [System.Xml.Serialization.XmlIgnore, System.Web.Script.Serialization.ScriptIgnore]
        public Guid UniqueId { get; set; }

        [Db(_Editable = false),
        Common(EditTemplate = EditTemplates.DateTimeInput, _Sortable = true),
        Access(DisplayMode = DisplayMode.Advanced | DisplayMode.Simple)]
        public DateTime LastLogin { get; set; }

        [Db(_Editable = false, _Populate = false, _ReadableOnlyName = true)]
        public User UpdatedBy { get; set; }
        #endregion

        public bool HasPermissions(long binaryFlags)
            => Permissions.HasPermissions(this.Permission, binaryFlags) || (this.Role != null && Permissions.HasPermissions(this.Role.Permission, binaryFlags));

        public bool HasAtLeastOnePermission(long binaryFlags)
        {
            if (binaryFlags == 0)
                return true;

            return Permissions.HasAtLeastOnePermission(this.Permission, binaryFlags) || (this.Role != null && Permissions.HasAtLeastOnePermission(this.Role.Permission, binaryFlags));
        }

        #region POPULATE Methods
        public static bool CheckLoginForUniqueness(SqlConnection conn, string login, int id)
        {
            var cmd = new SqlCommand("User_CheckLoginForUniqueness", conn) { CommandType = CommandType.StoredProcedure };

            var param = new SqlParameter("@Login", SqlDbType.NVarChar, 100);
            param.Value = Crypt.Encrypt(login, ConfigurationManager.AppSettings["CryptKey"]);
            cmd.Parameters.Add(param);

            param = new SqlParameter("@UserId", SqlDbType.Int);
            param.Value = id;
            cmd.Parameters.Add(param);

            return (bool)cmd.ExecuteScalar();
        }

        public static List<ItemBase> LoadList()
        {
            var cmd = new SqlCommand("User_Populate_Latests", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var Users = new List<ItemBase>();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                {
                    var usr = (User)(new User()).FromDataRow(rdr);
                    Users.Add(usr);
                }

                rdr.Close();
            }

            return Users;
        }
        #endregion

        #region Utils Methods
        public virtual bool IsPasswordValid(string password)
        {
            return Password == System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
        }

        public string GetpasswordHash()
        {
            if (!string.IsNullOrEmpty(this.Password))
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.Password, "md5");
            }

            return string.Empty;
        }

        public override string GetCaption() => nameof(this.Login);

        public override string GetName() => this.Login;
        #endregion
    }
}