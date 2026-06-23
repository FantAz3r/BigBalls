using UnityEngine;

namespace BigBalls.Configs
{
    public abstract class ItemConfig : ScriptableObject, IItemConfig
    {
        [SerializeField] private string _nameEN;
        [SerializeField] private string _nameRU;
        [SerializeField] private string _nameTR;

        [SerializeField] private string _descriptionEN;
        [SerializeField] private string _descriptionRU;
        [SerializeField] private string _descriptionTR;

        [field: SerializeField] public Rarity Rarity { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public bool IsOpen { get; private set; } = false;

        public string GetName (string lang)
        {
            if (lang == "ru")
                return _nameRU;
            else if (lang == "en")
                return _nameEN;
            else if (lang == "tr")
                return _nameTR;

            return string.Empty;
        }

        public string GetDescription (string lang)
        {
            if (lang == "ru")
                return _descriptionRU;
            else if (lang == "en")
                return _descriptionEN;
            else if (lang == "tr")
                return _descriptionTR;

            return string.Empty;
        }
    }
}
