using UnityEngine;
public class SettingsTabHandler : MonoBehaviour
{
    [SerializeField] private SettingsTabItem[] _allTabs;
    private void Awake()
    {
        foreach (var tab in _allTabs)
        {
            tab.OnTabClicked += OnTabClicked;
        }
    }

    private void OnTabClicked(int clickedID)
    {
        foreach (var tab in _allTabs)
        {
            tab.SetIsCurrent(false);
        }
        
        _allTabs[clickedID].SetIsCurrent(true);
    }

    private void OnDestroy()
    {
        foreach (var tab in _allTabs)
        {
            tab.OnTabClicked -= OnTabClicked;
        }
    }
}
