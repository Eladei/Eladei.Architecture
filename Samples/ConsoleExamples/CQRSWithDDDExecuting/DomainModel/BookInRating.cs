using Eladei.Architecture.Ddd.Entities;

namespace CqrsWithDddExecuting.DomainModel;

/// <summary>
/// Book information in the rating
/// </summary>
public sealed class BookInRating : Aggregate<Guid>
{
    /// <summary>
    /// Creates an instance of BookInRating
    /// </summary>
    /// <param name="id">Book identifier</param>
    /// <param name="name">Book name</param>
    /// <param name="author">Author</param>
    /// <param name="votes">Number of votes for the book</param>
    /// <exception cref="ArgumentException"></exception>
    public BookInRating(Guid id, string name, string author, uint votes = 0) : base(id)
    {
        Name = name ?? throw new ArgumentException("Book name is not specified");
        Author = author ?? throw new ArgumentException("Book author is not specified");
        Votes = votes;
    }

    /// <summary>
    /// Book name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Author
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Votes cast for the book
    /// </summary>
    public uint Votes { get; private set; }

    /// <summary>
    /// Vote for the book
    /// </summary>
    public void Vote()
    {
        Votes++;

        AddDomainEvent(new BookWasVotedDomainEvent(Id));
    }
}