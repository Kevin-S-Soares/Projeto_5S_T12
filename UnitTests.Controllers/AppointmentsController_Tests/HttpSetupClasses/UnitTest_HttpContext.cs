using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace UnitTests.Controllers.AppointmentsController_Tests.HttpSetupClasses
{
    public class UnitTest_HttpContext : HttpContext
    {
        private readonly int? _requestValue;

        public UnitTest_HttpContext(int? requestValue)
        {
            _requestValue = requestValue;
        }

        public override HttpResponse Response
        {
            get
            {
                return new UnitTest_HttpResponse();
            }
        }

        public override HttpRequest Request
        {
            get
            {
                return new UnitTest_HttpRequest(_requestValue);
            }
        }

        public override IServiceProvider RequestServices
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------

        public override IFeatureCollection Features => throw new NotImplementedException();
        public override ConnectionInfo Connection => throw new NotImplementedException();
        public override WebSocketManager WebSockets => throw new NotImplementedException();
        public override AuthenticationManager Authentication => throw new NotImplementedException();
        public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
