using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;

namespace RaceSimulatorGUI {
    public class MainWindowDataContext : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public string TrackName { get => $"Track: {Data.CurrentRace.Track.Name}"; } 

        public MainWindowDataContext() {
            if(Data.CurrentRace != null)
                Data.CurrentRace.DriversChanged += OnDriversChanged;
        }

        public void OnDriversChanged(object sender, EventArgs e) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
