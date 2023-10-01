using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        #region Constructor
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        #region CreateUserAsync
        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp 
            { 
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
            };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(r => r.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), StatusCodes.Status400BadRequest);
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
        }
        #endregion

        #region GetUserByNameAsync
        public async Task<Response<UserAppDto>> GetUserByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null) return Response<UserAppDto>.Fail("Username not found", StatusCodes.Status404NotFound, true);
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
        }
        #endregion

        #region CreateUserRoles
        public async Task<Response<NoContent>> CreateUserRoles(string username)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new() { Name = "Admin" });
                await _roleManager.CreateAsync(new() { Name = "Manager" });
            }
            
            var user = await _userManager.FindByNameAsync(username);
            await _userManager.AddToRoleAsync(user, "Admin");
            await _userManager.AddToRoleAsync(user, "Manager");

            return Response<NoContent>.Success(StatusCodes.Status201Created);
        }
        #endregion
    }
}
