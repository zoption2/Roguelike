using Cysharp.Threading.Tasks;
using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class CharacterPanelPooler : ObjectPooler<CharacterType>
    {
        public CharacterPanelPooler(CharacterPanelPrefabHolder provider)
        {
            _prefabHolder = provider;
        }
    }
}
