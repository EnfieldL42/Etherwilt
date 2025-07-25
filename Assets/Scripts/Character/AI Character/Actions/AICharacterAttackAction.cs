using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Actions/Attack")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Action")]
    public AICharacterAttackAction comboAction; //combo action of this attack action

    [Header("Action Values")]
    public int attackWeight = 50;
    [SerializeField] AttackType attackType;
    //attack can be repeated
    public float actionRecoveryTime = 1.5f; //the time before the ai can make another attack after this one
    public float minimumAttackAngle = -35;
    public float maximumAttackAngle = 35;
    public float minimumAttackDistance = 0;
    public float maximumAttackDistance = 2;

    public void AttemptToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayTargetActionAnimation(attackAnimation, true);
    }
}
