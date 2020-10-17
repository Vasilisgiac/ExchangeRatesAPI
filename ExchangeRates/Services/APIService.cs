using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRates.Models;
using Microsoft.AspNetCore.Hosting;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace ExchangeRates.Services
{
    public class APIService
    {
        public IWebHostEnvironment WebHostEnvironment { get; }
        public APIService(IWebHostEnvironment webHostEnvironment) 
        {
            WebHostEnvironment = webHostEnvironment;
        }

        //Get instructions for API
        public Default GetDefault(string host) 
        {
            Default details = new Default();
            details.Details = host;
            return details;
        }

        //Parsing XML data to object (type Rates)
        public IEnumerable<Rates> ParsingData(bool latest)
        {
            DataFile DataXml = new DataFile();
            Rates DataRates;
            IEnumerable<Rates>ListDataRates = Enumerable.Empty<Rates>(); ;

            if (latest == true)
                DataXml = DataXml.XmlToDataFile();
            else
                DataXml = DataXml.XmlToDataFileHistory();

            for (int j = 0; j < DataXml.CubeRootEl.Count(); j++)
            { 
                var time = DataXml.CubeRootEl.ElementAtOrDefault(j).Time;
                IEnumerable<Rates.Currency> currencies = Enumerable.Empty<Rates.Currency>();
                for (int i = 0; i < DataXml.CubeRootEl.ElementAtOrDefault(j).CubeItems.Count(); i++)
                {
                    Rates.Currency currency = new Rates.Currency();
                    currency.Name = DataXml.CubeRootEl.ElementAtOrDefault(j).CubeItems.ElementAtOrDefault(i).Currency;
                    currency.Rate = DataXml.CubeRootEl.ElementAtOrDefault(j).CubeItems.ElementAtOrDefault(i).Rate;

                    currencies = currencies.Append(currency);
                }

                DataRates = new Rates(time, currencies);
                ListDataRates = ListDataRates.Append(DataRates);
            }
            return ListDataRates;
        }

        //Changing the base currency of the records
        public IEnumerable<Rates> CalculateRates(IEnumerable<Rates> listdatarates, string currency)
        {
            
            IEnumerable <Rates> Listdatarates = Enumerable.Empty<Rates>();

            if (currency == "EUR" || currency == null)
                return listdatarates;
            else
            {
                for (int a = 0; a < listdatarates.Count(); a++)
                {
                    Rates datarates = new Rates();
                    datarates = listdatarates.ElementAtOrDefault(a);
                    for (int i = 0; i < datarates.Currencies.Count(); i++)
                    {
                        if (datarates.Currencies.ElementAtOrDefault(i).Name == currency)
                        {
                            var temp = datarates.Currencies.ElementAtOrDefault(i).Name;
                            datarates.Currencies.ElementAtOrDefault(i).Name = datarates.Base;
                            datarates.Base = temp;
                            datarates.Currencies.ElementAtOrDefault(i).Rate = Decimal.Round(1 / datarates.Currencies.ElementAtOrDefault(i).Rate, 10);
                            for (int j = 0; j < datarates.Currencies.Count(); j++)
                            {
                                if (i != j)
                                {
                                    datarates.Currencies.ElementAtOrDefault(j).Rate = Decimal.Round(datarates.Currencies.ElementAtOrDefault(i).Rate * datarates.Currencies.ElementAtOrDefault(j).Rate, 10);
                                }
                            }
                            break;
                        }
                    }
                    Listdatarates = Listdatarates.Append(datarates);
                }
                for(int i=0; i < Listdatarates.Count(); i++)
                { 
                    if (Listdatarates.ElementAtOrDefault(i).Base == "EUR")
                        throw new Exception($"Base '{currency}' is invalid. Maybe it is lowercase or not supported");
                }
                
                return Listdatarates;
                    
            }  
        }

        //Filtering exchange rates records to get a specific timespan
        public IEnumerable<Rates> FindDates(IEnumerable<Rates> listdatarates, string startdate, string enddate) 
        {
            if (string.IsNullOrEmpty(startdate))
            { throw new Exception("startdate parameter is missing or no value was assigned"); }
            if (string.IsNullOrEmpty(enddate))
            { throw new Exception("enddate parameter is missing or no value was assigned"); }
            DateTime startDate;
            DateTime endDate;
            if (!DateTime.TryParseExact(startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            { throw new Exception($"{startdate} does not match 'yyyy - MM - dd' format"); }
            else if (!DateTime.TryParseExact(enddate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            { throw new Exception($"{enddate} does not match 'yyyy - MM - dd' format"); }
            else if (DateTime.ParseExact(startdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None) > DateTime.ParseExact(enddate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None)) 
            { throw new Exception($"{enddate} should be greater than {startdate}"); }
            else
            {
                IEnumerable<Rates> Listdatarates = Enumerable.Empty<Rates>();
                for (int a = 0; a < listdatarates.Count(); a++)
                {
                    Rates datarates;
                    datarates = listdatarates.ElementAtOrDefault(a);
                    DateTime datedatarates = DateTime.ParseExact(datarates.Time, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (datedatarates >= startDate && datedatarates <= endDate)
                    {
                        Listdatarates = Listdatarates.Append(datarates);
                    }
                }
                if (Listdatarates.Count() == 0)
                { throw new Exception($"There are no exchange rates records for the timespan {startdate} to {enddate}"); }

                return Listdatarates;
            }

        }

        //Filtering exchange rates records to get a specific date
        public IEnumerable<Rates> FindDate(IEnumerable<Rates> listdatarates, string date)
        {
            if (string.IsNullOrEmpty(date))
            { throw new Exception("date parameter is missing or no value was assigned"); }

            DateTime Date;
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Date))
            { throw new Exception($"{date} does not match 'yyyy - MM - dd' format"); }
            else 
            {
                
                
                IEnumerable<Rates> Listdatarates = Enumerable.Empty<Rates>();
                for (int a = 0; a < listdatarates.Count(); a++)
                {
                    Rates datarates = new Rates();
                    datarates = listdatarates.ElementAtOrDefault(a);
                    DateTime datedatarates = DateTime.ParseExact(datarates.Time, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (datedatarates == Date)
                    {
                        Listdatarates = Listdatarates.Append(datarates);
                    }
                }
                int b = 0;
                while(!Listdatarates.Any())
                {
                    for (int a = 0; a < listdatarates.Count(); a++)
                    {
                        Rates datarates = new Rates();
                        datarates = listdatarates.ElementAtOrDefault(a);
                        DateTime datedatarates = DateTime.ParseExact(datarates.Time, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        if (datedatarates == Date)
                        {
                            Listdatarates = Listdatarates.Append(datarates);
                        }
                    }
                    Date = Date.AddDays(-1);
                }
                return Listdatarates;
            }
            

        }

        //Filtering exchange rates records to get specific currencies
        public IEnumerable<Rates> FindRates(IEnumerable<Rates> listdatarates, string symbols)
        {
            if (symbols != null)
            {
                var symbolss = symbols.Split(',');
                
                
                IEnumerable<Rates> Listdatarates = Enumerable.Empty<Rates>();
                
                for (int a = 0; a < listdatarates.Count(); a++) 
                {
                    Rates datarates = new Rates();
                    Rates newrates = new Rates();
                    datarates = listdatarates.ElementAtOrDefault(a);
                    IEnumerable<Rates.Currency> currencies = Enumerable.Empty<Rates.Currency>();
                    for (int i = 0; i < datarates.Currencies.Count(); i++)
                    {
                        for (int j = 0; j < symbolss.Length; j++)
                        {
                            if (datarates.Currencies.ElementAtOrDefault(i).Name == symbolss[j])
                            {
                                Rates.Currency currency = new Rates.Currency();
                                
                                currency.Name = datarates.Currencies.ElementAtOrDefault(i).Name;
                                currency.Rate = datarates.Currencies.ElementAtOrDefault(i).Rate;
                                currencies = currencies.Append(currency);
                                
                            }
                        }
                        
                    }
                    newrates = new Rates(datarates.Base, datarates.Time, currencies);
                    Listdatarates = Listdatarates.Append(newrates);
                }
                for (int i = 0; i < Listdatarates.Count(); i++)
                {
                    if (Listdatarates.ElementAtOrDefault(i).Currencies.Count() != symbolss.Length)
                        throw new Exception($"Symbols '{symbols}' are invalid. Maybe they are lowercase or not supported. Carefull with whitespaces between symbols.");
                }
                
                return Listdatarates;
                
                    
            }
            else return listdatarates;
        }

        //Get exchange rates using euro as base currency
        public IEnumerable<Rates> GetRates(bool latest) => CalculateRates(ParsingData(latest), "EUR"); 

        //Get exchange rates for specific currencies using a different base currency
        public IEnumerable<Rates> GetRates(bool latest, string currency, string symbols) => FindRates(CalculateRates(ParsingData(latest), currency), symbols);

        //Get exchange rates for specified currencies and timespan using a different base currency 
        public IEnumerable<Rates> GetRates(bool latest, string currency, string startdate, string enddate, string symbols) => FindRates(CalculateRates(FindDates(ParsingData(latest), startdate, enddate), currency), symbols);

        //Get exchange rates for specified currencies and date using a different base currency 
        public IEnumerable<Rates> GetRates(bool latest, string currency, string date, string symbols) => FindRates(CalculateRates(FindDate(ParsingData(latest), date), currency), symbols);
        
        //Handling errors
        public Error GetError(IExceptionHandlerFeature exception, int statuscode) 
        {
            Error error = new Error();
            error.statusCode = statuscode;
            error.message = exception.Error.Message;
            return error;
        }

        public Error GetError2(Exception exception)
        {
            Error error = new Error();
            error.statusCode = exception.HResult;
            error.message = exception.Message;
            return error;
        }
    }
}
