using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserService service, IMapper mapper, IPhotoService photoService)
    {
        _service = service;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetFilter([FromQuery] UserGetRequest request)
    {
        var filter = _mapper.Map<UserBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<UserResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<UserResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(UserRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<UserBM>(request));
        var response = _mapper.Map<UserResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.UserId }, response);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserResponse>> Update(int id, [FromForm] UserUpdateRequest request)
    {
        // Kiểm tra validation
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Lấy thông tin user hiện tại
        var existingUser = await _service.GetByIdAsync(id, includeDetails: false);
        if (existingUser == null) return NotFound(new { message = "Người dùng không tồn tại" });

        // Kiểm tra email đã tồn tại chưa (nếu có thay đổi)
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != existingUser.Email)
        {
            var emailExists = await _service.IsEmailExistsAsync(request.Email, id);
            if (emailExists)
            {
                return BadRequest(new { message = "Email đã được sử dụng bởi người dùng khác" });
            }
        }

        // Kiểm tra số điện thoại đã tồn tại chưa (nếu có thay đổi)
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != existingUser.PhoneNumber)
        {
            var phoneExists = await _service.IsPhoneNumberExistsAsync(request.PhoneNumber, id);
            if (phoneExists)
            {
                return BadRequest(new { message = "Số điện thoại đã được sử dụng bởi người dùng khác" });
            }
        }

        var model = _mapper.Map<UserBM>(request);

        // Giữ nguyên các trường không được phép cập nhật
        model.Username = existingUser.Username;  // Không cho phép cập nhật Username
        model.PasswordHash = existingUser.PasswordHash;
        model.Role = existingUser.Role;

        // Cập nhật email nếu có, nếu không giữ nguyên
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            model.Email = existingUser.Email;
        }

        // Cập nhật phone nếu có, nếu không giữ nguyên
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            model.PhoneNumber = existingUser.PhoneNumber;
        }

        // Cập nhật address nếu có, nếu không giữ nguyên
        if (string.IsNullOrWhiteSpace(request.Address))
        {
            model.Address = existingUser.Address;
        }

        // Chỉ cập nhật avatarUrl khi có AvatarFile mới
        if (request.AvatarFile != null && request.AvatarFile.Length > 0)
        {
            var url = await _photoService.UploadImageAsync(request.AvatarFile);
            if (!string.IsNullOrWhiteSpace(url))
            {
                model.Avatarurl = url;
            }
        }
        else
        {
            // Giữ nguyên avatarUrl hiện tại nếu không có file mới
            model.Avatarurl = existingUser.Avatarurl;
        }

        var updated = await _service.UpdateAsync(id, model);
        if (updated == null) return NotFound(new { message = "Cập nhật thất bại" });
        return Ok(_mapper.Map<UserResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


