using System.Collections.Generic;

namespace FOSSGames
{
    public class PoppableList<T> : List<T>
    {
        public T PopFirst()
        {
            T item = this[0];
            Remove(item);
            return item;
        }
        public T Pop()
        {
            T item = this[Count - 1];
            Remove(item);
            return item;
        }

        public T PopLast() => Pop();
    }
}