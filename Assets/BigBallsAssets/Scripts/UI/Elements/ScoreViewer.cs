using TMPro;
using UnityEngine;
using VContainer;

public class ScoreViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private ScoreCounter _scoreCounter;

    [Inject]
    public void Construct (ScoreCounter scoreCounter)
    {
        _scoreCounter = scoreCounter;
    }

    private void Start ()
    {
        _scoreCounter.ScoreChanged += OnScoreChanged;
        _scoreText.text = ((int) _scoreCounter.CurrentScore).ToString();
    }

    private void OnScoreChanged (float value)
    {
        _scoreText.text = ((int) value).ToString();
    }

    private void OnDestroy ()
    {
        _scoreCounter.ScoreChanged -= OnScoreChanged;
    }
}
