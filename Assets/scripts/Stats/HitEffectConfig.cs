using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Hit Effect Config")]
public class HitEffectConfig : ScriptableObject
{
    public GameObject prefabOverride;
    public Vector3 scale = Vector3.one;
    public Vector3 rotation = Vector3.zero;
    public Vector3 positionOffset = Vector3.zero;
}
