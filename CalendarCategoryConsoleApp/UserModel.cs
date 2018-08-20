using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class UserModel         //user model to klasa która przechowuje usera którego wyfiltrowaliśmy po to by dostać jego displayNAme
    {
        public List<ValueUserModel> Value { get; set; }
    }
    class ValueUserModel
    {
        public string Id { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> BusinessPhones { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public object JobTitle { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public object OfficeLocation { get; set; }
        public string PreferredLanguage { get; set; }
        public string Surname { get; set; }
    }
}
