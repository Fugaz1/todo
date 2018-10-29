using System;
using System.Collections.Generic;
using System.Text;
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
        private TodoController _sut;
        private Mock<ITodoService> _todoServiceMock;

        public TodoControllerTests()
        {
            _todoServiceMock = new Mock<ITodoService>();
            _sut = new TodoController(_todoServiceMock.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(10)]
        public void Get_Should_Return_AllItems(int itemCount)
        {
            // arrange
            var items = new List<TodoItem>();
            for (var i = 1; i <= itemCount; i++)
            {
                items.Add(new TodoItem()
                {
                    Name = $"Item {i}"
                });
            }

            _todoServiceMock.Setup(x => x.GetAll()).Returns(items);

            // act
            var result = _sut.Get();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<List<TodoItem>>>();

            var resultValue = result.Value;
            resultValue.Should().NotBeNull();
            resultValue.Should().BeOfType<List<TodoItem>>();
            resultValue.Should().HaveCount(itemCount);
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
    }
}
