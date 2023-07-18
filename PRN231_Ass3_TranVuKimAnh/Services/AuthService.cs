using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.AccountRepository;
using BookManagement.Infrastructure.Repositories.BookRepository;
using Grpc.Core;
using PRN231_Ass3_TranVuKimAnh.Protos;

namespace PRN231_Ass3_TranVuKimAnh.Services
{
    public class AuthService: GrpcAuth.GrpcAuthBase
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly JwtService _jwtService;
        public AuthService(ILogger<AuthService> logger, IAccountRepository accountRepository, JwtService jwtService)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _jwtService = jwtService;
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context) {

            var account = _accountRepository.FirstOrDefault(expression: x => x.Username == request.Username && x.Password == request.Password);

            if (account == null)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Not authorized"));
            }

            return new LoginResponse() { AccessToken = _jwtService.GenerateJSONWebToken(account) };
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var account = _accountRepository.FirstOrDefault(expression: x => x.Username == request.Username);

            if (account != null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad request"));
            }

            Account newAccount = new Account()
            {
                Password = request.Password,
                Username = request.Username,
                Role = Role.User
            };

            _accountRepository.Add(newAccount);

            return new RegisterResponse()
            {
                Success = true,
            };
        }
    }
}
