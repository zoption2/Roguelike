using Interactions;
using System.Collections.Generic;

namespace Obstacles
{
    public interface IBuff
    {
        public void Init(BuffPooler pooler);
        public BuffType GetBuffType();
        public float GetBuffProbability();
        public List<IEffect> UseBuff();
        void RemoveBuff();
    }
}


