using Prefab;
using UnityEngine;

namespace Pool
{

    public class CharacterPooler : ObjectPooler<CharacterType>
    {
        private CharacterPrefabHolder _provider;
        public CharacterPooler(CharacterPrefabHolder provider)
        {
            _provider = provider;
        }

        protected override GameObject GetPrefab(CharacterType tag)
        {
            return _provider.GetPrefab(tag);
        }
    }
}
