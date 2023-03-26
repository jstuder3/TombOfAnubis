using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TombOfAnubis
{
    public class Altar : Entity
    {

        List<bool> placedArtefacts;
        List<Texture2D> artefactTextures;
        int numArtefacts;
        float artefactScale = 0.05f;

        public Altar(Vector2 position, Vector2 scale, Texture2D texture, List<Texture2D> artefactTextures)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 1);
            AddComponent(sprite);

            // TODO: Add collider
            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);

            numArtefacts = artefactTextures.Count;

            placedArtefacts = new List<bool>(new bool[numArtefacts]);

            this.artefactTextures = artefactTextures;
        }

        public void PlaceArtefactIfPossible(Character character)
        {
            if (character == null) return;

            int playerID = character.GetComponent<Player>().PlayerID;

            if (character.GetComponent<Inventory>().HasArtefact() && !placedArtefacts[playerID])
            {
                Sprite sprite = new Sprite(artefactTextures[playerID], 2);
                AddComponent(sprite);
                character.GetComponent<Inventory>().InventorySlots[0].ClearItem();
                placedArtefacts[playerID] = true;
                Console.WriteLine("Artefact of player " + playerID + " was placed!");
            }
            else
            {
                Console.WriteLine("Either player " + playerID + " doesn't have an artefact or their artefact is already placed!");
            }

            if(AllArtefactsPlaced())
            {
                Console.WriteLine("All artefacts placed! Anubis was defeated!");
            }
            else
            {
                int remaining = 0;
                foreach(bool var in placedArtefacts)
                {
                    if (!var) remaining++;
                }
                Console.WriteLine("You need " + remaining + " more artefacts!");
            }

        }

        public bool AllArtefactsPlaced()
        {
            return !placedArtefacts.Contains(false);
        }

    }
}
