using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkServer.Data;
using CourseWorkServer.Models;
using System.Linq.Expressions;
using System.Text;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace CourseWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _key = "kT7x2j9Pq5zL8mW3nF6vR9yB4cH2tK=="; // 32 байта для AES-256
        private string Decrypt(string cipherText)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);
                byte[] iv = new byte[16];
                Array.Copy(buffer, 0, iv, 0, iv.Length);
                aes.IV = iv;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public UsersController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public class UserDto
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<ObservableCollection<User>>> GetUsers() // используется в admin.cs, cashier.cs
        {
            ObservableCollection<User> Users = new ObservableCollection<User>();
            if (_context.Users == null) return Ok(null);
            foreach (User user in _context.Users)
            {
                Users.Add(user);
            }
            return Ok(Users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPut("{id}/cart/add")]
        public async Task<IActionResult> AddToCart(int id, [FromBody] string? newprod) // cart.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Пользователь не найден");
            user.Cart = newprod;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/purchasestatus")]
        public async Task<IActionResult> PurchaseStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Пользователь не найден");
            user.PurchaseStatus++;
            if (user.PurchaseStatus > 3) user.PurchaseStatus = 0;
            await _context.SaveChangesAsync();
            return Ok("Успешно");
        }

        [HttpPost("add")]
        public async Task<ActionResult<User>> PostNewUser([FromBody] UserDto userdto)
        {
            string decryptedPassword = Decrypt(userdto.Password);
            var user = new User(userdto.Login, BCrypt.Net.BCrypt.HashPassword(decryptedPassword));
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }
        
        [HttpPut]
        public async Task<IActionResult> EditUsers([FromBody] User newuser) // используется в admin.cs, cashier.cs
        {
            var user = await _context.Users.FindAsync(newuser.id);
            if (user == null) return NotFound("Такого пользователя не существует");
            user.id = newuser.id;
            user.Login = newuser.Login;
            user.Password = newuser.Password;
            user.History = newuser.History;
            user.Cart = newuser.Cart;
            user.PurchaseStatus = newuser.PurchaseStatus;
            user.IsCashier = newuser.IsCashier;
            user.Place = newuser.Place;
            await _context.SaveChangesAsync();
            return Ok("Изменения успешны");
        }

        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteUser(int id) // admin.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthUser([FromBody] UserDto userdto)
        {
            var user = await _context.Users
            .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Login == userdto.Login);
            if (user == null) return NotFound();
            string decryptedPassword = Decrypt(userdto.Password);
            if (BCrypt.Net.BCrypt.Verify(decryptedPassword, user.Password))
            {
                var token = GenerateJwtToken(user);
                return Ok(new
                {
                    Token = token,
                    id = user.id
                });
            }
            else return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("{id}/getiscashier")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetIsCashier(int id) // signin.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            return Ok(user.IsCashier);
        }

        [HttpGet("{id}/getname")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetName(int id) // cart.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            return Ok(user.Login);
        }

        [HttpGet("{id}/getcart")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetCart(int id) // cart.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            return Ok(user.Cart ?? "");
        }

        [HttpGet("{id}/getpurchasestatus")]
        public async Task<ActionResult<int>> GetPurchaseStatus(int id) // cart.cs, item.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            return Ok(user.PurchaseStatus);
        }

        [HttpGet("{id}/gethistory")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetHistory(int id) // history.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            return Ok(user.History ?? "");
        }

        [HttpGet("{id}/getplace")]
        [Produces("application/json")]
        public async Task<ActionResult<string>> GetPlace(int id) // signin.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            return Ok(user.Place);
        }

        [HttpPut("{id}/purchase")]
        public async Task<IActionResult> MakePurchase(int id, [FromBody] string place) // cart.cs
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Такого пользователя не существует");
            user.PurchaseStatus = 1;
            user.Place = place;
            await _context.SaveChangesAsync();
            return Ok("Заказ оформлен");
        }

        [HttpGet("{name}/signup")]
        public async Task<ActionResult<bool>> IsLoginTaken(string name) // signup.cs
        {
            var user = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Login == name);
            if (user == null) return Ok(false);
            else return Ok(true);
        }
    }
}
