using UnityEngine;

namespace ZakhanSpellsPack2
{
    public class SP2_VFX_Tome : MonoBehaviour
    {
        [SerializeField] private Transform Tome;
        [SerializeField] private Vector3 RotationAxis;
        [SerializeField] private float Speed = 1.0f;
        [SerializeField] private AnimationCurve PositionAnimation;
        [SerializeField] private Vector2 PositionRange;
        [SerializeField] private float AnimationSpeed = 1.0f;

        private float AnimationTime = 0;

        void FixedUpdate()
        {
            // Continuously rotate the object around the specified axis
            Tome.Rotate(RotationAxis, Speed, Space.Self);
            // Increment the animation timer based on fixed delta time and speed
            AnimationTime += Time.fixedDeltaTime * AnimationSpeed;
            // Create a ping-pong effect between 0 and 1
            float pingPongTime = Mathf.PingPong(AnimationTime, 1f);
            // Evaluate the animation curve using the ping-pong time
            float curveValue = PositionAnimation.Evaluate(pingPongTime);
            // Map the curve output (usually 0 to 1) to the desired Y position range
            float mappedY = Mathf.Lerp(PositionRange.x, PositionRange.y, curveValue);
            // Apply the new local Y position while keeping X and Z unchanged
            Tome.localPosition = new Vector3(0, mappedY, 0);
        }
    }
}
