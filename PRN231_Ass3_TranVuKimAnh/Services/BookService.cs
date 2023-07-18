using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.BookRepository;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PRN231_Ass3_TranVuKimAnh.Protos;

namespace PRN231_Ass3_TranVuKimAnh.Services
{
    [Authorize]
    public class BookService: GrpcBook.GrpcBookBase
    {
        private readonly ILogger<BookService> _logger;
        private readonly IBookRepository _bookRepository;
        public BookService(ILogger<BookService> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository= bookRepository;
        }

        public override async Task<GetByIdResponse> GetById(GetByIdRequest request, ServerCallContext context)
        {
            var res = _bookRepository.FirstOrDefault(x => x.Id == request.Id, includeFunc: query => query.Include(book => book.Press).Include(book => book.Location));

            return new GetByIdResponse()
            {
                Author = res.Author,
                Id= res.Id,
                ISBN= res.ISBN,
                PressId= res.PressId,
                Price= (float)res.Price,
                Title= res.Title,
                Press = new ()
                {
                    Id= res.Press.Id,
                    Category= res.Press.Category.ToString(),
                    Name= res.Press.Name,
                },
                Location = new ()
                {
                    BookId = res.Location.BookId,
                    City= res.Location.City,
                    Street= res.Location.Street,
                }
            };
        }

        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            var booksQuery = _bookRepository.Pagination(
                includeFunc: query => query.Include(x => x.Press).Include(x => x.Location), 
                page: !request.HasPage ? 1 : request.Page, 
                pageSize: !request.HasPageSize ? 20 : request.PageSize,
                expression: book => book.Title.Contains(request.Search));
            var res = new GetAllResponse();

            res.Total = booksQuery.Item1;
            res.Page = !request.HasPage ? 1 : request.Page;
            res.PageSize = !request.HasPageSize ? 20 : request.PageSize;
            res.TotalPage = ((int)Math.Ceiling(((double)booksQuery.Item1 / res.PageSize)));


            res.Items.AddRange(booksQuery.Item2.Select(res => new GetByIdResponse()
            {
                Author = res.Author,
                Id = res.Id,
                ISBN = res.ISBN,
                PressId = res.PressId,
                Price = (float)res.Price,
                Title = res.Title,
                Press = new()
                {
                    Id = res.Press.Id,
                    Category = res.Press.Category.ToString(),
                    Name = res.Press.Name,
                },
                Location = new()
                {
                    BookId = res.Location.BookId,
                    City = res.Location.City,
                    Street = res.Location.Street,
                }
            }).ToList());
            return res;
        }

        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            Book book = new Book()
            {
                Author = request.Author,
                ISBN = request.ISBN,
                Location = new Address()
                {
                    City = request.Location.City,
                    Street = request.Location.Street
                },
                PressId = request.PressId,
                Title = request.Title,
                Price = (decimal)request.Price,

            };
            _bookRepository.Add(book);

            return new CreateResponse()
            {
                Success = true
            };
        }

        public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            var book = _bookRepository.FirstOrDefault(expression: x => x.Id == request.Id, includeFunc: x => x.Include(book => book.Location));

            if (book == null)
            {
                return new UpdateResponse()
                {
                    Success = false
                };
            }

            book.Author = request.Author;
            book.ISBN = request.ISBN;
            book.Location.City = request.Location.City;
            book.Location.Street = request.Location.Street;
            book.PressId = request.PressId;
            book.Title = request.Title;
            book.Price = (decimal)request.Price;

            _bookRepository.Update(book);
            return new UpdateResponse()
            {
                Success = true
            };
        }

        public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            var book = _bookRepository.FirstOrDefault(expression: x => x.Id == request.Id, includeFunc: query => query.Include(x => x.Location));
            if (book == null)
                return new DeleteResponse()
                {
                    Success = false
                };

            _bookRepository.Remove(book);
            return new DeleteResponse()
            {
                Success = true
            };
        }
    }
}
