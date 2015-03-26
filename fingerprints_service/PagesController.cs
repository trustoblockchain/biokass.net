using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;

namespace fingerprints_service
{
    public class PagesController: ApiController
    {
        public HttpResponseMessage GetPage(string page)
        {
            HttpResponseMessage response = null;
            Console.WriteLine("Asked for static page: " + page);
            if (page == "waitdevice")
            {
                response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent("<html lang=\"en\">\r\n\r\n    <head>\r\n\r\n        <meta charset=\"utf-8\">\r\n        <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n        <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n        <meta name=\"description\" content=\"\">\r\n        <meta name=\"author\" content=\"\">\r\n\r\n        <title>Fingerprints DB</title>\r\n\r\n        <!-- Latest compiled and minified CSS -->\r\n        <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css\">\r\n\r\n        <!-- Optional theme -->\r\n        <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap-theme.min.css\">\r\n\r\n    </head>\r\n\r\n    <body>\r\n\r\n        <div class=\"container-fluid\">\r\n            <div class=\"progress\">\r\n                <div class=\"progress-bar progress-bar-striped active\" role=\"progressbar\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 100%\">\r\n                    <span class=\"sr-only\">Waiting for device</span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <script src=\"https://code.jquery.com/jquery-1.11.2.min.js\"></script>\r\n\r\n        <!-- Latest compiled and minified JavaScript -->\r\n        <script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js\"></script>\r\n        \r\n        <script src=\"http://october-user.codio.io:3000/js/scan-page.js\"></script>\r\n\r\n    </body>\r\n</html>\r\n",
                    Encoding.UTF8, "text/html");
            }

            if (response == null)
            {
                Console.WriteLine("Page not found: " + page);
                response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
            
            return response;
        }
    }
}
