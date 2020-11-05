using System;
using System.Collections.Generic;
using Model;
using NUnit.Framework;

namespace ControllerTest {
	
	[TestFixture]
	public class Model_RaceData {

		private RaceData<ParticipantPoints> _participantPoints;
		private RaceData<ParticipantTime> _participantTime;
		private RaceData<ParticipantsOvertaken> _participantsOvertaken;
		private RaceData<ParticipantTimesBrokenDown> _particpantTimesBrokenDown;
		private RaceData<ParticipantSectionTime> _participantSectionTime;
		private List<IParticipant> _participants;
		
		[SetUp]
		public void Setup() {
			_participantPoints = new RaceData<ParticipantPoints>();
			_participantTime = new RaceData<ParticipantTime>();
			_participantsOvertaken = new RaceData<ParticipantsOvertaken>();
			_particpantTimesBrokenDown = new RaceData<ParticipantTimesBrokenDown>();
			_participantSectionTime = new RaceData<ParticipantSectionTime>();
			_participants = new List<IParticipant> {new Driver("Tester1", new Car()), new Driver("Tester2", new Car())};
		}

		[Test]
		public void AddItem_ListShouldContainThatItem() {
			var points = new ParticipantPoints() {Name = "Tester1", Points = 5, Participant = _participants[0]};
			var time = new ParticipantTime() { Name = "Tester1", Time = DateTime.Now.TimeOfDay, Participant = _participants[0]};
			var sectionTime = new ParticipantSectionTime() {
				Name = "Tester1", Section = new Section(SectionTypes.StartGrid), Participant = _participants[0], Time = DateTime.Now.TimeOfDay
			};
			var overtaken = new ParticipantsOvertaken("Tester1", "Tester2") {Participant = _participants[0]};
			var brokenDown = new ParticipantTimesBrokenDown() {Name = "Tester1", Count = 1, Participant = _participants[0]};
			_participantPoints.AddToList(points);
			_participantTime.AddToList(time);
			_participantsOvertaken.AddToList(overtaken);
			_particpantTimesBrokenDown.AddToList(brokenDown);
			_participantSectionTime.AddToList(sectionTime);
			int expected = 1;
			int pointsActual = _participantPoints.GetListCount();
			int timeActual = _participantTime.GetListCount();
			int overtakenActual = _participantsOvertaken.GetListCount();
			int brokenDownActual = _particpantTimesBrokenDown.GetListCount();
			int sectionTimeActual = _participantSectionTime.GetListCount();
			Assert.AreEqual(expected, pointsActual);
			Assert.AreEqual(expected, timeActual);
			Assert.AreEqual(expected, overtakenActual);
			Assert.AreEqual(expected, brokenDownActual);
			Assert.AreEqual(expected, sectionTimeActual);
		}

		[Test]
		public void BestParticipant_Points() {
			var expected = new ParticipantPoints() {Name = "Tester1", Points = 5, Participant = _participants[0]};
			var test = new ParticipantPoints() {Name = "Tester2", Points = 3, Participant = _participants[1]};
			_participantPoints.AddToList(expected);
			_participantPoints.AddToList(test);
			var actual = _participantPoints.GetBestParticipant();
			Assert.AreEqual(expected.Name, actual);
		}

		[Test]
		public void BestParticipant_Overtaken() {
			_participantsOvertaken.AddToList(new ParticipantsOvertaken("Tester1", "Tester2") {Participant = _participants[0]});
			_participantsOvertaken.AddToList(new ParticipantsOvertaken("Tester2", "Tester1") {Participant = _participants[1]});
			_participantsOvertaken.AddToList(new ParticipantsOvertaken("Tester1", "Tester2") {Participant = _participants[0]});
			var expected = _participants[0].Name;
			var actual = _participantsOvertaken.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void BestParticipant_Time() {
			_participantTime.AddToList(new ParticipantTime() { Name = "Tester1", Time = new TimeSpan(0, 5, 30), Participant = _participants[0]});
			_participantTime.AddToList(new ParticipantTime() { Name = "Tester2", Time = new TimeSpan(0, 7, 45), Participant = _participants[1]});
			var expected = _participants[0].Name;
			var actual = _participantTime.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void BestParticipant_BrokenDown() {
			_particpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() {Name = "Tester1", Count = 1, Participant = _participants[0]});
			_particpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() {Name = "Tester2", Count = 2, Participant = _participants[1]});
			_particpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() {Name = "Tester2", Count = 1, Participant = _participants[1]});
			var expected = _participants[0].Name;
			var actual = _particpantTimesBrokenDown.GetBestParticipant();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetParticipantData() {
			_particpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() {Name = "Tester1", Count = 1, Participant = _participants[0]});
			_particpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() {Name = "Tester2", Count = 2, Participant = _participants[1]});
			_particpantTimesBrokenDown.AddToList(new ParticipantTimesBrokenDown() {Name = "Tester2", Count = 1, Participant = _participants[1]});
			var expected = _participants[0].Name;
			var actual = _particpantTimesBrokenDown.GetParticipantData(_participants[0]).Participant.Name;
			Assert.AreEqual(expected, actual);
		}
	}
}