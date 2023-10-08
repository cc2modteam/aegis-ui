
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

        public List<Location> History { get; } = new List<Location>();
        public int FirstTick { get; set; }
        public double ScreenSize { get; set; } = 10;   // eg, always 20 px
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
            var location = Loc;
            
            var circle = new Ellipse
            {
                Height = ScreenSize,
                Width = ScreenSize
            };
            var col = GetColor();

            var b = new SolidColorBrush(col);
            circle.Fill = b;
            viewport.AddShape(circle, location);
        }
        
        public bool Tick()
        {
            if (Ttl > 0)
            {
                Ttl -= 1;
                History.Insert(0, Loc.Copy());

                if (History.Count > 10)
                {
                    History.RemoveAt(History.Count - 1);
                }
            }

            return Ttl == 0;
        }

        public override string ToString()
        {
            return $"{Id} x={Loc.X} y={Loc.Y}";
        }
    }

}