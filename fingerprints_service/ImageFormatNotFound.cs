using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fingerprints_service
{
    class ImageFormatNotFound : Exception
    {
        private string jpegGuid;

        public ImageFormatNotFound(string jpegGuid)
        {
            // TODO: Complete member initialization
            this.jpegGuid = jpegGuid;
        }
        public string Format
        {
            get { return jpegGuid; }
        }
    }
}
