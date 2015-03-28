using System.Collections.Generic;
using System.Web.Http;
using fingerprints_service.Models;
using System;
using WIA;
using System.Threading.Tasks;
using System.Net.Http;

namespace fingerprints_service
{
    public class PhotosController: ApiController
    {
        public async Task<IHttpActionResult> PostTakePhoto(TokenForm token)
        {
            UriBuilder uriBuilder = new UriBuilder(Program.ServerUrl);
            uriBuilder.Path = "/api/photos";
            uriBuilder.Query = "tokenid=" + token.tokenid;

            if (ModelState.IsValid) 
            {
                Console.WriteLine("Received hsid: " + token.tokenid);

                Task<Vector> CaptureImageAsync = Task.Run<Vector>(() => CaptureImage());
                Vector image = await CaptureImageAsync;

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(uriBuilder.Uri, 
                    new ByteArrayContent((byte [])image.get_BinaryData()));

                if (response.IsSuccessStatusCode)
                {
                    EntityIdResponse scan = await response.Content.ReadAsAsync<EntityIdResponse>();
                    return Created(response.Headers.Location, scan);
                }
                else
                {
                    return InternalServerError(new ServerResponseError(response));
                }
            }
            else
            {
                return BadRequest("Expected hsid");
            }   
        }

        Vector CaptureImage()
        {
            DeviceManager manager = new DeviceManager();
            Device device = null;
            foreach (DeviceInfo info in manager.DeviceInfos)
            {
                Console.WriteLine("Device: " + info.DeviceID);
                if (info.Type == WiaDeviceType.CameraDeviceType || info.Type == WiaDeviceType.VideoDeviceType)
                {
                    device = info.Connect();
                    Console.WriteLine("Selected device " + device.DeviceID);
                    break;
                }
            }
            if (device == null)
            {
                throw new NoDeviceException();
            }

            Microsoft.Win32.RegistryKey jpegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"CLSID\{D2923B86-15F1-46FF-A19A-DE825F919576}\SupportedExtension\.jpg");
            string jpegGuid = jpegKey.GetValue("FormatGUID") as string;

            Item item = device.ExecuteCommand(CommandID.wiaCommandTakePicture);
            foreach (string format in item.Formats)
            {
                if (format == jpegGuid)
                {
                    ImageFile imagefile = item.Transfer(format) as ImageFile;
                    return imagefile.FileData;
                }
            }
            throw new ImageFormatNotFound(jpegGuid);
        }
    }
}
