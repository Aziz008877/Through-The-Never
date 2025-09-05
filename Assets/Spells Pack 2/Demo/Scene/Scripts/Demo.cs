using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZakhanSpellsPack2
{
    public class Demo : MonoBehaviour
    {
		[Serializable]
		class VFXData
		{
            public string Name;
			public GameObject VFX;
			public List<GameObject> Dummies = new List<GameObject>();
		}

        [SerializeField] private List<VFXData> VFX = new List<VFXData>();
        [SerializeField] private List<VFXData> Projectiles = new List<VFXData>();
        [SerializeField] private List<VFXData> Auras = new List<VFXData>();
        [SerializeField] private List<VFXData> Shields = new List<VFXData>();
        [SerializeField] private List<VFXData> Buffs = new List<VFXData>();
        [SerializeField] private List<VFXData> Tomes = new List<VFXData>();

        private List<VFXData> CurrentVFXList = new List<VFXData>();
        [SerializeField] private Transform DummiesGroup;
		//[SerializeField] private Demo_Cinemachine_Impulses ImpulsesController;
		[SerializeField] private int CurrentSelection = 0;

		public InputSystem_Actions InputAction;

		[Header("UI Settings")]
		[SerializeField] private Demo_UI UI;
		private void Awake()
		{
			InputAction = new InputSystem_Actions();

			for (int i = 0; i < DummiesGroup.childCount; i++)
			{
				GameObject Dummy = DummiesGroup.transform.GetChild(i).gameObject;
				Dummy.SetActive(false);
			}

			SetCurrentList(VFX); // Spells
        }

		void Start()
		{
			CurrentSelection = 0;
            CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(true);
			Dummies(true);

			//UI
			UpdateUI();

		}

		private void OnEnable()
		{
			InputAction.Enable();
			InputAction.Player.Next.performed += Next_Performed;
			InputAction.Player.Previous.performed += Back_Performed;
			InputAction.UI.HideUI.performed += HideUI_performed;
		}

		private void OnDisable()
		{
			InputAction.Disable();
			InputAction.Player.Next.performed -= Next_Performed;
			InputAction.Player.Previous.performed -= Back_Performed;
			InputAction.UI.HideUI.performed -= HideUI_performed;
		}

		private void HideUI_performed(InputAction.CallbackContext context)
		{
			UI.EnableCanvas();
		}

		private void Next_Performed(InputAction.CallbackContext context)
		{
			Next();
		}

		private void Back_Performed(InputAction.CallbackContext context)
		{
			Back();	
		}
		public void Next()
		{
			if (CurrentSelection >= 0 && CurrentSelection != CurrentVFXList.Count - 1)
			{
				Dummies(false);
                CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(false);
				//ImpulsesController.StopImpulse(); // Stopping Impulses to avoid overlap with other VFXs
				CurrentSelection++;
                CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(true);
				Dummies(true);

				//UI
				UpdateUI();
			}
		}
		public void Back()
		{
			if (CurrentSelection > 0)
			{
				Dummies(false);
                CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(false);
				//ImpulsesController.StopImpulse(); // Stopping Impulses to avoid overlap with other VFXs
				CurrentSelection--;
                CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(true);
				Dummies(true);

				//UI
				UpdateUI();
			}
		}
		private void UpdateUI()
		{
			//UI
			UI.ChangeName(CurrentVFXList[CurrentSelection].Name);

            if (CurrentSelection > 0)
            {
                UI.EnableBackButton(true);
            }
			else if (CurrentSelection == 0)
            {
                UI.EnableBackButton(false);
            }


            if (CurrentSelection >= 0 && CurrentSelection != CurrentVFXList.Count - 1)
			{
                UI.EnableNextButton(true);
            }
            else if (CurrentSelection == CurrentVFXList.Count - 1)
            {
                UI.EnableNextButton(false);
            }

        }

		public void ChangeType(string value)
		{
			switch (value)
			{
				case "Spells":
					SetCurrentList(VFX); // Spells List
                    break;
                case "Projectiles":
                    SetCurrentList(Projectiles);
                    break;
				case "Auras":
					SetCurrentList(Auras);
					break;
                case "Shields":
                    SetCurrentList(Shields);
                    break;
				case "Buffs":
					SetCurrentList(Buffs);
					break;
                case "Tomes":
                    SetCurrentList(Tomes);
                    break;
            }
		}
		private void SetCurrentList (List<VFXData> Clone)
		{
			if(CurrentVFXList.Count > 0)
			{
                Dummies(false);
                CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(false);
            }
				
            //ImpulsesController.StopImpulse();

			CurrentVFXList.Clear();

            foreach (var VFX in Clone)
            {
                CurrentVFXList.Add(VFX);
            }

			CurrentSelection = 0;

            CurrentVFXList[CurrentSelection].VFX.gameObject.SetActive(true);
            Dummies(true);

            //UI
            UpdateUI();

        }

		private void Dummies(bool State)
		{
			foreach(GameObject Dummy in CurrentVFXList[CurrentSelection].Dummies)
			{
				Dummy.gameObject.SetActive(State);
			}
		}

    }
}
