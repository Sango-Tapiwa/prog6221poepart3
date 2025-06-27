using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot
{
    public class VoiceGreetingPlayer
    {
        public void Play(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    SoundPlayer player = new SoundPlayer(fileName);
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not play greeting sound: {ex.Message}");
            }
        }
    }
}