using Interactions;
using System.Collections.Generic;

namespace Obstacles
{
    public interface IBuff
    {
        public List<IEffect> ProcessTrigger();
        void DisableBuff();
    }
}


