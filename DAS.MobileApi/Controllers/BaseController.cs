using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESD.Application.Constants;
using ESD.Application.Models.CustomModels;

namespace DAS.MobileApi.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
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
        }

        #region Functions
        private Hashtable GetPostData()
        {
            var data = new Hashtable();
            try
            {
                foreach (string key in Request.Form.Keys)
                {
                    if (!data.ContainsKey(key))
                        data.Add(key, Request.Form[key]);
                }
            }
            catch { }
            return data;
        }
        #endregion
    }
}
