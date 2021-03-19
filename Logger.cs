using ColorConsole;
using System;

namespace spotify_chroma
{
    public enum LogSource
    {
        Spotify,
        Chroma
    }

    public static class Logger
    {
        private static readonly ConsoleWriter console = new();

        public static void Log(string message, LogSource source)
        {
            switch (source)
            {
                case LogSource.Spotify:
                    console.WriteLine($"[Spotify] {message}", ConsoleColor.Gray);
                    break;

                case LogSource.Chroma:
                    console.WriteLine($"[Chroma] {message}", ConsoleColor.Magenta);
                    break;
            }
        }
    }
}