using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;
using BookStore_UI.Pages.Users;
using BookStore_UI.Providers;
using BookStore_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthenticationRepository(IHttpClientFactory client, 
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.LoginEndPoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return false;

            // get the token from the response
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            // store the token in the localstorage
            await _localStorage.SetItemAsync(LocalStorageKeys.AuthToken, token.Token);

            // change authentication state of app
            var apiAuthStateProvider = _authStateProvider as ApiAuthenticationStateProvider;
            if (apiAuthStateProvider == null)
                return false;

            await apiAuthStateProvider.SetStateToLoggedIn();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(LocalStorageKeys.AuthToken);
            // change authentication state of app
            var apiAuthStateProvider = _authStateProvider as ApiAuthenticationStateProvider;
            if (apiAuthStateProvider != null)
                apiAuthStateProvider.SetStateToLoggedOut();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.RegisterEndPoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;

        }
    }
}
