using System.Collections.Generic;
using System.Web.Http;
using fingerprints_service.Models;
using System;

namespace fingerprints_service
{
    public class PhotosController: ApiController
    {
        public IHttpActionResult PostTakePhoto(HardwareSessionForm hsf)
        {
            if (ModelState.IsValid) 
            {
                Console.WriteLine("Received hsid: " + hsf.hsid);
                return InternalServerError();
            }
            else
            {
                return BadRequest("Expected hsid");
            }
            
        }
    }
}
