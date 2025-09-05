using UnityEngine;

namespace ZakhanSpellsPack2
{
	public class Demo_ChangeParent : MonoBehaviour
    {
		[SerializeField] private Transform Object;
		[SerializeField] private Transform NewParent;
		[SerializeField] bool ResetRotation = false;

        void OnEnable()
        {
            Quaternion originalRotation = Object.transform.localRotation;
            Object.parent = NewParent.transform;

            Object.transform.position = NewParent.transform.position;
            Object.transform.localRotation = originalRotation;
          
            if (ResetRotation)
			{
				Object.transform.rotation = new Quaternion(0, 0, 0, 0);
            }

        }

		private void OnDisable()
		{
			if(Object)
			Object.gameObject.SetActive(false);
		}

	}
}
