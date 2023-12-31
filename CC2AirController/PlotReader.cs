using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CC2AirController
{
    public class PlotReader
    {
        private static PlotReader instance = null;
        public static PlotReader GetInstance()
        {
            if (instance == null)
            {
                instance = new PlotReader();
            }

            return instance;
        }
        
        public StreamReader GameOutput { get; set; }
        public StreamWriter SaveFile { get; set; }    
        public int GameSeconds { get; set; }
        private Dictionary<string, Plot> _plots = new Dictionary<string, Plot>();
        private Dictionary<string, Island> _islands = new Dictionary<string, Island>();

        public IEnumerable<Plot> Plots => new List<Plot>(_plots.Values);
        public IEnumerable<Island> Islands => new List<Island>(_islands.Values);

        public void UpdatePlot(Plot p)
        {
            if (p.Id != null)
            {
                if (_plots.TryGetValue(p.Id, out Plot item)) {
                    if (item is Unit)
                    {
                        item.Loc = p.Loc.Copy();
                    }
                } else {
                    // new
                    p.FirstTick = GameSeconds;
                    _plots[p.Id] = p;
                }
            }
        }
        
        public void UpdateIsland(Island p)
        {
            if (p.Id != null)
            {
                if (p.Name == null)
                {
                    // just an update (eg team)
                    if (_islands.TryGetValue(p.Id, out var island))
                    {
                        island.Team = p.Team;
                    }
                }
                else
                {
                    p.FirstTick = GameSeconds;
                    _islands[p.Id] = p;
                }
            }
        }

        Thread _worker;

        public void Start()
        {
            _worker = new Thread(ThreadBody);
            _worker.IsBackground = true;
            _worker.Start();
        }

        KeyValuePair<string, string> SplitPart(string part)
        {
            var nvp = new KeyValuePair<string, string>();
            var parts = part.Split('=');
            if (parts.Length == 2)
            {
                nvp = new KeyValuePair<string, string>(parts[0], parts[1]);
            }
            return nvp;
        }
        
        void ThreadBody()
        {
            while (true)
            {
                var line = GameOutput.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (SaveFile != null)
                {
                    SaveFile.WriteLine(line);
                }
                Console.Error.WriteLine(line);

                var text = line.Trim();

                var parts = text.Split(':');
                if (parts.Length > 0)
                {
                    var msg = parts[0];
                    if (parts.Length > 2)
                    {
                        var iid = int.Parse(parts[1]);
                        var data = new Dictionary<string, string>();
                        for (int i = 2; i < parts.Length; i++)
                        {
                            var pair = SplitPart(parts[i]);
                            if (pair.Key != null)
                            {
                                data[pair.Key] = pair.Value;
                            }
                        }

                        switch (msg)
                        {
                            case "AI":
                                // island
                                var island = Island.FromDict(iid, data);
                                UpdateIsland(island);
                                break;
                                
                            case "AU":
                                // unit
                                var unit = Unit.FromDict(iid, data);
                                UpdatePlot(unit);
                                break;
                            case "AIC":
                                // island command center
                                break;
                            case "AIT":
                                // island turret spot
                                Island current;
                                if (_islands.TryGetValue(iid.ToString(), out current))
                                {
                                    string tx;

                                    if (data.TryGetValue("x", out tx))
                                    {
                                        string ty;
                                        if (data.TryGetValue("y", out ty))
                                        {
                                            var turret = new Location()
                                            {
                                                X = double.Parse(tx),
                                                Y = double.Parse(ty)
                                            };
                                            current.TurretSpawns.Add(turret);
                                        }
                                    }
                                }

                                break;
                        }
                    }

                    if (parts.Length == 2)
                    {
                        switch (msg)
                        {
                            case "T":
                                GameSeconds = int.Parse(parts[1]);
                                DoTick();
                                break;
                        }
                    }
                }
            }
        }

        public void DoTick()
        {
            // calc ttl of plots
            foreach (var p in Plots)
            {
                if (p.Tick())
                {
                    _plots.Remove(p.Id);
                }
            }
        }
    }
}