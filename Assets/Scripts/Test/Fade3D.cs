using System;
using System.Collections;
using UnityEngine;

public class Fade3D : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FadeOutObject(gameObject));
    }

    public IEnumerator FadeOutObject(GameObject objToFade)
    {
        MeshRenderer meshRenderer = objToFade.GetComponent<MeshRenderer>();
        Color color = meshRenderer.materials[0].color;
        while (color.a > 0)
        {
            color.a -= 0.1f;
            meshRenderer.materials[0].color = color;
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitUntil(() => meshRenderer.materials[0].color.a <= 0f);
    }
}
