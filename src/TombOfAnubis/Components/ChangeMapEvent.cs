using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.Components
{
    public class ChangeMapEvent : WorldEvent
    {
        Altar oldAltar;
        List<Character> oldCharacters;
        List<Ghost> oldGhosts;
        List<Artefact> oldArtefacts;


        public ChangeMapEvent() : base(3)
        {
        }
        public override void Start()
        {
            base.Start();
            oldAltar = Session.GetInstance().World.GetChildrenOfType<Altar>()[0];
            oldCharacters = Session.GetInstance().World.GetChildrenOfType<Character>();
            oldGhosts = Session.GetInstance().World.GetChildrenOfType<Ghost>();
            oldArtefacts = Session.GetInstance().World.GetChildrenOfType<Artefact>();
            Session.GetInstance().IsEarthquake = true;

        }
        public override void Stop()
        {
            base.Stop();
            Session.RegenerateMap();
            Session.GetInstance().PauseDrawing = false;


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Progress > 0.8f)
            {
                Session.GetInstance().IsEarthquake = false;
                Session.GetInstance().PauseDrawing = true;
            }
        }
    }
}
