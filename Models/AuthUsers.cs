// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Auth0WithRedis.Models
{
    public partial class AuthUsers
    {
        public AuthUsers()
        {
            AuthLogin = new HashSet<AuthLogin>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<AuthLogin> AuthLogin { get; set; }
    }
}