using System;
using System.IO;

namespace CybersecurityChatbot
{
    public class AsciiArtDisplayer
    {
        public void Display(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    string asciiArt = File.ReadAllText(fileName);
                    Console.WriteLine(asciiArt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not display ASCII art: {ex.Message}");
            }
        }

        public string GetAsciiArt(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    return File.ReadAllText(fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load ASCII art: {ex.Message}");
            }
            return "";
        }
    }
}