using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        #region Constructor
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        #endregion

        #region CreateToken
        // api/auth/createtoken
        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            return ActionResultInstance(await _authenticationService.CreateTokenAsync(loginDto));
        }
        #endregion

        #region CreateTokenByClient
        // api/auth/createtokenbyclient
        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            return ActionResultInstance(_authenticationService.CreateTokenByClient(clientLoginDto));
        }
        #endregion

        #region RevokeRefreshToken
        // api/auth/revokerefreshtoken
        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            return ActionResultInstance(await _authenticationService.RevokeRefreshTokenAsync(refreshTokenDto.Token));
        }
        #endregion

        #region CreateTokenByRefreshToken
        // api/auth/createtokenbyrefreshtoken
        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            return ActionResultInstance(await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.Token));
        }
        #endregion
    }
}
