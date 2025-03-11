using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimations", menuName = "Player Animations")]
public class PlayerAnimations : ScriptableObject
{
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int Fall = Animator.StringToHash("Fall");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Attack_1 = Animator.StringToHash("Attack1");
    public static readonly int Attack_2 = Animator.StringToHash("Attack2");
}
