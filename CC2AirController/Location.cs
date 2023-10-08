namespace CC2AirController
{
    public class Location
    {
        public double X { get; set; }

        public double Y { get; set; }

        public int Alt { get; set; }

        public Location Copy()
        {
            var cp = new Location()
            {
                X = X,
                Y = Y,
                Alt = Alt
            };

            return cp;
        }
    }
}