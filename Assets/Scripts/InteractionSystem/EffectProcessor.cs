using CharactersStats;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace Interactions
{
    public interface IEffectProcessor
    {
        void Init(CharacterUIViewmodel viewModel);
        void AddEffects(List<IEffect> effects);
        ReactiveStats ProcessStatsBeforeInteraction(ReactiveStats stats);
        void ProcessEffectsOnStart(ReactiveStats stats);
        void ProcessEffectsOnEnd(ReactiveStats stats);
        List<IEffect> GetPreInteractionEffects();
        List<IEffect> GetOnStartTurnInteractionEffects();
        List<IEffect> GetOnEndTurnInteractionEffects();
        ReactiveCollection<IEffect> AllEffects { get; set; }
        ReactiveCollection<IEffect> GetAllEffects();
        void PrintEffects(List<IEffect> effects);
    }
    public class EffectProcessor : IEffectProcessor
    {
        List<IEffect> _preInteractionEffects = new();
        List<IEffect> _onStartTurnEffects = new();
        List<IEffect> _onEndTurnEffects = new();
        public ReactiveCollection<IEffect> AllEffects { get; set; }

        private CharacterUIViewmodel _viewModel;

        public void Init(CharacterUIViewmodel viewModel)
        {
            _viewModel = viewModel;
            //AllEffects = new();
        }

        public EffectProcessor()
        {
            AllEffects = new ReactiveCollection<IEffect>();
        }

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

                int existingIndex = AllEffects.IndexOf(effect);
                if (existingIndex != -1)
                {
                    AllEffects[existingIndex] = effect;
                }
                else
                {
                    AllEffects.Add(effect);
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

        public ReactiveCollection<IEffect> GetAllEffects()
        {
            return AllEffects;
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

            //_viewModel.VisualiseEffects(_allEffects);

            if (_preInteractionEffects.Count > 0)
            {
                for (int i = _preInteractionEffects.Count - 1; i >= 0; i--)
                {
                    if (_preInteractionEffects[i].Duration > 0)
                    {
                        _preInteractionEffects[i].UseEffect(statsCopy);
                    }
                    else
                    {
                        //AllEffects.Remove(_preInteractionEffects[i]);
                        AllEffects.RemoveAt(i);
                        _preInteractionEffects.RemoveAt(i);
                    }
                }
            }
            return statsCopy;
        }


        public void ProcessEffectsOnStart(ReactiveStats stats)
        {
            //_viewModel.VisualiseEffects(_allEffects);

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
                        //AllEffects.Remove(_onStartTurnEffects[i]);
                        AllEffects.RemoveAt(i);
                        _onStartTurnEffects.RemoveAt(i);
                    }
                }
            }
        }


        public void ProcessEffectsOnEnd(ReactiveStats stats)
        {
            //_viewModel.VisualiseEffects(_allEffects);

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
                        //AllEffects.Remove(_onEndTurnEffects[i]);
                        AllEffects.RemoveAt(i);
                        _onEndTurnEffects.RemoveAt(i);
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

