using TMPro;
using UnityEngine;

public class EndLevelMarker : MonoBehaviour
{
    private Player _player;
    private TMP_Text _text;

    public void Start()
    {
        _player = FindAnyObjectByType<Player>();
        _text = GameObject.Find("Text").GetComponent<TMP_Text>();
    }

    public void Update()
    {
        if (!_player)
        {
            return;
        }

        if (_player.transform.position.x > transform.position.x)
        {
            _text.text = "You won!";
        }
    }
}
