using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class CharacterPanelPooler : ObjectPooler<CharacterType>
    {
        private CharacterPanelPrefabHolder _holder;
        public CharacterPanelPooler(CharacterPanelPrefabHolder provider)
        {
            _holder = provider;
        }

        protected override GameObject GetPrefab(CharacterType tag)
        {
            return _holder.GetPrefab(tag);
        }
    }
}
