using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BCJobClock
{
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //convert GDI+ bitmap to image source
            Bitmap bmp = value as Bitmap;
            if (bmp != null)
            {
                BitmapSource bitmapsource = null;
                try
                {
                    bitmapsource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero,
                                                                                      Int32Rect.Empty,
                                                                                      BitmapSizeOptions.FromEmptyOptions
                                                                                          ());
                }
                catch (Exception exp)
                {
                    Bitmap tempbmp = new Bitmap(32, 32);
                    Graphics g = Graphics.FromImage(tempbmp);
                    g.Clear(Color.Magenta);
                    bitmapsource = Imaging.CreateBitmapSourceFromHBitmap(tempbmp.GetHbitmap(Color.Magenta), 
                        IntPtr.Zero, Int32Rect.Empty, 
                        BitmapSizeOptions.FromEmptyOptions());


                }
                return bitmapsource;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();

        }



    }
}
