using UnityEngine;

public struct CanvasGroupSnapshot
{
    public CanvasGroupSnapshot(CanvasGroup canvasGroup)
    {
        Alpha = canvasGroup.alpha;
        Interactable = canvasGroup.interactable;
        BlocksRaycasts = canvasGroup.blocksRaycasts;
        IgnoreParentGroups = canvasGroup.ignoreParentGroups;
    }

    public float Alpha { get; }
    public bool Interactable { get; }
    public bool BlocksRaycasts { get; }
    public bool IgnoreParentGroups { get; }
}
