using CharactersStats;
using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Player
{
    [Serializable]
    public class PlayerModel : CharacterModelBase
    {
        public PlayerType Type;
        public Stats PlayerStats;

        public PlayerModel(Stats stats, PlayerType playerType) : base(stats)
        {
            Type = playerType;
            PlayerStats = stats;
        }
    }   
}
