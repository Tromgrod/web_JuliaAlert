using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.AdminArea;
using LIB.Tools.BO;
using LIB.Tools.Security;
using LIB.Tools.Utils;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Order
   , ModulesAccess = (long)(Modulesenum.ControlPanel)
   , DisplayName = "Клиенты"
   , SingleName = "Клиент"
   , DoCancel = true
   , LogRevisions = true)]
    public class Client : ItemBase
    {

        #region Constructors
        public Client()
            : base(0) { }

        public Client(long id)
            : base(id) { }

        public Client(string name, SqlConnection conn = null)
             : base(name, throwException: false, conn: conn) => Name = name;
        #endregion

        #region Properties
        [Common(DisplayName = "Имя"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Страна"), Template(Mode = Template.ParentDropDown)]
        public Countries Countries { get; set; }

        [Common(DisplayName = "Штат"), Template(Mode = Template.ParentDropDown)]
        public State State { get; set; }

        [Common(DisplayName = "Город"), Template(Mode = Template.ParentDropDown)]
        public City City { get; set; }

        [Common(DisplayName = "Почтовый индекс"), Template(Mode = Template.String)]
        public string Index { get; set; }

        [Common(DisplayName = "Адрес"), Template(Mode = Template.String)]
        public string Address { get; set; }

        [Common(DisplayName = "Телефон"), Template(Mode = Template.VisibleString)]
        public string Phone { get; set; }

        [Common(DisplayName = "Комментарий"), Template(Mode = Template.String)]
        public string Comment { get; set; }

        [Common(DisplayName = "Email"), Template(Mode = Template.Email)]
        public string Email { get; set; }

        [Common(DisplayName = "Скидка"), Template(Mode = Template.Number)]
        public int Discount { get; set; }

        [Common(DisplayName = "День рождение"), Template(Mode = Template.Date)]
        public DateTime Birthday { get; set; }

        [Db(_Ignore = true)]
        public string BirthdayString => Birthday != DateTime.MinValue ? Birthday.ToString("dd/MM/yyyy") : string.Empty;
        #endregion

        public override object LoadPopupData(long itemId) => PopulateById(itemId);

        public override Dictionary<long, ItemBase> PopulateAutocomplete(string Param, string search, string AdvancedFilter = "")
        {
            var cmd = new SqlCommand("Client_PopulateAutocomplete", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@search", SqlDbType.NVarChar, -1) { Value = search });

            if (string.IsNullOrEmpty(Param) is false)
                cmd.Parameters.Add(new SqlParameter("@param", SqlDbType.NVarChar, -1) { Value = Param });

            var clients = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var client = (Client)new Client().FromDataRow(dr);
                    clients.Add(client.Id, client);
                }
                dr.Close();
            }

            return clients;
        }

        public long GetIdByName()
        {
            var cmd = new SqlCommand($"SELECT ClientId FROM Client WHERE DeletedBy IS NULL AND Name = N'{this.Name}'", DataBase.ConnectionFromContext());

            long clientId = default;

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (dr.Read())
                {
                    var idObj = dr["ClientId"];
                    clientId = idObj != DBNull.Value ? Convert.ToInt64(idObj) : default;
                }

                dr.Close();
            }

            return clientId;
        }

        public static Client PopulateById(long clientId, SqlConnection conn = null)
        {
            var cmd = new SqlCommand("Client_Populate_One", conn ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ClientId", SqlDbType.BigInt) { Value = clientId });

            var ds = new DataSet();
            var da = new SqlDataAdapter
            {
                SelectCommand = cmd
            };
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
                return (Client)new Client().FromDataRow(ds.Tables[0].Rows[0]);
            else
                return new Client();
        }

        public bool HasOrder()
        {
            var result = new SqlCommand($"SELECT CASE WHEN EXISTS(SELECT c.ClientId FROM Client c JOIN [Order] o ON o.ClientId = c.ClientId WHERE c.ClientId = {this.Id} AND o.DeletedBy IS NULL AND c.DeletedBy IS NULL) THEN 1 ELSE 0 END", DataBase.ConnectionFromContext()).ExecuteScalar();

            return result != DBNull.Value && Convert.ToBoolean(result);
        }

        public Currency GetCurrency()
        {
            var cmd = new SqlCommand("Currency_Populate_ByClient", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("ClientId", SqlDbType.BigInt) { Value = this.Id });

            var ds = new DataSet();
            var da = new SqlDataAdapter
            {
                SelectCommand = cmd
            };
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            else if (ds.Tables[0].Rows.Count > 1)
            {
                throw new Exception("У клиента много валют!");
            }

            return (Currency)new Currency().FromDataRow(ds.Tables[0].Rows[0]);
        }

        public override void CollectFromForm(string prefix = "")
        {
            base.CollectFromForm(prefix);

            var formName = HttpContext.Current.Request.Form[nameof(Client) + "_autocomplete"];

            if (this.Id <= 0 && !string.IsNullOrEmpty(formName))
                this.Name = formName;
        }

        public override void Insert(ItemBase item, string Comment = "Created", SqlConnection connection = null, User user = null)
        {
            var client = (Client)item;

            var cmd = new SqlCommand("Client_Insert", connection ?? DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var currentUser = Authentication.GetCurrentUser();

            cmd.Parameters.Add(new SqlParameter("Name", SqlDbType.NVarChar, 100) { Value = client.Name });

            if (client?.Countries != null && client.Countries.Id > 0)
                cmd.Parameters.Add(new SqlParameter("CountriesId", SqlDbType.BigInt) { Value = client.Countries.Id });

            if (client?.State != null && client.State.Id > 0)
                cmd.Parameters.Add(new SqlParameter("StateId", SqlDbType.BigInt) { Value = client.State.Id });

            if (client?.City != null && client.City.Id > 0)
                cmd.Parameters.Add(new SqlParameter("CityId", SqlDbType.BigInt) { Value = client.City.Id });

            if (string.IsNullOrEmpty(client.Index) is false)
                cmd.Parameters.Add(new SqlParameter("Index", SqlDbType.NVarChar, 100) { Value = client.Index });

            if (string.IsNullOrEmpty(client.Address) is false)
                cmd.Parameters.Add(new SqlParameter("Address", SqlDbType.NVarChar, 200) { Value = client.Address });

            if (string.IsNullOrEmpty(client.Phone) is false)
                cmd.Parameters.Add(new SqlParameter("Phone", SqlDbType.NVarChar, 100) { Value = client.Phone });

            if (string.IsNullOrEmpty(client.Comment) is false)
                cmd.Parameters.Add(new SqlParameter("Comment", SqlDbType.NVarChar, -1) { Value = client.Comment });

            cmd.Parameters.Add(new SqlParameter("Discount", SqlDbType.Int) { Value = client.Discount });

            if (client.Birthday != default)
                cmd.Parameters.Add(new SqlParameter("Birthday", SqlDbType.DateTime) { Value = client.Birthday });

            if (string.IsNullOrEmpty(client.Email) is false)
                cmd.Parameters.Add(new SqlParameter("Email", SqlDbType.NVarChar, 100) { Value = client.Email });

            cmd.Parameters.Add(new SqlParameter("CreatedBy", SqlDbType.BigInt) { Value = currentUser.Id });

            var idObj = cmd.ExecuteScalar();

            this.Id = idObj != DBNull.Value ? Convert.ToInt64(idObj) : 0;
        }
    }
}