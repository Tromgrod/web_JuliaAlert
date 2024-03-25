// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemBase.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the ItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using LIB.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LIB.Tools.BO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.Security;
    using LIB.Tools.Utils;

    using Translate = LIB.Tools.Utils.Translate;
    using LIB.Tools.Controls;
    using LIB.Helpers;
    using LIB.Models.Common;
    using LIB.Models;

    /// <summary>
    /// The item base.
    /// </summary>
    [Serializable]
    [JsonObject]
    public class ItemBase : IConvertible
    {
        #region fields

        /// <summary>
        /// The zero id.
        /// </summary>
        public const string ZeroId = "0";

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemBase"/> class.
        /// </summary>
        public ItemBase()
        {
            this.Id = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemBase"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ItemBase(long id)
        {
            this.Id = id;
        }

        public ItemBase(string name, string nameProp = "Name", bool throwException = true, SqlConnection conn = null)
        {
            this.SetName(name);

            var cmd = new SqlCommand($"SELECT {this.GetType().Name}Id AS Id FROM [{this.GetType().Name}] WHERE [{nameProp}] = N'{name}'", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.Text };

            var ds = new DataSet();
            var da = new SqlDataAdapter
            {
                SelectCommand = cmd
            };
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0 && !throwException)
                Id = 0;
            else if (ds.Tables[0].Rows.Count == 0 && throwException)
                throw new Exception($"{this.GetType().Name} {nameProp} нет в базе данных: " + name);
            else if (throwException)
                Id = ds.Tables[0].Rows[0]["Id"] != DBNull.Value && ds.Tables[0].Rows[0]["Id"] != null ? Convert.ToInt64(ds.Tables[0].Rows[0]["Id"]) : throw new Exception($"В базе даных нет {this.GetType().Name} {nameProp}: " + name);
            else
                Id = ds.Tables[0].Rows[0]["Id"] != DBNull.Value && ds.Tables[0].Rows[0]["Id"] != null ? Convert.ToInt64(ds.Tables[0].Rows[0]["Id"]) : 0;
        }

        #endregion

        #region operators
        public static bool operator ==(ItemBase a, ItemBase b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Id == b.Id;
        }

        public static bool operator !=(ItemBase a, ItemBase b) => !(a == b);
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Db(_Ignore = true)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        [Db(_Editable = false, _Populate = false)]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [Db(_Editable = false, _Populate = false, _ReadableOnlyName = true)]
        [JsonIgnore]
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        [Db(_Editable = false, _Populate = false, _ReadableOnlyName = true)]
        [JsonIgnore]
        public virtual User DeletedBy { get; set; }

        [Db(_Ignore = true)]
        [JsonIgnore]
        public string Category { get; set; } // for autocomplete

        [Db(_Ignore = true)]
        [JsonIgnore]
        public Dictionary<long, ItemBase> SearchItems { get; set; } // for search by multyselect
        #endregion

        #region methods
        public virtual string GetName()
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this.GetType());
            var properties = pss.GetProperties(pdc);
            if (properties.Get(GetCaption()) != null)
            {
                var property = properties.Get(GetCaption());
                if (property.PropertyDescriptor.GetValue(this) != null)
                {
                    return property.PropertyDescriptor.GetValue(this).ToString();
                }
                return "";
            }
            return this.GetType().Name;
        }

        public virtual string GetReportGridName() => GetName();

        public virtual string GetAutocompleteName() => EscapeForJson(GetName());

        static string EscapeForJson(string s)
        {
            string quoted = System.Web.Helpers.Json.Encode(s);
            return quoted.Substring(1, quoted.Length - 2).Replace("\"", "''").Replace("\\", "\\\\");
        }
        public virtual string GetValue(string fieldName)
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this.GetType());
            var properties = pss.GetProperties(pdc);
            if (properties.Get(fieldName) != null)
            {
                var property = properties.Get(fieldName);
                if (property.PropertyDescriptor.GetValue(this) != null)
                {
                    return property.PropertyDescriptor.GetValue(this).ToString();
                }
                return "";
            }
            return this.GetType().Name;
        }

        public virtual string GetGroupField(string fieldName)
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this.GetType());
            var properties = pss.GetProperties(pdc);
            if (properties.Get(fieldName) != null)
            {
                var property = properties.Get(fieldName);
                if (property.PropertyDescriptor.GetValue(this) != null)
                {
                    if (property.PropertyDescriptor.GetValue(this) as ItemBase != null)
                        return ((ItemBase)property.PropertyDescriptor.GetValue(this)).GetName();

                    return property.PropertyDescriptor.GetValue(this).ToString();
                }
                return "";
            }
            return this.GetType().Name;
        }

        public virtual void SetByFullCode(string[] fullCodeData) { }

        public virtual string GetAdvancedValue() => "";

        public virtual object GetId() => Id;

        public virtual void SetId(object Id)
        {
            if (long.TryParse(Id?.ToString(), out long lId))
                this.Id = lId;
        }

        public virtual string GetLinkName() => GetName();

        public virtual string GetLink() => "DocControl/" + this.GetType().Name + "/" + GetId();

        public virtual string GetAction() => string.Empty;

        public virtual void SetName(object name)
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this.GetType());
            var properties = pss.GetProperties(pdc);
            if (properties.Get(GetCaption()) != null)
            {
                var property = properties.Get(GetCaption());
                property.PropertyDescriptor.SetValue(this, name);
            }
        }
        public virtual void SetName(string name)
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this.GetType());
            var properties = pss.GetProperties(pdc);
            if (properties.Get(GetCaption()) != null)
            {
                var property = properties.Get(GetCaption());
                property.PropertyDescriptor.SetValue(this, name);
            }
        }

        public virtual void SetNameAddl(IDataReader rdr, DataRow dr, string prefix) { }

        public virtual string GetCaption() => "Name";

        public virtual string AutocompleteControl() => "List";
        public virtual string GetAdditionalSelectQuery(AdvancedProperty property) => "";

        public virtual string GetAdditionalJoinQuery(AdvancedProperty property) => "";

        public virtual string GetAdditionalJoinQuery() => "";

        public virtual string GetAdditionalSimpleSearchQuery(AdvancedProperty property) => "";

        public virtual string GetAdditionalGroupQuery(AdvancedProperty property = null) => "";

        public virtual string GetAdvancedLookUpFilter(AdvancedProperty property) => "";

        public virtual string GetImageName() => "Image";

        public virtual ItemBase LoadFromString(string val, string field)
        {
            var Item = new ItemBase();
            Item.SetName(val);
            return Item;
        }

        public bool IsLoadedFromDB(Type Type)
        {
            BoAttribute boproperties = null;
            if (Type.GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boproperties = (BoAttribute)Type.GetCustomAttributes(typeof(BoAttribute), true)[0];

            return boproperties == null || (boproperties != null && boproperties.LoadFromDb);
        }

        public virtual bool ReportSingleItemRedirect(ItemBase item, out string redirectUrl)
        {
            redirectUrl = "";
            return false;
        }

        public virtual Type GetPermissionsType()
        {
            return this.GetType();
        }
        #endregion

        #region Convertable
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public long GetDoubleValue()
        {
            return this.Id;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return this.Id != 0;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.GetDoubleValue());
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.GetDoubleValue());
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.GetDoubleValue());
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(this.GetDoubleValue());
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return this.GetDoubleValue();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.GetDoubleValue());
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.GetDoubleValue());
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.GetDoubleValue());
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.GetDoubleValue());
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.GetDoubleValue());
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return string.Format("({0}, {1})", "ID", this.Id);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(string))
            {
                return Convert.ChangeType(this, conversionType);
            }

            var args = new object[1];
            args[0] = this.Id;

            return conversionType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, args);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.GetDoubleValue());
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.GetDoubleValue());
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.GetDoubleValue());
        }
        #endregion

        #region CommonMethods

        #region Delete
        public virtual bool Delete(string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            var itemsToDelete = new Dictionary<long, ItemBase>
            {
                { this.Id, this }
            };

            return Delete(itemsToDelete, Comment, connection, user);
        }

        public virtual bool Delete(Dictionary<long, ItemBase> dictionary, string Comment = "Удалено", SqlConnection connection = null, User user = null)
        {
            connection = connection ?? DataBase.ConnectionFromContext();

            if (!this.CanDelete(dictionary, connection))
                return false;

            BoAttribute boProperties = null;

            if (this.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boProperties = (BoAttribute)this.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            string strSql;
            var type = 0;
            bool buseGeneric = false;
            if (boProperties != null && !string.IsNullOrEmpty(boProperties.SpDelete))
                strSql = boProperties.SpDelete;
            else if (boProperties != null && (boProperties.UseCustomSp || boProperties.UseCustomSpDelete))
                strSql = this.GetType().Name + "_Delete_" + this.GetType().Name;
            else if (boProperties != null && boProperties.DoCancel)
            {
                strSql = "Generic_Cancel";
                buseGeneric = true;
                type = (int)OperationTypes.Cancel;
                if (string.IsNullOrEmpty(Comment))
                    Comment = "Canceled";
            }
            else
            {
                strSql = "Generic_Delete";
                buseGeneric = true;
                type = (int)OperationTypes.Delete;
                if (string.IsNullOrEmpty(Comment))
                    Comment = "Deleted";
            }

            var lcommand = new SqlCommand(strSql, connection) { CommandType = CommandType.StoredProcedure };
            this.CreateDeleteParams(lcommand, dictionary, out bool lcontinue);

            if (lcontinue)
            {
                var itemIDs = dictionary.Values.Aggregate(string.Empty, (current, item) => current + (item.Id.ToString() + ";"));

                var param = new SqlParameter("@ItemIDs", SqlDbType.VarChar, 2000) { Value = itemIDs };
                lcommand.Parameters.Add(param);

                if (user != null || Authentication.CheckUser())
                {
                    param = new SqlParameter("@CancelUserID", SqlDbType.Int) { Value = (user == null ? Authentication.GetCurrentUser().Id : user.Id) };
                    lcommand.Parameters.Add(param);
                }

                if (buseGeneric)
                {
                    param = new SqlParameter("@TableName", SqlDbType.NVarChar, 400) { Value = this.GetType().Name };
                    lcommand.Parameters.Add(param);
                }

                param = new SqlParameter("@Comment", SqlDbType.NVarChar, 1000) { Value = Comment };
                lcommand.Parameters.Add(param);

                param = new SqlParameter("@Type", SqlDbType.TinyInt) { Value = type };
                lcommand.Parameters.Add(param);

                if (boProperties != null && boProperties.LogRevisions)
                {
                    param = new SqlParameter("@LogRevisions", SqlDbType.Bit) { Value = 1 };
                    lcommand.Parameters.Add(param);

                    param = new SqlParameter("@BOName", SqlDbType.NVarChar, 100) { Value = dictionary.Values.FirstOrDefault(i => i.Id > 0).GetName() };
                    lcommand.Parameters.Add(param);
                }
            }

            lcommand.ExecuteNonQuery();

            return true;
        }

        public virtual void CreateDeleteParams(SqlCommand command, Dictionary<long, ItemBase> dictionary, out bool tocontinue)
        {
            tocontinue = true;
        }

        public virtual bool CanDelete(Dictionary<long, ItemBase> dictionary, SqlConnection connection) => true;
        #endregion

        #region Update
        public virtual void Update(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            if (connection == null)
                connection = DataBase.ConnectionFromContext();

            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];
            string strSql;
            var isGeneric = false;
            if (boproperties != null && !string.IsNullOrEmpty(boproperties.SpUpdate))
                strSql = boproperties.SpUpdate;
            else if (boproperties != null && (boproperties.UseCustomSp || boproperties.UseCustomSpUpdate))
                strSql = this.GetType().Name + "_Update";
            else
            {
                strSql = "Generic_Update";
                isGeneric = true;
            }

            var lcommand = new SqlCommand(strSql, connection) { CommandType = CommandType.StoredProcedure };

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var properties = pss.GetDbEditProperties(pdc, Authentication.GetCurrentUser(), DisplayMode);

            this.CreateUpdateParams(lcommand, item, out bool tocontinue);

            if (tocontinue)
            {
                if (!isGeneric)
                {
                    #region Update Custom

                    foreach (AdvancedProperty property in properties)
                    {
                        var param = property.PropertyDescriptor.GetValue(item);

                        if (param == null
                            || (param as ItemBase != null && (param as ItemBase).Id == 0)
                            || (param is DateTime && (DateTime)param == DateTime.MinValue))
                        {
                            continue;
                        }

                        var dbparam = new SqlParameter();
                        dbparam.SqlDbType = property.Db.ParamType;
                        dbparam.Size = property.Db.ParamSize;
                        dbparam.ParameterName = "@" + property.Db.ParamName;
                        if (param as ItemBase != null)
                        {
                            dbparam.Value = (param as ItemBase).Id;
                        }
                        else if (param as Dictionary<long, ItemBase> != null)
                        {
                            var itemids = (param as Dictionary<long, ItemBase>).Values.Aggregate(string.Empty, (current, i) => current + (i.Id.ToString(CultureInfo.InvariantCulture) + ";"));
                            dbparam.Value = itemids;
                        }
                        else
                        {
                            dbparam.Value = param;
                        }

                        if (property.Encryption != null && property.Encryption.Encrypted && dbparam.Value != null
                            && !string.IsNullOrEmpty(dbparam.Value.ToString()))
                        {
                            dbparam.Value = Crypt.Encrypt(dbparam.Value.ToString(), ConfigurationManager.AppSettings["CryptKey"]);
                        }

                        lcommand.Parameters.Add(dbparam);
                    }

                    var dparam = new SqlParameter("@" + item.GetType().Name + "ID", SqlDbType.BigInt) { Value = item.Id };
                    lcommand.Parameters.Add(dparam);

                    dparam = new SqlParameter("@TableName", SqlDbType.NVarChar, 400)
                    {
                        Value = this.GetType().Name
                    };
                    lcommand.Parameters.Add(dparam);

                    dparam = new SqlParameter("@UserID", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() };
                    lcommand.Parameters.Add(dparam);

                    dparam = new SqlParameter("@UpdatedBy", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() };
                    lcommand.Parameters.Add(dparam);

                    dparam = new SqlParameter("@Comment", SqlDbType.NVarChar, 1000) { Value = Comment };
                    lcommand.Parameters.Add(dparam);

                    dparam = new SqlParameter("@Type", SqlDbType.TinyInt) { Value = OperationTypes.Update };
                    lcommand.Parameters.Add(dparam);

                    if (boproperties != null && boproperties.LogRevisions)
                    {
                        dparam = new SqlParameter("@LogRevisions", SqlDbType.Bit) { Value = 1 };
                        lcommand.Parameters.Add(dparam);

                        dparam = new SqlParameter("@BOName", SqlDbType.NVarChar, 100) { Value = item.GetName() };
                        lcommand.Parameters.Add(dparam);
                    }

                    #endregion
                }
                else
                {
                    #region Update Generic

                    var dbparam = new SqlParameter("@TableName", SqlDbType.NVarChar, 400)
                    {
                        Value = this.GetType().Name
                    };
                    lcommand.Parameters.Add(dbparam);

                    dbparam = new SqlParameter("@UpdateQuery", SqlDbType.NVarChar, -1) { Value = string.Empty };
                    foreach (AdvancedProperty property in properties)
                    {
                        var param = property.PropertyDescriptor.GetValue(item);

                        if (param == null
                            || (param as ItemBase != null && (
                                                            ((param as ItemBase).GetId() is long) && (((long)(param as ItemBase).GetId()) == 0)
                                                            ||
                                                            ((param as ItemBase).GetId() is string) && string.IsNullOrEmpty(((string)(param as ItemBase).GetId())))
                                                            )
                            || (param is Dictionary<long, ItemBase>)
                            || (property.Translate != null && property.Translate.Translatable)
                            || (property.Common.EditTemplate == EditTemplates.Password && string.IsNullOrEmpty(param.ToString()))
                            || (param is DateTime && (DateTime)param == DateTime.MinValue && property.Db.AllowNull == false))
                        {
                            continue;
                        }

                        string parameterName;

                        this.CreateUpdateParamQuery(property, out parameterName);

                        if (string.IsNullOrEmpty(parameterName))
                            parameterName = property.Db.ParamName;

                        dbparam.Value += "[" + parameterName + "]= (SELECT [" + parameterName + "] FROM #ParameterValues WHERE ParamName='" + parameterName + "'),";
                    }

                    if (!string.IsNullOrEmpty(dbparam.Value.ToString()))
                    {
                        dbparam.Value = dbparam.Value.ToString().Remove(dbparam.Value.ToString().Length - 1);
                        lcommand.Parameters.Add(dbparam);
                    }

                    var updateParams = string.Empty;
                    var updateParamValues = string.Empty;
                    var parameterValues = string.Empty;
                    var updateToParameterValues = string.Empty;
                    var updateChildTables = string.Empty;
                    var translationUpdate = string.Empty;

                    foreach (AdvancedProperty property in properties)
                    {
                        var param = property.PropertyDescriptor.GetValue(item);

                        if (param == null
                            || (param as ItemBase != null && (
                                                            ((param as ItemBase).GetId() is long) && (((long)(param as ItemBase).GetId()) == 0)
                                                            ||
                                                            ((param as ItemBase).GetId() is string) && string.IsNullOrEmpty(((string)(param as ItemBase).GetId())))
                                                            )
                            || (property.Common.EditTemplate == EditTemplates.Password && string.IsNullOrEmpty(param.ToString()))
                            || (param is DateTime && (DateTime)param == DateTime.MinValue && property.Db.AllowNull == false))
                        {
                            continue;
                        }

                        var parameterName = property.Db.ParamName;

                        if (param is Dictionary<long, ItemBase>)
                        {
                            if (property.Custom is MultiCheck)
                            {
                                var child_type = ((MultiCheck)property.Custom).ItemType;

                                var TableName = ((MultiCheck)property.Custom).TableName;
                                if (string.IsNullOrEmpty(TableName))
                                    TableName = this.GetType().Name + child_type.Name;

                                updateChildTables += " DELETE FROM " + TableName + " WHERE " + this.GetType().Name + "Id = " + this.Id.ToString() + "\n\r";
                                foreach (var child_item in ((Dictionary<long, ItemBase>)param).Values)
                                {
                                    updateChildTables += " INSERT INTO " + TableName + "(" + this.GetType().Name + "Id," + child_type.Name + "Id) VALUES(" + this.Id.ToString() + "," + child_item.Id.ToString() + ")\n\r";
                                }
                            }
                        }
                        else
                        {
                            if (property.Translate != null && property.Translate.Translatable)
                            {
                                translationUpdate += parameterName + " + ';' +";
                                continue;
                            }

                            updateParams += parameterName + ";";
                            parameterValues += "[" + parameterName + "] " + property.Db.ParamType.ToString() + (property.Db.ParamType == System.Data.SqlDbType.Decimal ? "(14,4)" : string.Empty) + DataBase.GenerateParamSize(property.Db.ParamSize) + " null,";
                            if (property.Type == typeof(DateTime))
                            {
                                if (((DateTime)param) != DateTime.MinValue)
                                {
                                    updateToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType.ToString() + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                    updateToParameterValues += "SET  @" + parameterName + "=convert(datetime,(SELECT sv.value FROM dbo.Split(@UpdateParams,';') sp INNER JOIN dbo.Split(@UpdateParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "'), 120)  \n\r";
                                    updateToParameterValues += "INSERT INTO #ParameterValues(ParamName,[" + parameterName + "])\n\r";
                                    updateToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";
                                }
                                else
                                {
                                    updateToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType.ToString() + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                    updateToParameterValues += "SET  @" + parameterName + "=null  \n\r";
                                    updateToParameterValues += "INSERT INTO #ParameterValues(ParamName,[" + parameterName + "])\n\r";
                                    updateToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";
                                }
                            }
                            else
                            {
                                updateToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType.ToString() + (property.Db.ParamType == System.Data.SqlDbType.Decimal ? "(14,4)" : string.Empty) + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                updateToParameterValues += "SET  @" + parameterName + "=CAST((SELECT sv.value FROM dbo.Split(@UpdateParams,';') sp INNER JOIN dbo.Split(@UpdateParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "') AS " + property.Db.ParamType.ToString() + (property.Db.ParamType == System.Data.SqlDbType.Decimal ? "(14,4)" : string.Empty) + DataBase.GenerateParamSize(property.Db.ParamSize) + ")  \n\r";
                                updateToParameterValues += "INSERT INTO #ParameterValues(ParamName,[" + parameterName + "])\n\r";
                                updateToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";
                            }

                            tocontinue = true;
                            updateParamValues += this.CreateUpdateParamValues(item, param, out tocontinue);
                            if (!tocontinue)
                            {
                                continue;
                            }
                            if (param as ItemBase != null)
                            {
                                updateParamValues += (param as ItemBase).GetId().ToString() + ";";
                            }
                            else if (property.Translate != null && property.Translate.Translatable)
                            {
                                updateParamValues += Translate.ModifyStringToAlias(
                                    param.ToString(), property.Translate.TableName) + ";";
                                PropertyInfo pi = item.GetType().GetProperty(property.Translate.Alias);
                                pi.SetValue(
                                    item,
                                    Translate.ModifyStringToAlias(param.ToString(), property.Translate.TableName), null);

                            }
                            else if (property.Encryption != null && property.Encryption.Encrypted && dbparam.Value != null
                                     && !string.IsNullOrEmpty(param.ToString()))
                            {
                                updateParamValues += Crypt.Encrypt(
                                    param.ToString(), ConfigurationManager.AppSettings["CryptKey"]) + ";";
                            }
                            else if (property.Db.ParamType == SqlDbType.Bit)
                            {
                                updateParamValues += Convert.ToInt32(param).ToString(CultureInfo.InvariantCulture) + ";";
                            }
                            else if (property.Db.ParamType == SqlDbType.DateTime)
                            {
                                if (((DateTime)param) != DateTime.MinValue)
                                    updateParamValues += ((DateTime)param).ToString("yyyy-MM-dd HH:mm:ss") + ";";
                                else
                                    updateParamValues += "null;";
                            }
                            else if (property.Db.ParamType == SqlDbType.Decimal)
                            {
                                updateParamValues += ((decimal)param).ToString(CultureInfo.InvariantCulture) + ";";
                            }
                            else
                            {
                                updateParamValues += param.ToString().Replace(";", "|") + ";";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(updateParams))
                    {
                        dbparam = new SqlParameter("@UpdateParams", SqlDbType.NVarChar, -1) { Value = updateParams };
                        lcommand.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(updateChildTables))
                    {
                        dbparam = new SqlParameter("@updateChildTables", SqlDbType.NVarChar, -1) { Value = updateChildTables };
                        lcommand.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(updateParamValues))
                    {
                        dbparam = new SqlParameter("@UpdateParamValues", SqlDbType.NVarChar, -1)
                        {
                            Value = updateParamValues
                        };
                        lcommand.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(parameterValues))
                    {
                        parameterValues = parameterValues.Remove(parameterValues.Length - 1);

                        dbparam = new SqlParameter("@ParameterValues", SqlDbType.NVarChar, -1)
                        {
                            Value = parameterValues
                        };
                        lcommand.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(updateToParameterValues))
                    {
                        dbparam = new SqlParameter("@UpdateToParameterValues", SqlDbType.NVarChar, -1)
                        {
                            Value = updateToParameterValues
                        };
                        lcommand.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(translationUpdate))
                    {
                        translationUpdate = translationUpdate.Remove(translationUpdate.Length - 8);

                        dbparam = new SqlParameter("@TranslationUpdate", SqlDbType.NVarChar, -1)
                        {
                            Value = translationUpdate
                        };
                        lcommand.Parameters.Add(dbparam);
                    }


                    dbparam = new SqlParameter("@UpdatedBy", SqlDbType.BigInt) { Value = Authentication.GetCurrentUserId() };
                    lcommand.Parameters.Add(dbparam);

                    dbparam = new SqlParameter("@Comment", SqlDbType.NVarChar, 1000) { Value = Comment };
                    lcommand.Parameters.Add(dbparam);

                    dbparam = new SqlParameter("@Type", SqlDbType.TinyInt) { Value = OperationTypes.Update };
                    lcommand.Parameters.Add(dbparam);

                    var idparam = new SqlParameter("@ID", SqlDbType.BigInt) { Value = item.Id };
                    lcommand.Parameters.Add(idparam);

                    if (boproperties != null && boproperties.LogRevisions)
                    {
                        dbparam = new SqlParameter("@LogRevisions", SqlDbType.Bit) { Value = 1 };
                        lcommand.Parameters.Add(dbparam);

                        dbparam = new SqlParameter("@BOName", SqlDbType.NVarChar, 100) { Value = item.GetName() };
                        lcommand.Parameters.Add(dbparam);
                    }

                    #endregion
                }

            }

            var aliases = Convert.ToString(lcommand.ExecuteScalar());

            if (!string.IsNullOrEmpty(aliases))
            {
                var transalionItemsCouter = 0;
                foreach (AdvancedProperty property in properties)
                {
                    if (property.Translate != null && property.Translate.Translatable)
                    {

                        var translationItem = new Translation();

                        translationItem.Text = property.PropertyDescriptor.GetValue(item).ToString();
                        translationItem.Alias = aliases.Split(';')[transalionItemsCouter];
                        translationItem.Language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

                        Translation.Update(connection, translationItem);

                        transalionItemsCouter++;
                    }
                }
            }
        }

        public virtual RequestResult ActionUpdate(ItemBase item, DisplayMode DisplayMode = DisplayMode.Advanced, string Comment = "Updated", SqlConnection connection = null)
        {
            Update(item, DisplayMode, Comment, connection);

            return new RequestResult() { Message = "OK", Result = RequestResultType.Success };
        }

        public virtual void CreateUpdateParamQuery(AdvancedProperty property, out string paramName)
        {
            paramName = string.Empty;
        }

        public virtual object CreateUpdateParamValues(ItemBase item, object param, out bool tocontinue)
        {
            tocontinue = true;
            return string.Empty;
        }

        public virtual void CreateUpdateParams(SqlCommand command, ItemBase itemparameter, out bool tocontinue)
        {
            tocontinue = true;
        }

        public virtual void UpdateProperties(string properties, object newVal, SqlConnection conn = null)
        {
            var cmd = new SqlCommand($"UPDATE {this.GetType().Name} SET {properties} = {newVal} WHERE {this.GetType().Name}Id = {this.Id}", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.Text };
            cmd.ExecuteReader();
        }

        public virtual void UpdateProperties(Type properties, long newVal, SqlConnection conn = null)
        {
            var cmd = new SqlCommand($"UPDATE {this.GetType().Name} SET {properties.Name}Id = {newVal} WHERE {this.GetType().Name}Id = {this.Id}", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.Text };
            cmd.ExecuteReader();
        }
        #endregion

        #region Insert
        public virtual void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            if (connection == null)
                connection = DataBase.ConnectionFromContext();

            BoAttribute boproperties = null;
            if (item.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boproperties = (BoAttribute)item.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            string strSql;
            var toUseGeneric = false;
            if (boproperties != null && !string.IsNullOrEmpty(boproperties.SpInsert))
                strSql = boproperties.SpInsert;
            else if (boproperties != null && (boproperties.UseCustomSp || boproperties.UseCustomSpInsert))
                strSql = this.GetType().Name + "_Insert";
            else
            {
                strSql = "Generic_Insert";
                toUseGeneric = true;
            }

            var command = new SqlCommand(strSql, connection) { CommandType = CommandType.StoredProcedure };

            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(item.GetType());
            var properties = pss.GetDbEditProperties(pdc, null);

            this.CreateInsertParams(command, item, out bool tocontinue);

            if (tocontinue)
            {
                if (!toUseGeneric)
                {
                    #region Insert Custom

                    foreach (AdvancedProperty property in properties)
                    {
                        var param = property.PropertyDescriptor.GetValue(item);

                        if (param == null
                               || (param as ItemBase != null && (
                                                               ((param as ItemBase).GetId() is long) && (((long)(param as ItemBase).GetId()) == 0)
                                                               ||
                                                               ((param as ItemBase).GetId() is string) && string.IsNullOrEmpty(((string)(param as ItemBase).GetId())))
                                                               )
                               || (param is DateTime && (DateTime)param == DateTime.MinValue && property.Db.AllowNull == false))
                        {
                            continue;
                        }

                        var dbparam = new SqlParameter
                        {
                            SqlDbType = property.Db.ParamType,
                            Size = property.Db.ParamSize
                        };
                        if (property.Translate != null && property.Translate.Translatable)
                        {
                            dbparam.ParameterName = "@" + property.Translate.Alias;
                        }

                        if (property.Translate != null && property.Translate.Translatable)
                        {
                            dbparam.Value = Translate.ModifyStringToAlias(param.ToString(), property.Translate.TableName);
                            var pi = item.GetType().GetProperty(property.Translate.Alias);
                            pi.SetValue(item, Translate.ModifyStringToAlias(param.ToString(), property.Translate.TableName), null);
                        }
                        else
                        {
                            if (param as ItemBase != null)
                            {
                                dbparam.Value = (param as ItemBase).Id;
                            }
                            else
                            {
                                dbparam.Value = param;
                            }

                            if (property.Encryption != null && property.Encryption.Encrypted && dbparam.Value != null
                                && !string.IsNullOrEmpty(dbparam.Value.ToString()))
                            {
                                dbparam.Value = Crypt.Encrypt(
                                    dbparam.Value.ToString(), ConfigurationManager.AppSettings["CryptKey"]);
                            }
                        }

                        command.Parameters.Add(dbparam);
                    }
                    #endregion
                }
                else
                {
                    #region Insert Generic

                    var dbparam = new SqlParameter("@TableName", SqlDbType.NVarChar, 400)
                    {
                        Value = this.GetType().Name
                    };
                    command.Parameters.Add(dbparam);

                    dbparam = new SqlParameter("@InsertQuery", SqlDbType.NVarChar, -1) { Value = string.Empty };
                    foreach (AdvancedProperty property in properties)
                    {
                        var param = property.PropertyDescriptor.GetValue(item);

                        if (param == null
                                || (param as ItemBase != null && (
                                                                ((param as ItemBase).GetId() is long) && (((long)(param as ItemBase).GetId()) == 0)
                                                                ||
                                                                ((param as ItemBase).GetId() is string) && string.IsNullOrEmpty(((string)(param as ItemBase).GetId())))
                                                                )
                                || (param is Dictionary<long, ItemBase>)
                                || (param is DateTime && (DateTime)param == DateTime.MinValue && property.Db.AllowNull == false))
                        {
                            continue;
                        }

                        string parameterName;

                        this.CreateInsertParamQuery(property, out parameterName);

                        if (string.IsNullOrEmpty(parameterName))
                            parameterName = "[" + property.Db.ParamName + "]";

                        dbparam.Value += parameterName + ",";
                    }

                    dbparam.Value = dbparam.Value.ToString().Remove(dbparam.Value.ToString().Length - 1);
                    command.Parameters.Add(dbparam);

                    var insertParams = string.Empty;
                    var insertParamValues = string.Empty;
                    var parameterValues = string.Empty;
                    var insertToParameterValues = string.Empty;
                    var insertValuesQuery = string.Empty;
                    var insertChildTables = string.Empty;
                    var translationUpdate = string.Empty;

                    foreach (AdvancedProperty property in properties)
                    {
                        var param = property.PropertyDescriptor.GetValue(item);

                        if (param == null
                            || (param as ItemBase != null && (
                                                            ((param as ItemBase).GetId() is long) && (((long)(param as ItemBase).GetId()) == 0)
                                                            ||
                                                            ((param as ItemBase).GetId() is string) && string.IsNullOrEmpty(((string)(param as ItemBase).GetId()))))
                            || (param is DateTime && (DateTime)param == DateTime.MinValue && property.Db.AllowNull == false))
                        {
                            continue;
                        }

                        var parameterName = property.Db.ParamName;

                        if (param is Dictionary<long, ItemBase>)
                        {
                            if (property.Custom is MultiCheck)
                            {
                                var child_type = ((MultiCheck)property.Custom).ItemType;

                                var TableName = ((MultiCheck)property.Custom).TableName;
                                if (string.IsNullOrEmpty(TableName))
                                    TableName = this.GetType().Name + child_type.Name;

                                foreach (var child_item in ((Dictionary<long, ItemBase>)param).Values)
                                {
                                    insertChildTables += " INSERT INTO " + TableName + "(" + this.GetType().Name + "Id," + child_type.Name + "Id) VALUES(@ID," + child_item.Id.ToString() + ")\n\r";
                                }
                            }
                        }
                        else
                        {
                            insertParams += parameterName + ";";
                            parameterValues += "[" + parameterName + "] " + property.Db.ParamType.ToString() + (property.Db.ParamType == System.Data.SqlDbType.Decimal ? "(14,4)" : string.Empty) + DataBase.GenerateParamSize(property.Db.ParamSize) + " null,";
                            if (property.Type == typeof(DateTime))
                            {
                                if (((DateTime)param) != DateTime.MinValue)
                                {
                                    insertToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType.ToString() + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "=convert(datetime,(SELECT sv.value FROM dbo.Split(@InsertParams,';') sp INNER JOIN dbo.Split(@InsertParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "'), 120)  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName,[" + parameterName + "])\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";
                                }
                                else
                                {
                                    insertToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType.ToString() + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "=null  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName,[" + parameterName + "])\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";
                                }
                            }
                            else
                            {
                                insertToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType.ToString() + (property.Db.ParamType == System.Data.SqlDbType.Decimal ? "(14,4)" : string.Empty) + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                insertToParameterValues += "SET  @" + parameterName + "=CAST((SELECT sv.value FROM dbo.Split(@InsertParams,';') sp INNER JOIN dbo.Split(@InsertParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "') AS " + property.Db.ParamType.ToString() + (property.Db.ParamType == System.Data.SqlDbType.Decimal ? "(14,4)" : string.Empty) + DataBase.GenerateParamSize(property.Db.ParamSize) + ")  \n\r";
                                insertToParameterValues += "INSERT INTO #ParameterValues(ParamName,[" + parameterName + "])\n\r";
                                insertToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";
                            }
                            insertValuesQuery += "(SELECT [" + parameterName + "] FROM #ParameterValues WHERE ParamName='" + parameterName + "'),";

                            tocontinue = true;
                            insertParamValues += this.CreateInsertParamValues(item, param, out tocontinue);
                            if (tocontinue)
                            {
                                if (param as ItemBase != null)
                                {
                                    insertParamValues += (param as ItemBase).GetId().ToString() + ";";
                                }
                                else if (property.Translate != null && property.Translate.Translatable)
                                {
                                    insertParamValues += Translate.ModifyStringToAlias(
                                        param.ToString(), property.Translate.TableName) + ";";
                                    PropertyInfo pi = item.GetType().GetProperty(property.Translate.Alias);
                                    pi.SetValue(
                                        item,
                                        Translate.ModifyStringToAlias(
                                            param.ToString(), property.Translate.TableName),
                                        null);

                                    translationUpdate += parameterName + "=" + parameterName
                                                         + "+'_'+CAST(@ID AS varchar(10)),";
                                }
                                else if (property.Encryption != null && property.Encryption.Encrypted && dbparam.Value != null
                                         && !string.IsNullOrEmpty(param.ToString()))
                                {
                                    insertParamValues += Crypt.Encrypt(
                                        param.ToString(), ConfigurationManager.AppSettings["CryptKey"]) + ";";
                                }
                                else if (property.Db.ParamType == SqlDbType.Bit)
                                {
                                    insertParamValues += Convert.ToInt32(param).ToString() + ";";
                                }
                                else if (property.Db.ParamType == SqlDbType.DateTime)
                                {
                                    insertParamValues += ((DateTime)param).ToString("yyyy-MM-dd HH:mm:ss") + ";";
                                }
                                else if (property.Db.ParamType == SqlDbType.Decimal)
                                {
                                    insertParamValues += ((decimal)param).ToString(CultureInfo.InvariantCulture) + ";";
                                }
                                else
                                {
                                    insertParamValues += param.ToString().Replace(";", "|") + ";";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(insertParams))
                    {
                        dbparam = new SqlParameter("@InsertParams", SqlDbType.NVarChar, -1) { Value = insertParams };
                        command.Parameters.Add(dbparam);
                    }
                    if (!string.IsNullOrEmpty(insertChildTables))
                    {
                        dbparam = new SqlParameter("@insertChildTables", SqlDbType.NVarChar, -1) { Value = insertChildTables };
                        command.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(insertParamValues))
                    {
                        dbparam = new SqlParameter("@InsertParamValues", SqlDbType.NVarChar, -1)
                        {
                            Value =
                                              insertParamValues
                        };
                        command.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(parameterValues))
                    {
                        parameterValues = parameterValues.Remove(parameterValues.Length - 1);

                        dbparam = new SqlParameter("@ParameterValues", SqlDbType.NVarChar, -1)
                        {
                            Value = parameterValues
                        };
                        command.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(insertToParameterValues))
                    {
                        dbparam = new SqlParameter("@InsertToParameterValues", SqlDbType.NVarChar, -1)
                        {
                            Value =
                                              insertToParameterValues
                        };
                        command.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(translationUpdate))
                    {
                        translationUpdate = translationUpdate.Remove(translationUpdate.Length - 1);

                        dbparam = new SqlParameter("@TranslationUpdate", SqlDbType.NVarChar, -1)
                        {
                            Value =
                                              translationUpdate
                        };
                        command.Parameters.Add(dbparam);
                    }

                    if (!string.IsNullOrEmpty(insertValuesQuery))
                    {
                        insertValuesQuery = insertValuesQuery.Remove(insertValuesQuery.Length - 1);

                        dbparam = new SqlParameter("@InsertValuesQuery", SqlDbType.NVarChar, -1)
                        {
                            Value =
                                              insertValuesQuery
                        };
                        command.Parameters.Add(dbparam);
                    }
                    #endregion
                }
                if (user == null)
                    user = new User(Authentication.GetCurrentUserId());

                var uparam = new SqlParameter("@CreatedBy", SqlDbType.BigInt)
                {
                    Value = user.Id
                };
                command.Parameters.Add(uparam);


                var logparam = new SqlParameter("@Comment", SqlDbType.NVarChar, 1000) { Value = Comment };
                command.Parameters.Add(logparam);

                logparam = new SqlParameter("@Type", SqlDbType.TinyInt) { Value = OperationTypes.Insert };
                command.Parameters.Add(logparam);

                if (boproperties != null && boproperties.LogRevisions)
                {
                    logparam = new SqlParameter("@LogRevisions", SqlDbType.Bit) { Value = 1 };
                    command.Parameters.Add(logparam);

                    logparam = new SqlParameter("@BOName", SqlDbType.NVarChar, 100) { Value = item.GetName() };
                    command.Parameters.Add(logparam);
                }
            }

            var objId = command.ExecuteScalar();

            item.Id = objId != null && objId != DBNull.Value ? Convert.ToInt64(objId) : default;

            #region Post process translations
            if (item.Id > 0)
            {
                foreach (AdvancedProperty property in properties)
                {
                    if (property.Translate != null && property.Translate.Translatable)
                    {
                        var pi = item.GetType().GetProperty(property.Translate.Alias);
                        var alias = pi.GetValue(item, null).ToString();
                        var o = property.PropertyDescriptor.GetValue(item);
                        if (o != null)
                        {
                            var value = o.ToString();

                            Translate.NewTranslation(connection, alias + "_" + item.Id.ToString(), value, System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);

                            property.PropertyDescriptor.SetValue(item, value);
                        }
                    }
                }
            }
            #endregion
        }

        public virtual RequestResult ActionInsert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            Insert(item, Comment, connection, user);

            return new RequestResult() { Message = "OK", Result = RequestResultType.Reload };
        }

        public virtual void CreateInsertParams(SqlCommand command, ItemBase itemparameter, out bool tocontinue)
        {
            tocontinue = true;
        }

        public virtual void CreateInsertParamQuery(AdvancedProperty property, out string paramName)
        {
            paramName = string.Empty;
        }

        public virtual object CreateInsertParamValues(ItemBase item, object param, out bool tocontinue)
        {
            tocontinue = true;
            return string.Empty;
        }
        #endregion

        #region Populate
        public ItemBase GetLast()
        {
            var cmd = new SqlCommand($"SELECT TOP(1) * FROM {this.GetType().Name} ORDER BY DateCreated DESC", DataBase.ConnectionFromContext());

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    this.FromDataRow(rdr);

                rdr.Close();
            }

            return this;
        }

        public static T GetLast<T>() where T: ItemBase, new()
        {
            var cmd = new SqlCommand($"SELECT TOP(1) * FROM {typeof(T).Name} ORDER BY DateCreated DESC", DataBase.ConnectionFromContext());

            var item = new T();

            using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                    item.FromDataRow(rdr);

                rdr.Close();
            }
            return item;
        }

        public virtual Dictionary<long, ItemBase> PopulateGridTools(SqlConnection conn,
                                                                ItemBase item,
                                                                int iPagingStart,
                                                                int iPagingLen,
                                                                string sSearch,
                                                                List<SortParameter> SortParameters,
                                                                bool ShowCanceled,
                                                                User sUser,
                                                                out long idisplaytotal)
        {
            if (SortParameters == null || (SortParameters != null && SortParameters.Count == 0))
                SortParameters = new List<SortParameter>
                {
                    new SortParameter() { Field = this.GetType().Name + "Id", Direction = "desc" }
                };

            return PopulateGeneric(conn,
                                 item,
                                 false,
                                 "",
                                 iPagingStart,
                                 iPagingLen,
                                 sSearch,
                                 SortParameters,
                                 ShowCanceled,
                                 sUser,
                                 false,
                                 false,
                                 out idisplaytotal,
                                 out _,
                                 this.GetType().Name);
        }

        public virtual Dictionary<long, ItemBase> PopulateReport(SqlConnection conn,
                                                                ItemBase item,
                                                                int iPagingStart,
                                                                int iPagingLen,
                                                                string sSearch,
                                                                List<SortParameter> SortParameters,
                                                                User sUser,
                                                                out long idisplaytotal,
                                                                out Dictionary<string, decimal> ColumsSum)
        {
            BoAttribute boproperties = null;
            if (this.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boproperties = (BoAttribute)this.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            var tableName = string.IsNullOrEmpty(boproperties.SpPopulateReport) is false ? boproperties.SpPopulateReport : this.GetType().Name;

            return PopulateGeneric(conn,
                                 item,
                                 false,
                                 "",
                                 iPagingStart,
                                 iPagingLen,
                                 sSearch,
                                 SortParameters,
                                 false,
                                 sUser,
                                 true,
                                 false,
                                 out idisplaytotal,
                                 out ColumsSum,
                                 tableName);
        }
        public virtual Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            return PopulateGeneric(null, null, false, "", 0, 0, search, null, false, null, false, false, out _, out _, this.GetType().Name);
        }

        public virtual Dictionary<long, ItemBase> Populate(List<SortParameter> SortParameters)
        {
            return PopulateGeneric(null, null, false, "", 0, 0, "", SortParameters, false, null, false, false, out _, out _, this.GetType().Name);
        }

        public virtual Dictionary<long, ItemBase> Populate(ItemBase item = null,
                                                                SqlConnection conn = null,
                                                                bool sortByName = false,
                                                                string AdvancedFilter = "",
                                                                bool ShowCanceled = false,
                                                                User sUser = null,
                                                                bool ignoreQueryFilter = false)
        {
            return PopulateGeneric(conn, item, sortByName, AdvancedFilter, 0, 0, "", null, ShowCanceled, sUser, false, ignoreQueryFilter, out _, out _, this.GetType().Name);
        }

        public virtual Dictionary<long, ItemBase> Populate(List<SortParameter> SortParameters,
                                                        ItemBase item = null,
                                                        SqlConnection conn = null,
                                                        bool sortByName = false,
                                                        string AdvancedFilter = "",
                                                        bool ShowCanceled = false,
                                                        User sUser = null)
        {
            if (SortParameters == null || (SortParameters != null && SortParameters.Count == 0))
                SortParameters = new List<SortParameter>
                {
                    new SortParameter() { Field = this.GetType().Name + "Id", Direction = "desc" }
                };

            return PopulateGeneric(conn, item, sortByName, AdvancedFilter, 0, 0, "", SortParameters, ShowCanceled, sUser, false, false, out _, out _, this.GetType().Name);
        }


        public virtual Dictionary<long, ItemBase> PopulateFrontEndItems()
        {
            return PopulateGeneric(null, null, false, "", 0, 0, "", null, false, null, false, false, out _, out _, this.GetType().Name);
        }

        public virtual Dictionary<long, ItemBase> PopulateByParent(ItemBase item = null,
                                                                SqlConnection conn = null,
                                                                bool sortByName = false,
                                                                string AdvancedFilter = "",
                                                                bool ShowCanceled = false,
                                                                User sUser = null)
        {
            return PopulateGeneric(conn, item, sortByName, AdvancedFilter, 0, 0, "", null, ShowCanceled, sUser, false, false, out _, out _, this.GetType().Name);
        }

        public Dictionary<long, ItemBase> PopulateGeneric(SqlConnection conn,
                                                                ItemBase item,
                                                                bool sortByName,
                                                                string AdvancedFilter,
                                                                int iPagingStart,
                                                                int iPagingLen,
                                                                string sSearch,
                                                                List<SortParameter> SortParameters,
                                                                bool ShowCanceled,
                                                                User sUser,
                                                                bool isReport,
                                                                bool ignoreQueryFilter,
                                                                out long idisplaytotal,
                                                                out Dictionary<string, decimal> ColumsSum,
                                                                string tableName)
        {
            if (conn == null)
                conn = DataBase.ConnectionFromContext();

            idisplaytotal = 0;

            BoAttribute boproperties = null;
            if (this.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boproperties = (BoAttribute)this.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            string strSql;
            var toUseGeneric = false;
            if (boproperties != null && !string.IsNullOrEmpty(boproperties.SpPopulate))
                strSql = boproperties.SpPopulate;
            else if (boproperties != null && (boproperties.UseCustomSp || boproperties.UseCustomSpPopulate))
                strSql = this.GetType().Name + "_PopulateTools";
            else
            {
                strSql = isReport ? "Generic_PopulateReport" : "Generic_PopulateTools";
                toUseGeneric = true;
            }

            if (sUser == null)
                sUser = Authentication.GetCurrentUser();

            if (!ignoreQueryFilter)
                AdvancedFilter += QueryFilter(sUser);

            var PreinitVar = "";
            if (boproperties != null && !string.IsNullOrEmpty(boproperties.DefaultFilter) && item == null && string.IsNullOrEmpty(sSearch))
            {
                AdvancedFilter += " AND " + boproperties.DefaultFilter + " ";
                PreinitVar = boproperties.PreinitVar;
            }

            var cmd = GetPopulateCommand(item,
                                             strSql,
                                             conn,
                                             sUser,
                                             toUseGeneric,
                                             sSearch,
                                             sortByName,
                                             ShowCanceled,
                                             SortParameters,
                                             AdvancedFilter,
                                             PreinitVar,
                                             isReport,
                                             boproperties,
                                             iPagingStart,
                                             iPagingLen,
                                             tableName);

            var items = new Dictionary<long, ItemBase>();

            var ordinal = 1;

            #region Read Data

            var ds = new DataSet();
            var da = new SqlDataAdapter
            {
                SelectCommand = cmd
            };
            da.Fill(ds);

            var dataRows = ds.Tables[0].Rows;

            foreach (DataRow dr in dataRows)
            {
                var databaseItem = (ItemBase)Activator.CreateInstance(this.GetType());
                if (isReport)
                    databaseItem.Id = ordinal;

                databaseItem = databaseItem.FromDataRow(dr);
                var addItem = true;
                if (databaseItem != null)
                {
                    var pss = new PropertySorter();
                    var pdc = TypeDescriptor.GetProperties(databaseItem.GetType());
                    var properties = pss.GetSearchProperties(pdc);

                    if ((from AdvancedProperty property in properties
                         where
                             (property.Translate != null && property.Translate.Translatable)
                             || (property.Encryption != null && property.Encryption.Encrypted)
                         let param = property.PropertyDescriptor.GetValue(databaseItem)
                         let sparam = property.PropertyDescriptor.GetValue(databaseItem)
                         where
                             !string.IsNullOrEmpty((string)sparam)
                             && !Strings.Like(param.ToString(), sparam.ToString())
                         select param).Any())
                        addItem = false;
                }

                if (addItem && boproperties != null && boproperties.CustomPage)
                {
                    if (ordinal >= iPagingStart)
                    {
                        if (iPagingLen <= 0)
                            break;

                        items.Add(databaseItem.Id, databaseItem);

                        iPagingLen--;
                    }
                    ordinal++;
                }
                else if (addItem)
                {
                    items.Add(databaseItem.Id, databaseItem);
                    ordinal++;
                }
            }

            if (isReport)
                ColumsSum = this.GetTotalColumSumReport(dataRows);
            else
            {
                ColumsSum = null;
            }

            if (boproperties != null && boproperties.CustomPage)
                idisplaytotal = dataRows.Count;
            else
                idisplaytotal = Convert.ToInt64(cmd.Parameters["@TotalDispalyValues"].Value);
            #endregion

            return items;
        }

        public virtual void CreatePopulateParamQuery(AdvancedProperty property, out string paramName) => paramName = string.Empty;

        public virtual ItemBase PopulateFrontEnd(string additional, ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            return PopulateOne(searchItem, ShowCanceled, sUser, conn);
        }

        public virtual ItemBase PopulateOne(ItemBase searchItem, bool ShowCanceled = false, User sUser = null, SqlConnection conn = null)
        {
            conn = conn ?? DataBase.ConnectionFromContext();

            BoAttribute boproperties = null;
            if (this.GetType().GetCustomAttributes(typeof(BoAttribute), true).Length > 0)
                boproperties = (BoAttribute)this.GetType().GetCustomAttributes(typeof(BoAttribute), true)[0];

            string strSql;
            var toUseGeneric = false;
            if (boproperties != null && !string.IsNullOrEmpty(boproperties.SpPopulate))
                strSql = boproperties.SpPopulate;
            else if (boproperties != null && (boproperties.UseCustomSp || boproperties.UseCustomSpPopulate))
                strSql = this.GetType().Name + "_PopulateTools";
            else
            {
                strSql = "Generic_PopulateTools";
                toUseGeneric = true;
            }

            sUser = sUser ?? Authentication.GetCurrentUser();

            var command = GetPopulateCommand(searchItem,
                                             strSql,
                                             conn,
                                             sUser,
                                             toUseGeneric,
                                             String.Empty,
                                             false,
                                             ShowCanceled,
                                             null,
                                             String.Empty,
                                             String.Empty,
                                             false,
                                             boproperties,
                                             0,
                                             0,
                                             this.GetType().Name);

            using (var rdr = command.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (rdr.Read())
                    return searchItem.FromDataRow(rdr);

                rdr.Close();
            }

            return null;
        }

        private SqlCommand GetPopulateCommand(ItemBase item,
                                                string strSql,
                                                SqlConnection conn,
                                                User sUser,
                                                bool toUseGeneric,
                                                string sSearch,
                                                bool sortByName,
                                                bool ShowCanceled,
                                                List<SortParameter> SortParameters,
                                                string AdvancedFilter,
                                                string PreinitVar,
                                                bool isReport,
                                                BoAttribute boproperties,
                                                int iPagingStart,
                                                int iPagingLen,
                                                string tableName)
        {
            var command = new SqlCommand(strSql, conn) { CommandType = CommandType.StoredProcedure };

            if (item != null && !toUseGeneric)
            {
                #region Populate Custom
                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(item.GetType());
                var properties = pss.GetSearchProperties(pdc);

                foreach (var dbparam in from AdvancedProperty property in properties
                                        let param = property.PropertyDescriptor.GetValue(item)
                                        where
                                            param != null
                                            && !string.IsNullOrEmpty(param.ToString())
                                            && (param as ItemBase != null)
                                            && (((ItemBase)param).Id != 0)
                                            && (!(param is DateTime) || (DateTime)param != DateTime.MinValue)
                                            && (property.Translate == null || !property.Translate.Translatable)
                                            && (property.Encryption == null || !property.Encryption.Encrypted)
                                        select new SqlParameter
                                        {
                                            SqlDbType = property.Db.ParamType,
                                            ParameterName = "@s" + property.Db.ParamName,
                                            Size = property.Db.ParamSize,
                                            Value = param as ItemBase != null ? ((ItemBase)param).Id : param
                                        })
                {
                    command.Parameters.Add(dbparam);
                }

                #endregion
            }

            if (toUseGeneric)
            {
                #region Populate Generic
                var dbparam = new SqlParameter("@TableName", SqlDbType.NVarChar, 400) { Value = tableName };
                command.Parameters.Add(dbparam);
                if (boproperties != null && boproperties.TimeOut > 0)
                {
                    command.CommandTimeout = boproperties.TimeOut;
                }
                var sortBy = string.Empty;

                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(this.GetType());
                var properties = pss.GetDbProperties(pdc, null);

                var joinQuery = string.Empty;
                var SimpleSearchQuery = string.Empty;
                var AdditionalJoin = string.Empty;

                if (!string.IsNullOrEmpty(sSearch))
                    SimpleSearchQuery = isReport ? " AND ( '' " : " AND ( CAST(t." + this.GetType().Name + "ID AS nvarchar(1000)) ";

                if (boproperties != null && !string.IsNullOrEmpty(boproperties.DefaultSimpleSearch))
                {
                    SimpleSearchQuery += boproperties.DefaultSimpleSearch;
                }
                dbparam = new SqlParameter("@SelectQuery", SqlDbType.NVarChar, -1)
                {
                    Value = isReport ? "" : "t." + this.GetType().Name + "ID,"
                };

                var fisrtProperty = "";

                if (boproperties != null)
                {
                    dbparam.Value += boproperties.DefaultQuery;
                }
                foreach (AdvancedProperty property in properties)
                {
                    string parameterNameForSort = "t.[" + property.Db.Prefix + property.Db.ParamName + "]";

                    this.CreatePopulateParamQuery(property, out string parameterName);

                    if (string.IsNullOrEmpty(parameterName))
                        parameterName = property.Db.ParamName;

                    if (property.Db.Sort != DbSortMode.None && !string.IsNullOrEmpty(parameterNameForSort))
                    {
                        if (!string.IsNullOrEmpty(sortBy))
                        {
                            sortBy += "," + parameterNameForSort + (property.Db.Sort == DbSortMode.Asc ? " asc" : " desc");
                        }
                        else
                        {
                            sortBy = " ORDER BY " + parameterNameForSort + (property.Db.Sort == DbSortMode.Asc ? " asc" : " desc");
                        }
                    }

                    if (property.Db._Populate)
                    {
                        if (property.Type == typeof(Graphic))
                        {
                            joinQuery += " LEFT JOIN Graphic " + parameterName.Replace("ID", string.Empty) + " ON t."
                                         + parameterName + "=" + parameterName.Replace("ID", string.Empty) + ".GraphicId ";
                            parameterName = "t." + parameterName + " AS " + property.Db.Prefix + property.PropertyName + "Id,"
                                             + parameterName.Replace("ID", string.Empty) + ".BOName AS "
                                             + property.Db.Prefix + property.PropertyName + "BOName,"
                                             + parameterName.Replace("ID", string.Empty) + ".Name AS "
                                             + property.Db.Prefix + property.PropertyName + "Name,"
                                             + parameterName.Replace("ID", string.Empty) + ".Ext AS "
                                             + property.Db.Prefix + property.PropertyName + "Ext";
                        }
                        else if (property.Type == typeof(Document))
                        {
                            joinQuery += " LEFT JOIN Document " + parameterName.Replace("ID", string.Empty) + " ON t."
                                         + parameterName + "=" + parameterName.Replace("ID", string.Empty) + ".DocumentId ";

                            parameterName = "t." + parameterName + " AS " + property.Db.Prefix + property.PropertyName + "Id,"
                                             + parameterName.Replace("ID", string.Empty) + ".Name AS "
                                             + property.Db.Prefix + property.PropertyName + "Name,"
                                             + parameterName.Replace("ID", string.Empty) + ".FileName AS "
                                             + property.Db.Prefix + property.PropertyName + "FileName,"
                                             + parameterName.Replace("ID", string.Empty) + ".Ext AS "
                                             + property.Db.Prefix + property.PropertyName + "Ext";
                        }
                        else if ((property.Type.BaseType == typeof(ItemBase)
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType == typeof(ItemBase))
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType == typeof(ItemBase))
                            ) && IsLoadedFromDB(property.Type))
                        {
                            if (property.Custom != null && (property.Custom is LookUp) && !((LookUp)property.Custom).JoinTable)
                                parameterName = "t." + parameterName + " AS " + property.Db.Prefix + parameterName;
                            else
                            {
                                var sub_item = (ItemBase)Activator.CreateInstance(property.Type);
                                joinQuery += " LEFT JOIN [" + property.Type.Name + "] [" + property.PropertyName + "] ON t." + property.PropertyName + "Id = [" + property.PropertyName + "]." + property.Type.Name + "Id";
                                joinQuery += sub_item.GetAdditionalJoinQuery(property);
                                if (!string.IsNullOrEmpty(sub_item.GetCaption()))
                                {
                                    parameterName = "t." + parameterName + " AS " + property.Db.Prefix + parameterName + ",[" + property.PropertyName + "]." + sub_item.GetCaption() + " AS " + property.Db.Prefix + property.PropertyName + sub_item.GetCaption() + sub_item.GetAdditionalSelectQuery(property);
                                }
                                else
                                {
                                    parameterName = "t." + parameterName + " AS " + property.Db.Prefix + parameterName;
                                }
                                if (string.IsNullOrEmpty(fisrtProperty))
                                    fisrtProperty = "t." + property.Db.ParamName;
                            }
                        }
                        else if (property.Type == typeof(Dictionary<long, ItemBase>) && (property.Custom is MultiCheck))
                        {
                            var sub_item_type = (property.Custom as MultiCheck).ItemType;

                            var LinktableName = ((MultiCheck)property.Custom).TableName;
                            if (string.IsNullOrEmpty(LinktableName))
                                LinktableName = this.GetType().Name + sub_item_type.Name;

                            joinQuery += " CROSS APPLY";
                            joinQuery += "(";
                            joinQuery += "SELECT CAST(" + property.PropertyName + "." + sub_item_type.Name + "Id as nvarchar(100)) + ';' ";
                            joinQuery += "FROM [" + LinktableName + "] [" + property.PropertyName + "]  ";
                            joinQuery += "WHERE [" + property.PropertyName + "]." + this.GetType().Name + "Id=t." + this.GetType().Name + "Id ";
                            joinQuery += "FOR XML PATH('') ";
                            joinQuery += ") [" + property.PropertyName + "] (" + property.PropertyName + "_list) ";

                            parameterName = "SUBSTRING([" + property.PropertyName + "]." + property.PropertyName + "_list,1, LEN([" + property.PropertyName + "]." + property.PropertyName + "_list) - 1) " + property.PropertyName;
                        }
                        else
                        {
                            parameterName = "t.[" + parameterName + "]";
                            if (string.IsNullOrEmpty(fisrtProperty))
                                fisrtProperty = parameterName;
                        }

                        if (!string.IsNullOrEmpty(sSearch))
                        {
                            if (property.Type == typeof(string) || property.Type == typeof(int) || property.Type == typeof(int?) || property.Type == typeof(decimal) || property.Type == typeof(decimal?) || property.Type == typeof(long) || property.Type == typeof(long?))
                            {
                                SimpleSearchQuery += " + '|' + COALESCE(CAST(" + parameterName + " as nvarchar(1000)),'')";
                            }
                            if ((property.Type.BaseType == typeof(ItemBase)
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType == typeof(ItemBase))
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType == typeof(ItemBase))
                            ) && IsLoadedFromDB(property.Type))
                            {
                                var sub_item = (ItemBase)Activator.CreateInstance(property.Type);
                                SimpleSearchQuery += "+ '|' + CAST(COALESCE([" + property.PropertyName + "]." + sub_item.GetCaption() + ",'') as nvarchar(1000))" + sub_item.GetAdditionalSimpleSearchQuery(property);
                            }
                        }

                        dbparam.Value += parameterName + ",";
                    }
                }

                dbparam.Value = dbparam.Value.ToString().Remove(dbparam.Value.ToString().Length - 1);
                command.Parameters.Add(dbparam);

                if (sortByName)
                    sortBy = " ORDER BY t." + this.GetCaption();

                if (SortParameters != null && SortParameters.Count > 0)
                {
                    sortBy = " ORDER BY ";

                    var ind = 0;
                    foreach (var col in SortParameters)
                    {
                        ind++;

                        sortBy += "t.[" + col.Field + "] " + col.Direction + (ind != SortParameters.Count ? ", " : "");
                    }
                }

                if (isReport && string.IsNullOrEmpty(sortBy))
                    sortBy = "Order by " + fisrtProperty;

                if (!string.IsNullOrEmpty(sortBy))
                {
                    dbparam = new SqlParameter("@SortBy", SqlDbType.NVarChar, -1) { Value = sortBy };
                    command.Parameters.Add(dbparam);
                }

                if (!string.IsNullOrEmpty(sSearch))
                {
                    SimpleSearchQuery += ") like '%'+@sSearch+'%'";

                    dbparam = new SqlParameter("@sSearch", SqlDbType.NVarChar, 100) { Value = sSearch };
                    command.Parameters.Add(dbparam);

                    dbparam = new SqlParameter("@SimpleSearchQuery", SqlDbType.NVarChar, -1) { Value = SimpleSearchQuery };
                    command.Parameters.Add(dbparam);
                }

                if (boproperties != null && !string.IsNullOrEmpty(boproperties.AfterPaginAdditionalQuery))
                {
                    dbparam = new SqlParameter("@AfterPaginAdditionalQuery", SqlDbType.NVarChar, -1) { Value = boproperties.AfterPaginAdditionalQuery };
                    command.Parameters.Add(dbparam);
                }

                if (item == null && boproperties != null && !string.IsNullOrEmpty(boproperties.AdditionalJoin))
                {
                    dbparam = new SqlParameter("@AdditionalJoin", SqlDbType.NVarChar, -1) { Value = boproperties.AdditionalJoin };
                    command.Parameters.Add(dbparam);
                }

                if (sUser != null && boproperties != null && boproperties.ReadAllAccess != 0 && !sUser.HasPermissions(boproperties.ReadAllAccess))
                {
                    dbparam = new SqlParameter("@sUser", SqlDbType.BigInt) { Value = sUser.Id };
                    command.Parameters.Add(dbparam);
                }

                if (isReport)
                {
                    dbparam = new SqlParameter("@AdditionalGroup", SqlDbType.NVarChar, -1) { Value = GetAdditionalGroupQuery() };
                    command.Parameters.Add(dbparam);
                }

                joinQuery += GetAdditionalJoinQuery();

                if (!string.IsNullOrEmpty(joinQuery))
                {
                    dbparam = new SqlParameter("@JoinQuery", SqlDbType.NVarChar, -1) { Value = joinQuery };
                    command.Parameters.Add(dbparam);
                }

                if (!string.IsNullOrEmpty(AdvancedFilter))
                {
                    dbparam = new SqlParameter("@AdvancedFilter", SqlDbType.NVarChar, -1) { Value = AdvancedFilter };
                    command.Parameters.Add(dbparam);
                }

                if (!string.IsNullOrEmpty(PreinitVar))
                {
                    dbparam = new SqlParameter("@PreinitVar", SqlDbType.NVarChar, -1) { Value = PreinitVar };
                    command.Parameters.Add(dbparam);
                }

                if (ShowCanceled)
                {
                    dbparam = new SqlParameter("@ShowCanceled", SqlDbType.Bit) { Value = ShowCanceled };
                    command.Parameters.Add(dbparam);
                }

                dbparam = new SqlParameter("@PagingStart", SqlDbType.Int) { Value = iPagingStart };
                command.Parameters.Add(dbparam);

                dbparam = new SqlParameter("@PagingLen", SqlDbType.Int) { Value = iPagingLen };
                command.Parameters.Add(dbparam);

                if (boproperties != null && boproperties.CustomPage)
                {
                    dbparam = new SqlParameter("@CustomPage", SqlDbType.Bit) { Value = boproperties.CustomPage };
                    command.Parameters.Add(dbparam);
                }

                dbparam = new SqlParameter("@TotalValues", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                command.Parameters.Add(dbparam);

                dbparam = new SqlParameter("@TotalDispalyValues", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                command.Parameters.Add(dbparam);

                #region Search Generic
                if (item != null)
                {
                    if (item.Id > 0)
                    {
                        dbparam = new SqlParameter("@sID", SqlDbType.BigInt) { Value = item.Id };
                        command.Parameters.Add(dbparam);
                    }
                    else
                    {
                        pss = new PropertySorter();
                        pdc = TypeDescriptor.GetProperties(this.GetType());
                        properties = pss.GetSearchProperties(pdc);

                        var searchParams = string.Empty;
                        var searchParamValues = string.Empty;
                        var parameterValues = string.Empty;
                        var insertToParameterValues = string.Empty;
                        var searchQuery = string.Empty;
                        var deepSort = string.Empty;
                        var applySortDefault = string.Empty;
                        var applySortDeep = string.Empty;

                        foreach (AdvancedProperty property in properties)
                        {
                            var searchQueryTemp = string.Empty;

                            if ((property.Translate != null && property.Translate.Translatable)
                                || (property.Encryption != null && property.Encryption.Encrypted)
                                || (property.PropertyName == "Id"))
                            {
                                continue;
                            }

                            var param = property.PropertyDescriptor.GetValue(item);

                            if (param == null
                                || string.IsNullOrEmpty(param.ToString())
                                || (param as ItemBase != null && (param as ItemBase).Id == 0 && (param as ItemBase).SearchItems == null)
                                || (param is bool && !Convert.ToBoolean(param))
                                || (param is int @int && @int == 0)
                                || (param is long @long && @long == 0)
                                || (param is Guid)
                                || (param is DateTime time && time == DateTime.MinValue)
                                || (param is DateRange dateRange && dateRange.From == DateTime.MinValue && dateRange.To == DateTime.MinValue)
                                || (param is NumbersRange numberRange && numberRange.From == 0 && numberRange.To == 0)
                                || (param is DecimalNumberRange decimalNumberRange && decimalNumberRange.From == 0 && decimalNumberRange.To == 0))
                            {
                                continue;
                            }

                            var parameterName = property.Db.ParamName;

                            if (param is DateRange range)
                            {
                                if (range.From != DateTime.MinValue)
                                {
                                    searchParams += parameterName + "_From;";
                                    parameterValues += parameterName + "_From datetime null,";

                                    insertToParameterValues += "DECLARE @" + parameterName + "_From datetime \n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "_From=convert(datetime,(SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "_From'), 120)  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + "_From)\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "_From',@" + parameterName + "_From)\n\r";

                                    var fromDate = range.From;
                                    if (fromDate.TimeOfDay.TotalSeconds == 0)
                                    {
                                        searchParamValues += fromDate.Date.ToString("yyyy-MM-dd") + ";";
                                    }
                                    else
                                    {
                                        searchParamValues += fromDate.ToString("yyyy-MM-dd HH:mm:ss") + ";";
                                    }
                                }

                                if (range.To != DateTime.MinValue)
                                {
                                    searchParams += parameterName + "_To;";
                                    parameterValues += parameterName + "_To datetime null,";

                                    insertToParameterValues += "DECLARE @" + parameterName + "_To datetime \n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "_To=convert(datetime,(SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "_To'), 120)  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + "_To)\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "_To',@" + parameterName + "_To)\n\r";

                                    var toDate = range.To;
                                    if (toDate.TimeOfDay.TotalSeconds == 0)
                                    {
                                        searchParamValues += toDate.AddDays(1).Date.AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + ";";
                                    }
                                    else
                                    {
                                        searchParamValues += toDate.ToString("yyyy-MM-dd HH:mm:ss") + ";";
                                    }

                                }
                                if (range.From != DateTime.MinValue && range.To != DateTime.MinValue)
                                {
                                    searchQueryTemp += " AND  t." + parameterName + " between (SELECT " + parameterName + "_From FROM #ParameterValues WHERE ParamName='" + parameterName + "_From') AND (SELECT " + parameterName + "_To FROM #ParameterValues WHERE ParamName='" + parameterName + "_To')\n\r";
                                }
                                else if (range.From != DateTime.MinValue)
                                {
                                    searchQueryTemp += " AND  t." + parameterName + " >= (SELECT " + parameterName + "_From FROM #ParameterValues WHERE ParamName='" + parameterName + "_From') \n\r";
                                }
                                else
                                {
                                    searchQueryTemp += " AND  t." + parameterName + " <= (SELECT " + parameterName + "_To FROM #ParameterValues WHERE ParamName='" + parameterName + "_To') \n\r";
                                }

                            }
                            else if (param is DecimalNumberRange decimal_range)
                            {
                                if (decimal_range.From != 0)
                                {
                                    searchParams += parameterName + "_From;";
                                    parameterValues += parameterName + "_From decimal(38, 2) null,";

                                    insertToParameterValues += "DECLARE @" + parameterName + "_From decimal(38, 2) \n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "_From=CAST((SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "_From') as decimal(38, 2))  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + "_From)\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "_From',@" + parameterName + "_From)\n\r";

                                    searchParamValues += decimal_range.From.ToString() + ";";
                                }

                                if (decimal_range.To != 0)
                                {
                                    searchParams += parameterName + "_To;";
                                    parameterValues += parameterName + "_To decimal(38, 2) null,";

                                    insertToParameterValues += "DECLARE @" + parameterName + "_To decimal(38, 2) \n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "_To=CAST((SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "_To') as decimal(38, 2))  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + "_To)\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "_To',@" + parameterName + "_To)\n\r";

                                    searchParamValues += decimal_range.To.ToString() + ";";
                                }
                                if (decimal_range.From != 0 && decimal_range.To != 0)
                                {
                                    searchQueryTemp += " AND t." + parameterName + " between (SELECT " + parameterName + "_From FROM #ParameterValues WHERE ParamName='" + parameterName + "_From') AND (SELECT " + parameterName + "_To FROM #ParameterValues WHERE ParamName='" + parameterName + "_To')\n\r";
                                }
                                else if (decimal_range.From != 0)
                                {
                                    searchQueryTemp += " AND t." + parameterName + " >= (SELECT " + parameterName + "_From FROM #ParameterValues WHERE ParamName='" + parameterName + "_From') \n\r";
                                }
                                else
                                {
                                    searchQueryTemp += " AND t." + parameterName + " <= (SELECT " + parameterName + "_To FROM #ParameterValues WHERE ParamName='" + parameterName + "_To') \n\r";
                                }

                            }
                            else if (param is NumbersRange number_range)
                            {
                                if (number_range.From != 0)
                                {
                                    searchParams += parameterName + "_From;";
                                    parameterValues += parameterName + "_From int null,";

                                    insertToParameterValues += "DECLARE @" + parameterName + "_From int \n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "_From=CAST((SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "_From') as int)  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + "_From)\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "_From',@" + parameterName + "_From)\n\r";

                                    searchParamValues += number_range.From.ToString() + ";";
                                }

                                if (number_range.To != 0)
                                {
                                    searchParams += parameterName + "_To;";
                                    parameterValues += parameterName + "_To int null,";

                                    insertToParameterValues += "DECLARE @" + parameterName + "_To int \n\r";
                                    insertToParameterValues += "SET  @" + parameterName + "_To=CAST((SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "_To') as int)  \n\r";
                                    insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + "_To)\n\r";
                                    insertToParameterValues += "VALUES('" + parameterName + "_To',@" + parameterName + "_To)\n\r";

                                    searchParamValues += number_range.To.ToString() + ";";
                                }
                                if (number_range.From != 0 && number_range.To != 0)
                                {
                                    searchQueryTemp += " AND t." + parameterName + " between (SELECT " + parameterName + "_From FROM #ParameterValues WHERE ParamName='" + parameterName + "_From') AND (SELECT " + parameterName + "_To FROM #ParameterValues WHERE ParamName='" + parameterName + "_To')\n\r";
                                }
                                else if (number_range.From != 0)
                                {
                                    searchQueryTemp += " AND t." + parameterName + " >= (SELECT " + parameterName + "_From FROM #ParameterValues WHERE ParamName='" + parameterName + "_From') \n\r";
                                }
                                else
                                {
                                    searchQueryTemp += " AND t." + parameterName + " <= (SELECT " + parameterName + "_To FROM #ParameterValues WHERE ParamName='" + parameterName + "_To') \n\r";
                                }

                            }
                            else if (param as ItemBase != null && (param as ItemBase).SearchItems != null)
                            {
                                searchQueryTemp += " AND t." + parameterName + " in (" + string.Join(",", (param as ItemBase).SearchItems.Values.Where(i => i.Id > 0).Select(p => p.Id.ToString())) + ")\n\r";
                            }
                            else
                            {
                                searchParams += parameterName + ";";
                                var collation = "";
                                if (!string.IsNullOrEmpty(Config.GetConfigValue("Collation")) && param is string)
                                    collation = " COLLATE " + Config.GetConfigValue("Collation");
                                parameterValues += parameterName + " " + property.Db.ParamType.ToString() + DataBase.GenerateParamSize(property.Db.ParamSize) + collation + " null,";

                                insertToParameterValues += "DECLARE @" + parameterName + " " + property.Db.ParamType + DataBase.GenerateParamSize(property.Db.ParamSize) + "\n\r";
                                insertToParameterValues += "SET  @" + parameterName + "=CAST((SELECT sv.value FROM dbo.Split(@SearchParams,';') sp INNER JOIN dbo.Split(@SearchParamValues,';') sv ON sp.Ident=sv.Ident WHERE sp.value='" + parameterName + "') AS " + property.Db.ParamType + DataBase.GenerateParamSize(property.Db.ParamSize) + ")  \n\r";
                                insertToParameterValues += "INSERT INTO #ParameterValues(ParamName," + parameterName + ")\n\r";
                                insertToParameterValues += "VALUES('" + parameterName + "',@" + parameterName + ")\n\r";

                                if (param is string)
                                {
                                    searchQueryTemp += " AND ((SELECT " + parameterName + " FROM #ParameterValues WHERE ParamName='" + parameterName + "') IS NULL OR t." + parameterName + " like '%'+(SELECT " + parameterName + " FROM #ParameterValues WHERE ParamName='" + parameterName + "') COLLATE Latin1_General_CI_AS +'%')\n\r";
                                }
                                else
                                {
                                    if (property.Custom != null && (property.Custom is LookUp lookUp) && !string.IsNullOrEmpty(lookUp.SearchQuery))
                                    {
                                        searchQueryTemp += $" AND ((SELECT {parameterName} FROM #ParameterValues WHERE ParamName='{parameterName}') IS NULL OR (SELECT {parameterName} FROM #ParameterValues WHERE ParamName='{parameterName}') IN {lookUp.SearchQuery})\n\r";
                                    }
                                    else
                                    {
                                        searchQueryTemp += $" AND ((SELECT {parameterName} FROM #ParameterValues WHERE ParamName='{parameterName}') IS NULL OR t.{parameterName}=(SELECT {parameterName} FROM #ParameterValues WHERE ParamName='{parameterName}'))\n\r";
                                    }
                                }

                                if (param as ItemBase != null)
                                {
                                    searchParamValues += (param as ItemBase).Id.ToString(CultureInfo.InvariantCulture) + ";";
                                }
                                else
                                {
                                    searchParamValues += param + ";";
                                }
                            }

                            if (property.Db.DeepFilter)
                            {
                                if (string.IsNullOrEmpty(deepSort))
                                    deepSort = $" WHERE 1 = 1 {searchQueryTemp}";
                                else
                                {
                                    deepSort += searchQueryTemp;
                                }

                                if (property.Db.ApplyFilter)
                                {
                                    applySortDeep += searchQueryTemp;
                                }
                            }
                            else
                            {
                                searchQuery += searchQueryTemp;

                                if (property.Db.ApplyFilter)
                                {
                                    applySortDefault += searchQueryTemp;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(searchParams))
                        {
                            dbparam = new SqlParameter("@SearchParams", SqlDbType.NVarChar, -1) { Value = searchParams };
                            command.Parameters.Add(dbparam);
                        }

                        if (!string.IsNullOrEmpty(deepSort))
                        {
                            dbparam = new SqlParameter("@GroupSort", SqlDbType.NVarChar, -1) { Value = deepSort };
                            command.Parameters.Add(dbparam);
                        }

                        if (boproperties != null && !string.IsNullOrEmpty(boproperties.AdditionalJoin))
                        {
                            if (!string.IsNullOrEmpty(applySortDefault))
                            {
                                boproperties.AdditionalJoin = boproperties.AdditionalJoin.Replace("1 = 1", "1 = 1" + applySortDefault);
                            }
                            if (!string.IsNullOrEmpty(applySortDeep))
                            {
                                boproperties.AdditionalJoin = boproperties.AdditionalJoin.Replace("2 = 2", "2 = 2" + applySortDeep);
                            }

                            dbparam = new SqlParameter("@AdditionalJoin", SqlDbType.NVarChar, -1) { Value = boproperties.AdditionalJoin };
                            command.Parameters.Add(dbparam);
                        }

                        if (!string.IsNullOrEmpty(searchParamValues))
                        {
                            dbparam = new SqlParameter("@SearchParamValues", SqlDbType.NVarChar, -1)
                            {
                                Value = searchParamValues
                            };
                            command.Parameters.Add(dbparam);
                        }

                        if (!string.IsNullOrEmpty(parameterValues))
                        {
                            parameterValues = parameterValues.Remove(parameterValues.Length - 1);

                            dbparam = new SqlParameter("@ParameterValues", SqlDbType.NVarChar, -1)
                            {
                                Value = parameterValues
                            };
                            command.Parameters.Add(dbparam);
                        }

                        if (!string.IsNullOrEmpty(insertToParameterValues))
                        {
                            dbparam = new SqlParameter("@InsertToParameterValues", SqlDbType.NVarChar, -1)
                            {
                                Value = insertToParameterValues
                            };
                            command.Parameters.Add(dbparam);
                        }

                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            dbparam = new SqlParameter("@SearchQuery", SqlDbType.NVarChar, -1) { Value = searchQuery };
                            command.Parameters.Add(dbparam);
                        }
                    }
                }
                #endregion

                #endregion
            }

            return command;
        }

        public virtual object PopulatePrintData() => this;

        public virtual Dictionary<string, decimal> GetTotalColumSumReport(DataRowCollection dataRows)
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this.GetType());
            var properties = pss.GetTotalColumSumProperties(pdc);

            var Colums = new Dictionary<string, decimal>();

            foreach (AdvancedProperty property in properties)
            {
                Colums.Add(property.Db.ParamName, default);
            }

            foreach (DataRow dr in dataRows)
            {
                foreach (AdvancedProperty property in properties)
                {
                    var Key = property.Db.ParamName;

                    Colums[Key] += dr[Key] == DBNull.Value ? default : Convert.ToDecimal(dr[Key]);
                }
            }

            return Colums;
        }
        #endregion

        #region Form
        public virtual void CollectFromForm(string prefix = "")
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(this);
            var properties = pss.GetFormSaveProperties(pdc, Authentication.GetCurrentUser());

            foreach (AdvancedProperty property in properties)
                property.PropertyDescriptor.SetValue(this, property.GetDataProcessor().GetValue(property, prefix));
        }

        public virtual void ManyCollectFromForm(int index, string prefix = "") { }

        public virtual RequestResult SaveForm()
        {
            if (this.Id > 0)
            {
                this.Update(this);

                return new RequestResult() { Result = RequestResultType.Reload, RedirectURL = URLHelper.GetUrl("DocControl/" + this.GetType().Name + "/" + Id.ToString()), Message = "Обновлено" };
            }

            this.Insert(this);

            return new RequestResult() { Result = RequestResultType.Reload, RedirectURL = URLHelper.GetUrl("DocControl/" + this.GetType().Name + "/" + Id.ToString()), Message = this.GetType().Name + " создан" };
        }

        public virtual string ActionRedirect() => string.Empty;

        public virtual string LoadPageTitle() => "";

        public virtual string QueryFilter(User User) => "";

        public virtual bool DefaultReportFilter(bool is_search) => is_search;

        public virtual string SimpleSearch(string sSearch) => sSearch;

        public virtual List<LinkModel> LoadBreadcrumbs(string viewModel = default) => new List<LinkModel>();

        public virtual Dictionary<string, object> LoadFrontEndViewdata()
            => new Dictionary<string, object>
            {
                { "ParentId", Convert.ToInt64(HttpContext.Current.Request.Form["ParentId"]) }
            };


        public virtual object LoadPopupData(long itemId) => this;
        #endregion

        #region FromDataRow
        public ItemBase FromDataRow(IDataReader dr)
        {
            return this.FromDataRow(dr, null);
        }

        public ItemBase FromDataRow(DataRow dr)
        {
            return this.FromDataRow(null, dr);
        }

        public ItemBase FromDataRow(IDataReader rdr, DataRow dr)
        {
            return this.FromDataRow(rdr, dr, string.Empty);
        }

        public ItemBase FromDataRow(IDataReader rdr, DataRow dr, string prefix)
        {
            this.FromDataRow(rdr, dr, prefix, out bool tocontinue);
            if ((tocontinue && (DataBase.ReaderValueExist(rdr, dr, prefix + this.GetType().Name + "Id") || DataBase.ReaderValueExist(rdr, dr, prefix + "Id"))) || Id > 0)
            {
                this.SetId(DataBase.GetExistReaderValue(rdr, dr, prefix + this.GetType().Name + "Id", DataBase.GetExistReaderValue(rdr, dr, prefix + "Id", this.GetId())));
                this.SetName(DataBase.GetExistReaderValue(rdr, dr, prefix + this.GetCaption(), null));
                this.SetNameAddl(rdr, dr, prefix);
                var pss = new PropertySorter();
                var pdc = TypeDescriptor.GetProperties(this.GetType());
                var properties = pss.GetDbReadProperties(pdc, null);

                foreach (AdvancedProperty property in properties)
                {
                    if (property.Db.Readable == null || property.Db.Readable == true)
                    {
                        if ((property.Type != null && property.Type.BaseType == typeof(ItemBase))
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType == typeof(ItemBase))
                            || (property.Type.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType != null && property.Type.BaseType.BaseType.BaseType == typeof(ItemBase)))
                        {
                            if (property.Db.ReadableOnlyName == null || property.Db.ReadableOnlyName == false)
                            {
                                if (property.Db.Prefix + property.PropertyName != prefix || string.IsNullOrEmpty(prefix))
                                {
                                    property.PropertyDescriptor.SetValue(
                                        this,
                                        ((ItemBase)
                                         property.Type.InvokeMember(
                                             string.Empty, BindingFlags.CreateInstance, null, null, new object[0])).FromDataRow(
                                                 rdr, dr, property.Db.Prefix + property.PropertyName));
                                }
                            }
                            else
                            {
                                var item = ((ItemBase)
                                     property.Type.InvokeMember(
                                         string.Empty, BindingFlags.CreateInstance, null, null, new object[0]));

                                var value = DataBase.GetExistReaderValue(
                                    rdr, dr, prefix + property.Db.ParamName, property.PropertyDescriptor.GetValue(this));

                                item.Id = Convert.ToInt64(value);

                                var user = item as User;

                                if (user != null)
                                {
                                    user.Login = Convert.ToString(DataBase.GetExistReaderValue(rdr, dr, prefix + "CreatedBylogin", property.PropertyDescriptor.GetValue(this)));
                                }

                                property.PropertyDescriptor.SetValue(
                                    this,
                                   item);
                            }
                        }
                        else if (property.Type != typeof(Dictionary<long, ItemBase>))
                        {
                            var value = DataBase.GetExistReaderValue(
                                rdr, dr, prefix + property.Db.ParamName, property.PropertyDescriptor.GetValue(this));

                            if (property.Translate != null && property.Translate.Translatable)
                            {
                                var pi = this.GetType().GetProperty(property.Translate.Alias);
                                if (pi != null)
                                {
                                    pi.SetValue(this, value, null);
                                }
                                if (!string.IsNullOrEmpty((string)value))
                                {
                                    property.PropertyDescriptor.SetValue(
                                        this, Translate.GetTranslatedValueFromDb(value.ToString()));
                                }
                            }
                            else if (property.Type == typeof(NumbersRange))
                            {
                                value = new NumbersRange()
                                {
                                    From = value != null ? Convert.ToInt32(value.ToString()) : 0
                                };
                                property.PropertyDescriptor.SetValue(this, value);
                            }
                            else if (property.Type == typeof(DecimalNumberRange))
                            {
                                value = new DecimalNumberRange()
                                {
                                    From = value != null ? Convert.ToDecimal(value.ToString()) : 0
                                };
                                property.PropertyDescriptor.SetValue(this, value);
                            }
                            else if (property.Type == typeof(DateRange))
                            {
                                value = new DateRange()
                                {
                                    From = value != null ? Convert.ToDateTime(value.ToString()) : DateTime.MinValue
                                };
                                property.PropertyDescriptor.SetValue(this, value);
                            }
                            else if (property.Encryption != null && property.Encryption.Encrypted)
                            {
                                if (value is DateTime)
                                    value = Convert.ToDateTime(Crypt.Decrypt(value.ToString(), ConfigurationManager.AppSettings["CryptKey"]));
                                else if (value is int)
                                    value = Convert.ToInt32(Crypt.Decrypt(value.ToString(), ConfigurationManager.AppSettings["CryptKey"]));
                                else if (value is long)
                                    value = Convert.ToInt64(Crypt.Decrypt(value.ToString(), ConfigurationManager.AppSettings["CryptKey"]));
                                else if (value is decimal)
                                    value = Convert.ToDecimal(Crypt.Decrypt(value.ToString(), ConfigurationManager.AppSettings["CryptKey"]));
                                else if (value != null && !string.IsNullOrEmpty(value.ToString()))
                                    value = Crypt.Decrypt(value.ToString(), ConfigurationManager.AppSettings["CryptKey"]);

                                property.PropertyDescriptor.SetValue(this, value);
                            }
                            else if (property.Type == typeof(string) && value != DBNull.Value && value != null)
                            {
                                value = value.ToString().Replace("|", ";");
                                property.PropertyDescriptor.SetValue(this, value);
                            }
                            else
                            {
                                property.PropertyDescriptor.SetValue(this, value);
                            }
                        }
                        else if (property.Custom is MultiCheck)
                        {
                            var value = (string)DataBase.GetExistReaderValue(rdr, dr, prefix + property.Db.ParamName, "");
                            if (!string.IsNullOrEmpty(value))
                            {
                                var list = new Dictionary<long, ItemBase>();
                                foreach (var sId in value.Split(';'))
                                {
                                    var lId = Convert.ToInt64(sId);
                                    var lItem = (ItemBase)Activator.CreateInstance((property.Custom as MultiCheck).ItemType);
                                    lItem.Id = lId;
                                    if (!list.ContainsKey(lId))
                                        list.Add(lId, lItem);
                                }
                                property.PropertyDescriptor.SetValue(this, list);
                            }
                        }
                    }
                }

                this.AfterFromDataRow(rdr, dr, prefix);
            }

            return this;
        }

        public virtual void FromDataRow(IDataReader rdr, DataRow dr, string prefix, out bool tocontinue) => tocontinue = true;

        public virtual void AfterFromDataRow(IDataReader rdr, DataRow dr, string prefix) { }
        #endregion

        #endregion

        #region Security
        public virtual bool CheckAutocompleteSecurity() => true;

        public virtual bool HaveAccess(string fullModel = default, string Id = default) => true;

        public virtual bool HaveSaveAccess(string fullModel = default, string Id = default) => HaveAccess(fullModel, Id);
        #endregion

        public override string ToString() => this.GetName();
    }
}