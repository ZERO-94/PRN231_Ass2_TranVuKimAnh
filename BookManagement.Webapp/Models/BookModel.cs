using BookManagement.Infrastructure.Models;

namespace BookManagement.Webapp.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public Address Location { get; set; }
        public int PressId { get; set; }
        public PressModel Press { get; set; }
    }
}
