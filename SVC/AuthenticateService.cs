using BO;
using DAO;
using DTO;
using HLP;
using Microsoft.Extensions.Options;
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Versioning;

namespace SVC
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly DataContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public AuthenticateService(DataContext context, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }

        [SupportedOSPlatform("windows")]
        public AuthenticateResponse Authenticate(AuthenticateRequest model, string applicationName, string ipAddress)
        {
            if (!ValidateUser(model.Username, model.Password))
            {
                throw new AppException("Username or password is incorrect");
            }

            if (applicationName == string.Empty)
            {
                throw new AppException("Invalid application");
            }

            UserEntity user = _context.Users.SingleOrDefault(user => user.Username == model.Username);

            if (user == null)
            {
                throw new AppException($"{model.Username} does not have authorization");
            }

            string jwtToken = _jwtUtils.GenerateJwtToken(user);

            RefreshTokenEntity refreshToken = _jwtUtils.GenerateRefreshToken(applicationName, ipAddress);

            user.RefreshTokens.Add(refreshToken);

            RemoveOldRefreshTokens(user);

            _context.Update(user);
            _context.SaveChanges();

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string applicationName, string ipAddress)
        {
            UserEntity user = GetUserByRefreshToken(token);
            RefreshTokenEntity refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Update(user);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            RefreshTokenEntity newRefreshToken = RotateRefreshToken(refreshToken, applicationName, ipAddress);

            user.RefreshTokens.Add(newRefreshToken);

            RemoveOldRefreshTokens(user);

            _context.Update(user);
            _context.SaveChanges();

            string jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public void RevokeToken(string token, string ipAddress)
        {
            UserEntity user = GetUserByRefreshToken(token);
            RefreshTokenEntity refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new AppException("Invalid token");
            }

            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");

            _context.Update(user);
            _context.SaveChanges();
        }

        #region Private Methods
        private UserEntity GetUserByRefreshToken(string token)
        {
            UserEntity user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                throw new AppException("Invalid token");
            }

            return user;
        }

        private RefreshTokenEntity RotateRefreshToken(RefreshTokenEntity refreshToken, string applicacion, string ipAddress)
        {
            RefreshTokenEntity newRefreshToken = _jwtUtils.GenerateRefreshToken(applicacion, ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(UserEntity user)
        {
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private void RevokeDescendantRefreshTokens(RefreshTokenEntity refreshToken, UserEntity user, string ipAddress, string reason)
        {
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                RefreshTokenEntity childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);

                if (childToken.IsActive)
                {
                    RevokeRefreshToken(childToken, ipAddress, reason);
                }
                else
                {
                    RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
                }
            }
        }

        private static void RevokeRefreshToken(RefreshTokenEntity token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        [SupportedOSPlatform("windows")]
        private bool ValidateUser(string username, string password)
        {
            bool valid = false;

            try
            {
                using PrincipalContext pc = new(ContextType.Domain, _appSettings.Domain);
                valid = pc.ValidateCredentials(username, password);
            }
            catch (Exception)
            {
                throw new AppException("Domain Error");
            }

            return valid;
        }
        #endregion
    }
}
