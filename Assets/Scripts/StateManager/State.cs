using Player;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public interface IState
    {
        IScenario _scenario { get; }

        public void OnEnter();
        public void OnExit();
    }

    public class PlayerTurnState : IState
    {
        public IScenario _scenario { get; }

        public ICharacterScenarioContext _characters { get; }

        public PlayerTurnState( IScenario scenario, ICharacterScenarioContext context)
        {
            _scenario = scenario;
            _characters = context;
        }

        public void OnEnter()
        {
            for (int i = 0; i < _characters.Players.Count; i++)
            {
                _characters.Players[i].IsActive = true;
                _characters.Players[i].OnSwitch += _scenario.OnStateEnd;
            }
            Debug.Log("Entered player turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _characters.Players.Count; i++)
            {
                _characters.Players[i].IsActive = false;
                _characters.Players[i].OnSwitch -= _scenario.OnStateEnd;
            }
            Debug.Log("Exited player turn state");
        }
    }
    public class EnemyTurnState : IState
    {
        public IScenario _scenario { get; }
        
        public ICharacterScenarioContext _characters { get; }

        public EnemyTurnState(IScenario scenario, ICharacterScenarioContext context)
        {
            _scenario = scenario;
            _characters = context;
        }
        public void OnEnter()
        {
            for(int i=0; i <_characters.Enemies.Count;i++ )
            {
                _characters.Enemies[i].IsActive = true;
                _characters.Enemies[i].OnSwitch += _scenario.OnStateEnd;
            }
            Debug.Log("Entered enemy turn state");
        }

        public void OnExit()
        {
            for (int i = 0; i < _characters.Enemies.Count; i++)
            {
                _characters.Enemies[i].IsActive = false;
                _characters.Enemies[i].OnSwitch -= _scenario.OnStateEnd;
            }
            Debug.Log("Exited enemy turn state");
        }
    }

    public class InitLevelState : IState
    {
        public IScenario _scenario { get; }

        public ICharacterScenarioContext _characters { get; }

        public InitLevelState(IScenario scenario, ICharacterScenarioContext context)
        {
            _scenario = scenario;
            _characters = context;
        }

        public void OnEnter()
        {
            Debug.Log("Entering init state");
            OnCreate();
            _scenario.OnStateEnd();
        }

        public void OnCreate()
        {

            int _id = 1;
            PlayerType _type = PlayerType.Warrior;
            Stats _stats = _scenario._gameplayService._statsProvider.GetStats(_type, _id);
            Debug.Log("Number of player spawn points ");
            foreach (Transform spawnPoint in _characters.PlayerSpawnPoints)
            {
                _scenario._gameplayService._playerFactory.CreatePlayer(spawnPoint, _type, new PlayerModel(_id ,_type, _stats),_characters);
            }

            Debug.Log(_stats.Health);
        }

        public void OnExit()
        {
            Debug.Log("Exited init state");
        }
    }

}
