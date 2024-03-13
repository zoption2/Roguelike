using System;
using System.Collections.Generic;
using System.Linq;
using CharactersStats;
using System.Text;
using System.Threading.Tasks;
using Prefab;

namespace Enemy
{
    public class EnemyModel : CharacterModelBase
    {
        private EnemyType _type;
        public EnemyType Type { get { return _type; } }
        public EnemyModel(Stats stats, EnemyType type, int id) : base(stats)
        {
            _type = type;
        }

        public EnemyType GetModelType()
        {
            return _type;
        }
    }
}
