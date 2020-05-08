namespace Conflicted.Grammar
{
    abstract class Reader<T>
    {
        private T[] array;
        private int index = 0;

        protected Reader(T[] array)
        {
            if (array is null)
            {
                throw new System.ArgumentNullException(nameof(array));
            }

            this.array = array;
        }

        protected Reader(Reader<T> reader)
        {
            array = (T[])reader.array.Clone();
        }

        public bool CanPeek()
        {
            return index >= 0 && index < array.Length;
        }

        public T Peek()
        {
            return array[index];
        }

        public bool CanPeek(int delta)
        {
            return index + delta >= 0 && index + delta < array.Length;
        }

        public T Peek(int delta)
        {
            return array[index + delta];
        }

        public bool CanRead()
        {
            return index >= 0 && index < array.Length;
        }

        public T Read()
        {
            return array[index++];
        }
    }
}
