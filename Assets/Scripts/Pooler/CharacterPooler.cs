using Cysharp.Threading.Tasks;
using Prefab;
using UnityEngine;

namespace Pool
{
    public class CharacterPooler : ObjectPooler<CharacterType>
    {
        public CharacterPooler(CharacterPrefabHolder provider)
        {
            _prefabHolder = provider;
        }
    }
}
