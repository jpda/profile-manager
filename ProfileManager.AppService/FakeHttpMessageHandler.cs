using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    // neat. see http://hamidmosalla.com/2017/02/08/mock-httpclient-using-httpmessagehandler/
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            throw new NotImplementedException("Don't use me except for mocks");
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
