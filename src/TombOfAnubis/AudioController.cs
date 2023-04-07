using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public static class AudioController
    {
        public static Dictionary<string, Song> Songs {  get; set; }
        public static Dictionary<string, SoundEffect> SoundEffects { get; set; }

        public static void LoadContent(ContentManager content)
        {
            Songs = new Dictionary<string, Song>
            {
                { "background_music", content.Load<Song>(@"Audio\background_music") }
            };
            SoundEffects = new Dictionary<string, SoundEffect>()
            {
                { "amazing_soundeffect", content.Load<SoundEffect>(@"Audio\amazing_soundeffect") }
            };
            MediaPlayer.IsRepeating = true;

        }

        public static void PlaySong(string song)
        {
            if(Songs.ContainsKey(song)) 
            {
                MediaPlayer.Play(Songs[song]);
            }
        }

        public static void PlaySoundEffect(string effect)
        {
            if (SoundEffects.ContainsKey(effect))
            {
                SoundEffects[effect].Play();
            }
        }
    }
}
