
using UnityEngine;

#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

#if USING_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace ZakhanSpellsPack2
{
    [ExecuteInEditMode]
    public class SP2_VFXAuraDecal : MonoBehaviour
    {
            #if USING_URP
            [SerializeField] private UnityEngine.Rendering.Universal.DecalProjector Projector; //URP
            #endif
            #if USING_HDRP
            [SerializeField] private UnityEngine.Rendering.HighDefinition.DecalProjector Projector_HDRP;
            #endif

            [SerializeField] private bool ExecuteInEditor = false;
            [SerializeField][Range(0,1)] private float Alpha = 1;
		    [SerializeField] private Material Material;

		    private Material MaterialClone;

            void Start()
            {
                if (!Application.isPlaying && !ExecuteInEditor) { return; }

                Material Mat = Instantiate(Material);
                MaterialClone = Mat;
                #if USING_URP
                 if(Projector)
                    Projector.material = MaterialClone;
                #endif
                #if USING_HDRP
                 if (Projector_HDRP)
                    Projector_HDRP.material = MaterialClone;
                #endif

        }

            void LateUpdate()
            {
                if (!Application.isPlaying && !ExecuteInEditor) { return; }

                if (MaterialClone != null)
                {
                    MaterialClone.SetFloat("_Alpha", Alpha);
                }
            }
    }
}
