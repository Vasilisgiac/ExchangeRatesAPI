using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ExchangeRates.Models
{
    [DataContract]
    public class Error
    {
        [DataMember]
        public int statusCode { get; set; }
        
        [DataMember]
        public string message { get; set; }

        public Error() 
        {
            
        }
    }
}
