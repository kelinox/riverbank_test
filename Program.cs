using System;
using System.Diagnostics;
using System.Text;

namespace riverbank
{
    /**
    Least recently used cache
    */


    class Program
    {
        static void Main(string[] args)
        {
            var cache = new LeastRecentlyUsedCache<string>(3);
            Stopwatch s = new Stopwatch();
            s.Start();
            cache.GetElement("A");
            cache.GetElement("B");
            cache.GetElement("C");
            cache.GetElement("D");
            cache.GetElement("B");
            cache.ToString();
            s.Stop();
            Console.WriteLine($"Ellapsed time {s.ElapsedMilliseconds}ms");
        }
    }

    public class LeastRecentlyUsedCache<T>
    {
        private int maxSize;
        private LeastRecentlyUsedCacheItem<T> Root { get; set; }

        public LeastRecentlyUsedCache(int maxSize)
        {
            this.maxSize = maxSize;
        }

        // Retrieve an element from the cache
        // Add it in the cache if it is not already there
        public T GetElement(T itemToGet)
        {
            var current = Root;

            while (current != null && !current.Item.Equals(itemToGet))
            {
                current = current.Next;
            }

            current = Add(itemToGet);
            SortList(Root);
            return itemToGet;
        }

        // Sort the list from the least used to the most recently used
        private void SortList(LeastRecentlyUsedCacheItem<T> root)
        {
            if (root is null || root.Next is null)
                return;

            // If the current node has been used more recently than the next one we swap them
            if (root.UsedAt > root.Next.UsedAt)
            {
                var tmp = root.Next;
                var previous = root.Previous;

                root.Next = tmp.Next;
                root.Previous = tmp;

                tmp.Previous = previous;
                tmp.Next = previous?.Next ?? root;

                // If we swap the first item in the list there is no previous element to it
                if (previous != null)
                    previous.Next = tmp;
                else
                    Root = tmp; // We set the new root of the list

                // Sort the list with the current node
                SortList(root);
            }
            else
            {
                // Sort from the next node
                SortList(root.Next);
            }
        }

        // Add an item in the cache
        private LeastRecentlyUsedCacheItem<T> Add(T itemToAdd)
        {
            // Create the root node
            if (Root is null)
            {
                Root = new LeastRecentlyUsedCacheItem<T>
                {
                    Item = itemToAdd,
                    Next = null,
                    Previous = null,
                    UsedAt = DateTime.UtcNow
                };

                return Root;
            }
            else
            {
                // Remove an item if the list is full
                RemoveIfNecessary();

                var tmp = Root;
                while (tmp.Next != null)
                {
                    tmp = tmp.Next;
                }

                // We had the new node at the end as it is the most recently used element
                tmp.Next = new LeastRecentlyUsedCacheItem<T>
                {
                    Item = itemToAdd,
                    Next = null,
                    Previous = tmp,
                    UsedAt = DateTime.UtcNow
                };

                return tmp;
            }
        }

        // If the size is exceeded we remove the root element as it is the least used element
        private void RemoveIfNecessary()
        {
            if (GetSize() == maxSize)
            {
                var tmp = Root.Next;
                tmp.Previous = null;
                Root = tmp;
            }
        }

        // Get the number of element in the cache
        private int GetSize()
        {
            var tmp = Root;
            int count = 0;
            while (tmp != null)
            {
                count++;
                tmp = tmp.Next;
            }

            return count;
        }

        public override string ToString()
        {
            var tmp = Root;
            StringBuilder sb = new StringBuilder();
            while (tmp != null)
            {
                sb.AppendLine(tmp.ToString());
                Console.WriteLine(tmp.ToString());
                tmp = tmp.Next;
            }
            return sb.ToString();
        }
    }

    public class LeastRecentlyUsedCacheItem<T>
    {
        public T Item { get; set; }
        public DateTime UsedAt { get; set; }
        public LeastRecentlyUsedCacheItem<T> Next { get; set; }
        public LeastRecentlyUsedCacheItem<T> Previous { get; set; }

        public override string ToString()
        {
            return Item.ToString();
        }
    }
}
