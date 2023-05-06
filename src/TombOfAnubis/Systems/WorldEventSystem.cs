using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class WorldEventSystem : BaseSystem<WorldEvent>
    {

        public static readonly int NoEventStartupTime = 15;

        /// <summary>
        /// Try to start an event all TryStartInterval seconds
        /// </summary>
        public static readonly int TryStartInterval = 3;

        /// <summary>
        /// Every TryStartInterval the value of BaseSpawnProbabilityIncrease is added to the base event spawn probability
        /// </summary>
        public static readonly float BaseSpawnProbabilityIncrease = 0.25f;


        private Random rand = new Random();
        private Session session;

        private float eventStartProbability = 0;
        private float Cooldown = 0;
        private WorldEvent currentEvent;
        private float currentEventElapsedSeconds = 0;

        /// <summary>
        /// Time since game start when the game was actually playing. Not counting time spent in the pause menu.
        /// </summary>
        private int elapsedGamePlayingTimeSinceGameStart;

        public WorldEventSystem()
        {
            session = Session.GetInstance();
        }
        public override void Update(GameTime gameTime)
        {
            elapsedGamePlayingTimeSinceGameStart += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if(currentEvent != null)
            {
                currentEventElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(currentEventElapsedSeconds > currentEvent.Duration)
                {
                    StopEvent();
                }
                else
                {
                    currentEvent.Progress = Math.Min(1.0f, currentEventElapsedSeconds / currentEvent.Duration);
                    currentEvent.Update(gameTime);
                }
            }
            else
            {
                if(elapsedGamePlayingTimeSinceGameStart > NoEventStartupTime * 1000 && Cooldown <= 0 && elapsedGamePlayingTimeSinceGameStart % (TryStartInterval * 1000) == 0)
                {
                    TryStartEvent();
                }
            }
            if(Cooldown > 0)
            {
                Cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void TryStartEvent()
        {
            float artefactPenalty = GetArtefactProgress();
            float pStart = eventStartProbability * artefactPenalty;

            //Debug.WriteLine("Try start event: Event Start P: "+eventStartProbability+", penalty: "+artefactPenalty+", total: "+pStart);
            if (rand.NextDouble() < pStart)
            {
                StartEvent();
                eventStartProbability = 0;
            }
            else
            {
                eventStartProbability += 0.25f;
                eventStartProbability = Math.Min(1, eventStartProbability);
            }
            
        }
        private void StartEvent()
        {
            WorldEvent e = GetComponents()[rand.Next(GetComponents().Count)];
            currentEvent = e;
            currentEventElapsedSeconds = 0;
            e.Start();

        }

        private void StopEvent()
        {
            if(currentEvent != null) 
            {
                currentEvent.Stop();
                Cooldown = currentEvent.CooldownAfterEvent;
                currentEvent = null;
            }
        }

        private float GetArtefactProgress()
        {
            return (GetNumCollectedArtefacts() + 1f) / (session.NumberOfPlayers + 1f);
        }

        private int GetNumCollectedArtefacts()
        {
            int collected = 0;
            foreach(Character character in session.World.GetChildrenOfType<Character>())
            {
                if (character.GetComponent<Inventory>().HasArtefact())
                {
                    collected++;
                }
            }
            return collected;
        }

    }
}
