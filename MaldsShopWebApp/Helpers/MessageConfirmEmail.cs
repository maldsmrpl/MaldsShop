﻿namespace MaldsShopWebApp.Helpers
{
    public class MessageConfirmEmail
    {
        public string Body(string link)
        {
            var bodyText = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>Confirmation Email</title>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n        }}\r\n\r\n        .container {{\r\n            background-color: #fff;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            max-width: 600px;\r\n            border-radius: 8px;\r\n        }}\r\n\r\n        .header {{\r\n            background-color: #007bff;\r\n            color: #ffffff;\r\n            padding: 10px;\r\n            text-align: center;\r\n            border-top-left-radius: 8px;\r\n            border-top-right-radius: 8px;\r\n        }}\r\n\r\n        .content {{\r\n            padding: 20px;\r\n            text-align: center;\r\n        }}\r\n\r\n        .footer {{\r\n            margin-top: 20px;\r\n            text-align: center;\r\n            padding: 10px;\r\n            font-size: 12px;\r\n            color: #888;\r\n        }}\r\n\r\n        a {{\r\n            color: #007bff;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            <h2>Confirmation Required</h2>\r\n        </div>\r\n        <div class=\"content\">\r\n            <p>Hello,</p>\r\n            <p>Thank you for registering with us. Please confirm your email address by clicking on the link below:</p>\r\n            <a href=\"{link}\">Confirm my email address</a>\r\n            <p>If you did not request this, please ignore this email.</p>\r\n        </div>\r\n        <div class=\"footer\">\r\n            Regards,<br>\r\n            The Team at MaldsShop<br>\r\n            <a href=\"https://malds.dev\">malds.dev</a>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
            return bodyText;
        }
    }
}
