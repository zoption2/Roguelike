using CharactersStats;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Interactions
{
    public interface IEffectProcessor
    {
        void AddEffects(IEffect effect);
        void ProcessStatsBeforeInteraction(ModifiableStats stats);
        void ProcessEffectsOnStart(ModifiableStats stats);
        void ProcessEffectsOnEnd(ModifiableStats stats);
        List<IEffect> GetPreInteractionEffects();
        List<IEffect> GetOnStartTurnInteractionEffects();
        List<IEffect> GetOnEndTurnInteractionEffects();
    }
    public class EffectProcessor : IEffectProcessor
    {
        List<IEffect> _preInteractionEffects = new();
        List<IEffect> _onStartTurnEffects = new();
        List<IEffect> _onEndTurnEffects = new();
        public void AddEffects(IEffect effect)
        {
            if(effect.IsOnInteractionStart)
            {
                ReplaceOrAddEffect(effect, _preInteractionEffects);
            }
          
            if(effect.IsOnTurnStart) 
            { 
                ReplaceOrAddEffect(effect, _onStartTurnEffects);
            }
            else
            {
                ReplaceOrAddEffect(effect, _onEndTurnEffects);
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

        public void ProcessStatsBeforeInteraction(ModifiableStats stats) 
        {
            string logMessage = "EffectsOnStart: ";

            if (_preInteractionEffects.Count > 0)
            {
                for (int i = 0; i < _preInteractionEffects.Count; i++)
                {
                    if (_preInteractionEffects[i].Duration > 0)
                    {
                        _preInteractionEffects[i].UseEffect(stats);

                        logMessage += _preInteractionEffects[i].ToString() + ", ";
                        Debug.LogWarning(logMessage);
                    }
                    else
                    {
                        _preInteractionEffects.Remove(_preInteractionEffects[i]);
                    }
                }
            }
        }

        public void ProcessEffectsOnStart(ModifiableStats stats)
        {
            string logMessage = "EffectsOnStart: ";

            if (_onStartTurnEffects.Count > 0)
            {
                for (int i = 0; i < _onStartTurnEffects.Count; i++)
                {
                    if (_onStartTurnEffects[i].Duration > 0)
                    {
                        _onStartTurnEffects[i].UseEffect(stats);

                        logMessage += _onStartTurnEffects[i].ToString() + ", ";
                        Debug.LogWarning(logMessage);
                    }
                    else
                    {
                        _onStartTurnEffects.Remove(_onStartTurnEffects[i]);
                    }
                }
            }
        }

        public void ProcessEffectsOnEnd(ModifiableStats stats)
        {
            string logMessage = "EffectsOnEnd: ";

            if (_onEndTurnEffects.Count > 0)
            {
                for (int i = 0; i < _onEndTurnEffects.Count; i++)
                {
                    if (_onEndTurnEffects[i].Duration > 0)
                    {
                        _onEndTurnEffects[i].UseEffect(stats);

                        logMessage += _onEndTurnEffects[i].ToString() + ", ";
                        Debug.LogWarning(logMessage);
                    }
                    else
                    {
                        _onEndTurnEffects.Remove(_onEndTurnEffects[i]);
                    }
                }
            }
        }


        //////////////////////////////////////////////////
        public void DebugLogPreInteractionEffects()
        {
            string logMessage = "PreInteractionEffects: ";
            foreach (IEffect effect in _preInteractionEffects)
            {
                if (_preInteractionEffects.Count > 0)
                {
                    logMessage += effect.ToString() + ", ";
                    Debug.LogWarning(logMessage);
                }   
            }
        }

        //public void DebugLogPostInteractionEffects()
        //{
        //    string logMessage = "PostInteractionEffects: ";
        //    foreach (IEffect effect in _postInteractionEffects)
        //    {
        //        if (_postInteractionEffects.Count > 0)
        //        {
        //            logMessage += effect.ToString() + ", ";
        //            Debug.LogWarning(logMessage);
        //        } 
        //    }
        //}
    }

}

