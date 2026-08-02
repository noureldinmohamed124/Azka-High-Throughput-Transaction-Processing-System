using Azka_Transaction_Processing_System.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static readonly string[] UserIdClaimTypes =
        {
            ClaimTypes.NameIdentifier,
            JwtRegisteredClaimNames.Sub,
        };

        public int UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                foreach (var claimType in UserIdClaimTypes)
                {
                    var value = user?.FindFirst(claimType)?.Value;

                    if (int.TryParse(value, out var id))
                        return id;
                }

                throw new UnauthorizedAccessException("User ID claim not found.");
            }
        }

        

    }
}
