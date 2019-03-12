using System.Collections.Generic;

namespace Genus2D.Utililities
{
    /// <summary>
    /// Add things to it, move the cursor left to right and it wraps around. Get values at the cursor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularQueue<T>
    {
        private List<T> _entries = new List<T>();
        private int _entryCursor = 0;

        public CircularQueue()
        {
            _entries = new List<T>();
        }

        public void Add(T pLogEntry)
        {
            _entries.Add(pLogEntry);
        }

        public T GetAtCursor()
        {
            return _entries[_entryCursor];
        }

        public void MoveCursorLeft()
        {
            _entryCursor--;

            if (_entryCursor < 0)
            {
                _entryCursor = _entries.Count - 1;
            }
        }

        public void MoveCursorRight()
        {
            _entryCursor++;

            if (_entryCursor >= _entries.Count - 1)
            {
                _entryCursor = 0;
            }
        }
    }
}