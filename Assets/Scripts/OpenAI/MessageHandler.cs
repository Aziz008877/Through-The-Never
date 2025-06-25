using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    [SerializeField] private MessageBubble _messageBubble;
    [SerializeField] private NPCHandler _npcHandler;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private TMP_InputField _playerInput;
    [SerializeField] private Button _sendQuestionButton;
    private void Awake()
    {
        _npcHandler.OnNPCAnswered += NPCMessage;
        
        _sendQuestionButton.onClick.AddListener(delegate
        {
            _npcHandler.SendReply(_playerInput.text);
            PlayerMessage();
        });
    }

    private void NPCMessage(string text)
    {
        MessageBubble newMessage = Instantiate(_messageBubble, _messageContainer);
        newMessage.Init("Naara", text, false);
    }

    private void PlayerMessage()
    {
        MessageBubble newMessage = Instantiate(_messageBubble, _messageContainer);
        newMessage.Init("Player", _playerInput.text, true);
    }

    private void OnDestroy()
    {
        _npcHandler.OnNPCAnswered -= NPCMessage;
    }
}
