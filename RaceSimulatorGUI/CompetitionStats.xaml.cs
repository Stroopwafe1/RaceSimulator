using Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RaceSimulatorGUI {
    /// <summary>
    /// Interaction logic for CompetitionStats.xaml
    /// </summary>
    public partial class CompetitionStats : Window {

        private CompetitionStatsWindowDataContext _competitionStatsContext = new CompetitionStatsWindowDataContext();
        public CompetitionStats() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            _competitionStatsContext.UpdateList();
        }
    }
}
