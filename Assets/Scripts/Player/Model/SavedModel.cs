using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    [CreateAssetMenu]
    public class SavedModel : ScriptableObject
    {
        public int ID;
        public int Health, Damage, Speed;
    }
}
