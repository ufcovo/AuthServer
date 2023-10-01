﻿using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Exceptions;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // api/user/createuser
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            //throw new CustomException("Veri tabanı ile ilgili bir hata meydana geldi.");
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        // api/user/createuser
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name));
        }

        // api/user/CreateUserRole/{username}
        [HttpPost("CreateUserRole/{username}")]
        public async Task<IActionResult> CreateUserRoles(string username)
        {
            return ActionResultInstance(await _userService.CreateUserRoles(username));
        }
    }
}
