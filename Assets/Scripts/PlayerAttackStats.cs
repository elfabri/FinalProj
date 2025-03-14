using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackStats", menuName = "Player Attack")]
public class PlayerAttackStats : ScriptableObject
{
    // minimum times to run anims
    [Header("Attacks Timers")]
    [Range(0.1f, 1f)] public float A1MinTime = 0.2f;
    [Range(0.1f, 1f)] public float A2MinTime = 0.2f;
    [Range(0.1f, 1f)] public float HAMinTime = 0.4f;

    // window time after the first attack
    // to cast the next
    [Range(0.1f, 1f)] public float TriggerComboTime = 0.2f;

    // window time after the first attack
    // to hold button and cast heavy attack
    [Range(0.1f, 1f)] public float TriggerHeavyTime = 1f;

    // TODO add damages values and critic probability
}
