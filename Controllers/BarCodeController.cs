using BarcodeApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;


namespace BarcodeApi.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]/api/[action]")]
    [ApiController]
    public class BarCodeController : ControllerBase
    {

        [HttpPost(Name = "UploadVideo"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadVideo()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "video");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"')??DateTime.Now.ToString();
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (FileStream DestinationStream = System.IO. File.Create(fullPath))
                    {
                         file.CopyTo(DestinationStream);
                    }

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost(Name = "GetBarCodeFromImageFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> GetBarCodeFromImageFile()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var ret = new List<string>();

                foreach (var file in formCollection.Files)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    if (file.ContentType.Contains("image"))
                    {
                        ret.AddRange(GdPictureUtils.GetBarcodesFromImage(filePath));
                    }

                    else
                    ret.AddRange(GdPictureUtils.GetBarcodesFromFile(filePath));
                }
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost(Name = "GetBarCodeFromImage64"), DisableRequestSizeLimit]
        public async Task<IActionResult> GetBarCodeFromImage64()
        {
            try
            {
                using var stream = new MemoryStream();
                using var writer = new BinaryWriter(stream);
                var formCollection = await Request.ReadFormAsync();
                var val = formCollection["base64image"].First();
                if (val != null)
                {
                    var ret = new List<string>();

                    byte[] file = System.Convert.FromBase64String(val);
                    ret.AddRange(GdPictureUtils.CreateGdPictureImageFromByteArray(file));

                    return Ok(ret);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet(Name = "Test")]
        [AllowAnonymous]
        public string Test()
        {
            return "test";
        }
    }
}
