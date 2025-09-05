using UnityEngine;
using UnityEngine.InputSystem;

namespace ZakhanSpellsPack2
{
	public class DemoCamera : MonoBehaviour
    {
		public Transform target;
		public float distance = 5.0f;
		public float xSpeed = 120.0f;
		public float ySpeed = 120.0f;

		public float yMinLimit = -20f;
		public float yMaxLimit = 80f;

		public float distanceMin = .5f;
		public float distanceMax = 15f;
		public float ScrollSpeed = 1.5f;

		private Rigidbody rb;

		float x = 0.0f;
		float y = 0.0f;

		public InputSystem_Actions InputAction;


		private void Awake()
		{
			InputAction = new InputSystem_Actions();
		}
		// Use this for initialization
		void Start()
		{
			Vector3 angles = transform.eulerAngles;
			x = angles.y;
			y = angles.x;

			rb = GetComponent<Rigidbody>();

			// Make the rigid body not change rotation
			if (rb != null)
			{
				rb.freezeRotation = true;
			}
		}

		private void OnEnable()
		{
			InputAction.Enable();
			InputAction.Player.Camera.performed += Camera_performed;
			InputAction.Player.CameraClick.performed += CameraClick_performed;
			InputAction.Player.CameraScroll.performed += CameraScroll_performed;
		}

		private void OnDisable()
		{
			InputAction.Disable();
			InputAction.Player.Camera.performed -= Camera_performed;
			InputAction.Player.CameraClick.performed -= CameraClick_performed;
			InputAction.Player.CameraScroll.performed -= CameraScroll_performed;
		}

		Vector2 Direction;
		private void Camera_performed(InputAction.CallbackContext context)
		{
			Vector2 MouseAxis = context.ReadValue<Vector2>();
			Direction = MouseAxis;
			//Debug.Log(MouseAxis);
		}

		float Click = 0;
		private void CameraClick_performed(InputAction.CallbackContext context)
		{
			Click = context.ReadValue<float>();
			Direction = new Vector2(0, 0);
			//Debug.Log(Click);
		}

		Vector2 ScrollAxis;
		private void CameraScroll_performed(InputAction.CallbackContext context)
		{
			ScrollAxis = context.ReadValue<Vector2>();
			//Debug.Log(ScrollAxis);
		}

		void LateUpdate()
		{
			if (target)
			{
				if (Click == 1)
				{
					x += Direction.x * xSpeed * distance * 0.02f;
					y -= Direction.y * ySpeed * 0.02f;

					y = ClampAngle(y, yMinLimit, yMaxLimit);

				}

				Quaternion rotation = Quaternion.Euler(y, x, 0);
				distance = Mathf.Clamp(distance - ScrollAxis.y * ScrollSpeed, distanceMin, distanceMax);

				RaycastHit hit;
				if (Physics.Linecast(target.position, transform.position, out hit))
				{
					distance -= hit.distance;
				}
				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
				Vector3 position = rotation * negDistance + target.position;

				transform.rotation = rotation;
				transform.position = position;
			}
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
				angle += 360F;
			if (angle > 360F)
				angle -= 360F;
			return Mathf.Clamp(angle, min, max);
		}
	}
}
