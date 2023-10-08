using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
                
                var outline = new Polygon();
                outline.StrokeThickness = 1;
                outline.HorizontalAlignment = HorizontalAlignment.Center;
                outline.VerticalAlignment = VerticalAlignment.Center;
                outline.Stroke = new SolidColorBrush(GetColor());

                var scale = viewport.Scale;

                if (TurretSpawns.Count > 3)
                {
                    // draw a polygon
                    // find cardinal turrets
                    Location north = Loc.Copy();
                    Location east = Loc.Copy();
                    Location south = Loc.Copy();
                    Location west = Loc.Copy();
                    foreach (var turret in TurretSpawns)
                    {
                        if (turret.Y > north.Y)
                        {
                            north = turret;
                        }

                        if (turret.Y < south.Y)
                        {
                            south = turret;
                        }

                        if (turret.X > east.X)
                        {
                            east = turret;
                        }

                        if (turret.X < west.X)
                        {
                            west = turret;
                        }
                    }
                    
                    outline.Points.Add(viewport.WorldToRelativePoint(Loc, north));
                    outline.Points.Add(viewport.WorldToRelativePoint(Loc, east));
                    outline.Points.Add(viewport.WorldToRelativePoint(Loc, south));
                    outline.Points.Add(viewport.WorldToRelativePoint(Loc, west));
                    
                } else {
                    // make a box around the middle of the island and the command center
                    outline.Points.Add(new Point(scale * -500, scale * -600));
                    outline.Points.Add(new Point(scale * 600, scale * -500));
                    outline.Points.Add(new Point(scale * 500, scale * 600));
                    outline.Points.Add(new Point(scale * -650, scale * 600));
                }
                viewport.AddShape(outline, Loc);
            }
        }
    }
}