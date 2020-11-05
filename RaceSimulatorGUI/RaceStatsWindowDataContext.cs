using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RaceSimulatorGUI {
    public class RaceStatsWindowDataContext : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<ParticipantRankingDisplay> _rankingDisplay;
        public List<ParticipantRankingDisplay> RankingDisplay { get => DetermineRanking(_rankingDisplay); set => _rankingDisplay = value; }
        private List<ParticipantLapTimeDisplay> _lapTimeDisplay;
        public List<ParticipantLapTimeDisplay> LapTimeDisplay { get => DetermineLapTime(_lapTimeDisplay); set => _lapTimeDisplay = value; }

        public string BestOvertaker { get => $"Best overtaker: {Data.Competition.ParticipantsOvertaken.GetBestParticipant()}"; }

        public RaceStatsWindowDataContext() {
            RankingDisplay = new List<ParticipantRankingDisplay>();
            LapTimeDisplay = new List<ParticipantLapTimeDisplay>();
            Race.RaceStarted += OnRaceStarted;
            if (Data.CurrentRace != null)
                Data.CurrentRace.DriversChanged += OnDriversChanged;
        }

        ~RaceStatsWindowDataContext() {
            Race.RaceStarted -= OnRaceStarted;
            Data.CurrentRace.DriversChanged -= OnDriversChanged;
        }

        public void OnDriversChanged(object sender, EventArgs e) {
            DetermineRanking(RankingDisplay);
            DetermineLapTime(LapTimeDisplay);
            Debug.WriteLine($"RaceStats Driverschanged {sender.GetType()}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        private List<ParticipantRankingDisplay> DetermineRanking(List<ParticipantRankingDisplay> display) {
            display.Clear();
            Data.Competition.Participants.ForEach(participant => display.Add(new ParticipantRankingDisplay(participant, Data.CurrentRace.GetRankingOfParticipant(participant), Data.CurrentRace.GetIsFinished(participant))));
            display = display.OrderBy(p => p.Place).ToList();
            for (int i = 1; i <= display.Count; i++)
                display[i - 1].Place = i;
            return display;
        }

        private List<ParticipantLapTimeDisplay> DetermineLapTime(List<ParticipantLapTimeDisplay> display) {
            display.Clear();
            Data.Competition.Participants.ForEach(participant => display.Add(new ParticipantLapTimeDisplay(participant, display.Count + 1, Data.CurrentRace.GetIsFinished(participant))));
            display = display.OrderBy(p => p.TimeSpan).ToList();
            for (int i = 1; i <= display.Count; i++)
                display[i - 1].Place = i;
            return display;
        }

        public void OnRaceStarted(object sender, EventArgs e) {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.DriversChanged += OnDriversChanged;
        }
    }
}

