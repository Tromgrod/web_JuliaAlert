using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;
using System.Collections.Generic;
using System.Data.SqlClient;
using LIB.Tools.Utils;
using System.Data;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Production
       , ModulesAccess = (long)(Modulesenum.ControlPanel)
       , DisplayName = "Виды фунитуры"
       , SingleName = "Вид фурнитуры"
       , DoCancel = true
       , LogRevisions = true)]
    public class FindingSpecie : ItemBase
    {
        #region Constructors
        public FindingSpecie()
            : base(0) { }

        public FindingSpecie(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Название"), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(DisplayName = "Код"), Template(Mode = Template.VisibleString)]
        public string Code { get; set; }
        #endregion

        public static Dictionary<long, ItemBase> GetSubspecieList(long findingSpecieId)
        {
            var cmd = new SqlCommand("FindingSpecie_GetSubspecieList", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("FindingSpecieId", SqlDbType.BigInt) { Value = findingSpecieId });

            var findingSubspecies = new Dictionary<long, ItemBase>();

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
                while (dr.Read())
                {
                    var findingSubspecie = (FindingSubspecie)new FindingSubspecie().FromDataRow(dr);
                    findingSubspecies.Add(findingSubspecie.Id, findingSubspecie);
                }

                dr.Close();
            }

            return findingSubspecies;
        }
    }
}