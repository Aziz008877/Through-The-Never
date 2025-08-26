using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    [SerializeField] private MessageBubble _playerBubble, _npcBubble;
    [SerializeField] private NPCHandler _npcHandler;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private TMP_InputField _playerInput;
    [SerializeField] private Button _sendQuestionButton;
    private void Awake()
    {
        _npcHandler.OnNPCAnswered += NPCMessage;
        
        _sendQuestionButton.onClick.AddListener(delegate
        {
            _sendQuestionButton.interactable = false;
            _npcHandler.SendReply(_playerInput.text);
            PlayerMessage();
        });
    }
    private void ScrollToBottom()
    {
        StartCoroutine(ScrollEndOfFrame());
    }

    private IEnumerator ScrollEndOfFrame()
    {
        yield return null;
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    private void NPCMessage(string text)
    {
        _sendQuestionButton.interactable = true;
        MessageBubble newMessage = Instantiate(_npcBubble, _messageContainer);
        newMessage.Init(text);
        ScrollToBottom();
    }

    private void PlayerMessage()
    {
        MessageBubble newMessage = Instantiate(_playerBubble, _messageContainer);
        newMessage.Init(_playerInput.text);
        _playerInput.text = "";
        ScrollToBottom();
    }

    private void OnDestroy()
    {
        _npcHandler.OnNPCAnswered -= NPCMessage;
    }
}
