using Moq;
using ProfileManager.AppService;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ProfileManager.Test
{
    public class FaceTest
    {
        private readonly Mock<FakeHttpMessageHandler> _fakeHttpHandler;
        private readonly HttpClient _fakeHttpClient;
        private readonly HttpClient _realHttpClient;

        private IConfiguration Configuration { get; set; }

        private IOptions<FaceInfoProviderOptions> _options;

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

            _options = Options.Create(new FaceInfoProviderOptions() { Endpoint = Configuration["FaceInfoProvider:Endpoint"], Key = Configuration["FaceInfoProvider:Key"] });
        }

        // todo: clean all these up
        [Fact]
        public async Task SingleFacePictureShouldReturnSingleFaceInArray()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test1.jpg");
            var sampleData = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test1.jpg.json");

            _fakeHttpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(await File.ReadAllTextAsync(sampleData), System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceInfoProvider(_fakeHttpClient, _options);
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

            var azr = new AzureFaceInfoProvider(_fakeHttpClient, _options);
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count == 3);
        }

        [Fact]
        public async Task NoFacePictureShouldReturnEmptyArray()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "no-face.jpg");
            var sampleData = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "no-face.jpg.json");

            _fakeHttpHandler.Setup(x => x.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(await File.ReadAllTextAsync(sampleData), System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceInfoProvider(_fakeHttpClient, _options);
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count == 3);
        }
    }
}
