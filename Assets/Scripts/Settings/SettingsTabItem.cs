using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTabItem : MonoBehaviour
{
    [SerializeField] private Sprite _defaultSprite, _selectedSprite;
    [SerializeField] private int _id;
    private Image _tabImage;
    private Button _tabButton;
    public Action<int> OnTabClicked;
    private void Start()
    {
        _tabImage = GetComponent<Image>();
        _tabButton = GetComponent<Button>();
        
        _tabButton.onClick.AddListener(delegate
        {
            OnTabClicked?.Invoke(_id);
        });
    }

    public void SetIsCurrent(bool state)
    {
        _tabImage.sprite = state ? _selectedSprite : _defaultSprite;
    }
}
