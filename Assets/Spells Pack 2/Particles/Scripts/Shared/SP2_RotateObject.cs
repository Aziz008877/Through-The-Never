using UnityEngine;

namespace ZakhanSpellsPack2
{
        public class SP2_RotateObject : MonoBehaviour
        {
            [SerializeField] private Vector3 Axis = new(0,1,0);
            [SerializeField] private float Speed = 1.0f;
            void Update()
            {
                transform.Rotate(Axis, Speed, Space.Self);
            }

        }
}
