using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Slidable.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(16)]
        public string Handle { get; set; }
    }
}
