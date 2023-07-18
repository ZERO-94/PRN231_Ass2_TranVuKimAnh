using BookManagement.Webapp.Models;
using BookManagement.Webapp.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Diagnostics;

namespace BookManagement.Webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string endpoint = "https://localhost:7185";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Book");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel request)
        {
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Book");
            }
            using (var channel = GrpcChannel.ForAddress(endpoint))
            {
                try
                {
                    var client = new GrpcAuth.GrpcAuthClient(channel);
                    var res = await client.LoginAsync(new LoginRequest
                    {
                        Username = request.Username,
                        Password = request.Password
                    });

                    var token = res.AccessToken;
                    Response.Cookies.Append("token", token);
                    return RedirectToAction("Index", "Book");
                } catch (RpcException ex)
                {
                    if (ex.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
                    {
                        ViewBag.Error = "Invalid credential";
                    }
                    else
                    {
                        ViewBag.Error = "Something went wrong, please try again later";
                    }
                }
                return View(request);
                    
            }
        }

        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Book");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginModel request)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Book");
            }

            using (var channel = GrpcChannel.ForAddress(endpoint))
            {
                try
                {
                    var client = new GrpcAuth.GrpcAuthClient(channel);
                    var res = await client.RegisterAsync(new RegisterRequest
                    {
                        Username = request.Username,
                        Password = request.Password
                    });

                    return RedirectToAction("Login");
                }
                catch (RpcException ex)
                {
                    if (ex.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
                    {
                        ViewBag.Error = "Username already existed";
                    }
                    else
                    {
                        ViewBag.Error = "Something went wrong, please try again later";
                    }
                }
                return View(request);

            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Login");
        }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set;}
    }
}
