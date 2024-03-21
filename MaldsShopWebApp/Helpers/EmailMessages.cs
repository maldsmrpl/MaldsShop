using Stripe.Climate;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MaldsShopWebApp.Helpers
{
    public class EmailMessages
    {
        public string ConfirmEmail(string link)
        {
            var bodyText = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>Confirmation Email</title>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n        }}\r\n\r\n        .container {{\r\n            background-color: #fff;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            max-width: 600px;\r\n            border-radius: 8px;\r\n        }}\r\n\r\n        .header {{\r\n            background-color: #007bff;\r\n            color: #ffffff;\r\n            padding: 10px;\r\n            text-align: center;\r\n            border-top-left-radius: 8px;\r\n            border-top-right-radius: 8px;\r\n        }}\r\n\r\n        .content {{\r\n            padding: 20px;\r\n            text-align: center;\r\n        }}\r\n\r\n        .footer {{\r\n            margin-top: 20px;\r\n            text-align: center;\r\n            padding: 10px;\r\n            font-size: 12px;\r\n            color: #888;\r\n        }}\r\n\r\n        a {{\r\n            color: #007bff;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            <h2>Confirmation Required</h2>\r\n        </div>\r\n        <div class=\"content\">\r\n            <p>Hello,</p>\r\n            <p>Thank you for registering with us. Please confirm your email address by clicking on the link below:</p>\r\n            <a href=\"{link}\">Confirm my email address</a>\r\n            <p>If you did not request this, please ignore this email.</p>\r\n        </div>\r\n        <div class=\"footer\">\r\n            Regards,<br>\r\n            The Team at MaldsShop<br>\r\n            <a href=\"https://malds.dev\">malds.dev</a>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
            return bodyText;
        }
        public string ResetPassword(string link)
        {
            var bodyText = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>Password Reset Request</title>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n        }}\r\n\r\n        .container {{\r\n            background-color: #fff;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            max-width: 600px;\r\n            border-radius: 8px;\r\n        }}\r\n\r\n        .header {{\r\n            background-color: #f44336;\r\n            color: #ffffff;\r\n            padding: 10px;\r\n            text-align: center;\r\n            border-top-left-radius: 8px;\r\n            border-top-right-radius: 8px;\r\n        }}\r\n\r\n        .content {{\r\n            padding: 20px;\r\n            text-align: center;\r\n        }}\r\n\r\n        .footer {{\r\n            margin-top: 20px;\r\n            text-align: center;\r\n            padding: 10px;\r\n            font-size: 12px;\r\n            color: #888;\r\n        }}\r\n\r\n        a {{\r\n            color: #007bff;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            <h1>Password Reset Request</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n            <p>Hello,</p>\r\n            <p>You are receiving this email because we received a password reset request for your account. Please click the link below to reset your password:</p>\r\n            <a href=\"{link}\">Reset my password</a>\r\n            <p>This password reset link will expire in 5 minutes.</p>\r\n            <p>If you did not request a password reset, no further action is required.</p>\r\n        </div>\r\n        <div class=\"footer\">\r\n            Regards,<br>\r\n            MaldsShop Team<br>\r\n            <a href=\"https://malds.dev\">malds.dev</a>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
            return bodyText;
        }
        public string OrderConfirmation(ShippingCart shippingCart)
        {
            var firstPart = $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>Order Confirmation</title>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n        }}\r\n\r\n        .container {{\r\n            background-color: #fff;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            max-width: 600px;\r\n            border-radius: 8px;\r\n        }}\r\n\r\n        .header {{\r\n            background-color: #4CAF50;\r\n            color: #ffffff;\r\n            padding: 10px;\r\n            text-align: center;\r\n            border-top-left-radius: 8px;\r\n            border-top-right-radius: 8px;\r\n        }}\r\n\r\n        .content {{\r\n            padding: 20px;\r\n            text-align: center;\r\n        }}\r\n\r\n        .footer {{\r\n            margin-top: 20px;\r\n            text-align: center;\r\n            padding: 10px;\r\n            font-size: 12px;\r\n            color: #888;\r\n        }}\r\n\r\n        a {{\r\n            color: #007bff;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            <h1>Your order successfully procesed</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n            <p>Your order details:</p>\r\n            <table>\r\n                <thead>\r\n                    <tr>\r\n                        <th>#</th>\r\n                        <th>Title</th>\r\n                        <th>Quantity</th>\r\n                        <th>Price</th>\r\n                        <th>Total</th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody>\r\n";
            int i = 1;
            int grandTotal = 0;
            List<string> secondPart = new List<string>();
            foreach (var item in shippingCart.ShippingCartItems)
            {
                string title = item.Product.Title;
                int quantity = item.Quantity;
                int price = item.Product.Price;
                int totalPrice = quantity * price;
                grandTotal += totalPrice;

                string text = $"<tr><td>{i}</td>\r\n                        <td>{title}</td>\r\n                        <td>{quantity}</td>\r\n                        <td>{(price / 100.0).ToString("F2")}</td>\r\n                        <td>{(totalPrice / 100.0).ToString("F2")}</td></tr>";

                secondPart.Add(text);

                i++;
            }
            var thirdPart = $"\r\n                </tbody>\r\n            </table>\r\n            <h3>Grand total: {(grandTotal / 100.0).ToString("F2")}</h3>\r\n        </div>\r\n        <div class=\"footer\">\r\n            Regards,<br>\r\n            MaldsShop Team<br>\r\n            <a href=\"https://malds.dev\">malds.dev</a>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

            string bodyText = firstPart;
            foreach (var item in secondPart)
            {
                bodyText += item; 
            }
            bodyText += thirdPart;
            return bodyText;
        }
    }
}
