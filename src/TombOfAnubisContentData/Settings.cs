using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TombOfAnubis
{
    public class Settings
    {
        public bool IsFullscreen { get; set; }

        public float VolumeSetting { get; set; }

        public float SoundFXVolumeSetting { get; set; }

        private static readonly string filename = "settings.xml";

        public Settings() { }

        public Settings(bool isFullscreen, float volume, float soundfx) 
        {
            IsFullscreen = isFullscreen;
            VolumeSetting = volume;
            SoundFXVolumeSetting = soundfx;
        }

        public void Write()
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static Settings Read()
        {
            if (!File.Exists(filename))
            {
                return new Settings(true, 1, 1);
            }
            else
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using (StreamReader reader = new StreamReader(filename))
                {
                    return (Settings)serializer.Deserialize(reader);
                }
            }
            
        }
    }

}
