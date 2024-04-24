using Prefab;
using UnityEngine;

namespace Pool
{
    public class CharacterUIPooler : ObjectPooler<UIType>
    {
        private CharacterUIPrefabHolder _provider;
        public CharacterUIPooler(CharacterUIPrefabHolder provider)
        {
            _provider = provider;
        }

        protected override GameObject GetPrefab(UIType tag)
        {
            return _provider.GetPrefab(tag);
        }
    }
}
