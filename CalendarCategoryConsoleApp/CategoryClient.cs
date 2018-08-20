using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Configuration;

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
            get { string connectionString = ConfigurationManager.ConnectionStrings["MyDomain"].ConnectionString;  return connectionString; }
           
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
            get { string connectionString = ConfigurationManager.ConnectionStrings["ClientId"].ConnectionString; return connectionString; }
        }

        //uri do aplikacji
        public static Uri RedirectUri
        {
            get {
                string connectionString = ConfigurationManager.ConnectionStrings["RedirectUri"].ConnectionString;
                return new Uri(connectionString); }
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

        public static async Task<bool> CategoryExcist(HttpClient httpClient, string categoryName, string mail)
        {

            var categoryListResponse = await httpClient.GetStringAsync(GraphResource + GraphVersion + "/users/" + mail + "/outlook/masterCategories");
            var categoryList = JsonConvert.DeserializeObject<CategoryListModel>(categoryListResponse);
            bool contains = categoryList.Value.Any(p => p.displayName == categoryName);
            if(contains)
                Console.WriteLine("User: " + mail + " already have category named: " +categoryName);
            return contains;
        }

        private static async Task<bool> CreateCategoryAsync(HttpClient httpClient, CategoryModel category,string mail) //tworzymy kategorie asynchronicznie już w otlooku 
        {
            var stringContent = JsonConvert.SerializeObject(category); //konwertujemy naszą klasę categoryModel do formatu json ponieważ właśnie takie jest obsługiwany przez API
                                                                       //konwertujemy za pomoc newtonsoft.json
            var response = await httpClient.PostAsync(GraphResource + GraphVersion + "/users/" + mail + "/outlook/masterCategories", // pierwszy argmunet  uri do POSTa
                new StringContent(stringContent, Encoding.UTF8, "application/json"));    // drugi to format danych w jakim będziemy przesyłać
            return response.IsSuccessStatusCode; 

        }
 
        public static async Task<bool> CreateCategory(string categoryName, string colour, string mail) //ostateczna metoda którą wywyołujemy mainie
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

                if (user.Value.Count() != 0)       //sprawdzamy czy lista pusta jeśli nie to znaczy że znelźliśmy naszego usera
                {
                    bool containsCategory = await CategoryExcist(httpClient, categoryName, mail); //sprawdzamy czy kategoria już nie istnieje u użytkownika
                    if (!containsCategory)
                    {
                        colourMapped = colourCollection.MappedColour(colour); //mapuje kolor to odpowiedniego formatu czyli np red = preset1 
                        var category = CreateCategoryObject(categoryName, colourMapped);

                        var isSuccess = await CreateCategoryAsync(httpClient, category, mail);
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
            else
                return false;
        }
       
    }
}
