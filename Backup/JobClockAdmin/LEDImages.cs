using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace JobClockAdmin
{
    
    class LEDImages
    {
        private static readonly Color[] DefaultMiddles = new Color[] { Color.Red, Color.Green };
        private static readonly Color[] DefaultOuters =  new Color[] { Color.DarkRed, Color.DarkGreen };
        private Color[] LEDColorsMiddle;
        private Color[] LEDColorsOuter;
        private Bitmap[] LEDs = null;
        private Graphics[] LEDGraphics = null;
        private Size _usesize;
        
        protected Bitmap GetLEDImage(Color innercolor,Color Outercolor, Size usesize)
        {
            Bitmap drawbitmap = new Bitmap(usesize.Width,usesize.Height);
            drawbitmap = new Bitmap(16, 16);
            Graphics g =  Graphics.FromImage(drawbitmap);
            g.Clear(Color.Transparent);
            //draw an ellipse...
            GraphicsPath gpp = new GraphicsPath();
            gpp.AddEllipse(2, 2, 14, 14);


            PathGradientBrush pathbrush = new PathGradientBrush(gpp);
            pathbrush.CenterColor = innercolor;
            Color[] surround = pathbrush.SurroundColors;

            for (int j = 0; j < surround.Length; j++)
                surround[j] = Outercolor;

            pathbrush.SurroundColors = surround;

            pathbrush.CenterPoint = new PointF(8, 8);


            g.FillPath(pathbrush, gpp);

            gpp.Dispose();
            pathbrush.Dispose();
            return drawbitmap;
        }
        
        public LEDImages(Color[] InnerColours, Color[] OuterColours,Size usesize)
        {

            
            LEDColorsMiddle = InnerColours;
            LEDColorsOuter = OuterColours;

            LEDs = new Bitmap[LEDColorsMiddle.Length];
            LEDGraphics = new Graphics[LEDColorsMiddle.Length];


            for (int i = 0; i < LEDColorsMiddle.Length; i++)
            {

                LEDs[i] = GetLEDImage(LEDColorsMiddle[i], LEDColorsOuter[i], usesize);





            }
            _usesize=usesize;
        }
    


    }
      
}
