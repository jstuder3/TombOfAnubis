using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class WorldEventActivated : Entity
    {
        public WorldEventActivated()
        {
            Transform transform = new Transform(Vector2.Zero, Vector2.One * 5, Visibility.Both);
            AddComponent(transform);


            Animation animation = new Animation(new List<AnimationClip>() { Session.GetInstance().WorldEffectAnimation}, Visibility.Minimap);
            animation.SetActiveClip(AnimationClipType.Flexing);
            AddComponent(animation);

            Sprite sprite = new Sprite(Session.GetInstance().WorldEffectTexture, animation.DefaultSourceRectangle, 5, Visibility.Minimap);
            AddComponent(sprite);


            Vector2 MapCenter = Session.GetInstance().Map.MapSize / 2;
            Vector2 CurrentCenter = MinimapSize() / 2;
            transform.Position = MapCenter - CurrentCenter;

            GameplayEffect selfDestroy = new GameplayEffect(EffectType.Lifetime, 3, Visibility.Both);
            AddComponent(selfDestroy);

            InputController.VibrateGamepads(1.5f);

        }
    }
}
