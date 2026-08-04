using ApiUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiUsuarios.Services.Senha
{
    public class SenhaService : ISenhaInterface
    {
        private const int TempoPadraoTokenAcessoMinutos = 30;
        private const int TempoPadraoRefreshTokenDias = 7;
        private readonly IConfiguration _config;

        public SenhaService(IConfiguration config)
        {
            _config = config;
        }

        public void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                senhaSalt = hmac.Key;
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
            }
        }

        public bool VerificaSenhaHash(string senha, byte[] senhaHash, byte[] senhaSalt)
        {
            using (var hmac = new HMACSHA512(senhaSalt))
            {
                var computerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                return computerHash.SequenceEqual(senhaHash);
            }
        }

        public string CriarToken(UsuarioModel usuario, DateTime expiracao)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Role),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("Username", usuario.Usuario)
            };

            var tokenSecret = _config["AppSettings:Token"]
                ?? throw new InvalidOperationException("Configure AppSettings:Token para emissao de JWT.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiracao,
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public DateTime ObterDataExpiracaoTokenAcesso()
        {
            return DateTime.UtcNow.AddMinutes(ObterInteiroConfiguracao(
                "AppSettings:AccessTokenExpirationMinutes",
                TempoPadraoTokenAcessoMinutos));
        }

        public string CriarRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string CriarHashToken(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }

        public DateTime ObterDataExpiracaoRefreshToken()
        {
            return DateTime.UtcNow.AddDays(ObterInteiroConfiguracao(
                "AppSettings:RefreshTokenExpirationDays",
                TempoPadraoRefreshTokenDias));
        }

        private int ObterInteiroConfiguracao(string chave, int valorPadrao)
        {
            return int.TryParse(_config[chave], out var valor) && valor > 0 ? valor : valorPadrao;
        }
    }
}
