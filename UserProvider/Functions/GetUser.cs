using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions
{
    public class GetUser
    {
        private readonly ILogger<GetUser> _logger;
        private readonly DataContext _context;

        public GetUser(ILogger<GetUser> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.UserAddress)
                    .ToListAsync();

                return new OkObjectResult(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
