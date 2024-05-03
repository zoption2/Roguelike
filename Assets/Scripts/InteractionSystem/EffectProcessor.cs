using CharactersStats;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Interactions
{
    public interface IEffectProcessor
    {
        void Init(CharacterUIViewmodel viewModel, ReactiveList<IEffect> allEffects);
        void AddEffects(List<IEffect> effects);
        ReactiveStats ProcessStatsBeforeInteraction(ReactiveStats stats);
        void ProcessEffectsOnStart(ReactiveStats stats);
        void ProcessEffectsOnEnd(ReactiveStats stats);
        List<IEffect> GetPreInteractionEffects();
        List<IEffect> GetOnStartTurnInteractionEffects();
        List<IEffect> GetOnEndTurnInteractionEffects();
        //ReactiveCollection<IEffect> AllEffects { get; set; }
        //ReactiveCollection<IEffect> GetAllEffects();
        void PrintEffects(List<IEffect> effects);
    }
    public class EffectProcessor : IEffectProcessor
    {
        List<IEffect> _preInteractionEffects = new();
        List<IEffect> _onStartTurnEffects = new();
        List<IEffect> _onEndTurnEffects = new();
        //public ReactiveCollection<IEffect> AllEffects { get; set; }
        private ReactiveList<IEffect> _allEffects;

        private CharacterUIViewmodel _viewModel;

        public void Init(CharacterUIViewmodel viewModel, ReactiveList<IEffect> allEffects)
        {
            _viewModel = viewModel;
            _allEffects = allEffects;
        }

        //public EffectProcessor()
        //{
        //    AllEffects = new ReactiveCollection<IEffect>();
        //}

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

                ReplaceOrAddEffect(effect, _allEffects);
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

        private ReactiveList<IEffect> ReplaceOrAddEffect(IEffect effect, ReactiveList<IEffect> effectList)
        {
            for (int i = 0; i < effectList.Value.Count; i++)
            {
                if (effectList.Value[i].GetType() == effect.GetType())
                {
                    effectList.Value[i].ON_DURATION_CHANGED -= HandleDurationChanged;
                    effectList.Value[i] = effect;
                    effectList.Value[i].ON_DURATION_CHANGED += HandleDurationChanged;
                    return effectList;
                }
            }
            effectList.Value.Add(effect);
            SubscribeToEffectValues(_allEffects);
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
                for (int i = _preInteractionEffects.Count - 1; i >= 0; i--)
                {
                    if (_preInteractionEffects[i].Duration > 0)
                    {
                        _preInteractionEffects[i].UseEffect(stats);
                    }
                    else
                    {
                        RemoveEffect(_preInteractionEffects[i]);
                    }
                }
            }

            return statsCopy;
        }


        public void ProcessEffectsOnStart(ReactiveStats stats)
        {
            if (_onStartTurnEffects.Count > 0)
            {
                for (int i = _onStartTurnEffects.Count - 1; i >= 0; i--)
                {
                    if (_onStartTurnEffects[i].Duration > 0)
                    {
                        _onStartTurnEffects[i].UseEffect(stats);
                    }
                    else
                    {
                        RemoveEffect(_onStartTurnEffects[i]);
                    }
                }
            }
        }


        public void ProcessEffectsOnEnd(ReactiveStats stats)
        {
            if (_onEndTurnEffects.Count > 0)
            {
                for (int i = _onEndTurnEffects.Count - 1; i >= 0; i--)
                {
                    if (_onEndTurnEffects[i].Duration > 0)
                    {
                        _onEndTurnEffects[i].UseEffect(stats);
                    }
                    else
                    {
                        RemoveEffect(_onEndTurnEffects[i]);
                    }
                }
            }
        }

        private void ProcessEffects(List<IEffect> effectList, ReactiveStats stats)
        {
            
            if (effectList.Count > 0)
            {
                for (int i = effectList.Count - 1; i >= 0; i--)
                {
                    if (effectList[i].Duration > 0)
                    {
                        effectList[i].UseEffect(stats);
                    }
                    else
                    {
                        RemoveEffect(effectList[i]);
                    }
                }
            }
            
        }

        private void SubscribeToEffectValues(ReactiveList<IEffect> effects)
        {
            foreach (IEffect effect in effects.Value)
            {
                effect.ON_DURATION_CHANGED -= HandleDurationChanged;
                effect.ON_DURATION_CHANGED += HandleDurationChanged;
            }
        }

        private void HandleDurationChanged()
        {
            _viewModel.VisualiseEffects(_allEffects.Value);
        }


        public void RemoveEffect(IEffect effect)
        {
            if (_preInteractionEffects.Contains(effect))
            {
                _preInteractionEffects.Remove(effect);
                _allEffects.Value.Remove(effect);
            }

            if (_onStartTurnEffects.Contains(effect))
            {
                _onStartTurnEffects.Remove(effect);
                _allEffects.Value.Remove(effect);
            }

            if (_onEndTurnEffects.Contains(effect))
            {
                _onEndTurnEffects.Remove(effect);
                _allEffects.Value.Remove(effect);
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

