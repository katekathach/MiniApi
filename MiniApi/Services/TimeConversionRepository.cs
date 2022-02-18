using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

namespace MiniApi.Services
{
    public class TimeConversionRepository
    {

        public string SayHello(string name)
        {
            return $"Hello {name}";
        }

        //public System.Web.Mvc.JsonResult TimeConversion(string input)
        //{
        //    string jsonData = @"[{	'Country': 'India',    'City': 'New Delhi'}]";
        //    return Json(jsonData, JsonRequestBehavior.AllowGet);
        //}

        //public Task<IHttpActionResult> GetTimeCoversion(string input) {

        //    string jsonData = @"[{	'Country': 'India',    'City': 'New Delhi'}]";
        //    return Task.FromResult(Ok(jsonData));
        //}
    }
}
