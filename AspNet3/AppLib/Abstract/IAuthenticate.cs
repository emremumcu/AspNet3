using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Abstract
{
    public interface IAuthenticate
    {
        public bool AuthenticateUser(string username, string password);        
    }
}
