using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.Tools
{
    public class Image
    {
        public static byte[] ImageToByte(Bitmap img)
        {
            byte[] bytes = null;

            using (MemoryStream stream = new MemoryStream())
            {
                bytes = stream.ToArray();
            }

            return (bytes);
        }

        public static Bitmap ByteToImage(byte[] bytes)
        {
            Bitmap bmp;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                bmp = new Bitmap(ms);
            }

            return (bmp);
        }
    }
}
