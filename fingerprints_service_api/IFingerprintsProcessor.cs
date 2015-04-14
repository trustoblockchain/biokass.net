using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fingerprints_service_api
{
    public interface IFingerprintsProcessor
    {
        string ExtractTemplate(byte[] bitmap);
        bool Verify(byte[] bitmap, string template);
    }
}
