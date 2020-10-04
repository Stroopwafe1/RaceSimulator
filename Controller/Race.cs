using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Timers;

namespace Controller {
	public class Race {
		public Track Track { get; set; }
		public List<IParticipant> Participants { get; set; }
		public DateTime StartTime { get; set; }
		private Random random;
		private Dictionary<Section, SectionData> positions;
		private Timer _timer;
		private Dictionary<int, IParticipant> _ranking;
		private const int SECTION_LENGTH = 100;

		public event EventHandler DriversChanged ;

		public SectionData GetSectionData(Section section) {
			if(positions.TryGetValue(section, out SectionData returnValue))
				return returnValue;
			returnValue = new SectionData();
			positions.Add(section, returnValue);
			return returnValue;
		}

		public Race(Track track, List<IParticipant> participants) {
			Track = track;
			Participants = participants;
			random = new Random(DateTime.Now.Millisecond);
			positions = new Dictionary<Section, SectionData>();
			_ranking = new Dictionary<int, IParticipant>();
			_timer = new Timer(500);
			_timer.Elapsed += OnTimedEvent;
			PlaceParticipantsOnTrack();
			RandomiseEquipment();
			Start();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e) {
			MoveParticipants();
		}

		private void Start() {
			_timer.Start();
		}

		public void RandomiseEquipment() {
			Participants.ForEach(_participant => {
				_participant.Equipment.Speed = random.Next(10);
				_participant.Equipment.Performance = random.Next(10);
				});
		}

		private void MoveParticipants() {
			for (int i = 1; i <= _ranking.Count; i++) {
				var data = GetSectionDataByParticipant(_ranking[i]);
				int velocity = _ranking[i].Equipment.Performance * _ranking[i].Equipment.Speed;
				if (data.Left == _ranking[i]) {
					data.DistanceLeft += velocity;
					if (data.DistanceLeft >= SECTION_LENGTH) {
						var section = GetSectionBySectionData(data);
						var nextSection = GetNextSection(section);
						if (positions[nextSection].Left == null)
							positions[nextSection].Left = _ranking[i];
						else if (positions[nextSection].Right == null)
							positions[nextSection].Right = _ranking[i];
						else
							continue;
						data.Left = null;
					} 
				} else {
					data.DistanceRight +=  velocity;
					if (data.DistanceRight >= SECTION_LENGTH) {
						var section = GetSectionBySectionData(data);
                        var nextSection = GetNextSection(section);
						if (positions[nextSection].Right == null)
							positions[nextSection].Right = _ranking[i];
						else if (positions[nextSection].Left == null)
							positions[nextSection].Left = _ranking[i];
						else
							continue;
						data.Right = null;
					}
				}
			}
			DriversChanged?.Invoke(this, new DriversChangedEventArgs() {Track = Track});
		}

		public SectionData GetSectionDataByParticipant(IParticipant participant) {
			foreach (Section section in Track.Sections) {
				SectionData data = positions[section];
				if(data.Left == null && data.Right == null) continue;
				if(data.Left != participant && data.Right != participant) continue;
				return data;
			}

			return null;
		}

		public Section GetSectionBySectionData(SectionData data) {
			Section returnValue = Track.Sections.First(_section => GetSectionData(_section) == data);
			return returnValue;
		}

		public Section GetNextSection(Section section) {
			Section returnValue = null;
			returnValue = Track.Sections.Find(section)?.Next?.Value;
			if (returnValue == null)
				returnValue = Track.Sections.First.Value;
			return returnValue;
		}

		public void PlaceParticipantsOnTrack() {
			SortedDictionary<int, Section> helpDict = new SortedDictionary<int, Section>();
			int counter = 0;
			var startNode = Track.Sections.First;
			if(startNode.Value.SectionType != SectionTypes.StartGrid) throw new Exception("First section should be a start!");
			for (var node = startNode; node != null; node = node.Next) {
				bool isStart = node.Value.SectionType == SectionTypes.StartGrid;
				if (isStart) {
					helpDict.Add(counter, node.Value);
					counter--;
				} else {
					counter++;
				}
			}
			List<Section> startSections = new List<Section>(helpDict.Values);
			
			for (int i = 0; i < startSections.Count; i++) {
				SectionData sectionData = GetSectionData(startSections[i]);
				for (int j = 2 *i; j <= 2 * i + 1; j++) {
					if (j >= Participants.Count) break;
					_ranking.Add(j + 1, Participants[j]);
					//Console.WriteLine($"Participant: {Participants[j].Name}, on {j}" );
					if (j % 2 == 0)
						sectionData.Left = Participants[j];
					else
						sectionData.Right = Participants[j];
				}
			}
		}
	}
}
