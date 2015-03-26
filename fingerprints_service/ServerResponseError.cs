using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fingerprints_service
{
    class ServerResponseError: Exception
    {
        private System.Net.HttpStatusCode httpStatusCode;

        public ServerResponseError(System.Net.HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
        }
    }
}
