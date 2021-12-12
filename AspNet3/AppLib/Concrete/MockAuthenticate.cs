using AspNet3.AppLib.Abstract;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Concrete
{
    public class MockAuthenticate : IAuthenticate
    {
        public bool AuthenticateUser(string username, string password)
        {
            return true;
        }
    }
}
