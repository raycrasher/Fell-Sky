using Artemis;
using Stateless;
using System.Collections;
using System.Collections.Generic;
using System;
using FellSky.Services;
using FellSky.Components;

namespace FellSky.Game.Campaign.Storyline
{
    public interface IStoryAct
    {
        void ConfigureStates(StateMachine<StoryUpdateFunction, string> state);
        IEnumerable Master(Story story, EntityWorld world);
    }

    public delegate IEnumerable StoryUpdateFunction(Story story, EntityWorld world);

    public class Story
    {
        public StateMachine<StoryUpdateFunction, string> State { get; private set; }

        public static class Triggers
        {
            public const string NextScene = "NextScene";
            public const string NextAct = "NextAct";
        }

        public List<IStoryAct> Acts { get; private set; }
        public EntityWorld World { get; private set; }

        public Story(EntityWorld world)
        {
            Acts = new List<IStoryAct>(new[] { new ActOne() });
            State = new StateMachine<StoryUpdateFunction, string>(Acts[0].Master);
            State.OnTransitioned(OnTransition);

            for (int i = 0; i < Acts.Count; i++)
            {
                var act = Acts[i];
                act.ConfigureStates(State);

                State.Configure(act.Master)
                    .Permit(Triggers.NextAct, i < Acts.Count - 1 ? (StoryUpdateFunction)Acts[i + 1].Master : EndOfStory);
            }
            World = world;
            State.Fire(ActOne.Triggers.Start);
        }

        private void OnTransition(StateMachine<StoryUpdateFunction, string>.Transition transition)
        {
            const string tag = "StoryState";
            var entity = World.TagManager.GetEntity(tag);
            if (entity != null)
            {
                entity.Tag = null;
                entity.Delete();
            }
            var component = new CoroutineComponent(transition.Destination(this, World), () => entity.Delete());
            entity = World.CreateEntity();
            entity.AddComponent(component);
            entity.Refresh();
            entity.Tag = tag;
        }

        private IEnumerable EndOfStory(Story story, EntityWorld world)
        {
            yield break;
        }

        #region Story Variables
        

        #endregion
    }
}