using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IPhotoService photoService;

    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.photoService = photoService;
    }

    [HttpGet]

    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();
        return Ok(users);

    }

    [HttpGet("{username}")]

    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await userRepository.GetMemberAsync(username);

    }


    // [HttpGet("{id}")]

    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     return await userRepository.GetUserByIdAsync(id);
    // }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {

        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();
        mapper.Map(memberUpdateDto, user);
        System.Console.WriteLine(memberUpdateDto);
        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("failed to update user");

    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        if (user.Photos.Count == 0) photo.IsMain = true;
        user.Photos.Add(photo);
        if (await userRepository.SaveAllAsync()) 
        {
            
        return CreatedAtAction(nameof (GetUser),new {username = user.UserName}, mapper.Map<PhotoDto>(photo));

        } 
        return BadRequest("Problem Adding Photo");

    }

    [HttpPut("set-main-photo/{photoId}")]

    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null ) return NotFound();

        var photo = user.Photos.FirstOrDefault (x=>x.Id == photoId);

        if(photo == null) return NotFound();

        if(photo.IsMain) return BadRequest("this is already your main photo");

        var currentMain =  user.Photos.FirstOrDefault(x=>x.IsMain);
        if(currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if(await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem Setting Main Photo");
    }


    [HttpDelete("delete-photo/{photoId}")]

    public async Task<ActionResult> DeletePhoto(int photoId){
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

        if(photo == null) return NotFound();
        
        if(photo.IsMain) return BadRequest("you can not delete your main photo");
    
        if(photo.PublicId !=null){

            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
        }
            user.Photos.Remove(photo);
            if(await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting photo");
    }
}
