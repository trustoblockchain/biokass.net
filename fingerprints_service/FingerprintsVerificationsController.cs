using fingerprints_service.Models;
using fingerprints_service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace fingerprints_service
{
    public class FingerprintsVerificationsController: ApiController
    {
        FingerprintsScanningService scanningService;

		public FingerprintsVerificationsController(FingerprintsScanningService scanningService)
        {
            this.scanningService = scanningService;
        }

		public async Task<IHttpActionResult> PostVerifyPerson(TokenForm token)
        {
            if (ModelState.IsValid)
            {
                if (token == null)
                {
                    Console.WriteLine("Token not received.");
                    return BadRequest("Expected tokenid");
                }
				else
                {
                    Console.WriteLine("Received verify-token with id " + token.tokenid);
                    var response = await scanningService.VerifyFingerprint(token.tokenid);
                    return Ok(new MatchResponse() { match = response });
                }
            }
			else
            {
                return BadRequest("Expected tokenid");
            }
        }
    }
}
