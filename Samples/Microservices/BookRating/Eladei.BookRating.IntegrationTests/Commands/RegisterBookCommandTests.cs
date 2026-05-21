using Eladei.Architecture.Tests.EntityFramework.Integration;
using Eladei.BookRating.Domain.Commands;
using Eladei.BookRating.Domain.Exceptions;
using Eladei.BookRating.Domain.Properties;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities;
using Shouldly;

namespace Eladei.BookRating.IntegrationTests.Commands;

/// <summary>
/// Integration tests for the RegisterBookCommand
/// </summary>
/// <see cref="RegisterBookCommand"/>
public sealed class RegisterBookCommandTests : NpgsqlIntegrationTestsBase<BookRatingDbContext>
{
    public RegisterBookCommandTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new BookRatingDbContext(opts)) { }

    [Fact]
    public async Task Command_Should_Throw_BookWithCurrentInfoAlreadyExistsException_When_Book_Already_Exists()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);
        var expectedError = string.Format(Resource.BookWithCurrentInfoAlreadyExists, name, author);

        using var context = CreateContext();

        context.Books.Add(new Book
        {
            Id = Guid.NewGuid(),
            Name = name,
            Author = author
        });

        await context.SaveChangesAsync(CancellationToken.None);

        // Act, Assert
        var exception = await Assert.ThrowsAsync<BookWithCurrentInfoAlreadyExistsException>(
            async () => await command.ExecuteAsync(context, CancellationToken.None));

        exception.Message.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Command_Should_Save_New_Book()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);
        var expectedError = string.Format(Resource.BookWithCurrentInfoAlreadyExists, name, author);

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