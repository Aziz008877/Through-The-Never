using UnityEngine;

[CreateAssetMenu(menuName = "Ultimates/Ultimate Definition")]
public class UltimateDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public AspectStars[] Cost;
    public GameObject Behaviour;
}

public enum Aspect { Sol, Ifrit, Phoenix }

[System.Serializable]
public struct AspectStars
{
    public Aspect Aspect;
    public int Stars;
}
