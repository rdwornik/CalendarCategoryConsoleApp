using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class Program
    {
        private const string askForUserMail = "Please write users mail which you want create mail";
        private const string askForCategoryColour = "Great now please enter colour of category \n You can choose such a colours:\n";
        private const string askForCategoryName = "Hello here you can create your categories. \n First please wirte how you want to name your category:";

        static void Main(string[] args)
        {
            string categoryName;
            string colour;
            string mail;

            Console.WriteLine(askForCategoryName);

            categoryName = Console.ReadLine();

            Console.WriteLine(askForCategoryColour + ColourCollection.DisplayColours());
            colour = Console.ReadLine();
            //ColourCollection colourCollection = new ColourCollection();
            //Console.WriteLine(colourCollection.MappedColour(colour));

            Console.WriteLine(askForUserMail);

            mail = Console.ReadLine();

            
           

            CategoryClient.CreateCategory(categoryName, colour, mail).Wait();


            Console.ReadKey();
        }
    }
}
