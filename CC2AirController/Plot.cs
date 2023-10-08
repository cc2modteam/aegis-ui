
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CC2AirController
{
    public class Plot
    {
        public String Id { get; set; }
        public Location Loc { get; set; } = new Location();
        public double ScreenSize { get; set; } = 20;   // eg, always 20 px
        public Color ScreenColor { get; set; }
        public List<string> Info { get; private set; } = new List<string>();
        public List<string> Labels { get; private set; } = new List<string>();

        public int Ttl { get; set; } = 10;

        public Color GetColor()
        {
            var col = ScreenColor;
            if (col.A == 0)
            {
                col.A = 128;
                col.R = col.G = col.B = 255;
            }

            return col;
        }
        
        public virtual void Draw(ZoomViewport viewport)
        {
            var circle = new Ellipse
            {
                Height = ScreenSize,
                Width = ScreenSize
            };
            var col = GetColor();

            var b = new SolidColorBrush(col);
            circle.Fill = b;
            viewport.AddShape(circle, Loc);
        }

        public bool Tick()
        {
            if (Ttl > 0)
            {
                Ttl -= 1;
            }

            return Ttl == 0;
        }
    }

}