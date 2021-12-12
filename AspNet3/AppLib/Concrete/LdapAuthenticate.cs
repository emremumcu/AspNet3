using AspNet3.AppLib.Abstract;
using AspNet3.AppLib.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Concrete
{
    // Install-Package AntiXSS.NetStandard -Version 0.1.125
    // Install-Package Microsoft.Windows.Compatibility 
    public class LdapAuthenticate : IAuthenticate
    {
        private readonly string _ldapPath;
        private readonly string _domainName;

        public LdapAuthenticate(string ldapPath, string domaiName)
        {
            _ldapPath = ldapPath;
            _domainName = domaiName;
        }

        public bool AuthenticateUser(string username, string password)
        {
            return AuthenticateUser(_ldapPath, _domainName, username, password, out _);
        }

        public bool AuthenticateUser(string username, string password, out SearchResult result)
        {
            return AuthenticateUser(_ldapPath, _domainName, username, password, out result);
        }

        private bool AuthenticateUser(string ldapPath, string domainName, string username, string password, out SearchResult result)
        {
            username = Microsoft.Security.Application.Encoder.LdapFilterEncode(username);
            password = Microsoft.Security.Application.Encoder.LdapFilterEncode(password);

            using (var context = new PrincipalContext(ContextType.Domain, domainName, username, password))
            {

                if (context.ValidateCredentials(username, password))
                {

                    using (DirectoryEntry de = new DirectoryEntry(ldapPath))
                    using (DirectorySearcher ds = new DirectorySearcher(de))
                    {
                        ds.Filter = $"(sAMAccountName={username})";

                        try
                        {
                            SearchResult adsSearchResult = ds.FindOne();
                            result = adsSearchResult;
                            return true;
                        }
                        //catch (DirectoryServicesCOMException deComEx)
                        //{
                        //    throw deComEx;
                        //}
                        //catch (LdapException ldapEx)
                        //{
                        //    throw ldapEx;
                        //}
                        //catch (Exception ex)
                        //{
                        //    throw ex;
                        //}
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            de.Close();
                        }
                    }
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}
