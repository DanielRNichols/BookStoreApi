using Blazored.LocalStorage;
using BookStore_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore_UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, JwtSecurityTokenHandler tokenHandler)
        {
            _localStorage = localStorage;
            _tokenHandler = tokenHandler;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var tokenString = await _localStorage.GetItemAsStringAsync(LocalStorageKeys.AuthToken);
                if (String.IsNullOrWhiteSpace(tokenString))
                {
                    // return empty claim, basically nobody logged in
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var token = _tokenHandler.ReadJwtToken(tokenString);
                var expiry = token.ValidTo;
                if (expiry < DateTime.Now)
                {
                    // remove token from local storage
                    await _localStorage.RemoveItemAsync(LocalStorageKeys.AuthToken);

                    // return empty claim, basically nobody logged in
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Get claims from token
                var claims = ParseClaims(token);

                // Build authenticated user 
                var user = new ClaimsPrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));

                // return authenticated user
                return new AuthenticationState(user);

            }
            catch (Exception)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task SetStateToLoggedIn()
        {
            var tokenString = await _localStorage.GetItemAsStringAsync(LocalStorageKeys.AuthToken);
            var token = _tokenHandler.ReadJwtToken(tokenString);
            var claims = ParseClaims(token);
            var user = new ClaimsPrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public void SetStateToLoggedOut()
        {
            // set user to nobody
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }


        private IList<Claim> ParseClaims(JwtSecurityToken token)
        {
            var claims = token.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, token.Subject));

            return claims;
        }
    }
}
