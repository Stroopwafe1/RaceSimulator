using System.Collections.Generic;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest {
	[TestFixture]
	public class Controller_Data_ParticipantsOrderFinish {

		private Competition _competition;

		[SetUp]
		public void Setup() {
			_competition = new Competition();
			Data.Initialise(_competition);
		}

		[Test]
		public void PutParticipantsInOrderOfFinish_NoCurrentRace_ListStaysSame() {
			Data.CurrentRace = null;
			List<IParticipant> before = new List<IParticipant>(_competition.Participants);
			Data.PutParticipantsInOrderOfFinish();
			List<IParticipant> after = new List<IParticipant>(_competition.Participants);
			CollectionAssert.AreEqual(before, after);
		}

		[Test]
		public void PutParticipantsInOrderOfFinish_CurrentRace_DifferentOrder() {
			List<IParticipant> before = new List<IParticipant>(_competition.Participants);
			Data.NextRace();
			Dictionary<int, IParticipant> finalRanking = new Dictionary<int, IParticipant>();
			Data.CurrentRace.Participants.Reverse();
			for (int i = 0; i < Data.CurrentRace.Participants.Count; i++) {
				finalRanking.Add(i + 1, Data.CurrentRace.Participants[i]);
			}
			Data.CurrentRace.SetFinalRanking(finalRanking);
			Data.PutParticipantsInOrderOfFinish();
			List<IParticipant> after = new List<IParticipant>(_competition.Participants);
			CollectionAssert.AreNotEqual(before, after);
		}
	}
}