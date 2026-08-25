using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.DomainModel;

public sealed class BookInRating : Aggregate<Guid>
{
    /// <exception cref="ArgumentException"></exception>
    public BookInRating(Guid id, string name, string author, uint votes = 0) : base(id)
    {
        Name = name ?? throw new ArgumentException("Book name is not specified");
        Author = author ?? throw new ArgumentException("Book author is not specified");
        Votes = votes;
    }

    public string Name { get; }

    public string Author { get; }

    public uint Votes { get; private set; }

    public void Vote()
    {
        Votes++;

        AddDomainEvent(new BookWasVotedDomainEvent(Id));
    }
}
