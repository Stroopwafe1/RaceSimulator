using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RaceSimulatorGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Data.Initialise(new Competition());
            Race.RaceStarted += OnRaceStarted;
            Data.NextRace();
            Visualiser.Initialise();

            Data.CurrentRace.DriversChanged += OnDriversChanged;
        }

        public void OnDriversChanged(object sender, EventArgs e) {
            DriversChangedEventArgs e1 = (DriversChangedEventArgs)e;
            this.TrackImage.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => {
                this.TrackImage.Source = null;
                this.TrackImage.Source = Visualiser.DrawTrack(e1.Track);
            }));
        }

        public void OnRaceStarted(object sender, EventArgs e) {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.DriversChanged += OnDriversChanged;
            ImageLoader.ClearCache();
            this.TrackImage.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => {
                this.TrackImage.Source = null;
                this.TrackImage.Source = Visualiser.DrawTrack(e1.Race.Track);
            }));
        }
    }
}
