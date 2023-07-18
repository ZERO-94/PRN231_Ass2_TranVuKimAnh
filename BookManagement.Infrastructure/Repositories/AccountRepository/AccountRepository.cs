using BookManagement.Infrastructure.Data;
using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Infrastructure.Repositories.AccountRepository
{
    public class AccountRepository : BaseRepository<BookManagementContext, Account>, IAccountRepository
    {
        public AccountRepository(BookManagementContext context) : base(context)
        {
        }
    }
}
