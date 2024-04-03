using CharactersStats;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IEffectProcessor
    {
        void AddEffects(List<IEffect> effects);
        ReactiveStats ProcessStatsBeforeInteraction(ReactiveStats stats);
        void ProcessEffectsOnStart(ReactiveStats stats);
        void ProcessEffectsOnEnd(ReactiveStats stats);
        List<IEffect> GetPreInteractionEffects();
        List<IEffect> GetOnStartTurnInteractionEffects();
        List<IEffect> GetOnEndTurnInteractionEffects();
        void PrintEffects(List<IEffect> effects);
    }
    public class EffectProcessor : IEffectProcessor
    {
        List<IEffect> _preInteractionEffects = new();
        List<IEffect> _onStartTurnEffects = new();
        List<IEffect> _onEndTurnEffects = new();
        public void AddEffects(List<IEffect> effects)
        {
            foreach (IEffect effect in effects)
            {
                if (effect.IsOnInteractionStart)
                {
                    ReplaceOrAddEffect(effect, _preInteractionEffects);
                }

                if (effect.IsOnTurnStart)
                {
                    ReplaceOrAddEffect(effect, _onStartTurnEffects);
                }

                if (effect.IsOnTurnEnd)
                {
                    ReplaceOrAddEffect(effect, _onEndTurnEffects);
                }
            }
        }

        private List<IEffect> ReplaceOrAddEffect(IEffect effect, List<IEffect> effectList)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                if (effectList[i].GetType() == effect.GetType())
                {
                    effectList[i] = effect;
                    return effectList;
                }
            }
            effectList.Add(effect);
            return effectList;
        }

        public List<IEffect> GetPreInteractionEffects()
        { 
            return _preInteractionEffects;
        }

        public List<IEffect> GetOnStartTurnInteractionEffects()
        {
            return _onStartTurnEffects;
        }
        public List<IEffect> GetOnEndTurnInteractionEffects()
        {
            return _onEndTurnEffects;
        }

        public ReactiveStats ProcessStatsBeforeInteraction(ReactiveStats stats) 
        {
            ReactiveStats statsCopy = new ReactiveStats();

            statsCopy.Speed.Value = stats.Speed.Value;
            statsCopy.Health.Value = stats.Health.Value;
            statsCopy.Damage.Value = stats.Damage.Value;
            statsCopy.LaunchPower.Value = stats.LaunchPower.Value;
            statsCopy.Velocity.Value = stats.Velocity.Value;

            if (_preInteractionEffects.Count > 0)
            {
                for (int i = 0; i < _preInteractionEffects.Count; i++)
                {
                    if (_preInteractionEffects[i].Duration > 0)
                    {
                        _preInteractionEffects[i].UseEffect(statsCopy);
                    }
                    else
                    {
                        _preInteractionEffects.Remove(_preInteractionEffects[i]);
                    }
                }
            }
            return statsCopy;
        }

        public void ProcessEffectsOnStart(ReactiveStats stats)
        {
            if (_onStartTurnEffects.Count > 0)
            {
                for (int i = 0; i < _onStartTurnEffects.Count; i++)
                {
                    if (_onStartTurnEffects[i].Duration > 0)
                    {
                        _onStartTurnEffects[i].UseEffect(stats);
                    }
                    else
                    {
                        _onStartTurnEffects.Remove(_onStartTurnEffects[i]);
                    }
                }
            }
        }

        public void ProcessEffectsOnEnd(ReactiveStats stats)
        {
            if (_onEndTurnEffects.Count > 0)
            {
                for (int i = 0; i < _onEndTurnEffects.Count; i++)
                {
                    if (_onEndTurnEffects[i].Duration > 0)
                    {
                        _onEndTurnEffects[i].UseEffect(stats);
                    }
                    else
                    {
                        _onEndTurnEffects.Remove(_onEndTurnEffects[i]);
                    }
                }
            }
        }

        //////////////////////////////////////////////////
        public void PrintEffects(List<IEffect> effects)
        {
            foreach (IEffect effect in effects)
            {
                string effectName = effect.GetType().Name;
                float effectDuration = effect.Duration;

                Debug.Log($"{effectName}, Duration: {effectDuration}");
            }
        }

    }

}

