using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses
{
    public class UnitTest_ResponseCookie : IResponseCookies
    {
        public Dictionary<string, string> CookiesDictionary = new Dictionary<string, string>();

        public void Append(string key, string value, CookieOptions options)
        {
            CookiesDictionary.Add(key, value);
        }

        // ------------------------------------------------------------------------------------------------------------------------

        public void Append(string key, string value)
        {
            throw new NotImplementedException();
        }
        public void Delete(string key)
        {
            throw new NotImplementedException();
        }
        public void Delete(string key, CookieOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
