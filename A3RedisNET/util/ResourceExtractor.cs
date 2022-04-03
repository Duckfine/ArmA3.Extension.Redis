using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3Redis_CS.util
{
    class ResourceExtractor
    {
        public static void ExtractResourceToFile(string resName, string fileName)
        {
            if(!System.IO.File.Exists(fileName))
            {
                using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resName))
                    using(System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, b.Length);
                    fs.Write(b, 0, b.Length);
                }
            }


        }

    }
}
