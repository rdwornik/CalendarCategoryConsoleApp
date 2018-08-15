using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {      

            Console.WriteLine("Hello here you can create your categories. \n" +
                "First please wirte how you want to name your category:");

            string displayName = Console.ReadLine();

            Console.WriteLine("Great now please enter colour of category \n" +
                "You can choose such a colours:\n" + ColourCollection.DisplayColours());

            string colour = Console.ReadLine();
            colour = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(colour.ToLower());
            
            CategoryClient.CreateCategory(displayName, ColourCollection.MappedColour(colour)).Wait();
            Console.ReadKey();
        }
    }
}
