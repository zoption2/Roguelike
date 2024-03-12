using Prefab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class CharacterPanelPooler : ObjectPooler<PlayerType>
    {
        private CharacterPanelPrefabHolder _holder;
        public CharacterPanelPooler(CharacterPanelPrefabHolder provider)
        {
            _holder = provider;
        }

        protected override GameObject GetPrefab(PlayerType tag)
        {
            return _holder.GetPrefab(tag);
        }
    }
}
