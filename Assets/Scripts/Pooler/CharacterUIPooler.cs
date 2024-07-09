using Cysharp.Threading.Tasks;
using Prefab;
using UnityEngine;

namespace Pool
{
    public class CharacterUIPooler : ObjectPooler<UIType>
    {
        public CharacterUIPooler(CharacterUIPrefabHolder provider)
        {
            _prefabHolder = provider;
        }
    }
}
