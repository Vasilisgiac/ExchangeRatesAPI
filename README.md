ExchangeRates is an API for current and old foreign exchange rates
published by the European Central Bank 
(https://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/index.en.html)
The API is built on ASP.NET Core (v3.1) with Razor pages and controllers.
Hosted for limited time at Azure: https://exchangecurrencyratesapi.azurewebsites.net

Usage:
Get the latest foreign exchange rates.
GET /api/rates/latest

Get historical rates from 1999 until today.
GET /api/rates/2005-05-25

Default base currency is Euro. Base curency can be changed by setting the 'basecur' parameter in the request.
GET /api/rates/latest?base=USD

Get exchange rates for specific currencies by setting the 'symbols' parameter.
GET api/rates/latest?symbols=USD,GBP

Rates history
Get historical rates for a timespan.
GET api/rates/history?startdate=2008-01-01&enddate=2020-09-01

Get historical rates for specific currencies by setting the 'symbols' parameter.
GET api/rates/history?startdate=2008-01-01&enddate=2020-09-01&symbols=ILS,JPY

Get historical rates against a different currency.
GET api/rates/history?startdate=2008-01-01&enddate=2020-09-01&base=USD

Copyrights 2020 ExchangeRates. The API is only for personal usage.
