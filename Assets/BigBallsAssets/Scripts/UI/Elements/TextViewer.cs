using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextViewer : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _texts = new List<TMP_Text>();
    private int _id;

    public void WriteNext ()
    {
        if (_id > 0)
        {
            _texts[_id - 1].Disable();
        }

        _texts[_id].Enable();
        Debug.Log(123);
        if (_texts[_id].gameObject.TryGetComponent(out LetterWriter letterWriter))
        {
            letterWriter.StartTyping();

        }

        _id++;
    }
}
