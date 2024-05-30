using Prefab;
using UnityEngine;

namespace Pool
{
    public class ProjectilePooler : ObjectPooler<ProjectileType>
    {
        private ProjectilePrefabHolder _provider;
        public ProjectilePooler(ProjectilePrefabHolder provider)
        {
            _provider = provider;
        }

        protected override GameObject GetPrefab(ProjectileType tag)
        {
            return _provider.GetPrefab(tag);
        }
    }
}
