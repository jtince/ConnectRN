using ConnectRn_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ConnectRn_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        public UserInfo[] payloadUsers = new UserInfo[]
        {
            new UserInfo()
            {
                user_id = 1,
                name = "Joe Smith",
                date_of_birth = DateTime.Parse("1983-05-12"),
                created_on = 1642612034
            },
            new UserInfo()
            {
                user_id = 2,
                name = "Jane Doe",
                date_of_birth = DateTime.Parse("1990-08-06"),
                created_on = 1642612034
            }
        };

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserInfo>> Get(IEnumerable<UserInfo> usersInfo)
        {
            try
            {
                var result = new List<UserInfo>();
                foreach (var user in usersInfo)
                {
                    //get day of week from DateTime object
                    user.day_of_week_of_birth = user.date_of_birth.DayOfWeek.ToString();

                    //get DateTime object of epoch time
                    user.created_on_rfc = DateTimeOffset.FromUnixTimeSeconds(user.created_on);
                    result.Add(user);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex });
            }
        }

        [HttpPost("convert-image")]
        public ActionResult<FileStream> ConvertImage([FromBody] FileStream file)
        {
            try
            {
                //set desired width and height
                int width = 256;
                int height = 256;

                //save file to memory
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    
                    using (FileStream pngStream = new FileStream(s, FileMode.Open, FileAccess.Read))
                    using (var image = new Bitmap(pngStream))
                    {
                        //get size ratio to preserver aspect ratio
                        double ratio = Math.Max((double)image.Width / (double)width, (double)image.Height / (double)height);
                        var resizeWidth = (int)(image.Width / ratio);
                        var resizeHeight = (int)(image.Height / ratio);
                        //resize image
                        var resized = new Bitmap(resizeWidth, resizeHeight);

                        //Use graphic library to convert to png
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighSpeed;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.DrawImage(image, 0, 0, width, height);
                            resized.Save($"resized-{file}", ImageFormat.Png);
                        }
                        return Ok(resized);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex });
            }
        }
    }
}
