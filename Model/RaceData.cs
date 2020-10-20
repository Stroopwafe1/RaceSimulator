using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model {
    public class RaceData<T> where T : IParticipantData {

        private List<T> _list = new List<T>();

        public void AddToList(T item) {
            item.Add(_list.Cast<IParticipantData>().ToList());
        }

        public string GetBestParticipant() {
            if (_list.Count == 0) return "";
            return _list[0].GetBestParticipant(_list.Cast<IParticipantData>().ToList());
        }
    }
}
