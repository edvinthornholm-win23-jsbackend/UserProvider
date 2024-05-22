using Data.Contexts;
using Data.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        [Function("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string email = req.Query["email"];

            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult("Please provide an email address.");
            }

            try
            {
                var user = await _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.UserAddress)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new NotFoundObjectResult("User not found.");
                }

                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("UpdateUser")]
        public async Task<IActionResult> UpdateUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<ApplicationUser>(requestBody);

            if (user == null)
            {
                return new BadRequestObjectResult("User data is missing or invalid.");
            }

            try
            {
                var existingUser = await _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.UserAddress)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (existingUser == null)
                {
                    return new NotFoundObjectResult("User not found.");
                }

                // Update user properties
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

                if (existingUser.UserProfile != null)
                {
                    existingUser.UserProfile.FirstName = user.UserProfile.FirstName;
                    existingUser.UserProfile.LastName = user.UserProfile.LastName;
                    existingUser.UserProfile.Biography = user.UserProfile.Biography;
                }

                if (existingUser.UserAddress != null)
                {
                    existingUser.UserAddress.AddressLine_1 = user.UserAddress.AddressLine_1;
                    existingUser.UserAddress.AddressLine_2 = user.UserAddress.AddressLine_2;
                    existingUser.UserAddress.PostalCode = user.UserAddress.PostalCode;
                    existingUser.UserAddress.City = user.UserAddress.City;
                }

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return new OkObjectResult(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        //    [Function("UpdateUser")]
        //public async Task<IActionResult> UpdateUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        //{
        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    var user = JsonConvert.DeserializeObject<ApplicationUser>(requestBody);

        //    if (user == null)
        //    {
        //        return new BadRequestObjectResult("User data is missing or invalid.");
        //    }

        //    try
        //    {
        //        var existingUser = await _context.Users.FindAsync(user.Id);

        //        if (existingUser == null)
        //        {
        //            return new NotFoundObjectResult("User not found.");
        //        }

        //        // Update user properties
        //        existingUser.Email = user.Email;
        //        // Update other properties as needed

        //        _context.Users.Update(existingUser);
        //        await _context.SaveChangesAsync();

        //        return new OkObjectResult(existingUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while updating the user.");
        //        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        //    }
        //}

    }
}




//using Data.Contexts;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//namespace UserProvider.Functions
//{
//    public class GetUser
//    {
//        private readonly ILogger<GetUser> _logger;
//        private readonly DataContext _context;

//        public GetUser(ILogger<GetUser> logger, DataContext context)
//        {
//            _logger = logger;
//            _context = context;
//        }

//        [Function("GetUser")]
//        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
//        {
//            try
//            {
//                var users = await _context.Users
//                    .Include(u => u.UserProfile)
//                    .Include(u => u.UserAddress)
//                    .ToListAsync();

//                return new OkObjectResult(users);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching users.");
//                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
//            }
//        }
//    }
//}
