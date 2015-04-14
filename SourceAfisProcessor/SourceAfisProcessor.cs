using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fingerprints_service_api;
using System.IO;
using SourceAFIS.Simple;
using System.Drawing;
using System.Xml.Linq;

namespace SourceAfisProcessor
{
    public class SourceAfisProcessor: IFingerprintsProcessor
    {
        public float Treshold { get; set; }

        public string ExtractTemplate(byte[] bitmap)
        {
            AfisEngine Afis = new AfisEngine();

            Stream bitmapStream = new MemoryStream(bitmap);
            Fingerprint fp = new Fingerprint();
            fp.AsBitmap = new Bitmap(bitmapStream);
            Person p = new Person();
            p.Fingerprints.Add(fp);
            Afis.Extract(p);

            return fp.AsXmlTemplate.ToString();
        }

        public bool Verify(byte[] bitmap, string template)
        {
            AfisEngine Afis = new AfisEngine();
            Afis.Threshold = Treshold;

            Stream bitmapStream = new MemoryStream(bitmap);
            Fingerprint fp1 = new Fingerprint();
            fp1.AsBitmap = new Bitmap(bitmapStream);
            Person p1 = new Person();
            p1.Fingerprints.Add(fp1);
            Afis.Extract(p1);

            Fingerprint fp2 = new Fingerprint();
            fp2.AsXmlTemplate =  XElement.Parse(template);
            Person p2 = new Person();
            p2.Fingerprints.Add(fp2);

            float score =  Afis.Verify(p1, p2);
            Console.WriteLine("Matching score " + score + " - fingerprints " + (score > 0 ? "match" : "do not match"));
            Console.WriteLine("Biometrics Match Treshold used: " + Treshold);

            return score > 0;
        }
    }
}
