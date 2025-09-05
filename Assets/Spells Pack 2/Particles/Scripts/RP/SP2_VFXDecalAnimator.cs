
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
	public class SP2_VFXDecalAnimator : MonoBehaviour
	{
		#if USING_URP
        [SerializeField] private UnityEngine.Rendering.Universal.DecalProjector Projector; //URP
		#endif

		#if USING_HDRP
        [SerializeField] private UnityEngine.Rendering.HighDefinition.DecalProjector Projector_HDRP;
		#endif

        [SerializeField] private bool ExecuteInEditor = false;
		[SerializeField][Range(0,1)] private float Conjuration = 1f;
		[SerializeField] private Color Color = new Color(1f,1f,1f,1f);
		[SerializeField][Range(1, 10)] private float ColorIntensity = 1f;
		[SerializeField] private Material Material;

		[SerializeField] private string ConjurationName = "_Conjuration";
        [SerializeField] private string IntensityAlphaName = "_Intensity_Alpha";
       
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
				if(Projector_HDRP)
				Projector_HDRP.material = MaterialClone;
			#endif
        }

        void LateUpdate()
		{
			if (!Application.isPlaying && !ExecuteInEditor) { return; }

			if (MaterialClone != null)
			{
				MaterialClone.SetFloat(ConjurationName, Conjuration);
				Color hdrColor = Color * ColorIntensity;
				MaterialClone.SetColor(IntensityAlphaName, hdrColor);
			}
		}
	}
}
