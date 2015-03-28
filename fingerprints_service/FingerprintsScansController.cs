using System.Collections.Generic;
using System.Web.Http;
using fingerprints_service.Models;
using System;
using System.Threading.Tasks;
using fingerprints_service.Services;

namespace fingerprints_service
{
    public class FingerprintsScansController: ApiController 
    {
        FingerprintsScanningService scanningService;

        public FingerprintsScansController(FingerprintsScanningService scanningService)
        {
            this.scanningService = scanningService;
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
                    var response = await scanningService.PostFingerprintScan(token.tokenid);
                    return Created(response.FingerprintLocaltion, response.FingerprintId);
                }
            }
            else
            {
                return BadRequest("Expected tokenid");
            }
        }

    }
}
