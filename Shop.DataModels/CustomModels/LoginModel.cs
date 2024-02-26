﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.DataModels.CustomModels
{
    public class LoginModel
    {
        public string UserKey { get; set; }
        public string Name { get; set; }

        // [Required(ErrorMessage = "EmailId is required")]
        [Required]
        public string EmailId { get; set; }

        // [Required(ErrorMessage = "Password is required")]
        [Required]
        public string Password { get; set; }
    }
}
