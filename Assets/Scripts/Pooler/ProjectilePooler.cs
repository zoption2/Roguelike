using Prefab;
using UnityEngine;

namespace Pool
{
    public class ProjectilePooler : ObjectPooler<ProjectileType>
    {
        public ProjectilePooler(ProjectilePrefabHolder provider)
        {
            _prefabHolder = provider;
        }
    }
}
