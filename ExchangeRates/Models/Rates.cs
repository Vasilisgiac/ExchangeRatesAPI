using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ExchangeRates.Models
{
    [DataContract]
    public class Rates
    {
        [DataMember]
        public string Base { get; set; }
        [DataMember]
        public string Time { get; set; }
        [DataMember]
        public IEnumerable<Currency> Currencies{ get; set; }

        [DataContract(Name = "Currency")]
        public class Currency
        { 
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public decimal Rate { get; set; }
        }

        public Rates() 
        {
            
        }

        public Rates(string time, IEnumerable<Currency> currencies) 
        {
            Base = "EUR";
            Time = time;
            Currencies = currencies;
        }

        public Rates(string currencybase, string time, IEnumerable<Currency> currencies) 
        {
            Base = currencybase;
            Time = time;
            Currencies = currencies;
        }

        public override string ToString() => JsonSerializer.Serialize<Rates>(this);
    }
}
