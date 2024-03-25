using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.CustomModels;
using ESD.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAS.Web.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly ILogBySqlRepository _logBySql;
        public ErrorController(ILogBySqlRepository logBySql)
        {
            _logBySql = logBySql;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult Error500(string id="")
        {
            //var message= System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(id));
           // var logInfo = new LogInfo("","", LogStateConst.Error, message, null);
           // _logBySql.InsertCRUDLog(logInfo);
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
