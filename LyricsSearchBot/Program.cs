using System;

using LyricsSearchBot.Client;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace LyricsSearchBot
{
    class Program
    {
        static void Main(string[] args)
        {
            LyricsSearchBot lyricsSearchBot = new LyricsSearchBot();
            lyricsSearchBot.Start();
            Console.ReadKey();

        }
    }
}
