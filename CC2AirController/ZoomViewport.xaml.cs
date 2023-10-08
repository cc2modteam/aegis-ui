using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace CC2AirController
{
    public partial class ZoomViewport : UserControl
    {
        public Bounds Area { get; private set; } = new Bounds();

        public double ActualAspect => ActualHeight / ActualWidth;

        public double Zoom
        {
            get => Area.Width / ActualWidth;
            set =>
                // x = a / b
                // a = x / b
                Area.Width = value / ActualWidth;
        }

        public double Scale => 1f / Zoom;

        private Dictionary<string, List<Shape>> layers = new Dictionary<string, List<Shape>>();
        private Dictionary<string, List<TextBlock>> textLayers = new Dictionary<string, List<TextBlock>>();

        private List<Shape> Shapes => layers["shapes"];
        
        
        public ZoomViewport()
        {
            InitializeComponent();
            layers["shapes"] = new List<Shape>();
            textLayers["islands"] = new List<TextBlock>();
            layers["islands"] = new List<Shape>();
            textLayers["shapes"] = new List<TextBlock>();
        }

        public void ClearLayer(string name)
        {
            List<Shape> layer;
            if (layers.TryGetValue(name, out layer))
            {
                foreach (var shape in layer.ToArray())
                {
                    View.Children.Remove(shape);
                    layer.Remove(shape);
                }
            }
            List<TextBlock> layer2;
            if (textLayers.TryGetValue(name, out layer2))
            {
                foreach (var x in layer2.ToArray())
                {
                    View.Children.Remove(x);
                    layer2.Remove(x);
                }
            }
        }

        public Location WorldToScreen(Location w)
        {
            var s = new Location();

            s.X = (w.X - Area.X) * Scale;
            s.Y = (w.Y - Area.Y) * Scale;
  
            return s;
        }

        public double ScreenToWorld(double pix)
        {
            return pix * Zoom;
        }

        public Point WorldToRelativePoint(Location center, Location worldpoint)
        {
            var dx = worldpoint.X - center.X;
            var dy = worldpoint.Y - center.Y;
            return new Point(dx * Scale, dy * Scale);
        }
        
        public Location ScreenToWorld(Location screen)
        {
            var scale = 1f / Zoom;
            
            var world = new Location
            {
                X = scale * screen.X + Area.X,
                Y = scale * screen.Y + Area.Y
            };

            return world;
        }

        public Location LocationWithinScreen(Location w)
        {
            var screen = WorldToScreen(w);
            if (screen.X > -100 && screen.X < ActualWidth + 100)
            {
                if (screen.Y > -100 && screen.Y < ActualHeight + 100)
                {
                    return screen;
                }
            }

            return null;
        }

        public void AddShape(Shape s, Location world)
        {
            var screen = LocationWithinScreen(world);
            if (screen != null) {
                Shapes.Add(s);
                View.Children.Add(s);
                Canvas.SetLeft(s, screen.X);
                Canvas.SetBottom(s, screen.Y);
            }
        }
        
        public void AddText(string layer, TextBlock b, Location world)
        {
            var screen = LocationWithinScreen(world);
            if (screen != null) {
                textLayers[layer].Add(b);
                View.Children.Add(b);
                Canvas.SetLeft(b, screen.X);
                Canvas.SetBottom(b, screen.Y);
            }
        }

        public void AdjustZoom(double amount)
        {
            if (amount > 0)
            {
                // zoom out (increase window size by amount meters)
                if (Area.Width < 1000000)
                {
                    var step = Area.Width * 0.1;
                    
                    Area.Width += step;
                    Area.X -= step / 2; // shift left of window right
                    Area.Y -= ActualAspect * step / 2;
                }
            }
            else
            {
                
                // zoom in, 
                if (Area.Width > 1000)
                {
                    var step = Area.Width * 0.1;
                    Area.Width -= step;
                    Area.X += step / 2;
                    Area.Y += ActualAspect * step / 2;
                }
            }
            
        }
        
        private void View_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                AdjustZoom(-1);
            }
            else
            {
                AdjustZoom(0.5);
            }
            
            e.Handled = true;
        }

        private bool isDragging = false;
        private Point dragStart = default(Point);
        private void DoPan(MouseEventArgs e) {
            if (!isDragging)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    isDragging = true;
                    dragStart = e.GetPosition(this);
                }
            } else {
                if (e.LeftButton == MouseButtonState.Pressed)
                {   
                    var pos = e.GetPosition(this);
                    var dx = dragStart.X - pos.X;
                    var dy = dragStart.Y - pos.Y;
                    //Area.X += dx;
                    //Area.Y -= dy;

                    var wdx = ScreenToWorld(dx);
                    var wdy = ScreenToWorld(dy);

                    Area.X += wdx;
                    Area.Y -= wdy;
                    
                    dragStart = e.GetPosition(this);
                }
                else
                {
                    isDragging = false;
                }
            }
            
        }

        private void View_OnMouseMove(object sender, MouseEventArgs e)
        {
            DoPan(e);
        }
    }
}