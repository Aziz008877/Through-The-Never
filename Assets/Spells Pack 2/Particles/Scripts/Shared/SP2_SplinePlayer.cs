using UnityEngine;
using UnityEngine.Splines;

namespace ZakhanSpellsPack2
{
	public class SP2_SplinePlayer : MonoBehaviour
	{
		[SerializeField] private SplineAnimate SplineAnimate;
		void OnEnable()
		{
			SplineAnimate.Play();
            foreach (Transform child in transform)
            {
				child.gameObject.SetActive(true);
            }
        }

		void OnDisable()
		{
			SplineAnimate.Restart(false);
		}
	}
}
