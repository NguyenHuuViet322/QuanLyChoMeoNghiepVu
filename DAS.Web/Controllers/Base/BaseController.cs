using ESD.Application.Constants;
using ESD.Application.Models.CustomModels;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    public class BaseController : Controller
    {
        private string _title;

        private readonly List<string> _errors = new List<string>();
        protected List<ErrorFieldModel> ErrorFields = new List<ErrorFieldModel>();
        protected string strMessage;
        #region Form Data
        private Hashtable _data;
        protected Hashtable DATA
        {
            get
            {
                if (Equals(_data, null))
                    _data = GetPostData();
                return _data;
            }
        }
        #endregion

        /// <summary>
        /// Trả về JSError hiển thị theo popup
        /// </summary>
        /// <returns></returns>
        protected IActionResult JSErrorModelState()
        {
            List<Microsoft.AspNetCore.Mvc.ModelBinding.ModelErrorCollection> modelErrorCollection = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0).ToList();
            string msgErr = string.Empty;
            foreach (var item in modelErrorCollection)
            {
                msgErr = !string.IsNullOrEmpty(msgErr) ? $"{msgErr}\n{item[0].ErrorMessage}" : item[0].ErrorMessage;
            }
            return JSErrorResult(msgErr);
        }

        /// <summary>
        /// Trả về JSError hiển thị theo từng dòng
        /// </summary>
        /// <returns></returns>
        protected IActionResult JSErrorModelStateByLine()
        {
            var modelErrorCollection = ModelState.ToDictionary(x => x.Key, x => x.Value.Errors)
                       .Where(y => y.Value.Count > 0).ToList();
            string msgErr = string.Empty;
            var errObj = new List<object>();
            var mss = string.Empty;
            foreach (var item in modelErrorCollection)
            {
                var modelError = item.Value[0];
                mss = modelError.ErrorMessage;
                if (mss == "The value '' is invalid.")
                    mss = "Giá trị không được để trống";

                msgErr = !string.IsNullOrEmpty(msgErr) ? $"{msgErr}\n{mss}" : mss;
                errObj.Add(new { Field = item.Key, Mss = mss });
            }
            return JSErrorResult(msgErr, errObj);
        }

        /// <summary>
        /// Set field lỗi hiển thị theo từng dòng
        /// </summary>
        /// <returns></returns>
        /// 
        protected void SetErrorModelStateByLine()
        {
            var modelErrorCollection = ModelState.ToDictionary(x => x.Key, x => x.Value.Errors)
                       .Where(y => y.Value.Count > 0).ToList();
            string msgErr = string.Empty;
            var msg = string.Empty;
            foreach (var item in modelErrorCollection)
            {
                var modelError = item.Value[0];
                msg = modelError.ErrorMessage;
                if (msg == "The value '' is invalid.")
                    msg = "Giá trị không được để trống";

                msgErr = !string.IsNullOrEmpty(msgErr) ? $"{msgErr}\n{msg}" : msg;
                ErrorFields.Add(new ErrorFieldModel { Field = item.Key, Mss = msg });
            }
        }

        /// <summary>
        /// Trả về JSError hiển thị theo từng dòng đã set
        /// </summary>
        /// <returns></returns>
        protected IActionResult GetJSErrorResult()
        {
            if (ErrorFields.IsNotEmpty())
            {
                strMessage = string.Join(Environment.NewLine, ErrorFields.Select(n=> n.Mss));
            }
            return JSErrorResult(strMessage, ErrorFields);
        }


        /// <summary>
        /// Trả về JSError theo trường
        /// </summary>
        /// <returns></returns>
        protected IActionResult JSErrorModelState(string msg, string fieldName)
        {
            var errObj = new List<object>
            {
                new { Field = fieldName, Mss = msg }
            };
            return JSErrorResult(msg, errObj);
        }

        /// <summary>
        /// Trả về JSError theo trường
        /// </summary>
        /// <returns></returns>
        protected void SetErrorModelState(string msg, string fieldName)
        {
            ErrorFields.Add(new ErrorFieldModel { Field = fieldName, Mss = msg });
        }

        protected IActionResult CustJSonResult(ServiceResult ServiceResult)
        {
            if (ServiceResult.Code == CommonConst.Success)
                return JSSuccessResult(ServiceResult.Message, ServiceResult.Data);
            else if (ServiceResult.Code == CommonConst.Error)
                return JSErrorResult(ServiceResult.Message, ServiceResult.Data);
            else if (ServiceResult.Code == CommonConst.Warning)
                return JSWarningResult(ServiceResult.Message, ServiceResult.Data);
            else return null;
        }

        protected IActionResult JSSuccessResult(string msg)
        {
            return new JsonResult(new
            {
                Type = CommonConst.Success,
                Message = msg
            });
        }

        protected IActionResult JSSuccessResult<T>(string msg, T val)
        {
            return new JsonResult(new
            {
                Type = CommonConst.Success,
                Message = msg,
                Data = val
            });
        }

        protected IActionResult JSErrorResult(string msg)
        {
            return new JsonResult(new
            {
                Type = CommonConst.Error,
                Message = msg
            });
        }

        protected IActionResult JSErrorResult<T>(string msg, T val)
        {
            return new JsonResult(new
            {
                Type = CommonConst.Error,
                Message = msg,
                Data = val
            });
        }


        protected IActionResult JSWarningResult(string msg)
        {
            return new JsonResult(new
            {
                Type = CommonConst.Warning,
                Message = msg
            });
        }

        protected IActionResult JSWarningResult<T>(string msg, T val)
        {
            return new JsonResult(new
            {
                Type = CommonConst.Warning,
                Message = msg,
                Data = val
            });
        }

        protected int GetCurrUser()
        {
            return 1;
        }
        /// <summary>
        ///     Set title page
        /// </summary>
        /// <param name="title"></param>
        protected void SetTitle(string title)
        {
            _title = title;
        }


        /// <summary>
        ///     Set errors
        /// </summary>
        /// <param name="error"></param>
        internal void SetError(string error)
        {
            _errors.Add(error);
            TempData["ErrMessage"] = error;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (!Equals(_title, null))
                ViewData["Title"] = _title;
            IServiceProvider services = filterContext.HttpContext.RequestServices;
            var userPrincipalService = (IUserPrincipalService)services.GetService(typeof(IUserPrincipalService));
            ViewBag.UserAcc = userPrincipalService;
        }

        protected string GetCData(ESD.Infrastructure.ContextAccessors.IUserPrincipalService _iUserPrincipalService, string _apiFile)
        {
            var cdata = new
            {
                Token = _iUserPrincipalService.AccessToken,
                ProductKey = Utils.Base64Encode("FSI-DAS"),
                CUser = new
                {
                    Name = _iUserPrincipalService.UserName,
                    Uid = _iUserPrincipalService.UserId
                },
                Storage = new
                {
                    largeSize = false,
                    isDisablePrintfile = true,
                    isDisableDownfile = true,
                    domain = _apiFile
                    //domain = "https://localhost:44370"
                }
            };
            return Utils.Serialize(cdata);
        }

        #region Functions
        private Hashtable GetPostData()
        {
            var data = new Hashtable();
            try
            {
                foreach (string key in Request.Query.Keys)
                {
                    if (!data.ContainsKey(key))
                        data.Add(key, Request.Query[key]);
                }
            }
            catch { }

            try
            {
                foreach (string key in Request.Form.Keys)
                {
                    if (!data.ContainsKey(key))
                        data.Add(key, Request.Form[key].ToString());
                }
            }
            catch { }

            return data;
        }
        #endregion
    }
}