using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Static
{
    public static class EndPoints
    {
        public static string BaseUrl = "https://localhost:44349/";
        public static string ApiBaseUrl = $"{BaseUrl}api/";
        public static string AuthorsEndPoint = $"{ApiBaseUrl}authors/";
        public static string BooksEndPoint = $"{ApiBaseUrl}books/";
        public static string UsersEndPoint = $"{ApiBaseUrl}users/";
        public static string LoginEndPoint = $"{UsersEndPoint}login/";
        public static string RegisterEndPoint = $"{UsersEndPoint}register/";

    }
}
