using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectRn_Api.Models;
using ConnectRn_Api.Controllers;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection.Metadata;

namespace UserTest
{
    [TestClass]
    public class UserTests
    {
        private readonly ILogger<UserController> _logger;

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

        public UserInfo[] expectedUserResult = new UserInfo[]
        {
            new UserInfo()
            {
                user_id = 1,
                name = "Joe Smith",
                date_of_birth = DateTime.Parse("1983-05-12T00:00:00"),
                day_of_week_of_birth = "Thursday",
                created_on = 1642612034,
                created_on_rfc = DateTime.Parse("2022-01-19T17:07:14+00:00")
            },
            new UserInfo()
            {
                user_id = 2,
                name = "Jane Doe",
                date_of_birth = DateTime.Parse("1990-08-06T00:00:00"),
                day_of_week_of_birth = "Monday",
                created_on = 1642612034,
                created_on_rfc = DateTime.Parse("2022-01-19T17:07:14+00:00")
            }
        };

        private string imageFileName = "JPEG_example_flower.jpg";


        [TestMethod]
        public void GetUserInfo_UserPayloadIsValid_ReturnsTrue()
        {
            // Arrange
            var controller = new UserController(_logger) ;

            // Act
            var response = controller.Get(payloadUsers);

            // Assert
            IEnumerable<UserInfo> userInfo  = expectedUserResult;
            Assert.AreEqual((response.Result as OkObjectResult).Value as IEnumerable<UserInfo>, userInfo);
        }

        [TestMethod]
        public void ConvertImage_ReturnsPNG_ReturnsTrue(FileStream fileStream)
        {
            // Arrange
            var controller = new UserController(_logger);

            // Act
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", imageFileName);
            FileStream file = new FileStream(path, FileMode.Open);

            var response = controller.ConvertImage(file);

            // Assert
            Assert.IsInstanceOfType(response, typeof(FileStream));
        }
    }
}
