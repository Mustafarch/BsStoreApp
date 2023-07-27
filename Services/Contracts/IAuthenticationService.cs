using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuthDto);
        Task<TokenDto> CreateToken(bool populateExp); //Task<string> CreateToken(); // Refresh Token özelliği eklemiş olduk. otomatik tokenları kendisi oluşturup tarihini atayacak.
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
