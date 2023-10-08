using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows;

namespace CC2AirController
{
    public class PlotReader
    {
        public StreamReader GameOutput { get; set; }

        private Thread worker;

        public void Start()
        {
            worker = new Thread(ThreadBody);
            worker.IsBackground = true;
            worker.Start();
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
                Console.Error.WriteLine(line);

                var text = line.Trim();

                var parts = text.Split(':');
                if (parts.Length > 2)
                {
                    var msg = parts[0];
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
   
                            break;
                        case "AIC":
                            // island command center
                            break;
                        case "AIT":
                            // island turret spot
                            break;
                        
                        default:
                            break;
                    } 
                }

            }
        }
    }
}