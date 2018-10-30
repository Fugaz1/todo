using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Todo.Api.Controllers;
using Todo.Application;
using Todo.Core.Entities;
using Xunit;

namespace Todo.Api.Tests.Controllers
{
    public class TodoControllerTests
    {
        public TodoControllerTests()
        {
            _todoServiceMock = new Mock<ITodoService>();
            _sut = new TodoController(_todoServiceMock.Object);
        }

        private readonly TodoController _sut;
        private readonly Mock<ITodoService> _todoServiceMock;

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(10)]
        public void Get_Should_Return_AllItems(int itemCount)
        {
            // arrange
            var itemsInput = new List<TodoItem>();
            for (var i = 1; i <= itemCount; i++)
                itemsInput.Add(new TodoItem
                {
                    Name = $"Item {i}"
                });

            _todoServiceMock.Setup(x => x.GetAll()).Returns(itemsInput);

            // act
            var result = _sut.Get();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<List<TodoItem>>>();

            var itemsResult = result.Value;
            itemsResult.Should().NotBeNull();
            itemsResult.Should().BeOfType<List<TodoItem>>();
            itemsResult.Should().HaveCount(itemCount);
        }

        [Fact]
        public void Create_Should_Redirect()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Name = "Item 1",
                IsComplete = true
            };

            _todoServiceMock.Setup(x => x.Create(itemInput)).Returns(itemInput);

            // act
            var result = _sut.Create(itemInput);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedAtRouteResult>();
        }

        [Fact]
        public void Delete_Should_Return_NoContent()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 2,
                Name = "Item 2",
                IsComplete = false
            };

            _todoServiceMock.Setup(x => x.Delete(itemInput.Id)).Returns(itemInput);

            // act
            var result = _sut.Delete(itemInput.Id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public void Delete_Should_Return_NotFound()
        {
            // arrange
            const int itemId = 666;
            _todoServiceMock.Setup(x => x.Delete(itemId)).Returns((TodoItem) null);

            // act
            var result = _sut.Delete(itemId);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetById_Should_Return_Item()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 1,
                Name = "Item 1",
                IsComplete = true
            };

            _todoServiceMock.Setup(x => x.GetById(itemInput.Id)).Returns(itemInput);

            // act
            var result = _sut.Get(itemInput.Id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<TodoItem>>();

            var itemResult = result.Value;
            itemResult.Should().NotBeNull();
            itemResult.Should().BeOfType<TodoItem>();
            itemResult.Id.Should().Be(itemInput.Id);
            itemResult.Name.Should().Be(itemInput.Name);
            itemResult.IsComplete.Should().Be(itemInput.IsComplete);
        }

        [Fact]
        public void GetById_Should_Return_NotFound()
        {
            // arrange
            const int id = -1;

            // act
            var result = _sut.Get(id);

            // assert
            result.Should().NotBeNull();
            result.Value.Should().BeNull();
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Update_Should_Return_NoContent()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 2,
                Name = "Item 2",
                IsComplete = false
            };

            _todoServiceMock.Setup(x => x.Update(itemInput)).Returns(itemInput);

            // act
            var result = _sut.Update(itemInput.Id, itemInput);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public void Update_Should_Return_NotFound()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 2,
                Name = "Item 2",
                IsComplete = false
            };

            _todoServiceMock.Setup(x => x.Update(itemInput)).Returns((TodoItem) null);

            // act
            var result = _sut.Update(itemInput.Id, itemInput);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}