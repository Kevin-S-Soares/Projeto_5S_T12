using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses
{
    public class UnitTest_RequestCookieCollection : IRequestCookieCollection
    {
        public Dictionary<string, string> CookiesDictionary = new Dictionary<string, string>();

        public UnitTest_RequestCookieCollection(int? show)
        {
            if (show.HasValue)
            {
                CookiesDictionary.Add("Show", show.Value.ToString());
            }
        }

        public string this[string key]
        {
            get
            {
                return CookiesDictionary[key];
            }
        }

        public bool ContainsKey(string key)
        {
            return CookiesDictionary.ContainsKey(key);
        }

        // ------------------------------------------------------------------------------------------------------------------------
        public int Count => throw new NotImplementedException();
        public ICollection<string> Keys => throw new NotImplementedException();
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
