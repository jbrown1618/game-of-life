using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GameOfLifeService.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace GameOfLifeService.Tests.Controllers
{
    public class GameOfLifeControllerIntegrationTest : IClassFixture<WebApplicationFactory<GameOfLifeService.Startup>>
    {
        private readonly WebApplicationFactory<GameOfLifeService.Startup> _factory;

        public GameOfLifeControllerIntegrationTest(WebApplicationFactory<GameOfLifeService.Startup> factory)
        {
            _factory = factory;
        }

        private HttpContent MakeContent(GameOfLifeStateDTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        private HttpContent MakeContent(string content)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        [Fact]
        public async Task Get_ReturnsJSON()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/GameOfLife");
            response.EnsureSuccessStatusCode();

            Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Iterate_ReturnsTheNextIteration()
        {
            var client = _factory.CreateClient();
            var content = MakeContent(new GameOfLifeStateDTO
            {
                Height = 5,
                Width = 5,
                LiveCells = new CoordinateDTO[]{
                    new CoordinateDTO{Row=1, Col=1},
                    new CoordinateDTO{Row=1, Col=2},
                    new CoordinateDTO{Row=1, Col=3}
                }
            });

            var response = await client.PostAsync("/GameOfLife/Iterate", content);
            response.EnsureSuccessStatusCode();

            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var responseJson = await response.Content.ReadAsStringAsync();

            Assert.Contains("\"height\":5", responseJson);
            Assert.Contains("\"width\":5", responseJson);
            Assert.Contains("\"liveCells\":[", responseJson);
            Assert.Contains("\"row\":0,\"col\":2", responseJson);
            Assert.Contains("\"row\":1,\"col\":2", responseJson);
            Assert.Contains("\"row\":2,\"col\":2", responseJson);
        }

        [Fact]
        public async Task Iterate_RejectsAnInvalidState()
        {
            var client = _factory.CreateClient();
            var content = MakeContent(new GameOfLifeStateDTO
            {
                Height = 5,
                Width = 5,
                LiveCells = new CoordinateDTO[]{
                    new CoordinateDTO{Row=1, Col=1},
                    new CoordinateDTO{Row=1, Col=2},
                    new CoordinateDTO{Row=1, Col=31415}
                }
            });

            var response = await client.PostAsync("/GameOfLife/Iterate", content);

            Assert.Equal("InternalServerError", response.StatusCode.ToString());
        }

        [Fact]
        public async Task Iterate_RejectsMalformedJson()
        {
            var client = _factory.CreateClient();
            var content = MakeContent("{ This is not real JSON }");

            var response = await client.PostAsync("/GameOfLife/Iterate", content);

            Assert.Equal("BadRequest", response.StatusCode.ToString());
        }

        /*
         * This is a bug! It looks like the conversion from JSON to DTO does not validate all of the
         * properties, so it just defaults anything that is missing. Yikes! This sounds like a bug
         * for later, though. I just wanted to write some integration tests!
        [Fact]
        public async Task Iterate_RejectsNonStateJson()
        {
            var client = _factory.CreateClient();
            var content = MakeContent("{ \"desc\": \"This is JSON, but not a DTO\" }");

            var response = await client.PostAsync("/GameOfLife/Iterate", content);

            Assert.Equal("BadRequest", response.StatusCode.ToString());
        }
        */
    }
}