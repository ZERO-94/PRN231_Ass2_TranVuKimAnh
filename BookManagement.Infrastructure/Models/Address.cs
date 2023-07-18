using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Infrastructure.Models
{
    public class Address
    {
        [Key]
        public int BookId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        
    }
}
