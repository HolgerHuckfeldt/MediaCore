using MediaCore.Database;
using MediaCore.Services;

namespace MediaCoreTests
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public async Task AddImage()
        {
            var imageService = new ImageService();

            var data = await File.ReadAllBytesAsync("C:\\Users\\hhuck\\OneDrive\\Bilder\\2.jpg");

            var image = await imageService.AddImage(data,"2",MediaType.Jpeg);
            Assert.IsTrue(image != Guid.Empty);

        }

        [TestMethod]
        public async Task DeleteImage()
        {
            var imageService = new ImageService();

            var image = await imageService.Delete(Guid.Parse("E7552D0D-16F2-48EF-A608-353820B140E8"));

        }
    }
}
