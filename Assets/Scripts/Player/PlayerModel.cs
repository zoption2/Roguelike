using Prefab;
using System;
using CharactersStats;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class PlayerModel : CharacterModelBase
    {
        private PlayerType _type;
        public PlayerType Type { get { return _type; } }
        public PlayerModel(int id, PlayerType type, Stats stats)
            : base(id, stats)
        {
            _type = type;
        }

        public PlayerType GetModelType()
        {
            return _type;
        }
    }
}
