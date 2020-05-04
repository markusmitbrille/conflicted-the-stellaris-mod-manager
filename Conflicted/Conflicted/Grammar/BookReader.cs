namespace Conflicted.Grammar
{
    sealed class BookReader : Reader<IToken>
    {
        public BookReader(Book book) : base(book.ToArray())
        {
        }

        public BookReader(BookReader reader) : base(reader)
        {
        }
    }
}
