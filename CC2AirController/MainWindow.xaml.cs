using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CC2AirController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
       
        private readonly PlotReader _reader = new PlotReader();
        private Cc2Process _cc2 = new Cc2Process();

        protected override void OnClosing(CancelEventArgs e)
        {
            _cc2.Stop();
            base.OnClosing(e);
        }

        void UpdatePlots(object sender, EventArgs e)
        {
            Viewport.ClearLayer("shapes");
            Viewport.ClearLayer("islands");
            
            var p1 = new Plot();
            p1.Loc = new Location()
            {
                X = -5010,
                Y = -3000
            };
            p1.Draw(Viewport);
            
            var p2 = new Plot();
            p2.Loc = new Location()
            {
                X = -4500,
                Y = -4000
            };
            p2.Draw(Viewport);

            var i1 = new Island();
            i1.Name = "Hades";
            i1.Loc.X = -5000;
            i1.Loc.Y = -5000;
            i1.Draw(Viewport);
        }

        public MainWindow()
        {
            InitializeComponent();
            Viewport.Area.X = -8000;
            Viewport.Area.Y = -8000;
            Viewport.Area.Width = 10000;
            
            CompositionTarget.Rendering += UpdatePlots;
        }

        private void ClickStartCc2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _cc2.Cc2Folder,
                FileName = _cc2.Cc2Exe
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var filename = openFileDialog.FileName;
                var folder = System.IO.Directory.GetParent(filename);
                var basename = System.IO.Path.GetFileName(filename);
                if (folder != null)
                {
                    _cc2.Cc2Folder = folder.FullName;
                    _cc2.Cc2Exe = basename;
                }
                _cc2.Start();
                _reader.GameOutput = _cc2.OutputStream;
                _reader.Start();
                StartButton.IsEnabled = false;
            }
        }
    }
}