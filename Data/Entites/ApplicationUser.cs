using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entites;

public class ApplicationUser : IdentityUser
{
    public string? UserProfileId { get; set; }
    public virtual UserProfile? UserProfile { get; set; }
    public string? UserAddressId { get; set; }
    public virtual UserAddress? UserAddress { get; set; }
}
