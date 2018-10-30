using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public TodoControllerTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

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
        public async void GetById_Should_Return_Item()
        {
            // arrange
            var itemInput = new TodoItem {Name = $"Item test", IsComplete = true };
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
    }
}
