using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public interface IEffector
    {
        void UpdateEffects(IEffect effect);
        Queue<IEffect> GetPreInteractionEffects();
        Queue<IEffect> GetPostInteractionEffects();
        void CheckEffectsDuration();
    }
    public class Effector : IEffector
    {
        //Queue<IEffect> _effectQueue = new();
        Queue<IEffect> _preInteractionEffects = new();
        Queue<IEffect> _postInteractionEffects = new();
        public void UpdateEffects(IEffect effect)
        {
            switch (effect)
            {
                case IPreInteractionEffect:
                    _preInteractionEffects.Enqueue(effect);
                    break;
                case IPostInteractionEffect: 
                    _postInteractionEffects.Enqueue(effect); 
                    break;
            }
        }

        public Queue<IEffect> GetPreInteractionEffects()
        { 
            return _preInteractionEffects;
        }

        public Queue<IEffect> GetPostInteractionEffects()
        {
            return _postInteractionEffects;
        }

        public void CheckEffectsDuration()
        {
            DebugLogPreInteractionEffects();
            foreach (IEffect effect in _preInteractionEffects)
            {
                if (effect.Duration <= 0) _preInteractionEffects.Dequeue();
            }
            DebugLogPreInteractionEffects();

            Debug.Log("//////////////////////////////////////////////////////////////");

            DebugLogPostInteractionEffects();
            foreach (IEffect effect in _postInteractionEffects)
            {
                if (effect.Duration <= 0) _postInteractionEffects.Dequeue();
            }
            DebugLogPostInteractionEffects();
        }

        //////////////////////////////////////////////////
        public void DebugLogPreInteractionEffects()
        {
            string logMessage = "PreInteractionEffects: ";
            foreach (IEffect effect in _preInteractionEffects)
            {
                logMessage += effect.ToString() + ", ";
            }
            Debug.LogWarning(logMessage);
        }

        public void DebugLogPostInteractionEffects()
        {
            string logMessage = "PostInteractionEffects: ";
            foreach (IEffect effect in _postInteractionEffects)
            {
                logMessage += effect.ToString() + ", ";
            }
            Debug.LogWarning(logMessage);
        }
    }

}

