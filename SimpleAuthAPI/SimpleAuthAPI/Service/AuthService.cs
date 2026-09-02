using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleAuthAPI.Model;
using SimpleAuthAPI.Repository;
using System.Security.Claims;
using System.Text;

namespace SimpleAuthAPI.Service;

public class AuthService(IAuthRepository authRepo, IConfiguration configuration)
{
    public string GenerateToken(int user_id, string email)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user_id.ToString()),
                new Claim(ClaimTypes.Email, email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
    public async Task<ResponseModel<int>> RegisterUser(RegisterUserRequest request)
    {
        var response = new ResponseModel<int>();

        try
        {
            // validasi 
            // cek password matching
            if (request.Password != request.ConfirmPassword)
            {
                throw new DomainException("Password does not match");
            }

            // cek user email
            if (await authRepo.IsUserExistsByEmail(request.Email))
            {
                throw new DomainException("Email already registered");
            }
            
            // hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            request.Password = hashedPassword; 

            var result = await authRepo.Register(request);

            response.Message = "User registered successfully";
            response.Data = result;
        }
        catch (DomainException ex)
        {
            response.ResponseType = ResponseType.ClientError;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ResponseModel<string>> LoginUser(LoginUserRequest request)
    {
        var response = new ResponseModel<string>();

        try
        {
            // get user
            var result = await authRepo.GetUserByEmail(request.Email);
            if (result is null)
            {
                throw new DomainException("email or password is wrong");
            }

            // hash password
            var passwordVerified = BCrypt.Net.BCrypt.Verify(request.Password, result.PasswordHashed);
            if (!passwordVerified)
            {
                throw new DomainException("email or password is wrong");
            }

            // generate token 
            var token = GenerateToken(result.UserId, result.Email);

            response.Data = token;
            response.Message = "User Login Successfully";
        }
        catch (DomainException ex)
        {
            response.ResponseType = ResponseType.ClientError;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ResponseModel<List<string>>> GetAll()
    {
        var response = new ResponseModel<List<string>>();

        try
        {
            var result = await authRepo.GetAll();

            response.Data = result;
            response.Message = "Success";
        }
        catch (DomainException ex)
        {
            response.ResponseType = ResponseType.ClientError;
            response.Message = ex.Message;
        }

        return response;
    }
}
