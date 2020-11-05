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
		private Random Random;
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
		private Dictionary<IParticipant, DateTime> _sectionTimeCache;
		private Dictionary<IParticipant, DateTime> _lapTimeCache;

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
			Random = new Random(DateTime.Now.Millisecond);
			_positions = new Dictionary<Section, SectionData>();
			_ranking = new Dictionary<int, IParticipant>();
			_rankingCache = new Dictionary<int, IParticipant>();
			_finalRanking = new Dictionary<int, IParticipant>();
			_finished = new Dictionary<IParticipant, bool>();
			_completedLaps = new Dictionary<IParticipant, int>();
			_currentTimeOnSection = new Dictionary<IParticipant, TimeSpan>();
			_sectionTimeCache = new Dictionary<IParticipant, DateTime>();
			_lapTimeCache = new Dictionary<IParticipant, DateTime>();
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
			MoveParticipants(e.SignalTime);
			HasEquipmentFailed();
			RepairEquipment();
		}

		private void FillDictionaries() {
			foreach (IParticipant participant in Participants) {
				_sectionTimeCache.Add(participant, DateTime.Now);
				_currentTimeOnSection.Add(participant, TimeSpan.Zero);
				_lapTimeCache.Add(participant, DateTime.Now);
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
				_participant.Equipment.Speed = Random.Next(7, 13);
				_participant.Equipment.Performance = Random.Next(6, 13);
				_participant.Equipment.Quality = Random.Next(74, 100) + 1;
			});
		}

		private void HasEquipmentFailed() {
			byte chance = 5;
			foreach (IParticipant participant in Participants) {
				if (participant.Equipment.IsBroken) continue;
				if (Random.Next(0, 1000) < chance) {
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
				participant.Equipment.IsBroken = !(Random.Next(0, 100) < chance);
			}
		}

		private void CompleteLap(IParticipant participant, DateTime time) {
			if (!_completedLaps.ContainsKey(participant)) {
				_completedLaps.Add(participant, 0);
			} else {
				AddLapTimeToParticipant(participant, time);
				ResetLapTime(participant, time);
			}
			_completedLaps[participant]++;
		}

		public int GetRankingOfParticipant(IParticipant participant) {
			return _ranking.FirstOrDefault(p => p.Value.Name == participant.Name).Key;
		}

		public Dictionary<int, IParticipant> GetFinalRanking() {
			return _finalRanking;
		}

		/// <summary>
		/// This is only used for testing purposes
		/// </summary>
		public void SetFinalRanking(Dictionary<int, IParticipant> finalRanking) {
			_finalRanking = finalRanking;
		}

		public bool MoveParticipant(IParticipant participant, SectionData data, float velocity, bool isLeft, bool isInInnerCorner, Section section, DateTime time) {
			bool isCorner = section.SectionType == SectionTypes.LeftCorner || section.SectionType == SectionTypes.RightCorner;
			int distance = isLeft ? data.DistanceLeft : data.DistanceRight;
			distance += (int)Math.Ceiling(velocity);
			if (isLeft)
				data.DistanceLeft = distance;
			else
				data.DistanceRight = distance;
			int outerCornerLength = isCorner && !isInInnerCorner ? 80 : 0;
			if (distance >= (SECTION_LENGTH - outerCornerLength) || (isInInnerCorner && distance >= INNER_CORNER_LENGTH)) {
				if (section.SectionType == SectionTypes.Finish && !_finished[participant]) {
					//Participant has completed a lap
					CompleteLap(participant, time);
					_finished[participant] = true;
					if (_completedLaps[participant] > 2) {
						_finalRanking.Add(_finalRanking.Count + 1, participant);
						if (isLeft)
							data.Left = null;
						else
							data.Right = null;
						return true;
					}
				}

				var nextSection = GetNextSection(section);
				if (_positions[nextSection].Left == null) {
					_positions[nextSection].Left = participant;
					_positions[nextSection].DistanceLeft = 0;
				} else if (_positions[nextSection].Right == null) {
					_positions[nextSection].Right = participant;
					_positions[nextSection].DistanceRight = 0;
				} else {
					AddTimeToParticipant(participant, time);
					return false;
				}
				if (isLeft)
					data.Left = null;
				else
					data.Right = null;
				SaveSectionTime(participant, section);
				ResetTime(participant, time);
				_finished[participant] = false;
				return true;
			} else {
				//Driver still on the current section
				AddTimeToParticipant(participant, time);
				return false;
			}
		}

		public void MoveParticipants(DateTime time) {
			bool driversChanged = false;
			for (int i = 1; i <= _ranking.Count; i++) {
				if (_ranking[i].Equipment.IsBroken) continue;
				var data = GetSectionDataByParticipant(_ranking[i]);
				if (data == null)
					continue;
				float velocity = (_ranking[i].Equipment.Performance * _ranking[i].Equipment.Speed) *
					((_ranking[i].Equipment.Quality * (float)Math.Sqrt(_ranking[i].Equipment.Quality) / 1000f)) + 10f;
				var section = GetSectionBySectionData(data);

				//Visual: https://www.wolframalpha.com/input/?i=%289*9%29*%28%28x+*+sqrt%28x%29%29%2F1000%29%2B5+from+x%3D0+to+100

				bool isLeft = data.Left == _ranking[i];
				bool driversChangedTemp = MoveParticipant(_ranking[i], data, velocity, isLeft,
					isLeft && section.SectionType == SectionTypes.LeftCorner, section, time);
				driversChanged = driversChangedTemp || driversChanged;
			}

			if (AllPlayersFinished()) {
				RaceFinished?.Invoke(this, new EventArgs());
			}

			if (driversChanged) {
				_rankingCache = new Dictionary<int, IParticipant>(_ranking);
				DetermineRanking();
				if(GetConsoleWindow() != IntPtr.Zero)
					DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
			}
			if (GetConsoleWindow() == IntPtr.Zero) {
				DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
			}
		}

		public bool GetIsFinished(IParticipant participant) {
			return _finished[participant];
		}

		public bool AllPlayersFinished() {
			return _ranking.All(rank => rank.Value.Points > 2);
		}

		private void SaveSectionTime(IParticipant participant, Section section) {
			Data.Competition.AddSectionTime(participant, _currentTimeOnSection[participant], section);
		}

		private void ResetTime(IParticipant participant, DateTime time) {
			_sectionTimeCache[participant] = time;
			_currentTimeOnSection[participant] = TimeSpan.Zero;
		}

		private void ResetLapTime(IParticipant participant, DateTime time) {
			_lapTimeCache[participant] = time;
		}

		private void AddTimeToParticipant(IParticipant participant, DateTime time) {
			DateTime cache = _sectionTimeCache[participant];
			_currentTimeOnSection[participant] = time - cache;
		}

		private void AddLapTimeToParticipant(IParticipant participant, DateTime time) {
			DateTime cache = _lapTimeCache[participant];
			Data.Competition.AddLapTime(participant, Track, time - cache);
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
			Section returnValue = Track.Sections.Find(section)?.Next?.Value ?? Track.Sections.First.Value;
			return returnValue;
		}

		public void DetermineRanking() {
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
			for (int i = 1; i <= _ranking.Count; i++) {
				if(!_rankingCache.ContainsKey(i)) continue;
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

		public Dictionary<int, IParticipant> GetRanking() {
			return _ranking;
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