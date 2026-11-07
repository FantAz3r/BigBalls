using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionComponent
{
    public static void Enable<T> (this T component) where T : Component
    {
        component.gameObject.SetActive(true);
    }

    public static void Disable<T> (this T component) where T : Component
    {
        component.gameObject.SetActive(false);
    }

    public static void SetActive<T> (this T component, bool isActive) where T : Component
    {
        component.gameObject.SetActive(isActive);
    }
}
