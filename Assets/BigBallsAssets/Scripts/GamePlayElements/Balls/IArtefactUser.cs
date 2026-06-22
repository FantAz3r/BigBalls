using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public interface IArtefactUser
    {
        void AddEffects (List<EffectBehaviour> effects);
    }
}