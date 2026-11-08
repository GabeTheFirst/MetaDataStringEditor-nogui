using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace LocalQuest
{
    internal static class UiTools
    {
        public static void ShowBanner(string Message, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;

            int Width = Console.WindowWidth - 2;
            string Top = "┏";
            string Mid = "";
            string Low = "┗";

            for (int i = 0; i < Width - 1; i++)
            {
                Top += "━";
                Low += "━";
            }
            Top += "┓\n";
            Low += "┛\n";

            int li = 0;
            for(int i = 0; i < Message.Length - 1;)
            {
                if (li == 0)
                {
                    Mid += "┃ ";
                    li++;
                }
                else if (li == 1)
                {
                    li++;
                }
                else if (li == Console.WindowWidth - 3)
                {
                    Mid += " ┃\n";
                    li = 0;
                }
                else
                {
                    Mid += Message[i];
                    i++;
                    li++;
                }
            }
            if(li < Console.WindowWidth - 1)
            {
                for(int i = 0; i < Console.WindowWidth - li - 2; i++)
                {
                    Mid += " ";
                }
            }
            Mid += "┃\n";
            Console.Write(Top + Mid + Low);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }

}
