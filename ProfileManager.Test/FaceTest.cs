using Moq;
using ProfileManager.AppService;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace ProfileManager.Test
{
    public class FaceTest
    {
        private readonly Mock<FakeHttpMessageHandler> _fakeHttpHandler;
        private readonly HttpClient _fakeHttpClient;
        private readonly HttpClient _realHttpClient;
        private readonly string _faceEndpoint;
        private readonly string _faceKey;

        private IConfiguration Configuration { get; set; }

        public FaceTest()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\", "ProfileManager.Web"))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            _fakeHttpHandler = new Mock<FakeHttpMessageHandler>() { CallBase = true };
            _fakeHttpClient = new HttpClient(_fakeHttpHandler.Object);
            _realHttpClient = new HttpClient();
            _faceEndpoint = Configuration["FaceInfoProvider:Endpoint"];
            _faceKey = Configuration["FaceInfoProvider:Key"];
        }

        [Fact]
        public async Task GetFaceInfosFromAzureFaceProvider()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "no-face.jpg");
            var azr = new AzureFaceProvider(_faceEndpoint, _faceKey);// https://eastus.api.cognitive.microsoft.com/face/v1.0", "");
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count > 0);
        }

        [Fact]
        public async Task SingleFacePictureShouldReturnSingleFaceInArray()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test1.jpg");
            var sampleData = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test1.jpg.json");

            _fakeHttpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(await File.ReadAllTextAsync(sampleData), System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceProvider(_fakeHttpClient, _faceEndpoint, _faceKey);
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count == 1);
        }

        [Fact]
        public async Task MultipleFacePictureShouldReturnMultipleFacesInArray()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test6.jpg");
            var sampleData = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test6.jpg.json");

            _fakeHttpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(await File.ReadAllTextAsync(sampleData), System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceProvider(_fakeHttpClient, _faceEndpoint, _faceKey);
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count == 3);
        }

        [Fact]
        public async Task NoFacePictureShouldReturnEmptyArray()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test6.jpg");
            var sampleData = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test6.jpg.json");

            _fakeHttpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(await File.ReadAllTextAsync(sampleData), System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceProvider(_fakeHttpClient, _faceEndpoint, _faceKey);
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count == 3);
        }
    }
}
