using Eladei.BookRating.Application.Commands;
using Eladei.BookRating.Application.Exceptions;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities;
using Shouldly;

namespace Eladei.BookRating.IntegrationTests.Commands;

public sealed class RegisterBookCommandTests : NpgsqlIntegrationTestsBase<BookRatingDbContext>
{
    public RegisterBookCommandTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new BookRatingDbContext(opts)) { }

    [Fact]
    public async Task Command_WhenBookAlreadyExists_ShouldThrowBookWithCurrentInfoAlreadyExistsException()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);

        var expectedError = $"A book with Name = '{name}' and Author = '{author}' already exists.";

        using var context = CreateContext();

        context.Books.Add(new Book
        {
            Id = Guid.NewGuid(),
            Name = name,
            Author = author
        });

        await context.SaveChangesAsync(CancellationToken.None);

        // Act, Assert
        var action = () => command.ExecuteAsync(context, CancellationToken.None);

        await action.ShouldThrowAsync<BookWithCurrentInfoAlreadyExistsException>(expectedError);
    }

    [Fact]
    public async Task Command_ShouldSaveNewBook()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);

        using var context = CreateContext();

        // Act
        var bookId = await command.ExecuteAsync(context, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var addedBook = context.Books.FirstOrDefault(b => b.Id == bookId);
        addedBook.ShouldNotBeNull();
        addedBook.Name.ShouldBe(name);
        addedBook.Author.ShouldBe(author);
    }
}
