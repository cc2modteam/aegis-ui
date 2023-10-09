using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CC2AirController
{
    public class Unit : Plot
    {
        public int Team { get; set; }
        
        public int DefinitionIndex { get; set; }
        
        public static Unit FromDict(int iid, Dictionary<string, string> data)
        {
            var u = new Unit();
            u.Id = iid.ToString();
            if (data.TryGetValue("team", out string team))
            {
                u.Team = int.Parse(team);
            }
            if (data.TryGetValue("def", out string def))
            {
                u.DefinitionIndex = int.Parse(def);
            }
            if (data.TryGetValue("x", out string x))
            {
                u.Loc.X = double.Parse(x);
            }
            if (data.TryGetValue("y", out string y))
            {
                u.Loc.Y = double.Parse(y);
            }
            return u;
        }

        public VehicleDefinition Definition
        {
            get
            {
                if (Enum.TryParse(DefinitionIndex.ToString(), out VehicleDefinition result))
                {
                    return result;
                }

                return VehicleDefinition.UNK;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $" type={Definition}";
        }

        public override void Draw(ZoomViewport viewport)
        {
            var location = Loc;
            var col = GetColor();
            var b = new SolidColorBrush(col);

            var unitWidth = ScreenSize;
            var unitHeight = ScreenSize;
            
            switch (Definition)
            {
                // boxes
                case VehicleDefinition.NDL:
                    unitWidth = ScreenSize * 0.4;
                    unitHeight = ScreenSize * 0.9;
                    goto case VehicleDefinition.CRR;
                    
                case VehicleDefinition.SWD:
                    unitWidth = ScreenSize * 0.5;
                    unitHeight = ScreenSize * 1.1;
                    goto case VehicleDefinition.CRR;
                    
                case VehicleDefinition.BRG:
                    unitWidth = ScreenSize * 0.6;
                    goto case VehicleDefinition.CRR;
                    
                case VehicleDefinition.CRR:
                    var rect = new Rectangle
                    {
                        Height = unitHeight,
                        Width = unitWidth,
                        Fill = b
                    };

                    viewport.AddShape(rect, location);
                    break;
                
                // ground units
                case VehicleDefinition.MUL:
                    goto case VehicleDefinition.BER;
                case VehicleDefinition.WLR:
                    goto case VehicleDefinition.BER;
                case VehicleDefinition.SEL:
                    goto case VehicleDefinition.BER;    
                case VehicleDefinition.BER:
                    base.Draw(viewport);
                    break;
                
                // aircraft
                case VehicleDefinition.ALB:
                    goto case VehicleDefinition.MNT;
                case VehicleDefinition.RZR:
                    goto case VehicleDefinition.MNT;
                case VehicleDefinition.PTR:
                    goto case VehicleDefinition.MNT;
                case VehicleDefinition.MNT:
                    var triangle = new Polygon
                    {
                        StrokeThickness = 1,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Fill = new SolidColorBrush(GetColor())
                    };

                    triangle.Points.Add(new Point(0, -0.7 * ScreenSize));
                    triangle.Points.Add(new Point(ScreenSize / 1.5, ScreenSize / 1.5));
                    triangle.Points.Add(new Point(ScreenSize / -1.5, ScreenSize / 1.5));

                    viewport.AddShape(triangle, location);
                    
                    break;
                
                default:
                    break;
            }
            
            
        }
    }
}