using Moq;
using ProfileManager.AppService;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ProfileManager.Test
{
    public class FaceTest
    {
        private Mock<FakeHttpMessageHandler> _fakeHttpHandler;
        private HttpClient _fakeHttpClient;
        private HttpClient _realHttpClient;

        public FaceTest()
        {
            _fakeHttpHandler = new Mock<FakeHttpMessageHandler>() { CallBase = true };
            _fakeHttpClient = new HttpClient(_fakeHttpHandler.Object);
            _realHttpClient = new HttpClient();
        }

        [Fact]
        public async Task GetFaceInfosFromAzureFaceProvider()
        {
            var samplePic = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\", "assets", "test_pics", "Test6.jpg");
            var azr = new AzureFaceProvider("https://eastus.api.cognitive.microsoft.com/face/v1.0", "");
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
                Content = new StringContent(sampleData, System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceProvider(_fakeHttpClient, "https://eastus.api.cognitive.microsoft.com/face/v1.0", "");
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
                Content = new StringContent(sampleData, System.Text.Encoding.UTF8, "application/json")
            });

            var azr = new AzureFaceProvider(_fakeHttpClient, "https://eastus.api.cognitive.microsoft.com/face/v1.0", "");
            var fileBytes = await File.ReadAllBytesAsync(samplePic);
            var faces = await azr.GetFacesFromPhotoAsync(fileBytes);
            Assert.True(faces.Count == 1);
        }
    }
}
