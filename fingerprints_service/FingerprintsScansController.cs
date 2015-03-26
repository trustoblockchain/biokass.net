using System.Collections.Generic;
using System.Web.Http;
using fingerprints_service.Models;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using ScanAPIHelper;
using fingerprints_service.Image;

namespace fingerprints_service
{
    public class FingerprintsScansController: ApiController 
    {
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

                    Task<byte[]> scanTask = Task.Run<byte[]> (() => ScanFingerprintAsync());
                    byte[] imageBytes = await scanTask;

                    UriBuilder uriBuilder = new UriBuilder(Program.ServerUrl);
                    uriBuilder.Path = "/api/fingerprintsscans";
                    uriBuilder.Query = "tokenid=" + token.tokenid;
                    Uri serverUri = uriBuilder.Uri;
                    Console.WriteLine("Sending the image bytes to " + serverUri);

                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync(serverUri,
                        new ByteArrayContent(imageBytes));

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
                        return InternalServerError(new ServerResponseError(response.StatusCode));
                    }
                }
            }
            else
            {
                return BadRequest("Expected tokenid");
            }
        }

        byte[] ScanFingerprintAsync()
        {
            Console.WriteLine("Scanning fingerprints...");
            Device scanner = new Device();
            byte[] bitmap;
            try
            {
                Console.WriteLine("Searching device...");
                scanner.Open();
            } 
            catch (ScanAPIException ex)
            {
                Console.WriteLine("Connection to device failed.");
                ShowError(ex);
                throw;
            }
            try
            {
                byte[] imageData = scanner.GetFrame();
                MyBitmapFile myFile = new MyBitmapFile(scanner.ImageSize.Width, scanner.ImageSize.Height, imageData);
                bitmap = myFile.BitmatFileData;
            }
            catch(ScanAPIException ex)
            {
                Console.WriteLine("Scanning failed");
                ShowError(ex);
                throw;
            }
            finally
            {
                scanner.Close();
            }
            Console.WriteLine("Fingerprint scanned.");
            return bitmap;
        }


        const int FTR_ERROR_EMPTY_FRAME = 4306; /* ERROR_EMPTY */
        const int FTR_ERROR_MOVABLE_FINGER = 0x20000001;
        const int FTR_ERROR_NO_FRAME = 0x20000002;
        const int FTR_ERROR_USER_CANCELED = 0x20000003;
        const int FTR_ERROR_HARDWARE_INCOMPATIBLE = 0x20000004;
        const int FTR_ERROR_FIRMWARE_INCOMPATIBLE = 0x20000005;
        const int FTR_ERROR_INVALID_AUTHORIZATION_CODE = 0x20000006;

        /* Other return codes are Windows-compatible */
        const int ERROR_NO_MORE_ITEMS = 259;                // ERROR_NO_MORE_ITEMS
        const int ERROR_NOT_ENOUGH_MEMORY = 8;              // ERROR_NOT_ENOUGH_MEMORY
        const int ERROR_NO_SYSTEM_RESOURCES = 1450;         // ERROR_NO_SYSTEM_RESOURCES
        const int ERROR_TIMEOUT = 1460;                     // ERROR_TIMEOUT
        const int ERROR_NOT_READY = 21;                     // ERROR_NOT_READY
        const int ERROR_BAD_CONFIGURATION = 1610;           // ERROR_BAD_CONFIGURATION
        const int ERROR_INVALID_PARAMETER = 87;             // ERROR_INVALID_PARAMETER
        const int ERROR_CALL_NOT_IMPLEMENTED = 120;         // ERROR_CALL_NOT_IMPLEMENTED
        const int ERROR_NOT_SUPPORTED = 50;                 // ERROR_NOT_SUPPORTED
        const int ERROR_WRITE_PROTECT = 19;                 // ERROR_WRITE_PROTECT
        const int ERROR_MESSAGE_EXCEEDS_MAX_SIZE = 4336;    // ERROR_MESSAGE_EXCEEDS_MAX_SIZE

        private void ShowError(ScanAPIException ex)
        {
            string szMessage;
            switch (ex.ErrorCode)
            {
                case FTR_ERROR_EMPTY_FRAME:
                    szMessage = "Empty Frame";
                    break;

                case FTR_ERROR_MOVABLE_FINGER:
                    szMessage = "Movable Finger";
                    break;

                case FTR_ERROR_NO_FRAME:
                    szMessage = "Fake Finger";
                    break;

                case FTR_ERROR_HARDWARE_INCOMPATIBLE:
                    szMessage = "Incompatible Hardware";
                    break;

                case FTR_ERROR_FIRMWARE_INCOMPATIBLE:
                    szMessage = "Incompatible Firmware";
                    break;

                case FTR_ERROR_INVALID_AUTHORIZATION_CODE:
                    szMessage = "Invalid Authorization Code";
                    break;

                case ERROR_NOT_ENOUGH_MEMORY:
                    szMessage = "Error code ERROR_NOT_ENOUGH_MEMORY";
                    break;

                case ERROR_NO_SYSTEM_RESOURCES:
                    szMessage = "Error code ERROR_NO_SYSTEM_RESOURCES";
                    break;

                case ERROR_TIMEOUT:
                    szMessage = "Error code ERROR_TIMEOUT";
                    break;

                case ERROR_NOT_READY:
                    szMessage = "Error code ERROR_NOT_READY";
                    break;

                case ERROR_BAD_CONFIGURATION:
                    szMessage = "Error code ERROR_BAD_CONFIGURATION";
                    break;

                case ERROR_INVALID_PARAMETER:
                    szMessage = "Error code ERROR_INVALID_PARAMETER";
                    break;

                case ERROR_CALL_NOT_IMPLEMENTED:
                    szMessage = "Error code ERROR_CALL_NOT_IMPLEMENTED";
                    break;

                case ERROR_NOT_SUPPORTED:
                    szMessage = "Error code ERROR_NOT_SUPPORTED";
                    break;

                case ERROR_WRITE_PROTECT:
                    szMessage = "Error code ERROR_WRITE_PROTECT";
                    break;

                case ERROR_MESSAGE_EXCEEDS_MAX_SIZE:
                    szMessage = "Error code ERROR_MESSAGE_EXCEEDS_MAX_SIZE";
                    break;

                default:
                    szMessage = String.Format("Error code: {0}", ex.ErrorCode);
                    break;
            }
            Console.WriteLine(szMessage);
        }
    }
}
