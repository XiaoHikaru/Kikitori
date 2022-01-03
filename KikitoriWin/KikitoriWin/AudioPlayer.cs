// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kikitori.Audio
{
    public class AudioPlayer
    {
        private string tempFileName;
        private MediaPlayer mediaPlayer;

        public AudioPlayer()
        {
            tempFileName = "play_" + this.GetHashCode() + ".mp3";
            mediaPlayer = new MediaPlayer();
        }

        public Task Play(byte[] mp3Content)
        {
            lock (this)
            {
                var tcs = new TaskCompletionSource<bool>();
                try
                {
                    System.IO.File.WriteAllBytes(tempFileName, mp3Content);
                    mediaPlayer.MediaEnded += (sender, e) =>
                    {
                        mediaPlayer.Close();
                        System.IO.File.Delete(tempFileName);
                        tcs.TrySetResult(true);
                    };
                    mediaPlayer.Open(new Uri(tempFileName, UriKind.Relative));
                    mediaPlayer.Play();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("Could not play audio file: problems with IO: " + e.Message);
                    tcs.TrySetResult(false);
                }
                return tcs.Task;
            }
        }


        public void SimplePlay(byte[] mp3Content)
        {
            lock (this)
            {
                try
                {
                    System.IO.File.WriteAllBytes(tempFileName, mp3Content);
                    mediaPlayer.MediaEnded += (sender, e) =>
                    {
                        mediaPlayer.Close();
                        System.IO.File.Delete(tempFileName);
                    };
                    mediaPlayer.Open(new Uri(tempFileName, UriKind.Relative));
                    mediaPlayer.Play();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("Could not play audio file: problems with IO: " + e.Message);
                }
            }
        }
    }
}
