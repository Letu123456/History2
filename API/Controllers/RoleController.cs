﻿using Business.DTO;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "admin")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepo _roleRepo;
        public RoleController(IRoleRepo roleRepo)
        {
            _roleRepo = roleRepo;
        }




        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {

            try
            {
                return Ok(await _roleRepo.GetRoles());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{rolename}")]


        public async Task<IActionResult> CreateRole(string rolename)
        {
            try
            {
                return Ok(await _roleRepo.CreateRole(rolename));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{roleId}")]


        public async Task<IActionResult> DeleteRole(string roleId)
        {
            try
            {
                return Ok(await _roleRepo.DeleteRole(roleId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);


            }
        }

        // User-Roles

        [HttpGet("user")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                return Ok(await _roleRepo.GetUserRole(userId));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [HttpPost("user")]
        public async Task<IActionResult> AddUserToRole(string userid, string role)
        {
            try
            {
                return Ok(await _roleRepo.AddUserToRole(userid, role));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("user")]

        public async Task<IActionResult> DeleteUserFromRole(string userid, string role)
        {
            try
            {
                return Ok(await _roleRepo.RemoveUserFromRole(userid, role));

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("user")]

        public async Task<IActionResult> ChangeUserRole(string userId, string OldRole, string NewRole)
        {
            try
            {
                return Ok(await _roleRepo.ChangeUserRole(userId, OldRole, NewRole));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateRole")]

        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {
                return Ok(await _roleRepo.UpdateUserRole(updateRoleDto.UserId, updateRoleDto.NewRole));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateRoleToManager")]

        public async Task<IActionResult> UpdateUserRoleToManager(string userId)
        {

            string manager = "manager";
            try
            {
                return Ok(await _roleRepo.UpdateUserRole(userId, manager));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
