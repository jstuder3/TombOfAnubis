using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public static class AudioController
    {
        public static float MusicVolume;
        public static float SoundeffectVolume;
        public static Dictionary<string, Song> Songs {  get; set; }
        public static Dictionary<string, SoundEffect> SoundEffects { get; set; }

        public static void LoadContent(ContentManager content)
        {
            Songs = new Dictionary<string, Song>
            {
                { "menuScreen", content.Load<Song>(@"Audio\Soundtrack/Slow1_100bpm") },
                { "gameSlowTrack", content.Load<Song>(@"Audio\Soundtrack/Slow2_100bpm") },
                { "gameFastTrack", content.Load<Song>(@"Audio\Soundtrack/Fast2.2_100bpm") },
                { "gameWonTrack", content.Load<Song>(@"Audio\Soundtrack/Slow3_95bpm") },
            };
            SoundEffects = new Dictionary<string, SoundEffect>()
            {
                { "intro", content.Load<SoundEffect>(@"Audio\SoundFX\IntroSound") },
                { "menuSelect", content.Load<SoundEffect>(@"Audio\SoundFX\MenuSelect") },
                { "menuAccept", content.Load<SoundEffect>(@"Audio\SoundFX\MenuAccept") },
                { "artefactPickup", content.Load<SoundEffect>(@"Audio\SoundFX\ArtefactPickup") },
                { "itemPickup1", content.Load<SoundEffect>(@"Audio\SoundFX\ItemPickup") },
                { "itemPickup2", content.Load<SoundEffect>(@"Audio\SoundFX\ItemPickup_v2") },
                { "itemPickup3", content.Load<SoundEffect>(@"Audio\SoundFX\ItemPickup_v3") },
                { "artefactPlaced", content.Load<SoundEffect>(@"Audio\SoundFX\ArtefactPlaced") },
                { "anubisRoar", content.Load<SoundEffect>(@"Audio\SoundFX\AnubisRoar") },
                { "revival", content.Load<SoundEffect>(@"Audio\SoundFX\Revival") },
                { "teleport", content.Load<SoundEffect>(@"Audio\SoundFX\Teleport") },
                { "punch", content.Load<SoundEffect>(@"Audio\SoundFX\Punch") },
                { "trapButtonPressed", content.Load<SoundEffect>(@"Audio\SoundFX\TrapButtonClick") },
                { "trapButtonReleased", content.Load<SoundEffect>(@"Audio\SoundFX\TrapButtonRelease") },
                { "trapDeactivated", content.Load<SoundEffect>(@"Audio\SoundFX\TrapdoorOpen") },
                { "Speedup", content.Load<SoundEffect>(@"Audio\SoundFX\Speedup") },
                { "LocationReveal", content.Load<SoundEffect>(@"Audio\SoundFX\LocationReveal") },
                { "Cloak", content.Load<SoundEffect>(@"Audio\SoundFX\Cloak") },
                { "FistThrow", content.Load<SoundEffect>(@"Audio\SoundFX\FistThrow") },
            };
            MediaPlayer.IsRepeating = true;

            Settings settings = Settings.Read();
            SetMusicVolume(settings.VolumeSetting);
            SetSoundeffectVolume(settings.SoundFXVolumeSetting);

        }

        public static void PlaySong(string song)
        {
            if(Songs.ContainsKey(song)) 
            {
                MediaPlayer.Play(Songs[song]);
            }
        }

        public static void StopSong()
        {
            MediaPlayer.Stop();
        }

        public static void PauseSong()
        {
            MediaPlayer.Pause();
        }

        public static void ResumeSong()
        {
            MediaPlayer.Resume();
        }

        public static bool isPlayingSong()
        {
            return (MediaPlayer.State == MediaState.Playing);
        }

        public static void PlaySoundEffect(string effect)
        {
            if (SoundEffects.ContainsKey(effect))
            {
                SoundEffects[effect].Play();
            }
        }

        public static void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
            MusicVolume = volume;
        }

        public static void SetSoundeffectVolume(float volume)
        {
            SoundEffect.MasterVolume = volume;
            SoundeffectVolume = volume;
        }
    }
}
