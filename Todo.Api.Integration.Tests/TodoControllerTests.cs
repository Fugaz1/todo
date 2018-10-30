using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Todo.Core;
using Todo.Core.Entities;
using Xunit;

namespace Todo.Api.Integration.Tests
{
    public class TodoControllerTests
    {
        public TodoControllerTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private readonly HttpClient _client;
        private readonly TestServer _server;

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(10)]
        public async void Get_Should_Return_AllItems(int itemCount)
        {
            // arrange
            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);

                for (var i = 1; i <= itemCount; i++)
                    ctx.TodoItems.Add(new TodoItem {Name = $"Item {i}"});

                ctx.SaveChanges();
            }

            // act
            var response = await _client.GetAsync("/api/todo/");

            // assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            result.Should().NotBeNullOrEmpty();

            var itemsResult = JsonConvert.DeserializeObject<List<TodoItem>>(result);
            itemsResult.Should().BeOfType<List<TodoItem>>();
            itemsResult.Count.Should().Be(itemCount);
        }

        [Fact]
        public async void Create_Should_Redirect()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Name = "Item 1",
                IsComplete = true
            };

            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.SaveChanges();
            }

            // act
            var response = await _client.PostAsJsonAsync("/api/todo/", itemInput);

            // assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            result.Should().NotBeNullOrEmpty();

            var itemResult = JsonConvert.DeserializeObject<TodoItem>(result);
            itemResult.Should().BeOfType<TodoItem>();
            itemResult.Id.Should().BeGreaterThan(0);
            itemResult.Name.Should().Be(itemInput.Name);
            itemResult.IsComplete.Should().Be(itemInput.IsComplete);
        }

        [Fact]
        public async void Delete_Should_Return_NoContent()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 2,
                Name = "Item 2",
                IsComplete = true
            };

            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.Add(itemInput);
                ctx.SaveChanges();
            }

            // act
            var response = await _client.DeleteAsync($"/api/todo/{itemInput.Id}");

            // assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public async void Delete_Should_Return_NotFound()
        {
            // arrange
            const int itemId = 2;

            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.SaveChanges();
            }

            // act
            var response = await _client.DeleteAsync($"/api/todo/{itemId}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GetById_Should_Return_Item()
        {
            // arrange
            var itemInput = new TodoItem {Name = "Item test", IsComplete = true};
            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.TodoItems.Add(itemInput);
                ctx.SaveChanges();
            }

            // act
            var response = await _client.GetAsync($"/api/todo/{itemInput.Id}");

            // assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            result.Should().NotBeNullOrEmpty();

            var itemResult = JsonConvert.DeserializeObject<TodoItem>(result);
            itemResult.Should().BeOfType<TodoItem>();
            itemResult.Id.Should().Be(itemInput.Id);
            itemResult.Name.Should().Be(itemInput.Name);
            itemResult.IsComplete.Should().Be(itemInput.IsComplete);
        }

        [Fact]
        public async void GetById_Should_Return_NotFound()
        {
            // arrange
            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.SaveChanges();
            }

            // act
            var response = await _client.GetAsync("/api/todo/1");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Update_Should_Return_NoContent()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 2,
                Name = "Item 2",
                IsComplete = true
            };

            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.Add(itemInput);
                ctx.SaveChanges();
            }

            itemInput.Name = "Item 2 Updated";
            itemInput.IsComplete = false;

            // act
            var response = await _client.PutAsJsonAsync($"/api/todo/{itemInput.Id}", itemInput);

            // assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await response.Content.ReadAsStringAsync();
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public async void Update_Should_Return_NotFound()
        {
            // arrange
            var itemInput = new TodoItem
            {
                Id = 2,
                Name = "Item 2",
                IsComplete = true
            };

            using (var ctx = _server.Host.Services.GetRequiredService<TodoContext>())
            {
                ctx.TodoItems.RemoveRange(ctx.TodoItems);
                ctx.SaveChanges();
            }

            // act
            var response = await _client.PutAsJsonAsync($"/api/todo/{itemInput.Id}", itemInput);

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}