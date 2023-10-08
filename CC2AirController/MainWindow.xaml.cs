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

            foreach (var island in _reader.Islands)
            {
                island.Draw(Viewport);
            }

            foreach (var unit in _reader.Plots)
            {
                unit.Draw(Viewport);
            }
            
            var p1 = new Plot();
            p1.Loc = new Location()
            {
                X = 1000,
                Y = 1000
            };
        }

        public MainWindow()
        {
            InitializeComponent();
            Viewport.Area.X = -1000;
            Viewport.Area.Y = -1000;
            Viewport.Area.Width = 10000;

            var p1 = new Plot()
            {
                Id = "999"
            };
            p1.Loc = new Location()
            {
                X = -5010,
                Y = -3000
            };
            _reader.AddPlot(p1);
            
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