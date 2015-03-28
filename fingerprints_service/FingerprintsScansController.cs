using System.Collections.Generic;
using System.Web.Http;
using fingerprints_service.Models;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using fingerprints_service_api;

namespace fingerprints_service
{
    public class FingerprintsScansController: ApiController 
    {
        private IFingerprintsScanner scanner;

        public FingerprintsScansController(IFingerprintsScanner scanner)
        {
            this.scanner = scanner;
        }

        public async Task<IHttpActionResult> PostFingerprintsScans(TokenForm token)
        {
            if (ModelState.IsValid)
            {
                if(token == null) 
                {
                    Console.WriteLine("Token not received. Signal the error and stop processing request.");
                    return BadRequest("Expected tokenid");
                }
                else
                {

                    Console.WriteLine("Received tokenid: " + token.tokenid);

                    

                    UriBuilder uriBuilder = new UriBuilder(Program.ServerUrl);
                    uriBuilder.Path = "/api/fingerprintsscans";
                    uriBuilder.Query = "tokenid=" + token.tokenid + "&" + "kind=scan";
                    Uri serverUri = uriBuilder.Uri;
                    Console.WriteLine("POST fingerprint bitmap to: " + serverUri);

                    var imageBytes = await scanner.ScanFingerprintAsync();

                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync(serverUri,
                        new ByteArrayContent(imageBytes));
                    Console.WriteLine("POST scanned bitmap response code: " + response.StatusCode + " " + response.ReasonPhrase);
                    Console.WriteLine("POST scanned bitmap response: " + await response.Content.ReadAsStringAsync());

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("The scan was created successfully on server.");
                        EntityIdResponse scan = await response.Content.ReadAsAsync<EntityIdResponse>();
                        Console.WriteLine("\tscan.id = " + scan.Id);
                        Console.WriteLine("\tLocation: " + response.Headers.Location);
                        foreach (var h in response.Headers)
                        {
                            Console.WriteLine("\t" + h.Key + ": " + h.Value);
                        }
                        Uri locationHeader;
                        if (response.Headers.Location != null)
                        {
                            locationHeader = response.Headers.Location;
                        }
                        else
                        {
                            Console.WriteLine("Creating Location header manually");
                            var locationBuilder = new UriBuilder(Program.ServerUrl);
                            locationBuilder.Path = "/api/fingerprintscans/" + scan.Id;
                            locationHeader = locationBuilder.Uri;

                        }
                        Console.WriteLine("Sending Location header: " + locationHeader);
                        return Created(locationHeader, scan);
                    }
                    else
                    {
                        return InternalServerError(new ServerResponseError(response));
                    }
                }
            }
            else
            {
                return BadRequest("Expected tokenid");
            }
        }

    }
}
