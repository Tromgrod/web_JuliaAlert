using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace JuliaAlert.Models.Objects
{
    public class CurrencyInfoRare
    {
        public CurrencyInfoRare(decimal value, DateTime date)
        {
            Value = value;
            DateExchangeRate = date;
        }

        public decimal Value { get; }
        public DateTime DateExchangeRate { get; }
    }

    public class ExchangeRate
    {
        private readonly static string ExchangeRateAPI = ConfigurationManager.AppSettings.Get("ExchangeRateAPI");

        private readonly static ConcurrentDictionary<Currency.NBM_Id, CurrencyInfoRare> ExchangeRates = new ConcurrentDictionary<Currency.NBM_Id, CurrencyInfoRare>();

        public static readonly ExchangeRate Instance = new ExchangeRate();

        public CurrencyInfoRare this[Currency.NBM_Id key]
        {
            get
            {
                var date = DateTime.Today;

                if (ExchangeRates.TryGetValue(key, out var currencyInfoRare))
                {
                    if (currencyInfoRare.Value == default || currencyInfoRare.DateExchangeRate.Date != date)
                    {
                        var rate = GetExchangeRateAsync(key, date).Result;

                        currencyInfoRare = new CurrencyInfoRare(rate, date);

                        ExchangeRates[key] = currencyInfoRare;
                    }
                }
                else
                {
                    var rate = GetExchangeRateAsync(key, date).Result;

                    currencyInfoRare = new CurrencyInfoRare(rate, date);

                    ExchangeRates.TryAdd(key, currencyInfoRare);
                }

                return currencyInfoRare;
            }
            set => ExchangeRates[key] = value;
        }

        public static decimal GetReverseCourse(decimal rate) => 1 / (rate / 1);

        private static decimal GetExchangeRate(Currency.NBM_Id currencyId, DateTime date)
        {
            var httpClient = new HttpClient();

            var apiUrl = ExchangeRateAPI.Replace("{date}", date.ToString("dd.MM.yyyy"));
            var response = httpClient.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(content);

                var currencyNode = xmlDoc.SelectSingleNode($"//Valute[@ID='{(int)currencyId}']");

                var currencyDenominationNode = currencyNode.SelectSingleNode("Nominal");
                var currencyValNode = currencyNode.SelectSingleNode("Value");

                var currencyDenomination = Convert.ToDecimal(currencyDenominationNode.InnerText, CultureInfo.InvariantCulture);
                var currencyVal = Convert.ToDecimal(currencyValNode.InnerText, CultureInfo.InvariantCulture);

                var rate = currencyVal / currencyDenomination;

                return rate;
            }

            return default;
        }

        private static Task<decimal> GetExchangeRateAsync(Currency.NBM_Id currencyId, DateTime date)
        {
            return Task.Run(() => GetExchangeRate(currencyId, date));
        }
    }
}