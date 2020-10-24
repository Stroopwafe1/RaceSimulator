using System;
using System.Collections.Generic;
using Model;
using NUnit.Framework;

namespace ControllerTest {

	[TestFixture]
	public class Model_Competition_AddingParticipantData {

		private Competition _competition;

		[SetUp]
		public void Setup() {
			_competition = new Competition();
			_competition.Participants.Add(new Driver("Tester1", new Car()));
			_competition.Participants.Add(new Driver("Tester2", new Car()));
		}

		[Test]
		public void AddPoints() {
			Dictionary<int, IParticipant> participants =
				new Dictionary<int, IParticipant> {
					{1, _competition.Participants[0]}, {2, _competition.Participants[1]}
				};
			_competition.AddPoints(participants);
			var expected = _competition.Participants[0].Name;
			var actual = _competition.ParticipantPoints.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddSectionTime() {
			Section s = new Section(SectionTypes.StartGrid);
			_competition.AddSectionTime(_competition.Participants[0], TimeSpan.FromSeconds(450), s);
			_competition.AddSectionTime(_competition.Participants[1], TimeSpan.FromSeconds(750), s);
			var expected = _competition.Participants[0].Name;
			var actual = _competition.ParticipantTime.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddOvertaken() {
			_competition.AddParticipantOvertaken(_competition.Participants[0], _competition.Participants[1]);
			_competition.AddParticipantOvertaken(_competition.Participants[1], _competition.Participants[0]);
			_competition.AddParticipantOvertaken(_competition.Participants[0], _competition.Participants[1]);
			var expected = _competition.Participants[0].Name;
			var actual = _competition.ParticipantsOvertaken.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddBrokenDown() {
			_competition.AddParticipantBrokenDown(_competition.Participants[0], 4);
			_competition.AddParticipantBrokenDown(_competition.Participants[1], 7);
			var expected = _competition.Participants[0].Name;
			var actual = _competition.ParticpantTimesBrokenDown.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}
	}
}