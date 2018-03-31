﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCard.Models
{
    public class SmartCcontractModel
    {
        [Required]
        public string hash_сustomer;
        [Required]
        public string hash_еxecutor;
        [Required]
        public string order_sum;
        [Required]
        public string prepaid_expense;
        [Required]
        public string condition;
        public bool is_Done;
        public bool is_freze;
    }

    public class LoginModel
    {
        [Required]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public string login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string code_phrase { get; set; }


    }
}
