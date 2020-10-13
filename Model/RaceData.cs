using System;
using System.Collections.Generic;
using System.Text;

namespace Model {
    class RaceData<T> {

        private List<T> _list = new List<T>();

        public void AddToList(T item) {
            _list.Add(item);
        }

    }
}
