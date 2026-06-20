using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Attack Config")]
public class AttackConfig : ScriptableObject
{
    [Header("Movement")]
    public float approachDuration = 0.3f;
    public float returnDuration = 0.4f;
    public float knockbackDistance = 1f;
    public Ease approachEase = Ease.OutBack;
    public Ease returnEase = Ease.Linear;

    [Header("VFX")]
    public HitEffectConfig hitEffect;
    public HitEffectConfig criticalHitEffect;
}
