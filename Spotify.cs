using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace spotify_chroma
{
    internal static class Spotify
    {
        private static string accessToken;

        public static void StartSpotifySongPooling(ChannelWriter<SpotifySongDTO> channelWriter, string webAuthUrl, string refreshToken)
        {
            RefreshToken( webAuthUrl,  refreshToken);

            _ = Task.Run(async delegate
           {
               var currentId = "";

               while (true)
               {
                   var song = GetCurrentPlaying();

                   if (song != null)
                   {
                       if (song.ID != currentId)
                       {
                           currentId = song.ID;

                           Logger.Log($"Currently playing : {song.Name} by {song.Artist}", LogSource.Spotify);
                           Logger.Log($"Song ID :{song.ID} ", LogSource.Spotify);
                           Logger.Log($"Retrieved album art URL : {song.ImageUrl } ", LogSource.Spotify);

                           await channelWriter.WriteAsync(song);
                       }
                   }

                   await Task.Delay(1000);
               }
           });
        }

        public static void RefreshToken(string webAuthUrl, string refreshToken)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(webAuthUrl);
                HttpResponseMessage response = client.GetAsync($"/refresh_token?refresh_token={refreshToken}").Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;

                dynamic dynamicResult = JsonConvert.DeserializeObject(result);
                accessToken = dynamicResult.access_token;

                Logger.Log($"Access token token refreshed succesfully", LogSource.Spotify);
            }
        }

        public static SpotifySongDTO? GetCurrentPlaying()
        {
            SpotifySongDTO songDTO;

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = client.GetAsync("https://api.spotify.com/v1/me/player/currently-playing").Result;
                string result = response.Content.ReadAsStringAsync().Result;

                switch ((int)response.StatusCode)
                {
                    case 204:
                        Logger.Log("No music playing", LogSource.Spotify);
                        return null;

                    default:

                        dynamic dynamicResult = JsonConvert.DeserializeObject(result);

                        songDTO = new SpotifySongDTO(
                            (string)dynamicResult.item.id,
                            (string)dynamicResult.item.name,
                            (string)dynamicResult.item.artists[0].name,
                            (string)dynamicResult.item.album.images[0].url
                        );

                        return songDTO;
                }
            }
        }
    }
}