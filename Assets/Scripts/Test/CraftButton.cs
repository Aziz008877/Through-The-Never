using UnityEngine;

public class CraftButton : MonoBehaviour
{
    [SerializeField] private CraftingUI _ui;

    public void OnClick()
    {
        _ui.Craft();
    }
}