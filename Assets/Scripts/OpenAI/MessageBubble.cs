using TMPro;
using UnityEngine;
public class MessageBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText, _talkingCharacterName;
    public void Init(string speakerName, string text, bool isPlayer)
    {
        _talkingCharacterName.text = speakerName;
        _messageText.text = text;
    }
}