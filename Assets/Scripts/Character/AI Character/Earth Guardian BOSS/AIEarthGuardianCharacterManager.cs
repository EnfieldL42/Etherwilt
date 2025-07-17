using UnityEngine;

public class AIEarthGuardianCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIEarthGuardianSoundFXManager earthGuardianSoundFXManager;
    [HideInInspector] public AIEarthGuardianTailCombatManager tailCombatManager;

    protected override void Awake()
    {
        base.Awake();
        earthGuardianSoundFXManager = GetComponent<AIEarthGuardianSoundFXManager>();
        tailCombatManager = GetComponent<AIEarthGuardianTailCombatManager>();
    }
}
