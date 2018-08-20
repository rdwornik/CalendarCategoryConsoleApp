using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
        
        private static async Task<UserModel> GetUserAsync(HttpClient httpClient, string mail) //szukamy user z mailem który wpisaliśmy
        {
            // Get and deserialize the user
            var userResponse = await httpClient.GetStringAsync(GraphResource + GraphVersion + "/users/?$filter=mail eq '" + mail + "'"); //filtrujemy api po mailu
            var user = JsonConvert.DeserializeObject<UserModel>(userResponse);
            if(user.Value.Count() == 0)                                         //jeśli lista jest pusta to znaczy, że nie znaleźliśmy maila i że nie ma usera z takim mailem
                Console.WriteLine("There is no such a user with such a mail");

            return user;
        }

        private static async Task<bool> CreateCategoryAsync(HttpClient httpClient, CategoryModel category,string displayName) //tworzymy kategorie asynchronicznie już w otlooku 
        {
            var stringContent = JsonConvert.SerializeObject(category); //konwertujemy naszą klasę categoryModel do formatu json ponieważ właśnie takie jest obsługiwany przez API
                                                                       //konwertujemy za pomoc newtonsoft.json
            var response = await httpClient.PostAsync(GraphResource + GraphVersion + "/users/" + displayName + "/outlook/masterCategories", // pierwszy argmunet  uri do POSTa
                new StringContent(stringContent, Encoding.UTF8, "application/json"));    // drugi to format danych w jakim będziemy przesyłać
            return response.IsSuccessStatusCode; 

        }
 
        public static async Task<bool> CreateCategory(string displayName, string colour, string mail) //ostateczna metoda którą wywyołujemy mainie
        {
            string colourMapped;
            colour = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(colour.ToLower()); //zmieniamy stringa do formatu w którym pierwsza litera jest wielka a reszta mała tak żeby user mógł wpisać
                                                                                                                  //wpisać kolor w każdej formie
            ColourCollection colourCollection = new ColourCollection();

            if (colourCollection.ColourExcist(colour))
            {
                ValueUserModel valueUserModel = new ValueUserModel();
                var accessToken = GetAccessToken();
                var httpClient = GetHttpClient(accessToken);
                var user = await GetUserAsync(httpClient, mail);
                
                valueUserModel = user.Value.First(); //pierwszego usera z listy. Wiemy że jest tylko jeden bo szukaliśmy po mailu a zakładam że nie ma dwóch różnych userów z tym samym mailem
                                                     // a dlaczego to robię ponieważ zakładam że displaname może różnić się od maila 
                                                     //dlatego na wszelki wypadek stworzyłem classe usermodel


                if (user.Value.Count() != 0)       //sprawdzamy czy lista pusta jeśli nie to znaczy że znelźliśmy naszego usera
                {
                    colourMapped = colourCollection.MappedColour(colour); //mapuje kolor to odpowiedniego formatu czyli np red = preset1 
                    var category = CreateCategoryObject(displayName, colourMapped); 

                    var isSuccess = await CreateCategoryAsync(httpClient, category, valueUserModel.DisplayName );
                    if (isSuccess)
                    {
                        Console.Write("Everything went correct. Check your categories!");
                        return true;
                    }
                    else
                    {
                        Console.Write("Error");
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }
       
    }
}
