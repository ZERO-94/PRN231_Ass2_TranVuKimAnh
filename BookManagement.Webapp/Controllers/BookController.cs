using BookManagement.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net;
using BookManagement.Webapp.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BookManagement.Webapp.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Grpc.Core;
using Grpc.Net.Client;
using BookManagement.Webapp.Protos;
using System.Net.Http;

namespace BookManagement.Webapp.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private string endpoint = "https://localhost:7185";
        // GET: BookController
        public async Task<ActionResult> Index(int page = 1, string search = "")
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                {
                    metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                }
                return Task.CompletedTask;
            });
            using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            }))
            {
                try
                {
                    
                    var client = new GrpcBook.GrpcBookClient(channel);
                    var res = await client.GetAllAsync(new GetAllRequest
                    {
                        Page= page,
                        Search= search,
                        PageSize=5
                    });

                    ViewBag.TotalPage = res.TotalPage;
                    ViewBag.Page = page;
                    ViewBag.Search = search;
                    return View(res.Items.ToList().ConvertAll(x => new BookModel()
                    {
                        Author= x.Author,
                        Id= x.Id,
                        ISBN= x.ISBN,
                        PressId= x.PressId,
                        Price= (decimal)x.Price,
                        Title= x.Title,
                        Location= new ()
                        {
                            BookId= x.Id,
                            City= x.Location.City,
                            Street= x.Location.Street
                        },
                        Press= new ()
                        {
                            Id= x.Press.Id,
                            Category= x.Press.Category,
                            Name= x.Press.Name
                        }
                    })); 
                }
                catch (RpcException ex)
                {

                }
                return View();
            }
        }

        // GET: BookController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                {
                    metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                }
                return Task.CompletedTask;
            });
            using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            }))
            {
                try
                {

                    var client = new GrpcBook.GrpcBookClient(channel);
                    var res = await client.GetByIdAsync(new GetByIdRequest
                    {
                        Id = id,
                    });

                    return View(new BookModel()
                    {
                        Author = res.Author,
                        Id = res.Id,
                        ISBN = res.ISBN,
                        Price = (decimal)res.Price,
                        Title = res.Title,
                        Location = new()
                        {
                            BookId = res.Id,
                            City = res.Location.City,
                            Street = res.Location.Street
                        },
                        PressId = res.PressId,
                        Press = new()
                        {
                            Id = res.Press.Id,
                            Category = res.Press.Category,
                            Name = res.Press.Name
                        }
                    });
                }
                catch (RpcException ex)
                {

                }
                return View();
            }
        }

        // GET: BookController/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookModel press)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                {
                    metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                }
                return Task.CompletedTask;
            });
            using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            }))
            {
                try
                {

                    var client = new GrpcBook.GrpcBookClient(channel);
                    var res = await client.CreateAsync(new CreateRequest
                    {
                        Author = press.Author,
                        ISBN = press.ISBN,
                        Price = (float)press.Price,
                        Title = press.Title,
                        Location = new()
                        {
                            City = press.Location.City,
                            Street = press.Location.Street
                        },
                        PressId = 1
                    });

                    if (res.Success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (RpcException ex)
                {

                }
                return View(press);
            }
        }

        // GET: BookController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                {
                    metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                }
                return Task.CompletedTask;
            });
            using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            }))
            {

                var client = new GrpcBook.GrpcBookClient(channel);
                var res = await client.GetByIdAsync(new GetByIdRequest
                {
                    Id = id,
                });

                return View(new BookModel()
                {
                    Author = res.Author,
                    Id = res.Id,
                    ISBN = res.ISBN,
                    Price = (decimal)res.Price,
                    Title = res.Title,
                    Location = new()
                    {
                        BookId = res.Id,
                        City = res.Location.City,
                        Street = res.Location.Street
                    },
                    PressId= res.PressId,
                    Press = new()
                    {
                        Id = res.Press.Id,
                        Category = res.Press.Category,
                        Name = res.Press.Name
                    }
                });
            }
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, BookModel press)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                {
                    metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                }
                return Task.CompletedTask;
            });
            using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            }))
            {
                try
                {

                    var client = new GrpcBook.GrpcBookClient(channel);
                    var res = await client.UpdateAsync(new UpdateRequest
                    {
                        Id = id,
                        Author = press.Author,
                        ISBN = press.ISBN,
                        Price = (float)press.Price,
                        Title = press.Title,
                        Location = new()
                        {
                            City = press.Location.City,
                            Street = press.Location.Street
                        },
                        PressId = 1
                    });

                    if(res.Success)
                    {
                        return RedirectToAction(nameof(Index));
                    } 
                    
                }
                catch (RpcException ex)
                {

                }
                return View(press);
            }
        }

        // GET: BookController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                {
                    metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                }
                return Task.CompletedTask;
            });
            using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            }))
            {

                var client = new GrpcBook.GrpcBookClient(channel);
                var res = await client.GetByIdAsync(new GetByIdRequest
                {
                    Id = id,
                });

                return View(new BookModel()
                {
                    Author = res.Author,
                    Id = res.Id,
                    ISBN = res.ISBN,
                    Price = (decimal)res.Price,
                    Title = res.Title,
                    Location = new()
                    {
                        BookId = res.Id,
                        City = res.Location.City,
                        Street = res.Location.Street
                    },
                    PressId = res.PressId,
                    Press = new()
                    {
                        Id = res.Press.Id,
                        Category = res.Press.Category,
                        Name = res.Press.Name
                    }
                });
            }
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var credentials = CallCredentials.FromInterceptor((context, metadata) =>
                {
                    if (!string.IsNullOrEmpty(Request.Cookies["token"]))
                    {
                        metadata.Add("Authorization", $"Bearer {Request.Cookies["token"]}");
                    }
                    return Task.CompletedTask;
                });
                using (var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
                }))
                {

                    var client = new GrpcBook.GrpcBookClient(channel);
                    var res = await client.DeleteAsync(new DeleteRequest
                    {
                        Id = id,
                    });

                    if (res.Success)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            } catch (Exception ex) { }
            return View();
        }
    }
}
