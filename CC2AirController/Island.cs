using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace CC2AirController
{
    public class Island : Plot
    {
        public int Team { get; set; }
        public string Name { get; set; }
        
        public int Production { get; set; }
        
        public Location CommandCenter { get; set; } = new Location();
        public List<Location> TurretSpawns { get; private set; } = new List<Location>();

        public static Island FromDict(int iid, Dictionary<string, string> data)
        {
            var island = new Island();
            island.Id = iid.ToString();
            if (data.TryGetValue("name", out string name))
            {
                island.Name = name;
            }
            if (data.TryGetValue("team", out string team))
            {
                island.Team = int.Parse(team);
            }
            if (data.TryGetValue("type", out string typ))
            {
                island.Production = int.Parse(typ);
            }
            if (data.TryGetValue("x", out string x))
            {
                island.Loc.X = double.Parse(x);
            }
            if (data.TryGetValue("y", out string y))
            {
                island.Loc.Y = double.Parse(y);
            }
            return island;
        }

        public override void Draw(ZoomViewport viewport)
        {
            if (Name != null)
            {
                var label_x = Loc.X;
                var label_y = Loc.Y + 50;

                var text = new TextBlock();
                text.Text = Name;
                text.Height = ScreenSize;
                text.Foreground = new SolidColorBrush(GetColor());
                viewport.AddText("islands", text, new Location(){ X= label_x, Y=label_y});
            }
        }
    }
}