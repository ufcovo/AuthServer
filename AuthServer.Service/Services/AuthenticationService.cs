using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Constructor
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        
        public AuthenticationService(IOptions<List<Client>> optionsClients, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }
        #endregion

        #region CreateTokenAsync
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if(loginDto is null) throw new ArgumentNullException(nameof(loginDto));
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) return Response<TokenDto>.Fail("Email or Password is wrong", StatusCodes.Status400BadRequest, true);
            if(!await _userManager.CheckPasswordAsync(user, loginDto.Password)) 
                return Response<TokenDto>.Fail("Email or Password is wrong", StatusCodes.Status400BadRequest, true);
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(r => r.UserId == user.Id).SingleOrDefaultAsync();
            if (userRefreshToken is null)
                await _userRefreshTokenService.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration
                });
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, StatusCodes.Status200OK);
        }
        #endregion

        #region CreateTokenByClient
        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(r => r.Id == clientLoginDto.ClientId && r.Secret == clientLoginDto.ClientSecret);
            if (client is null) return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", StatusCodes.Status404NotFound, true);
            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, StatusCodes.Status200OK);
        }
        #endregion

        #region CreateTokenByRefreshToken
        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(r => r.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken is null) return Response<TokenDto>.Fail("RefreshToken not found", StatusCodes.Status404NotFound, true);
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if(user is null) return Response<TokenDto>.Fail("UserID not found", StatusCodes.Status404NotFound, true);
            var tokenDto = _tokenService.CreateToken(user);
            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, StatusCodes.Status200OK);
        }
        #endregion

        #region RevokeRefreshToken
        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(r => r.Code == refreshToken).SingleOrDefaultAsync();
            if(existRefreshToken is null) return Response<NoDataDto>.Fail("RefreshToken not found", StatusCodes.Status404NotFound, true);
            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(StatusCodes.Status200OK);
        }
        #endregion
    }
}
