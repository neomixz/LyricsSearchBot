using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using LyricsSearchBot.Client;
using System.IO;
using System.Text.RegularExpressions;

namespace LyricsSearchBot
{
    public class LyricsSearchBot
    {
        TelegramBotClient telegramBotClient = new TelegramBotClient("5486287508:AAH_DnxInvfwJjpKkWuh1ARZCdPxeXf5Iac");

        ReceiverOptions receiverOptions = new ReceiverOptions() { AllowedUpdates = { } };
        CancellationToken cancellationToken = new CancellationToken();
        

        public async Task Start()
        {
            telegramBotClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await telegramBotClient.GetMeAsync();
            Console.WriteLine($"Bot {botMe.Username}'s started to work!!");
            Console.ReadKey();
        }
        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"API's Error: \n {apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }            

            if (update?.Type == UpdateType.CallbackQuery)
            {
                await HadlerCallBackQuery(botClient, update.CallbackQuery);
            }
            update1 = update;
        }
        Update update1 { get; set; }





        private async Task HadlerCallBackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery) //для інлайнів
        {
            if (callbackQuery.Data.StartsWith("Lyric"))
            {
                checkText1 = "Artist";
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Enter an Artist:", replyMarkup: new ForceReplyMarkup() { Selective = true });
            }
            else if (callbackQuery.Data.StartsWith("Letter"))
            {
                checkText1 = "ByLetter";
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Enter a Letter:", replyMarkup: new ForceReplyMarkup() { Selective = true });
            }
            else if (callbackQuery.Data.StartsWith("AllMusic"))
            {
                checkText1 = "AllArtistMusic";
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Enter an Artist:", replyMarkup: new ForceReplyMarkup() { Selective = true });
            }
            else if (callbackQuery.Data.StartsWith("Save"))
            {
                //await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Save");
                
                //DirectoryInfo myDir = new DirectoryInfo(path);
                //int count = myDir.GetFiles().Length; //кількість файлів
                var fileNames = Directory.GetFiles(path, "*.txt").Select(filename => Path.GetFileNameWithoutExtension(filename)).ToList(); //імя файлів
                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

                for (int i = 0; i < fileNames.Count; i++)
                {                    
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData($"{fileNames[i]}", callbackData: $"{fileNames[i]}") });
                }

                var TelegramKeyBoard = new InlineKeyboardMarkup(buttons.ToArray());
                 
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "SONGS", replyMarkup: TelegramKeyBoard);
                
            }
            if (callbackQuery.Data.Contains("-"))
            {
                string songText = null;
                using (var sr = new StreamReader(path + $"{callbackQuery.Data}.txt"))
                {
                    songText = sr.ReadToEnd();
                }
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"{songText}");
            }


            //callbackQuery.Data.StartsWith("Lyric")
            if (callbackQuery.Data.StartsWith("SongName"))
            {
                checkText1 = "SongName";
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Name:", replyMarkup: new ForceReplyMarkup() { Selective = true });
               
            }
            if (callbackQuery.Data.StartsWith("SavingMethod"))
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"{PostLyrics(Artist, SongName)}");
            }

            //callbackQuery.Data.StartsWith("change")
            if (callbackQuery.Data.StartsWith("NewSongName"))
            {
                checkText2 = "NewSongName";
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"NewName:", replyMarkup: new ForceReplyMarkup() { Selective = true });
            }





            if (callbackQuery.Data.StartsWith("delete"))
            {
                var fileNames = Directory.GetFiles(path, "*.txt").Select(filename => Path.GetFileNameWithoutExtension(filename)).ToList(); //імя файлів
                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                
                for (int i = 0; i < fileNames.Count; i++)
                {
                    var artist_songName = fileNames[i].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData($"{fileNames[i]}", callbackData: $"{artist_songName[0] + "." + artist_songName[1]}") });
                }
                
                var TelegramKeyBoard = new InlineKeyboardMarkup(buttons.ToArray());
                
                //await botClient.SendTextMessageAsync(message.Chat.Id, $"{buttons[0].InlineKeyboard}"); 
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Choose Song To Delete:", replyMarkup: TelegramKeyBoard);                
            }
            if (callbackQuery.Data.Contains("."))
            {
                var artist_songName = callbackQuery.Data.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();                
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"{DeleteLyrics($"{artist_songName[0].Trim()}", $"{artist_songName[1].TrimStart()}")}");
            }

            if (callbackQuery.Data.StartsWith("change"))
            {
                checkText2 = "NewArtist";
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Enter an NewArtist:", replyMarkup: new ForceReplyMarkup() { Selective = true });
            }
            if (callbackQuery.Data.StartsWith("cangeOn"))
            {
                var fileNames = Directory.GetFiles(path, "*.txt").Select(filename => Path.GetFileNameWithoutExtension(filename)).ToList(); //імя файлів
                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

                for (int i = 0; i < fileNames.Count; i++)
                {
                    var artist_songName = fileNames[i].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData($"{fileNames[i]}", callbackData: $"{artist_songName[0] + "&" + artist_songName[1]}") });
                }

                var TelegramKeyBoard = new InlineKeyboardMarkup(buttons.ToArray());

                //await botClient.SendTextMessageAsync(message.Chat.Id, $"{buttons[0].InlineKeyboard}"); 
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Choose Song To Chnage:", replyMarkup: TelegramKeyBoard);
            }
            if (callbackQuery.Data.Contains("&"))
            {
                checkText2 = null;
                //PostLyrics(NewArtist, NewSongName);
                var artist_songName = callbackQuery.Data.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"{PutLyrics($"{artist_songName[0].Trim()}", $"{artist_songName[1].TrimStart()}", NewArtist.Trim(), NewSongName.TrimStart())}");
                NewArtist = null;
                NewSongName = null;
            }


        }


        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Welcome To LyricsSearchBot!! Enter: /inline");
                return;
            }

            if (message.Text == "/inline")
            {
                InlineKeyboardMarkup keyboardMarkup = new
                  (
                    new[]
                    {
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("Search Lyrics", callbackData: $"Lyric")
               },
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("Search Artist By Letter", callbackData: $"Letter") //вася ключове слово
               },
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("Search All Artist's Music", callbackData: $"AllMusic")
               },
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("! Saved Lyrics !", callbackData: $"Save")
               },
               new[]
               {
                   InlineKeyboardButton.WithCallbackData($"DELETE", callbackData: "delete")
               },                                        
               new[]
               {
                   InlineKeyboardButton.WithCallbackData($"CHANGE", callbackData: "change")
               }
                    }
                  );
                await botClient.SendTextMessageAsync(message.Chat.Id, "|| OPTIONS ||", replyMarkup: keyboardMarkup);
                return;
            }

            if (checkText1 == "ByLetter")
            {
                bool check = true;
                for (int i = 0; i < message.Text.Length; i++)
                {
                    if (!Regex.IsMatch(message.Text[i].ToString(), @"\P{IsCyrillic}"))
                    {
                        check = false;
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Only Latin !!");
                        break;
                    }                    
                }
                if (message.Text.Length > 1)
                {
                    check = false;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! OnlyOne Letter !!");
                }

                if (check)
                {
                    Info = message.Text;
                    Console.WriteLine(Info);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{GetArtistByLetter(Info)}");
                }
                checkText1 = null;
            }
            if (checkText1 == "AllArtistMusic")
            {
                bool check = true;
                for (int i = 0; i < message.Text.Length; i++)
                {
                    if (!Regex.IsMatch(message.Text[i].ToString(), @"\P{IsCyrillic}"))
                    {
                        check = false;
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Only Latin !!");
                        break;
                    }
                }
                if (check)
                {
                    try 
                    {
                        Info = message.Text;
                        GetArtistMusic(Info);
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Artist is UnReachable !!");
                        check = false;
                    }
                }

                if (check)
                {
                    Info = message.Text;
                    Console.WriteLine(Info);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{GetArtistMusic(Info)}");
                }
                checkText1 = null;
            }

            if (checkText1 == "Artist")
            {
                bool check = true;
                for (int i = 0; i < message.Text.Length; i++)
                {
                    if (!Regex.IsMatch(message.Text[i].ToString(), @"\P{IsCyrillic}"))
                    {
                        check = false;
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Only Latin !!");
                        checkText1 = null;
                        break;
                    }
                }

                if (check)
                {
                    try
                    {
                        Artist = message.Text;
                        GetArtistMusic(Artist);
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Artist is UnReachable !!");
                        check = false;
                        checkText1 = null;
                    }
                }

                if (check)
                {
                    Artist = message.Text;
                    Console.WriteLine(Artist);
                    checkText1 = "Perehid1";
                }                              
            }
            if (checkText1 == "Perehid1")
            {
                InlineKeyboardMarkup keyboardMarkup = new
                  (
                    new[]
                    {
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("Enter SongName", callbackData: $"SongName")
               }
                    }
               );
                await botClient.SendTextMessageAsync(message.Chat.Id, "@", replyMarkup: keyboardMarkup);
                return;
            }
            if (checkText1 == "SongName")
            {
                bool check = true;
                for (int i = 0; i < message.Text.Length; i++)
                {
                    if (!Regex.IsMatch(message.Text[i].ToString(), @"\P{IsCyrillic}"))
                    {
                        check = false;
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Only Latin !!");
                        break;
                    }
                }

                if (check)
                {
                    SongName = message.Text;
                    if(GetLyrics(Artist, SongName).Contains("System.AggregateException"))
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Song is UnReachable !!");
                        check = false;
                        checkText1 = null;
                    }                           
                }

                if (check)
                {
                    SongName = message.Text;
                    Console.WriteLine(SongName);
                    checkText1 = null;


                    InlineKeyboardMarkup keyboardMarkup = new
                      (
                        new[]
                              {
                         new[]
                         {
                             InlineKeyboardButton.WithCallbackData("Save", callbackData: $"SavingMethod")
                         }
                              }
                         );
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{GetLyrics(Artist, SongName)}", replyMarkup: keyboardMarkup);
                    return;
                }
            }

            if(checkText2 == "NewArtist")
            {
                bool check = true;
                for (int i = 0; i < message.Text.Length; i++)
                {
                    if (!Regex.IsMatch(message.Text[i].ToString(), @"\P{IsCyrillic}"))
                    {
                        check = false;
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Only Latin !!");
                        checkText2 = null;
                        break;
                    }
                }

                if (check)
                {
                    try
                    {
                        NewArtist = message.Text;
                        GetArtistMusic(NewArtist);
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Artist is UnReachable !!");
                        check = false;
                        checkText2 = null;
                    }
                }

                if (check)
                {
                    NewArtist = message.Text;
                    Console.WriteLine(NewArtist);
                    checkText2 = "Perehid2";
                }
            }
            if(checkText2 == "Perehid2")
            {
                InlineKeyboardMarkup keyboardMarkup = new
                  (
                    new[]
                    {
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("Enter NewSongName", callbackData: $"NewSongName")
               }
                    }
               );
                await botClient.SendTextMessageAsync(message.Chat.Id, "@New", replyMarkup: keyboardMarkup);
                return;
            }
            if (checkText2 == "NewSongName")
            {
                bool check = true;
                for (int i = 0; i < message.Text.Length; i++)
                {
                    if (!Regex.IsMatch(message.Text[i].ToString(), @"\P{IsCyrillic}"))
                    {
                        check = false;
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Only Latin !!");
                        break;
                    }
                }

                if (check)
                {
                    NewSongName = message.Text;
                    if (GetLyrics(NewArtist, NewSongName).Contains("System.AggregateException"))
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"CAUTION !! Song is UnReachable !!");
                        check = false;
                        checkText2 = null;
                    }
                }

                if (check)
                {
                    NewSongName = message.Text;
                    Console.WriteLine(NewSongName);
                    checkText2 = null;
                    InlineKeyboardMarkup keyboardMarkup = new
                      (
                        new[]
                        {
                      new[]
                      {
                          InlineKeyboardButton.WithCallbackData("Change On", callbackData: $"cangeOn")
                      }
                           }
                      );
                       await botClient.SendTextMessageAsync(message.Chat.Id, $".", replyMarkup: keyboardMarkup);
                       return;                    
                }
            }            
        }

        string path = @"D:\programing\LyricsSearchAPI\LyricsSearchAPI\Lyrics\";
        string Info { get; set; }
        string checkText1{ get; set; }
        string Artist { get; set; }
        string SongName { get; set; }
        string checkText2 { get; set; }
        string NewArtist { get; set; }
        string NewSongName { get; set; }



        //методи які потрібно використати
        public string GetArtistByLetter(string letter)
        {
            HttpBotClient httpBot = new HttpBotClient();

            List<string> artistByLetter = httpBot.GetArtistByLetterAsync($"{letter.ToLower()}").Result.Select(x => x.artistName).ToList();

            string all_artists = "";
            for (int i = 0; i < artistByLetter.Count; i++)
            {
                all_artists += artistByLetter[i] + "\n";
            }

            return all_artists;
        }
        public string GetArtistMusic(string artist)
        {
            HttpBotClient httpBot = new HttpBotClient();
            List<string> artistMusic = httpBot.GetArtistMusicAsync(artist).Result.Take(25).Select(x => (x.singer + " - " + x.songName)).ToList();

            string all_music = "";
            for (int i = 0; i < artistMusic.Count; i++)
            {
                all_music += artistMusic[i] + "\n";
            }
            return all_music;
        }
        public string GetLyrics(string artist, string song_name)
        {
            HttpBotClient httpBot = new HttpBotClient();
            string lyric = httpBot.GetLyricsAsync($"{artist}", $"{song_name}");

            return lyric;
        }        
        public string PostLyrics(string artist, string song_name)
        {
            HttpBotClient httpBot = new HttpBotClient();
            string response = httpBot.PostLyricsAsync($"{artist}", $"{song_name}");

            return response;
        }
        public string DeleteLyrics(string artist, string song_name)
        {
            HttpBotClient httpBot = new HttpBotClient();
            string response = httpBot.DeleteLyricsAsync(artist, song_name);

            return response;
        }
        public string PutLyrics(string old_artist, string old_song_name, string new_artist, string new_song_name)
        {
            HttpBotClient httpBot = new HttpBotClient();
            string response = httpBot.PutLyricsAsync(old_artist, old_song_name, new_artist, new_song_name);

            return response;
        }
    }
}















/*if (message.Text == "/inline")
   {
       InlineKeyboardMarkup keyboardMarkup = new
         (
           new[]
           {
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("Погода у Косові", callbackData: $"Вася"), //вася ключове слово
                   InlineKeyboardButton.WithCallbackData("Погода у Франику")
               }
           }
         );
   
       await botClient.SendTextMessageAsync(message.Chat.Id, "Choose City:", replyMarkup: keyboardMarkup);
       return;
   }
   
   if (message.Text == "/start")
   {
       await botClient.SendTextMessageAsync(message.Chat.Id, "Choose the command /keyboard ot /inline");
       return;
   }
   
   if (message.Text == "Hello")
   {
       await botClient.SendTextMessageAsync(message.Chat.Id, "agsgsfg");
       return;
   }
   if (message.Text == "Bye")
   {
       await botClient.SendTextMessageAsync(message.Chat.Id, "nhfgfg");
       return;
   }
   
   if(message.Text == "/keyboard")
   {
       ReplyKeyboardMarkup replyKeyboard = new
         (
           new[]
           {
               new KeyboardButton[] { "Hello", "Bye" },
               new KeyboardButton[] { "Як справи", "Як погода" }
           }                    
         )
       {
           ResizeKeyboard = true
       };
   
       await botClient.SendTextMessageAsync(message.Chat.Id, "Choose option:", replyMarkup: replyKeyboard);
       return;
   }*/