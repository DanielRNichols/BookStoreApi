using BookStore_UI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Services
{
    public class LocalStorageService : BookStore_UI.Contracts.ILocalStorageService
    {
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorageService;

        public LocalStorageService(Blazored.LocalStorage.ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> GetBoolAsync(string key)
        {
            var val = await _localStorageService.GetItemAsStringAsync(key);

            // if value is null, empty or 0, return false
            if (String.IsNullOrWhiteSpace(val) || val == "0")
                return false;

            // if value parses to True or False, return the parsed result
            bool boolVal = false;
            if (Boolean.TryParse(val, out boolVal))
                return boolVal;

            // otherwise it is something that is not falsey, so return true
            return true;
        }

        public async Task<int> GetIntAsync(string key)
        {
            var val = await _localStorageService.GetItemAsStringAsync(key);
            int intVal = 0;
            Int32.TryParse(val, out intVal);

            return intVal;
        }

        public async Task<double> GetRealAsync(string key)
        {
            var val = await _localStorageService.GetItemAsStringAsync(key);
            double dblVal = 0.0;
            Double.TryParse(val, out dblVal);

            return dblVal;
        }

        public async Task<string> GetStringAsync(string key)
        {
            return await _localStorageService.GetItemAsStringAsync(key);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _localStorageService.RemoveItemAsync(key);
        }

        public async Task SetItemAsync(string key, object obj)
        {
            await _localStorageService.SetItemAsync(key, obj);
        }
    }
}
