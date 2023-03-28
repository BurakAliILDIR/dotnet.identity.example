﻿using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
    }
}