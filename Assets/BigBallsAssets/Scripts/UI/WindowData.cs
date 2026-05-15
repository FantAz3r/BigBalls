using System;
using System.Collections.Generic;
using UnityEngine;
using BigBalls.Srtucts;
using BigBalls.UI;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "Datas", menuName = "Datas/WindowData")]

    public class WindowData : ScriptableObject
    {
        public List<WindowInfo> WindowInfos = new List<WindowInfo>();

        public WindowBase Get(WindowType windowType)
        {
            foreach (WindowInfo info in WindowInfos)
            {
                if (info.Type == windowType)
                {
                    return info.Pefab;
                }
            }

            throw new ArgumentNullException(nameof(windowType));
        }
    }
}
