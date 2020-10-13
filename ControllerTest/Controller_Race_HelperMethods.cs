using System.Collections.Generic;
using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest {
	
	[TestFixture]
	public class Controller_Race_HelperMethods {

		private Race race;
		private Track track;
		private List<IParticipant> participants;
		
		[SetUp]
		public void Setup() {
			track = new Track("Test track", new [] {SectionTypes.StartGrid, SectionTypes.Finish});
			participants = new List<IParticipant>() {new Driver("Bob", new Car()), new Driver("Terry", new Car())};
			race = new Race(track, participants);
		}

		[Test]
		public void AllPlayersFinished_PlayersNotFinished() {
			bool actual = race.AllPlayersFinished();
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void AllPlayersFinished_PlayersFinished() {
			race.Participants.ForEach(_participant => _participant.Points = 3);
			bool actual = race.AllPlayersFinished();
			Assert.IsTrue(actual);
		}

		[Test]
		public void GetSectionDataByParticipant_ParticipantNotOnTrack_ReturnNull() {
			participants.Add(new Driver("Unknown", new Car()));
			SectionData sectionData = race.GetSectionDataByParticipant(participants[2]);
			Assert.IsNull(sectionData);
		}

		[Test]
		public void GetSectionDataByParticipant_ParticipantOnTrack_ReturnData() {
			SectionData expected = race.GetSectionData(track.Sections?.First?.Value);
			SectionData actual = race.GetSectionDataByParticipant(participants[0]);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetSectionData_KnownKey_ReturnData() {
			SectionData result = race.GetSectionData(track.Sections.First?.Value);
			Assert.IsNotNull(result);
		}

		[Test]
		public void GetSectionData_NewKey_ReturnData() {
			Section newSection = new Section(SectionTypes.Straight);
			SectionData result = race.GetSectionData(newSection);
			Assert.IsNotNull(result);
		}

		[Test]
		public void GetSectionBySectionData_UnknownData_ReturnNull() {
			SectionData data = new SectionData();
			Section result = race.GetSectionBySectionData(data);
			Assert.IsNull(result);
		}

		[Test]
		public void GetSectionBySectionData_KnownData_ReturnSection() {
			Section expected = track.Sections.First?.Value;
			SectionData data = race.GetSectionData(expected);
			Section actual = race.GetSectionBySectionData(data);
			Assert.AreEqual(expected, actual);
		}
	}
}