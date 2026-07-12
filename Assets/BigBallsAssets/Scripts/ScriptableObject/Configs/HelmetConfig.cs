using System.Collections.Generic;
using BigBalls.Attributes;
using BigBalls.GameplayObjects;
using UnityEngine;

namespace BigBalls.Configs
{
    [CustomScriptableObjectListEditor]
    [CreateAssetMenu(fileName = "HelmetConfig", menuName = "Configs/Item/HelmetConfigs")]

    public class HelmetConfig : ItemConfig, IBuffer
    {
        [SerializeField] private List<ArtefactConfig> _artefacts;
        public List<ArtefactConfig> Artefacts => _artefacts;

        public override List<ArtefactConfig> GetArtefacts(int level)
        {
            return _artefacts;
        }
    }
}