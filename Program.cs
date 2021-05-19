using System;
using System.Diagnostics;
using System.Text;

namespace riverbank
{
    /**
    Least recently used cache
        - maximum size is parameter(testing 3)
        - add element if size is 3 remove the least used

        Linked list 
        Normal list
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

        public T GetElement(T itemToGet)
        {
            var current = Root;

            while (current != null && !current.Item.Equals(itemToGet))
            {
                current = current.Next;
            }

            if (current is null)
            {
                current = Add(itemToGet);
                SortList(Root);
                return itemToGet;
            }
            else
            {
                current.UsedAt = DateTime.UtcNow;
                SortList(Root);
                return current.Item;
            }
        }

        private void SortList(LeastRecentlyUsedCacheItem<T> root)
        {
            if (root is null || root.Next is null)
                return;

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
                    Root = tmp;

                SortList(root);
            }
            else
            {
                SortList(root.Next);
            }
        }

        private LeastRecentlyUsedCacheItem<T> Add(T itemToAdd)
        {
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
                RemoveIfNecessary();

                var tmp = Root;
                while (tmp.Next != null)
                {
                    tmp = tmp.Next;
                }

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

        private void RemoveIfNecessary()
        {
            if (GetSize() == maxSize)
            {
                var tmp = Root.Next;
                tmp.Previous = null;
                Root = tmp;
            }
        }

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
