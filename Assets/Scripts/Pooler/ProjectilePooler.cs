using Prefab;

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
