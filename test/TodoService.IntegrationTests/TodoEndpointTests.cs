using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using TodoService.Controllers;
using Shouldly;
using System.Net;
using MySql.Data.MySqlClient;
using Dapper;
using System.Text;

namespace TodoService.IntegrationTests
{
    public class TodoEndpointTests : IDisposable
    {
        private string _endpoint = "/api/todo";
        private string _url;
        private string _connectionString;

        public TodoEndpointTests()
        {
            _url = Environment.GetEnvironmentVariable("API_URL") + _endpoint;
            _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }
        
        [Fact]
        public async void should_add_to_database_on_post()
        {
            var client = new HttpClient();
            var todo = new TodoModel{ Checked = false, Text = "Test Text" };
            
            var result = await client.PostAsync(_url, new StringContent(JsonConvert.SerializeObject(todo),Encoding.UTF8, "application/json"));
            var expectedModel = JsonConvert.DeserializeObject<TodoModel>(await result.Content.ReadAsStringAsync());

            var response = await client.GetAsync($"{_url}/{expectedModel.Id}");
            var actualModel = JsonConvert.DeserializeObject<TodoModel>(await result.Content.ReadAsStringAsync());
            
            actualModel.Id.ShouldBe(expectedModel.Id);
            actualModel.Text.ShouldBe(expectedModel.Text);
        }

        public void Dispose()
        {
            using(var connection = new MySqlConnection(_connectionString))
            {
                connection.Execute("truncate todo");
            }
        }
    }
}
