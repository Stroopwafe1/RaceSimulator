﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class RaceData<T> where T : IParticipantData {

        private List<IParticipantData> _list;

        public RaceData() {
            _list = new List<IParticipantData>();
        }

        public void AddToList(T item) {
            item.Add(_list);
        }

        public int GetListCount() {
            return _list.Count;
        }

        public IParticipantData GetParticipantData(IParticipant participant) {
            return _list.FirstOrDefault(data => data.Participant.Name == participant.Name);
        }

        public string GetBestParticipant() {
            if (_list.Count == 0) return "";
            return _list[0].GetBestParticipant(_list);
        }

        public void ClearList() {
            _list.Clear();
        }
    }
}
