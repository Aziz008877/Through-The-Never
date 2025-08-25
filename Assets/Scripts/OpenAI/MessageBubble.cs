using TMPro;
using UnityEngine;
public class MessageBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    public void Init(string text)
    {
        _messageText.text = text;
    }
}