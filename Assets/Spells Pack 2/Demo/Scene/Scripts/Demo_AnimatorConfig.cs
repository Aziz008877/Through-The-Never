using UnityEngine;

namespace ZakhanSpellsPack2
{
	public class Demo_AnimatorConfig : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		private Vector3 Position;
		private Quaternion Rotation;
		private Vector3 Scale;

		void Awake()
		{
			Position = transform.localPosition;
			Rotation = transform.localRotation;
			Scale = transform.localScale;
		}

		private void OnDisable()
		{
			animator.keepAnimatorStateOnDisable = true;
			transform.localPosition = Position;
			transform.localRotation = Rotation;
			transform.localScale = Scale;

		}
	}
}
