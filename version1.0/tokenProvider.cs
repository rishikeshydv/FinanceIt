using System;
using System.Text;
using System.Threading.Tasks;       

public class tokenProvider : tokenProvider.tokenProviderBase
{
    private readonly ApplicationDbContext _dbContext;

    public tokenProvider(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public override async Task<tokenResponse> getToken(tokenRequest request, ServerCallContext context)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.Password == request.Password);

        if (if (user != null))
        {
            var issuer = "https://localhost:5260/"; // Set your JWT issuer here
            var audience = "https://localhost:5260/"; // Set your JWT audience here
            var key = Encoding.ASCII.GetBytes("ertwet3245sgf2342werwergww4352345"); // Set your JWT secret key here

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, request.UserName),
                    // Add other relevant claims here, such as user roles, permissions, etc.
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return new tokenResponse { Token = jwtToken };
        }

        throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid credentials"));
    }


    public override async Task<registerResponse> registerUser(registerRequest request, ServerCallContext context)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (existingUser != null)
        {
            return new registerResponse
            {
                Success = false,
                Message = "Username already exists. Please choose a different username."
            };
        }

        var newUser = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return new registerResponse
        {
            Success = true,
            Message = "User registered successfully."
        };
    }
}
