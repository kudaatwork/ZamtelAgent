using System;
using Newtonsoft.Json;
using static System.DateTime;

namespace YomoneyApp
{
    public class UserAccount
    {
        public bool RequiresCall { get; set; }
        public string IdNumber { get; set; }
        public string ResponseDescription { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int ServiceType { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}

