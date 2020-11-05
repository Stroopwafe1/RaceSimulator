using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RaceSimulatorGUI {
    public class CompetitionStatsWindowDataContext : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        public List<ParticipantPointDisplay> ParticipantPoints { get; set; }

        public CompetitionStatsWindowDataContext() {
            ParticipantPoints = new List<ParticipantPointDisplay>();
            DetermineRanking();
            Race.RaceStarted += OnRaceStarted;
            if (Data.CurrentRace != null)
                Data.CurrentRace.RaceFinished += OnRaceFinished;
        }

        public void OnRaceFinished(object sender, EventArgs e) {
            DetermineRanking();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public void UpdateList() {
            DetermineRanking();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        private void DetermineRanking() {
            ParticipantPoints.Clear();
            Data.Competition.Participants.ForEach(participant => ParticipantPoints.Add(new ParticipantPointDisplay(participant, ParticipantPoints.Count + 1)));
            ParticipantPoints = ParticipantPoints.OrderByDescending(p => p.Points).ToList();
            for (int i = 1; i <= ParticipantPoints.Count; i++)
                ParticipantPoints[i - 1].Place = i;
        }

        public void OnRaceStarted(object sender, EventArgs e) {
            RaceStartedEventArgs e1 = (RaceStartedEventArgs)e;
            e1.Race.RaceFinished += OnRaceFinished;
        }
    }
}
