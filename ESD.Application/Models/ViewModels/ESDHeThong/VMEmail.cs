using ESD.Application.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMEmail
    {
        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string ToEmail { get; set; }

        [MaxLength(1000, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Content { get; set; }

        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Title { get; set; }     
    }
}