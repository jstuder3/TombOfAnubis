using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis.InventorySystem;

namespace TombOfAnubis.WorldObjects
{
    internal class WorldObjectManager
    {
        List<InventoryItem> items = new List<InventoryItem>();
        List<WorldTrap> traps = new List<WorldTrap>();
        List<WorldArtefact> artefacts = new List<WorldArtefact>();

        void AddArtefact(WorldArtefact artefact)
        {
            artefacts.Add(artefact);
        }

        void AddArtefact(Vector3 position, Texture2D texture, int assignedPlayer)
        {
            artefacts.Add(new WorldArtefact(position, texture, assignedPlayer));
        }

        void AddTrap(Vector3 position, Texture2D texture, TrapType trapType)
        {

        }

    }
}
