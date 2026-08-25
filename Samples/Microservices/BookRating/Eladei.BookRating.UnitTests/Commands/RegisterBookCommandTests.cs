using Eladei.BookRating.Application.Commands;
using Eladei.BookRating.Application.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;

namespace Eladei.BookRating.UnitTests.Commands;

public sealed class RegisterBookCommandTests : EFUnitTestsBase<BookRatingDbContext>
{
    private readonly List<Book> _books = [];

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public Task Command_WhenNameIsNullOrWhiteSpace_ShouldThrowArgumentException(string? name)
    {
        // Arrange
        var author = "A.S. Pushkin";

        // Act, Assert
        var expectedError = "Book title is not specified";

        // Act, Assert
        var action = () => new RegisterBookCommand(name!, author);

        action.ShouldThrow<ArgumentException>(expectedError);

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public Task Command_WhenAuthorIsNullOrWhiteSpace_ShouldThrowArgumentException(string? author)
    {
        // Arrange
        var name = "The Captain's Daughter";

        var expectedError = "Book author is not specified";

        // Act, Assert
        var action = () => new RegisterBookCommand(name, author!);

        action.ShouldThrow<ArgumentException>(expectedError);

        return Task.CompletedTask;
    }

    [Fact]
    public async Task Command_WheneBookWithCurrentInfoAlreadeExists_ShouldThrowBookWithCurrentInfoAlreadyExistsException()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);

        _books.Add(new Book
        {
            Id = Guid.NewGuid(),
            Name = name,
            Author = author
        });

        var expectedError = $"A book with Name = '{name}' and Author = '{author}' already exists.";

        // Act, Assert
        var action = () => command.ExecuteAsync(_context, CancellationToken.None);

        await action.ShouldThrowAsync<BookWithCurrentInfoAlreadyExistsException>(expectedError);
    }

    [Fact]
    public async Task Command_ShouldReturnIdOfRegisteredBook()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);

        // Act
        var result = await command.ExecuteAsync(_context, CancellationToken.None);

        var expectedBookDb = new Book
        {
            Id = result,
            Name = name,
            Author = author
        };

        // Assert
        result.ShouldNotBe(Guid.Empty);

        var books = new List<Book>();
        _contextMock.Verify(x => x.Books.AddAsync(Capture.In(books), It.IsAny<CancellationToken>()), Times.Once);

        books.Count.ShouldBe(1);
        books[0].ShouldBeEquivalentTo(expectedBookDb);
    }

    [Fact]
    public async Task Command_ShouldGenerateBookWasRegisteredInRatingIntegrationEvent()
    {
        // Arrange
        var name = "The Captain's Daughter";
        var author = "A.S. Pushkin";
        var command = new RegisterBookCommand(name, author);

        // Act
        await command.ExecuteAsync(_context, CancellationToken.None);

        // Assert
        command.Events.ShouldNotBeEmpty();
        command.Events.Count.ShouldBe(1);

        var evnt = command.Events.Single()
            .ShouldBeOfType<BookWasRegisteredInRatingIntegrationEvent>();

        evnt.Name.ShouldBe(name);
        evnt.Author.ShouldBe(author);
    }

    protected override BookRatingDbContext ConfigureContext(Mock<BookRatingDbContext> contextMock)
    {
        contextMock
            .Setup(s => s.Books)
            .ReturnsDbSet(_books);

        return contextMock.Object;
    }
}
