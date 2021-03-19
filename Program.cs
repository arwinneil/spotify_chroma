using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace spotify_chroma
{
    internal class Program
    {
        /// <param name="webAuthUrl">URL to Web Auth Server/Application</param>
        /// <param name="refreshToken">Spotify Account refresh token</param>
        private static async Task Main(string webAuthUrl, string refreshToken)
        {
            var c = Channel.CreateBounded<SpotifySongDTO>(1);

            Spotify.StartSpotifySongPooling(c.Writer, webAuthUrl, refreshToken);
            Chroma.StartListener(c.Reader);

            Console.ReadKey();
        }
    }
}