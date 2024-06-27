using Enemy;
using Obstacles;
using Player;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay
{
    public interface ILevelContext : IScenarioContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }
    }

    public class LevelContext : ILevelContext
    {
        public List<IPlayerController> Players { get; set; }
        public List<IEnemyController> Enemies { get; set; }

        public LevelContext()
        {
            Players = new List<IPlayerController>();
            Enemies = new List<IEnemyController>();
        }
    }
}

