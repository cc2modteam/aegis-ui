namespace CC2AirController
{
    public class Bounds
    {
        public Location Origin { get; set; } = new Location();
        public double Width { get; set; }
        public double Height { get; set; }

        public double X
        {
            get
            {
                return Origin.X;
            }
            set
            {
                Origin.X = value;
            }
        }

        public double Y
        {
            get
            {
                return Origin.Y;
            }
            set
            {
                Origin.Y = value;
            }
        }

        public Bounds(Location origin, double w, double h)
        {
            Origin = origin;
            Width = w;
            Height = h;
        }

        public Bounds()
        {
            Width = 1000;
            Height = 1000;
        }
    }
}