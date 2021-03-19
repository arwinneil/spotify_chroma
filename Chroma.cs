using Colore;
using ColorHelper;
using ColorThiefDotNet;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;

namespace spotify_chroma
{
    internal class Chroma
    {
        public static async void StartListener(ChannelReader<SpotifySongDTO> reader)
        {
            _ = Task.Run(async delegate
            {
                var chroma = await ColoreProvider.CreateNativeAsync();

                while (true)
                {
                    while (true)
                    {
                        var x = await reader.ReadAsync();

                        var hex = ExtractColor(fetchImage(x.ImageUrl));

                        uint color;
                        uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out color);

                        await chroma.SetAllAsync(ColoreColor.FromRgb(color));
                    }
                }
            });
        }

        private static string ExtractColor(Bitmap image)
        {
            var colorThief = new ColorThief();

            var x = colorThief.GetColor(image, 100);
  
            Logger.Log($"Extracted Color {x.Color.ToHexString()}", LogSource.Chroma);

            //Enhancing Dominant Color
            HSL hsl = ColorHelper.ColorConverter.HexToHsl(new HEX(x.Color.ToHexString()));
            hsl.L = 30;
            hsl.S = 70;
            var hex = ColorHelper.ColorConverter.HslToHex(hsl);

            Logger.Log($"Enhanced Color {hex.Value}", LogSource.Chroma);

            return hex.Value;
        }

        private static Bitmap fetchImage(string url)
        {
            Logger.Log("Fetching image...", LogSource.Chroma);

            var client = new WebClient();
            var stream = client.OpenRead(url);
            var bitmap = new Bitmap(stream);

            return bitmap;
        }
    }
}