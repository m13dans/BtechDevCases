namespace SimpleAuthAPI.Model;

public enum ResponseType
{
    Success,
    ClientError,
    ServerError
}

public class  ResponseModel<T>
{
    public T Data { get; set; }
    public ResponseType ResponseType { get; set; } = ResponseType.Success;
    public string Message { get; set; }
}

public class RegisterUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class LoginUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginUserResponse
{
    public int UserId { get; set; }
    public string Token { get; set; }
    public string Email { get; set; }
    public string PasswordHashed { get; set; }
}

