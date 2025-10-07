using Coling.Aplication.DTOs.UsersManagement;
using Coling.Aplication.Validators;
using Coling.Application.Interfaces.Services;
using Coling.Application.Mappers.ActionResponse;
using Coling.Application.Mappers.UsersManagement;
using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Interfaces.Repositories.UsersManagement;
using Microsoft.AspNetCore.Identity;

namespace Coling.Application.UseCases.UsersManagement;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly UserManager<Domain.Entities.UsersManagement.User> _userManager;

    public LoginUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        UserManager<Domain.Entities.UsersManagement.User> userManager)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<ActionResponse<LoginResponseDto>> ExecuteAsync(LoginRequestDto dto)
    {
        var dtoValidationResult = dto.TryValidateModel();
        if (!dtoValidationResult.WasSuccessful)
            return dtoValidationResult.ChangeNullActionResponseType<LoginRequestDto, LoginResponseDto>();

        var userResult = await _userRepository.GetAsync(u => u.UserName == dto.UserName);
        if (!userResult.WasSuccessful)
            return ActionResponse<LoginResponseDto>.NotFound("Usuario o contraseña incorrectos.");

        var user = userResult.Result!;

        if (!user.IsActive)
            return ActionResponse<LoginResponseDto>.Failure("La cuenta está desactivada.", ResultCode.Conflict);

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            return ActionResponse<LoginResponseDto>.NotFound("Usuario o contraseña incorrectos.");

        var fullUserResult = await _userRepository.GetFullData(user.Id);
        if (!fullUserResult.WasSuccessful)
            return ActionResponse<LoginResponseDto>.Failure("Error al obtener datos del usuario.", ResultCode.DatabaseError);

        user = fullUserResult.Result!;

        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault() ?? "";

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, userRole)
        };

        var tokenResult = await _tokenService.GenerateTokenAsync(user, claims);
        if (!tokenResult.WasSuccessful)
            return ActionResponse<LoginResponseDto>.Failure("Error al generar el token.", ResultCode.DatabaseError);

        var loginResponse = user.ToLoginResponseDto(userRole, tokenResult.Result!);
        return ActionResponse<LoginResponseDto>.Success(loginResponse, "Inicio de sesión exitoso.");
    }
}
