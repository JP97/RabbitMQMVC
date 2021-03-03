using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQMVC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Console.WriteLine("hello world");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            var booked = Request.Form["Booked"];
            var name = Request.Form["Name"];
            var email = Request.Form["Email"];
            var tour = Request.Form["Tour"];

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs",
                                        type: "topic");

                var routingKey = (booked == "on") ? "tour.booked" : "tour.canceled";
                var message = (tour != string.Empty)
                              ? $"you booked following tour: {tour}"
                              : "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic_logs",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
            }

            return View("Index");

        }
    }
}
