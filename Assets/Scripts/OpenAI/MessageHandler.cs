using DG.Tweening;
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
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private DotweenSettings _dotweenSettings;
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
    public void Fade(bool state)
    {
        if (state)
        {
            _canvasGroup.DOFade(1, _dotweenSettings.Duration);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        else
        {
            _canvasGroup.DOFade(0, _dotweenSettings.Duration);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        
    }

    private void OnDestroy()
    {
        _npcHandler.OnNPCAnswered -= NPCMessage;
    }
}
