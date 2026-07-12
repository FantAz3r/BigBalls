using System.Collections.Generic;
using BigBalls.UI;
using UnityEngine;
using UnityEngine.UI;

public class Background : WindowBase
{
    [SerializeField] private Image _image;
    [SerializeField] private List<Sprite> _sprites = new();

    private void Awake ()
    {
        _image.sprite = _sprites[Random.Range(0, _sprites.Count)];
    }
}
