﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class CategoryClient
    {
    
        // Instancja Azure AD gdzie jest hostowana domena
        public static string AADInstance
        {
            get { return "https://login.microsoftonline.com"; }
        }

        //moja domena w Office 365 
        public static string Domain
        {
            get { return "testGraphApi1996.onmicrosoft.com"; }
        }

        /// Uprawnienia do uwierzytelniania; łącząc AADInstance
        // i domene.
        public static string Authority
        {
            get { return string.Format("{0}/{1}/", AADInstance, Domain); }
        }

        // Id aplikacji
        public static string ClientId
        {
            get { return "3fd45a93-47db-49da-94b9-56e48e2499b9"; }
        }

        //uri do aplikacji
        public static Uri RedirectUri
        {
            get { return new Uri("https://CalendarCategoryConsoleApp"); }
        }

        
        public static string GraphResource
        {
            get { return "https://graph.microsoft.com/"; }
        }

        //Wersja Microsoft Graph
        public static string GraphVersion
        {
            get { return "v1.0"; }
        }

        //Pobieramy tokena przy pomocy adal(adal(Microsoft.IdentityModel.Clients.ActiveDirectory)) w wersji 2.0
        public static string GetAccessToken()
        {
            
            var authenticationContext = new AuthenticationContext(Authority);   
            var authenticationResult = authenticationContext.AcquireToken(GraphResource,
                ClientId, RedirectUri, PromptBehavior.RefreshSession);
            var accessToken = authenticationResult.AccessToken;
            return accessToken;
        }

        // pobierany nagłówki dzięki przy pomocy tokena
        public static HttpClient GetHttpClient(string accessToken)
        {
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                accessToken);
            return httpClient;
        }

        public static CategoryModel CreateCategoryObject(string displayName, string color) //tworzymy kategrie lokalnie
        {
            var category = new CategoryModel
            {
                DisplayName = displayName,
                Color = color
            };
            return category;
        }
        
        public static async Task<ContactModel> GetContactAsync(HttpClient httpClient)
        {
            var contactResponse = await httpClient.GetStringAsync("https://graph.microsoft.com/v1.0/users/?$select=mail");
            var contact = JsonConvert.DeserializeObject<ContactModel>(contactResponse);
            return contact;
        }

        private static async Task<bool> ExcistEmail(string mail)
        {
            var accessToken = GetAccessToken();
            var httpClient = GetHttpClient(accessToken);
            var contact = await GetContactAsync(httpClient);
    
            // contact.value.ForEach(Console.WriteLine);
            //foreach (var model in contact.Value)
            //{
            //    Console.WriteLine(model.Mail);
            //}
            return contact;  
        }

       
        private static async Task<UserModel> GetUserAsync(HttpClient httpClient)
        {
            // Get and deserialize the user
            var userResponse = await httpClient.GetStringAsync(GraphResource + GraphVersion + "/users/?$filter=mail eq 'robert@testGraphApi1996.onmicrosoft.com'");
            var user = JsonConvert.DeserializeObject<UserModel>(userResponse);
            return user;
        }

        private static async Task<bool> CreateCategoryAsync(HttpClient httpClient, CategoryModel category,string mail) //tworzymy kategorie asynchronicznie już w otlooku 
        {
            var stringContent = JsonConvert.SerializeObject(category); //konwertujemy naszą klasę categoryModel do formatu json ponieważ właśnie takie jest obsługiwany przez API
                                                                       //konwertujemy za pomoc newtonsoft.json
            var response = await httpClient.PostAsync(GraphResource + GraphVersion + "/users/"+mail+"/outlook/masterCategories", // pierwszy argmunet  uri do POSTa
                new StringContent(stringContent, Encoding.UTF8, "application/json"));    // drugi to format danych w jakim będziemy przesyłać
            return response.IsSuccessStatusCode; 

        }
 
        private static async Task CreateCategory(string displayName, string colour) //ostateczna metoda którą wywyołujemy mainie
        {
            var accessToken = GetAccessToken();
            var httpClient = GetHttpClient(accessToken);
            var user = await GetUserAsync(httpClient);
            var category = CreateCategoryObject(displayName,colour);
            var isSuccess = await CreateCategoryAsync(httpClient, category, user.Value.First().UserPrincipalName);

            if (isSuccess)
            {
                Console.Write("Everything went correct. Check your categories!");
            }
            else
            {
                Console.Write("Error");
            }
        }
       
    }
}
