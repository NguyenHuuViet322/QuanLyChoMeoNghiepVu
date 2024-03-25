﻿using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMExpiryDate : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(20, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Mã thời hạn bảo quản", Prompt = "Mã thời hạn bảo quản")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        [Display(Name = "Tên thời hạn bảo quản", Prompt = "Tên thời hạn bảo quản")]
        public string Name { get; set; }

        [Display(Name = "Mô tả", Prompt = "Mô tả")]
        [MaxLength(250, ErrorMessage = "{0} không được quá {1} ký tự")]
        public string Description { get; set; }

        public int Status { get; set; } = 1;

        [Display(Name = "Thời hạn", Prompt = "Thời hạn")]
        public int Value { get; set; }

        public int MaxValueExpiryDate { get; set; }
    }

    public class ExpiryDateCondition
    {
        public ExpiryDateCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
