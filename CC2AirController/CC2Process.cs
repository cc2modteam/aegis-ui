using System.Diagnostics;
using System.IO;
using System.Text;

namespace CC2AirController
{
    public class Cc2Process
    {
        private Process _cc2 = null;
        public string Cc2Folder { get; set; } = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Carrier Command 2";
        public string Cc2Exe { get; set; } = "carrier_command.exe";
        private readonly object _lck = new object();

        public void Start()
        {
            var psi = new ProcessStartInfo(Path.Combine(Cc2Folder, Cc2Exe))
            {
                UseShellExecute = false,
                Arguments = "-dev",
                WorkingDirectory = Cc2Folder,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            _cc2 = Process.Start(psi);
        }

        public StreamReader OutputStream
        {
            get
            {
                if (_cc2 != null)
                {
                    return _cc2.StandardOutput;
                }

                return null;
            }
        }

        public void Stop()
        {
            if (_cc2 != null)
            {
                lock (_lck)
                {
                    _cc2.Kill();
                    _cc2.WaitForExit();
                    _cc2 = null;    
                }
            }
        }
    }
}