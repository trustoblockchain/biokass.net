using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace fingerprints_service
{
    class ServerResponseError: Exception
    {
        private HttpStatusCode httpStatusCode;
        private string httpReasonPhrase;

        public ServerResponseError(System.Net.Http.HttpResponseMessage response) :
             base("REST api returned bad response code: (" + response.StatusCode + ") " + response.ReasonPhrase)
        {
            this.httpStatusCode = response.StatusCode;
            this.httpReasonPhrase = response.ReasonPhrase;
        }

        public string ReasonPhrase { get { return httpReasonPhrase; } }
        public HttpStatusCode StatusCode { get { return httpStatusCode; } } 
    }
}
