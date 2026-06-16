using System.Collections.Generic;
using BigBalls.Attributes;
using UnityEngine;

namespace BigBalls.Configs
{
    [CustomScriptableObjectListEditor]
    [CreateAssetMenu(fileName = "HelmetConfig", menuName = "Configs/Item/HelmetConfigs")]

    public class HelmetConfig : ItemConfig, IBuffer
    {
        [SerializeField] private List<ArtefactModel> _artefacts;
        public List<ArtefactModel> Artefacts => _artefacts;
    }
}