using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;


namespace UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses
{
    public class UnitTest_HttpResponse : HttpResponse
    {
        public override IResponseCookies Cookies
        {
            get
            {
                return new UnitTest_ResponseCookie();
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------

        public override HttpContext HttpContext => throw new NotImplementedException();
        public override int StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IHeaderDictionary Headers => throw new NotImplementedException();
        public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool HasStarted => throw new NotImplementedException();
        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }
        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }
        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }
    }
}
