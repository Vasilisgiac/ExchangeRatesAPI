using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRates.Models;
using ExchangeRates.Services;
using ExchangeRates.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using System.Globalization;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace ExchangeRates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        public RatesController(APIService apiService)
        {
            this.APIService = apiService;
        }

        public APIService APIService { get; }


        //[ViewData]
        //public int statusCode { get; private set; }

        //[ViewData]
        //public string message { get; private set; }

        //[ViewData]
        //public string statusTrace { get; private set; }


        
        [Produces("application/json")]
        [HttpGet()]
        public Default Get() 
        {
            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            return APIService.GetDefault(host);
        }

        //Get latest exchange rates
        //basecur variable changes the base currency of exchange rates
        //symbols variable filtering the records to get exchange rates only for specific currencies
        [HttpGet("latest/{format=json}"), FormatFilter]
        public IActionResult Latest(string basecur, string symbols)
        {
            try
            {
                return Ok(APIService.GetRates(true, basecur, symbols));

            }
            catch (Exception e)
            {
                return Error(e);
            }
            
        }

        //Get exchange rates for a timespan
        //basecur variable changes the base currency of exchange rates
        //symbols variable filtering the records to get exchange rates only for specific currencies
        [HttpGet("history/{format=json}"), FormatFilter]
        public IActionResult History(string basecur, string startdate, string enddate, string symbols)
        {

            try
            {
                return Ok(APIService.GetRates(false, basecur, startdate, enddate, symbols));

            }
            catch (Exception e)
            {
                return Error(e);
            }

        }

        //Get exchange rates for specific date
        //basecur variable changes the base currency of exchange rates
        //symbols variable filtering the records to get exchange rates only for specific currencies
        [HttpGet("{date}/{format=json}"), FormatFilter]
        public IActionResult Date(string basecur, string date, string symbols)
        {
            try
            {
                return Ok(APIService.GetRates(false, basecur, date, symbols));

            }
            catch (Exception e)
            {
                return Error(e);
            }

        }

        //Handling errors for developer environment
        [Route("errorhandler/{format=json}"), FormatFilter]
        public IActionResult Error()
        { 
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var statuscode = HttpContext.Response.StatusCode;
            return Ok(APIService.GetError(exception, statuscode));
            
            //string a = "{source: "+context.Error.Source +", requestId: "+RequestId+", site: "+context.Error.TargetSite +", title: "+ context.Error.Message+"}";
            //return Ok(a);
        }

        //Handling errors in general
        [Route("error/{format=json}"), FormatFilter]
        public IActionResult Error(Exception exception)
        {
            var xception = exception.Message;
            var statuscode = exception.HResult;
            return Ok(APIService.GetError2(exception));

            //string a = "{source: "+context.Error.Source +", requestId: "+RequestId+", site: "+context.Error.TargetSite +", title: "+ context.Error.Message+"}";
            //return Ok(a);
        }
    }
}
