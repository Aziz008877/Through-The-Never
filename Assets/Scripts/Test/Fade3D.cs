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
        // Get the mesh renderer of the object
        MeshRenderer meshRenderer = objToFade.GetComponent<MeshRenderer>();

        // Get the color value of the main material
        Color color = meshRenderer.materials[0].color;

        // While the color's alpha value is above 0
        while (color.a > 0)
        {
            // Reduce the color's alpha value
            color.a -= 0.1f;

            // Apply the modified color to the object's mesh renderer
            meshRenderer.materials[0].color = color;

            // Wait for the frame to update
            yield return new WaitForEndOfFrame();
        }

        // If the material's color's alpha value is less than or equal to 0, end the coroutine
        yield return new WaitUntil(() => meshRenderer.materials[0].color.a <= 0f);
    }
}
