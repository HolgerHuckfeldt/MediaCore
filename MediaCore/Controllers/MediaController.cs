using MediaCore.Database;
using MediaCore.Model;
using MediaCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : BaseController
    {

        private readonly ImageService _imageService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(

            ImageService imageService,
            ILogger<MediaController> logger)
            : base()
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpGet]
        [Route("{ressource}")]
        public async Task<ActionResult> GetImage(Guid ressource)
        {

            //_logger.LogInformation($"GetImage/{ressource}");

            Response.Headers["Cache-Control"] = $"public,max-age=100000";
            if (ressource == Guid.Empty)
            {
                try
                {
                    return File(Array.Empty<byte>(), "image/Jpeg");
                }
                catch (Exception /*ex*/)
                {
                    return File(Array.Empty<byte>(), "image/Jpeg");
                }
            }

            var image = await _imageService.GetImageByteStream(ressource);

            if (image.Item2 != null)
            {
                var mimeType = "image/png";
                switch (image.Item1)
                {
                    case MediaType.Jpeg:
                        mimeType = "image/Jpeg";
                        break;
                    case MediaType.Png:
                        mimeType = "image/png";
                        break;
                    case MediaType.Gif:
                        mimeType = "image/gif";
                        break;
                    case MediaType.PDF:
                        mimeType = "application/pdf";
                        break;
                    case MediaType.Svg:
                        mimeType = "application/svg";
                        break;
                }

                return File(image.Item2, mimeType);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("api/Image/")]
        public async Task<ActionResult> GetImages()
        {
            try
            {
                //_logger.LogInformation($"api/GetImages");

                Response.Headers["Cache-Control"] = $"public,max-age=100000";
                var result = await _imageService.GetImages();

                if (result.Any())
                    return Ok(result);

                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("{resource}")]
        public async Task<ActionResult> DeleteImages(Guid resource)
        {
            try
            {

                var result = await _imageService.Delete(resource);

                if (result)
                    return Ok();

                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("")]
        public async Task<ActionResult> PostUserImage([FromForm] IFormFile input)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                    var files = Request.Form;

                    foreach (var file in files.Files)
                    {

                        var postedFile = file;
                        if (postedFile.Length > 0)
                        {
                            int maxContentLength = 1024 * 1024 * 20 * 10; //Size = 10 MB

                            var allowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".pdf", ".svg" };
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();
                            if (!allowedFileExtensions.Contains(extension))
                            {
                                var message = string.Format("Please Upload image of type .jpg, .jpeg, .gif, .png, .pdf. .svg");
                                dict.Add("error", message);
                                return BadRequest(dict);
                            }
                            else if (postedFile.Length > maxContentLength)
                            {
                                var message = string.Format("Please Upload a file upto 10 mb.");
                                dict.Add("error", message);
                                return BadRequest(dict);
                            }
                            else
                            {
                                var resourceId = await _imageService.AddImage(ReadFully(postedFile.OpenReadStream()), postedFile.FileName, GetType(ext));
                                return Ok(resourceId);
                            }
                        }

                        var message1 = string.Format("Image Updated Successfully.");
                        return Ok(message1);
                    }
                    var res = string.Format("Please Upload a image.");
                    dict.Add("error", res);
                    return BadRequest();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }

        private static MediaType GetType(string ext)
        {
            switch (ext.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return MediaType.Jpeg;
                case ".png":
                    return MediaType.Png;
                case ".gif":
                    return MediaType.Gif;
                case ".pdf":
                    return MediaType.PDF;
                case ".svg":
                    return MediaType.Svg;
            }

            return MediaType.Unknown;
        }

    }
}
