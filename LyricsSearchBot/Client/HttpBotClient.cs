using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using LyricsSearchBot.Constant;
using LyricsSearchBot.Model;
using Newtonsoft.Json;

namespace LyricsSearchBot.Client
{
    public class HttpBotClient
    {
        private HttpClient _httpClient;
        private string _address;

        public HttpBotClient()
        {
            _httpClient = new HttpClient();
            _address = Constants.address;
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", "57d87350b0msh1b6350ce649b1b0p13fd04jsne6bf1ef73376");
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<List<ArtistByLetterResponseModel>> GetArtistByLetterAsync(string word)
        {
            var response = await _httpClient.GetAsync($"/SearchArtistByWord?OneWord={word}");
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<ArtistByLetterResponseModel>>(content);
            return result;
        }
        public async Task<List<ArtistMusicResponseModel>> GetArtistMusicAsync(string artist)
        {
            var response = await _httpClient.GetAsync($"/ArtistMusic?Artist={artist}");
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<ArtistMusicResponseModel>>(content);
            return result;
        }

        public string GetLyricsAsync(string artist, string song)
        {
            var response = _httpClient.GetAsync($"/Lyrics?Artist={artist}&SongName={song}").Result.Content.ReadAsStringAsync().Result;            
            return response;
        }
        public string PostLyricsAsync(string artist, string song)
        {
            var response = _httpClient.PostAsync($"/Lyrics?Artist={artist}&SongName={song}", null).Result.Content.ReadAsStringAsync().Result;
            return response;
        }
        public string DeleteLyricsAsync(string artist, string song)
        {
            var response = _httpClient.DeleteAsync($"/Lyrics?Artist={artist}&SongName={song}").Result.Content.ReadAsStringAsync().Result;
            return response;
        }
        public string PutLyricsAsync(string old_artist, string old_song, string new_artist, string new_song)
        {
            var response = _httpClient.PutAsync($"/Lyrics?Old_Artist={old_artist}&Old_SongName={old_song}&New_Artist={new_artist}&New_SongName={new_song}", null).Result.Content.ReadAsStringAsync().Result;
            return response;
        }

    }
}

