using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class ColourCollection
    {
        private static string[] colours = {"Red"         //paleta wszystkich kolorów dostępnych w kalendarzu
                        ,"Orange"
                        ,"Brown"
                        ,"Yellow"
                        ,"Green"
                        ,"Teal"
                        ,"Olive"
                        ,"Blue"
                        ,"Purple"
                        ,"Cranberry"
                        ,"Steel"
                        ,"DarkSteel"
                        ,"Gray"
                        ,"DarkGray"
                        ,"Black"
                        ,"DarkRed"
                        ,"DarkOrange"
                        ,"DarkBrown"
                        ,"DarkYellow"
                        ,"DarkGreen"
                        ,"DarkTeal"
                        ,"DarkOlive"
                        ,"DarkBlue"
                        ,"DarkPurple"
                        ,"DarkCranberry"
                        };

        private int GetColourIndex(string colour)    //Szukam indeksu koloru wybranego przez użytkownia który sparuje ze słowem preset
        {
           return Array.IndexOf(colours, colour);
        }

        public bool ColourExcist(string colour) //sprawdzamy czy istnieje w kolekcji
        {
            int index = GetColourIndex(colour);
            if (index < 0)
            {
                Console.WriteLine("There is no such a available colour");
                return false;
            }
            else
                return true;
        }  

        public static string DisplayColours() //wyświetlamy wszystkie kolory
        {
            return string.Join(", ", colours);
        }


        public string MappedColour(string colour) //mapuje kolory np z red na Preset0 żeby API mogło odczytać
        {
            int index = GetColourIndex(colour);
            return String.Format("{0}{1}", "Preset", index);
        }
    }

}
