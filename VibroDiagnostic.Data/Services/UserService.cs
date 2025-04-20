using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VibroDiagnostic.Core.Entities;
using VibroDiagnostic.Core.Interfaces;
using VibroDiagnostic.Data.Contexts;

namespace VibroDiagnostic.Data.Services;

public class UserService : IUserService
{
    private readonly AppSettings _appSettings;
    private readonly SensorContext db;

    public UserService(IOptions<AppSettings> appSettings, SensorContext _db)
    {
        _appSettings = appSettings.Value;
        db = _db;
    }

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);

        // return null if user not found
        if (user == null) return null;

        // authentication successful so generate jwt token
        var token = await generateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await db.Users.Where(x => x.isActive == true).ToListAsync();
    }

    public async Task<User?> GetById(int id)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> AddAndUpdateUser(User userObj)
    {
        bool isSuccess = false;
        if (userObj.Id > 0)
        {
            var obj = await db.Users.FirstOrDefaultAsync(c => c.Id == userObj.Id);
            if (obj != null)
            {
                // obj.Address = userObj.Address;
                obj.FirstName = userObj.FirstName;
                obj.LastName = userObj.LastName;
                db.Users.Update(obj);
                isSuccess = await db.SaveChangesAsync() > 0;
            }
        }
        else
        {
            await db.Users.AddAsync(userObj);
            isSuccess = await db.SaveChangesAsync() > 0;
        }

        return isSuccess ? userObj: null;
    }
    // helper methods
    private async Task<string> generateJwtToken(User user)
    {
        //Generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = await Task.Run(() =>
        {

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.CreateToken(tokenDescriptor);
        });

        return tokenHandler.WriteToken(token);
    }
}