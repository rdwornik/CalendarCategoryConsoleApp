using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarCategoryConsoleApp
{
    class ColourCollection
    {
        public static string[] colours = {"Red"         //paleta wszystkich kolorów dostępnych w kalendarzu
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

        private static int GetColour(string colour)    //Szukam indeksu koloru wybranego przez użytkownia który sparuje ze słowem preset
        {
              for (int i = 0; i < colours.Length; i++)
                  if(colours[i] == colour)
                  {
                      return i;
                  }
              throw new System.ArgumentOutOfRangeException(colour, "there is no such a colour");
              
            
        }

        public static string DisplayColours()
        {
            return string.Join(",", colours);
        }

        public static string MappedColour(string colour) //mapuje kolory np z red na Preset0 żeby API mogło odczytać
        {
            int index = GetColour(colour);
            return String.Format("{0}{1}", "Preset", index);
        }
    }

}
