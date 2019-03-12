using System.Collections.Generic;

namespace Genus2D.Utililities
{
    /// <summary>
    /// Add to it and it recurses from newest to oldest.
    /// </summary>
    public class ReversedStack<T>
    {
        private List<T> _entries = new List<T>();

        public void Add(T pEntry)
        {
            _entries.Add(pEntry);
        }

        public List<T> Get(int pMaxNumberOfEntries = 32)
        {
            int endIndex = _entries.Count;
            int startIndex = endIndex - pMaxNumberOfEntries;

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (pMaxNumberOfEntries > _entries.Count)
            {
                pMaxNumberOfEntries = _entries.Count;
            }

            List<T> entries = _entries.GetRange(startIndex, pMaxNumberOfEntries);
            entries.Reverse();
            return entries;
        }
    }
}