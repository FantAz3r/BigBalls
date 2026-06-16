using BigBalls.GameplayObjects;
using BigBalls.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : ButtonClickHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _stats;
    [SerializeField] private TMP_Text _level;

    private PlayerBallContainer _playerBallContainer;
    private ICard _card;

    public void Construct(PlayerBallContainer playerBallContainer)
    {
        _playerBallContainer = playerBallContainer;
    }

    public void Render(ICard card)
    {
        _card = card;
        _image.sprite = card.Icon;
        _name.text = card.Name;
        _description.text = card.Description;
        _stats.text = RenderStats();
        _level.text = card.Level.ToString();
    }

    private string RenderStats()
    {
        return string.Empty;
    }

    protected override void OnClick()
    {
        if(_card is BallModel model)
        {
            if(_card.Level == 1)
            {
                _playerBallContainer.ReplaceOneBallWithUnique(model);
            }
            else
            {
                model.Upgrade();
            }

        }
        else if (_card is ArtefactModel artefact)
        {

        }

    }
}
