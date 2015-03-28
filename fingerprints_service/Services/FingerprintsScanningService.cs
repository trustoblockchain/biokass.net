using fingerprints_service_api;
using fingerprints_service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace fingerprints_service.Services
{
    public struct FingerPrintResponse {
        public EntityIdResponse FingerprintId;
        public Uri FingerprintLocaltion;
    }

    public class FingerprintsScanningService
    {
        private IFingerprintsScanner scanner;
        public string ServerUrl { get; set; }

        public FingerprintsScanningService(IFingerprintsScanner scanner)
        {
            this.scanner = scanner;
        }

        private Uri buildRestUri(string tokenid, string kind) {
            UriBuilder uriBuilder = new UriBuilder(ServerUrl);
            uriBuilder.Path = "/api/fingerprintsscans";
            uriBuilder.Query = "tokenid=" + tokenid + "&" + "kind=" + kind;
            return uriBuilder.Uri;
        }

        public async Task<FingerPrintResponse> PostFingerprintScan(string tokenid)
        {
            var imageBytes = await scanner.ScanFingerprintAsync();
            var scanResponse = await PostBitmapAsync(imageBytes, tokenid);

            string template = "";
            await PostTemplateAsync(template, tokenid);

            return scanResponse;
        }

        async Task<FingerPrintResponse> PostBitmapAsync(byte[] imageBytes, string tokenid)
        {
            var serverUri = buildRestUri(tokenid, "scan");
            Console.WriteLine("POST fingerprint bitmap to: " + serverUri);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(serverUri,
                new ByteArrayContent(imageBytes));
            Console.WriteLine("POST scanned bitmap response code: " + response.StatusCode + " " + response.ReasonPhrase);
            Console.WriteLine("POST scanned bitmap response: " + await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("The scan was created successfully on server.");
                EntityIdResponse scan = await response.Content.ReadAsAsync<EntityIdResponse>();
                Console.WriteLine("\tscan.id = " + scan.id);
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
                    locationBuilder.Path = "/api/fingerprintscans/" + scan.id;
                    locationHeader = locationBuilder.Uri;

                }
                Console.WriteLine("Sending Location header: " + locationHeader);
                return new FingerPrintResponse { FingerprintId = scan, FingerprintLocaltion = locationHeader };
            }
            else
            {
                throw new ServerResponseError(response);
            }
        }

        async Task PostTemplateAsync(string template, string tokenid) {
            
        }

    }
}
