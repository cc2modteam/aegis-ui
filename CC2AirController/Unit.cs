using System.Collections.Generic;
using System.Security.Policy;

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

        public override string ToString()
        {
            return base.ToString() + $" type={DefinitionIndex}";
        }
    }
}