using LIB.AdvancedProperties;
using LIB.Tools.BO;
using LIB.Tools.Controls;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace JuliaAlertLib.BusinessObjects
{
    public abstract class ReportBase : ModelBase
    {
        public static string GetHiddenInput(string name, object value) => $"<input type=\"hidden\" name=\"{name}\" value=\"{value}\">";

        public virtual string GetHiddenInputs() => string.Empty;

        public virtual string GetConditionalClass() => string.Empty;

        public virtual string GetConditionalStyle() => string.Empty;

        public virtual string GetBtnHeaderLink() => string.Empty;

        public virtual string AdditionalName() => string.Empty;

        public virtual ReportBase GetSearchItem(ReportBase Item) => Item;

        public static void SetSearchProperties(ref SqlCommand cmd, in ItemBase searchItem)
        {
            var pss = new PropertySorter();
            var pdc = TypeDescriptor.GetProperties(searchItem.GetType());
            var searchProperties = pss.GetSearchProperties(pdc);

            foreach (AdvancedProperty searchProperty in searchProperties)
            {
                var value = searchProperty.PropertyDescriptor.GetValue(searchItem);

                if (value != null)
                {
                    if (value is ItemBase item && item.Id > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(item.Id), SqlDbType.BigInt) { Value = item.Id });
                    }
                    if (value is bool)
                    {
                        cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName, SqlDbType.Bit) { Value = value });
                    }
                    if (value is string @string && string.IsNullOrEmpty(@string) is false)
                    {
                        cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName, SqlDbType.NVarChar, 100) { Value = @string });
                    }
                    if (value is long @long && @long != 0)
                    {
                        cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName, SqlDbType.BigInt) { Value = @long });
                    }
                    if (value is int @int && @int != 0)
                    {
                        cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName, SqlDbType.Int) { Value = @int });
                    }
                    if (value is DateTime dateTime && dateTime != default)
                    {
                        cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName, SqlDbType.DateTime) { Value = dateTime });
                    }
                    if (value is DateRange dateRange)
                    {
                        if (dateRange.From != default)
                            cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(dateRange.From), SqlDbType.DateTime) { Value = dateRange.From });
                        if (dateRange.To != default)
                            cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(dateRange.To), SqlDbType.DateTime) { Value = dateRange.To });
                    }
                    if (value is NumbersRange numbersRange)
                    {
                        if (numbersRange.From != default)
                            cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(numbersRange.From), SqlDbType.Int) { Value = numbersRange.From });
                        if (numbersRange.To != default)
                            cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(numbersRange.To), SqlDbType.Int) { Value = numbersRange.To });
                    }
                    if (value is DecimalNumberRange decimalNumberRange)
                    {
                        if (decimalNumberRange.From != default)
                            cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(decimalNumberRange.From), SqlDbType.Decimal) { Value = decimalNumberRange.From });
                        if (decimalNumberRange.To != default)
                            cmd.Parameters.Add(new SqlParameter(searchProperty.PropertyName + nameof(decimalNumberRange.To), SqlDbType.Decimal) { Value = decimalNumberRange.To });
                    }
                }
            }
        }
    }
}