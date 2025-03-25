using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAnimations", menuName = "Enemy Animations")]
public class EnemyAnimations : ScriptableObject
{
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int CombatIdle = Animator.StringToHash("Combat Idle");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Death = Animator.StringToHash("Death");
    public static readonly int Hit = Animator.StringToHash("Hurt");
}
