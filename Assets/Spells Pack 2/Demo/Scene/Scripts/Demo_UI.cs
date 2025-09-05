using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZakhanSpellsPack2
{
    public class Demo_UI : MonoBehaviour
    {
		[SerializeField] private Demo Demo;
		[SerializeField] private Canvas Canvas;
		[SerializeField] private TMP_Text PrefabName;
		[SerializeField] private Button NextButton;
		[SerializeField] private Button BackButton;
        [SerializeField] private TMP_Dropdown CategoriesDropdown;

        private void Start()
		{
			NextButton.onClick.AddListener(Demo.Next);
			BackButton.onClick.AddListener(Demo.Back);
			if(CategoriesDropdown)
            CategoriesDropdown.onValueChanged.AddListener((index) =>
            {
				string currentSelection = CategoriesDropdown.options[index].text;
                Demo.ChangeType(currentSelection);
            });

        }
		public void EnableCanvas()
		{
			bool state = Canvas.gameObject.activeInHierarchy;
			switch (state)
			{
				case true:
					Canvas.gameObject.SetActive(false);
					break;
				case false:
					Canvas.gameObject.SetActive(true);
					break;
			}
		}
		public void ChangeName(string NewName)
        {
			PrefabName.text = NewName;
		}
		public void EnableNextButton(bool state)
		{
			NextButton.gameObject.SetActive(state);
		}
		public void EnableBackButton(bool state)
		{
			BackButton.gameObject.SetActive(state);
		}
	}
}
