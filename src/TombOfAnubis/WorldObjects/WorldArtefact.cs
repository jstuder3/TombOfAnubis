using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    internal class WorldArtefact : WorldObject
    {

        //only the assigned player can pick up the artefact, and the artefact is colour-tinted based on which player it is assigned to
        int assignedPlayer = -1;
        Color[] colorTint = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow};

        public WorldArtefact(Vector3 position, Texture2D texture, int assignedPlayer) {
            this.position = position;
            this.texture = texture;
            this.assignedPlayer = assignedPlayer;
        }

    }
}
