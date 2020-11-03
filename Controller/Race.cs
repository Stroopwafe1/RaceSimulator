using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;

namespace Controller {
	public class Race {
		public Track Track { get; set; }
		public List<IParticipant> Participants { get; set; }
		public DateTime StartTime { get; set; }
		private Random random;
		private Dictionary<Section, SectionData> _positions;
		private Timer _timer;
		private Dictionary<int, IParticipant> _ranking;
		private Dictionary<int, IParticipant> _rankingCache;
		private Dictionary<int, IParticipant> _finalRanking;
		private const int SECTION_LENGTH = 239;
		private const int INNER_CORNER_LENGTH = 75;
		private Dictionary<IParticipant, bool> _finished;
		private Dictionary<IParticipant, int> _completedLaps;

		private Dictionary<IParticipant, TimeSpan> _currentTimeOnSection;

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		public event EventHandler DriversChanged;
		public event EventHandler RaceFinished;
		public static event EventHandler RaceStarted;

		public SectionData GetSectionData(Section section) {
			if (_positions.TryGetValue(section, out SectionData returnValue))
				return returnValue;
			returnValue = new SectionData();
			_positions.Add(section, returnValue);
			return returnValue;
		}

		public Race(Track track, List<IParticipant> participants) {
			Track = track;
			Participants = participants;
			random = new Random(DateTime.Now.Millisecond);
			_positions = new Dictionary<Section, SectionData>();
			_ranking = new Dictionary<int, IParticipant>();
			_rankingCache = new Dictionary<int, IParticipant>();
			_finalRanking = new Dictionary<int, IParticipant>();
			_finished = new Dictionary<IParticipant, bool>();
			_completedLaps = new Dictionary<IParticipant, int>();
			_currentTimeOnSection = new Dictionary<IParticipant, TimeSpan>();
			_timer = new Timer(500);
			_timer.Elapsed += OnTimedEvent;
			InitialiseSectionData();
			ResetParticipantLaps();
			PlaceParticipantsOnTrack();
			RandomiseEquipment();
			FillDictionaries();
			Start();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e) {
			MoveParticipants();
			HasEquipmentFailed();
			RepairEquipment();
		}

		private void FillDictionaries() {
			foreach(IParticipant participant in Participants) {
				_currentTimeOnSection.Add(participant, DateTime.Now.TimeOfDay);
            }
        }

		private void Start() {
			RaceStarted?.Invoke(this, new RaceStartedEventArgs() { Race = this });
			_timer.Start();
		}

		public void DisposeEventHandler() {
			DriversChanged = null;
			RaceFinished = null;
		}

		private void ResetParticipantLaps() {
			Participants.ForEach(_participant => _participant.Points = 0);
		}

		private void RandomiseEquipment() {
			Participants.ForEach(_participant => {
				_participant.Equipment.Speed = random.Next(7, 13);
				_participant.Equipment.Performance = random.Next(6, 13);
				_participant.Equipment.Quality = random.Next(74, 100) + 1;
			});
		}

		private void HasEquipmentFailed() {
			byte chance = 5;
			foreach (IParticipant participant in Participants) {
				if (participant.Equipment.IsBroken) continue;
				if (random.Next(0, 1000) < chance) {
					participant.Equipment.IsBroken = true;
					Data.Competition.AddParticipantBrokenDown(participant, 1);

					//Impact the performance of the equipment
					participant.Equipment.Quality -= 5;
					if (participant.Equipment.Quality <= 0)
						participant.Equipment.Quality = 0;
				}
			}
		}

		private void RepairEquipment() {
			byte chance = 20;
			foreach (IParticipant participant in Participants) {
				if (!participant.Equipment.IsBroken) continue;
				participant.Equipment.IsBroken = !(random.Next(0, 100) < chance);
			}
		}

		private void CompleteLap(IParticipant participant) {
			if (!_completedLaps.ContainsKey(participant)) {
				_completedLaps.Add(participant, 0);
			}
			_completedLaps[participant]++;
		}

		public Dictionary<int, IParticipant> GetFinalRanking() {
			return _finalRanking;
		}

		private void MoveParticipants() {
			bool driversChanged = false;
			for (int i = 1; i <= _ranking.Count; i++) {
				if (_ranking[i].Equipment.IsBroken) continue;
				var data = GetSectionDataByParticipant(_ranking[i]);
				if (data == null)
					continue;
				float velocity = (_ranking[i].Equipment.Performance * _ranking[i].Equipment.Speed) *
					((_ranking[i].Equipment.Quality * (float)Math.Sqrt(_ranking[i].Equipment.Quality) / 1000f)) + 10f;
				var section = GetSectionBySectionData(data);
				bool isCorner = section.SectionType == SectionTypes.LeftCorner || section.SectionType == SectionTypes.RightCorner;

				//Visual: https://www.wolframalpha.com/input/?i=%289*9%29*%28%28x+*+sqrt%28x%29%29%2F1000%29%2B5+from+x%3D0+to+100

				if (data.Left == _ranking[i]) {
					data.DistanceLeft += (int)Math.Ceiling(velocity);
					bool isInInnerCorner = section.SectionType == SectionTypes.LeftCorner;
					int outerCornerLength = isCorner && !isInInnerCorner ? 80 : 0;
					if (data.DistanceLeft >= (SECTION_LENGTH - outerCornerLength) || (isInInnerCorner && data.DistanceLeft >= INNER_CORNER_LENGTH)) {
						if (section.SectionType == SectionTypes.Finish && !_finished[_ranking[i]]) {
							//Participant has completed a lap
							CompleteLap(_ranking[i]);
							_finished[_ranking[i]] = true;
							if (_completedLaps[_ranking[i]] > 2) {
								_finalRanking.Add(_finalRanking.Count + 1, _ranking[i]);
								data.Left = null;
								driversChanged = true;
								continue;
							}
						}

						var nextSection = GetNextSection(section);
						if (_positions[nextSection].Left == null) {
							_positions[nextSection].Left = _ranking[i];
							_positions[nextSection].DistanceLeft = 0;
						} else if (_positions[nextSection].Right == null) {
							_positions[nextSection].Right = _ranking[i];
							_positions[nextSection].DistanceRight = 0;
						} else
							continue;
						data.Left = null;
						driversChanged = true;
						SaveSectionTime(_ranking[i], section);
						ResetTime(_ranking[i]);
						_finished[_ranking[i]] = false;
					} else {
						//Driver still on the current section
						AddTimeToParticipant(_ranking[i], DateTime.Now.TimeOfDay);
                    }
				} else {
					data.DistanceRight += (int)Math.Ceiling(velocity);
					bool isInInnerCorner = section.SectionType == SectionTypes.RightCorner;
					int outerCornerLength = isCorner && !isInInnerCorner ? 80 : 0;
					if (data.DistanceRight >= (SECTION_LENGTH - outerCornerLength) || (isInInnerCorner && data.DistanceRight >= INNER_CORNER_LENGTH)) {
						if (section.SectionType == SectionTypes.Finish && !_finished[_ranking[i]]) {
							//Participant has completed a lap
							CompleteLap(_ranking[i]);
							_finished[_ranking[i]] = true;
							if (_completedLaps[_ranking[i]] > 2) {
								_finalRanking.Add(_finalRanking.Count + 1, _ranking[i]);
								data.Right = null;
								driversChanged = true;
								continue;
							}
						}

						var nextSection = GetNextSection(section);
						if (_positions[nextSection].Right == null) {
							_positions[nextSection].Right = _ranking[i];
							_positions[nextSection].DistanceRight = 0;
						} else if (_positions[nextSection].Left == null) {
							_positions[nextSection].Left = _ranking[i];
							_positions[nextSection].DistanceLeft = 0;
						} else
							continue;
						data.Right = null;
						driversChanged = true;
						SaveSectionTime(_ranking[i], section);
						ResetTime(_ranking[i]);
						_finished[_ranking[i]] = false;
					} else {
						//Participant still on the section
						AddTimeToParticipant(_ranking[i], DateTime.Now.TimeOfDay);
                    }
				}
			}

			if (AllPlayersFinished()) {
				RaceFinished?.Invoke(this, new EventArgs());
			}

			if (GetConsoleWindow() == IntPtr.Zero)
				DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
			if (driversChanged) {
				_rankingCache = new Dictionary<int, IParticipant>(_ranking);
				DetermineRanking();
				DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
			}
		}

		public bool AllPlayersFinished() {
			return _ranking.All(rank => rank.Value.Points > 2);
		}

		private void SaveSectionTime(IParticipant participant, Section section) {
			Data.Competition.AddSectionTime(participant, _currentTimeOnSection[participant], section);
        }

		private void ResetTime(IParticipant participant) {
			_currentTimeOnSection[participant] = DateTime.Now.TimeOfDay;
        }

		private void AddTimeToParticipant(IParticipant participant, TimeSpan time) {
			_currentTimeOnSection[participant].Add(time);
        }

		private void InitialiseSectionData() {
			foreach (Section section in Track.Sections) {
				_positions.Add(section, new SectionData());
			}
		}

		public SectionData GetSectionDataByParticipant(IParticipant participant) {
			foreach (Section section in Track.Sections) {
				SectionData data = _positions[section];
				if (data.Left == null && data.Right == null) continue;
				if (data.Left != participant && data.Right != participant) continue;
				return data;
			}

			return null;
		}

		public Section GetSectionBySectionData(SectionData data) {
			Section returnValue = Track.Sections.FirstOrDefault(_section => GetSectionData(_section) == data);
			return returnValue;
		}

		public Section GetNextSection(Section section) {
			Section returnValue = Track.Sections.Find(section)?.Next?.Value;
			if (returnValue == null)
				returnValue = Track.Sections.First.Value;
			return returnValue;
		}

		private void DetermineRanking() {
			_ranking.Clear();
			int pos = 1;
			for (var sectionNode = Track.Sections.Last; sectionNode != null; sectionNode = sectionNode.Previous) {
				Section section = sectionNode.Value;
				SectionData data = _positions[section];
				if (data.Left == null && data.Right == null)
					continue;
				if (data.Left != null) {
					_ranking.Add(pos, data.Left);
					pos++;
				}

				if (data.Right != null) {
					_ranking.Add(pos, data.Right);
					pos++;
				}
			}
			DetermineOvertakers();
		}

		private void DetermineOvertakers() {
			for(int i = 1; i <= _ranking.Count; i++) {
				if (_ranking[i] == _rankingCache[i]) continue;
				int prevPosition = _rankingCache.FirstOrDefault(x => x.Value == _ranking[i]).Key;
				//If the current position is lower than the previous, this player was overtaken... So we don't care about them for now
				if (prevPosition > i) continue; 

				//Previous position was lower than the current position, so this player has overtaken someone
				//Expecting the difference between i and prevPosition to always be 1
				//The overtaken driver should be _rankingCache[i]???
				Data.Competition.AddParticipantOvertaken(_ranking[i], _rankingCache[i]);
            }
        }

		public void PlaceParticipantsOnTrack() {
			SortedDictionary<int, Section> helpDict = new SortedDictionary<int, Section>();
			int counter = 0;
			var startNode = Track.Sections.First;
			if (startNode.Value.SectionType != SectionTypes.StartGrid)
				throw new Exception("First section should be a start!");
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
				for (int j = 2 * i; j <= 2 * i + 1; j++) {
					if (j >= Participants.Count) break;
					_ranking.Add(j + 1, Participants[j]);
					_finished.Add(Participants[j], false);

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