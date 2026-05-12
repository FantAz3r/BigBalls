using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]

public class LevelConfig : ScriptableObject
{
    [field: SerializeField] public LevelID Level { get; private set; } = LevelID.Level1;
}
