using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace BCJobClockLib
{

    public class ImageManager
    {
        private Dictionary<String, Image> loadedimages = new Dictionary<string, Image>();


        public ImageManager()
        {
            String imagefolder = Path.Combine(JobClockConfig.GetAppDataFolder(), "Images");


            foreach (FileInfo iteratefile in new DirectoryInfo(imagefolder).GetFiles("*.png"))
            {

                String basenameonly = Path.GetFileNameWithoutExtension(iteratefile.FullName).ToUpper();
                MemoryStream streamread = new MemoryStream(File.ReadAllBytes(iteratefile.FullName));

                AddImage(streamread, basenameonly);




            }




        }
        public Image this[String imageindex]
        {
            get { return GetLoadedImage(imageindex); }

        }
        public Image DefaultImage()
        {
            Image testimage = null;
            Bitmap clearblock = new Bitmap(32, 32);
            Graphics cb = Graphics.FromImage(clearblock);
            cb.Clear(Color.Transparent);
            cb.Dispose();
            return clearblock;


        }

        public Image GetLoadedImage(String imagekey)
        {
            imagekey = imagekey.ToUpper();
            if(!loadedimages.ContainsKey(imagekey)) return DefaultImage();
            return loadedimages[imagekey];




        }

        private bool AddImage(Stream streamread, string basenameonly)
        {
            basenameonly = basenameonly.ToUpper();
            Image loadedimage = Image.FromStream(streamread);


            if (!loadedimages.ContainsKey(basenameonly))
            {
                loadedimages.Add(basenameonly, loadedimage);
                return true;
            }
            else
            {
                Debug.Print("image already loaded with key " + basenameonly);
            }
            return false;
        }


    }


}
