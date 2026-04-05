using Eladei.Architecture.Tests.EntityFramework.Unit;
using Eladei.BookRating.Domain.Commands;
using Eladei.BookRating.Domain.Commands.DomainEvents;
using Eladei.BookRating.Domain.Properties;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;

namespace Eladei.BookRating.UnitTests.Commands;

/// <summary>
/// Unit-тесты команды RegisterBookCommand
/// </summary>
/// <see cref="RegisterBookCommand"/>
public sealed class RegisterBookCommandTests : EFUnitTestsBase<BookRatingDbContext> {
    private List<Book> _books = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public Task Command_Should_Throw_ArgumentException_When_Name_Is_Null_Or_Empty(string name) {
        // Arrange
        var author = "А.С. Пушкин";

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() => new RegisterBookCommand(name, author));
        exception.Message.ShouldBe(Resource.BookNameNotDefined);

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public Task Command_Should_Throw_ArgumentException_When_Author_Is_Null_Or_Empty(string author) {
        // Arrange
        var name = "Капитанская дочка";

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() => new RegisterBookCommand(name, author));
        exception.Message.ShouldBe(Resource.BookAuthorNotDefined);

        return Task.CompletedTask;
    }

    [Fact]
    public async Task Command_Should_Return_Id_Of_Registered_Book() {
        // Arrange
        var name = "Капитанская дочка";
        var author = "А.С. Пушкин";
        var command = new RegisterBookCommand(name, author);

        // Act
        var result = await command.ExecuteAsync(_context, CancellationToken.None);

        // Assert
        result.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Command_Should_Generate_BookWasRegisteredInRatingDomainEvent() {
        // Arrange
        var name = "Капитанская дочка";
        var author = "А.С. Пушкин";
        var command = new RegisterBookCommand(name, author);

        // Act
        await command.ExecuteAsync(_context, CancellationToken.None);

        // Assert
        command.Events.ShouldNotBeEmpty();
        command.Events.Count.ShouldBe(1);

        var evnt = command.Events.Single()
            .ShouldBeOfType<BookWasRegisteredInRatingDomainEvent>();

        evnt.Name.ShouldBe(name);
        evnt.Author.ShouldBe(author);
    }

    protected override BookRatingDbContext SetUpDbContext(Mock<BookRatingDbContext> contextMock) {
        contextMock.Setup(s => s.Books).ReturnsDbSet(_books);

        return contextMock.Object;
    }
}