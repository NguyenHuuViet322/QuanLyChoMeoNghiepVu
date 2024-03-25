using ESD.Application.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.CustomModels
{
    public class ServiceResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class ServiceResultSuccess : ServiceResult
    {
        public ServiceResultSuccess()
        {
            Code = CommonConst.Success;
        }
        public ServiceResultSuccess(string msg)
        {
            Code = CommonConst.Success;
            Message = msg;
        }
        public ServiceResultSuccess(string msg, object data)
        {
            Code = CommonConst.Success;
            Message = msg;
            Data = data;
        }
        public ServiceResultSuccess(object data)
        {
            Code = CommonConst.Success;
            Data = data;
        }
    }

    public class ServiceResultError : ServiceResult
    {
        public ServiceResultError()
        {
            Code = CommonConst.Error;
        }
        public ServiceResultError(string msg)
        {
            Code = CommonConst.Error;
            Message = msg;
        }
        /// <summary>
        /// Show error inline
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="field"></param>
        public ServiceResultError(string msg, string field)
        {
            Code = CommonConst.Error;
            Message = msg;
            Data = new[] { new { Field = field, Mss = msg } };
        }

        public ServiceResultError(string msg, object data)
        {
            Code = CommonConst.Error;
            Message = msg;
            Data = data;
        }
    }
}