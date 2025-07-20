using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [Header("Character Name")]
    public string characterName = "";

    [HideInInspector] public AICharacterCombatManager aICharacterCombatManager;
    [HideInInspector] public AICharacterNetworkManager aICharacterNetworkManager;
    [HideInInspector] public AICharacterLocomotionManager aICharacterLocomotionManager;

    [Header("Navmesh Agent")]
    public NavMeshAgent navmeshAgent;

    [Header("Curent State")]
    [SerializeField] protected AIState currentState;

    [Header("States")]
    public IdleState idle;
    public PursueTargetState pursueState;
    public CombatStanceState combatState;
    public AttackState attack;

    //[SerializeField] private float isPerformingActionTimer = 0f;
    //private float isPerformingActionMaxTime = 10f;

    [Header("Stats")]
    [SerializeField] int maxHealth = 100;


    protected override void Awake()
    {
        base.Awake();

        aICharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aICharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
        aICharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();

        navmeshAgent = GetComponentInChildren<NavMeshAgent>();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            idle = Instantiate(idle);
            pursueState = Instantiate(pursueState);
            combatState = Instantiate(combatState);
            attack = Instantiate(attack);
            currentState = idle;

            aICharacterNetworkManager.currentHealth.Value = maxHealth;
            aICharacterNetworkManager.maxHealth.Value = maxHealth;
        }

        aICharacterNetworkManager.currentHealth.OnValueChanged += aICharacterNetworkManager.CheckHP;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        aICharacterNetworkManager.currentHealth.OnValueChanged -= aICharacterNetworkManager.CheckHP;
    }

    protected override void Start()
    {
        base.Start();
        ResetNavmeshPostion();
    }

    protected override void Update()
    {
        base.Update();


        aICharacterCombatManager.HandleActionRecovery(this);

        //if (isPerformingAction)
        //{
        //    CheckForPerformingActionBug();
        //}
        //else
        //{
        //    isPerformingActionTimer = 0f;
        //}
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);
        //AIState nextState = currentState != null ? currentState.Tick(this) : null;

        if (nextState != null)
        {
            currentState = nextState;
        }

        navmeshAgent.transform.localPosition = Vector3.zero;
        navmeshAgent.transform.localRotation = Quaternion.identity;

        if (aICharacterCombatManager.currentTarget != null)
        {
            aICharacterCombatManager.targetsDirection = aICharacterCombatManager.currentTarget.transform.position - transform.position;
            aICharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aICharacterCombatManager.targetsDirection);
            aICharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aICharacterCombatManager.currentTarget.transform.position);
        }


        if (navmeshAgent.enabled)
        {
            Vector3 agentDestination = navmeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navmeshAgent.stoppingDistance)
            {
                aICharacterNetworkManager.isMoving.Value = true;
            }
            else
            {
                aICharacterNetworkManager.isMoving.Value = false;

            }
        }
        else
        {
            aICharacterNetworkManager.isMoving.Value = false;

        }
    }

    private void ResetNavmeshPostion()
    {

        navmeshAgent.Warp(this.transform.position);

        navmeshAgent.enabled = true;
    }

    //private void CheckForPerformingActionBug()
    //{
    //    isPerformingActionTimer += Time.deltaTime;

    //    if(isPerformingActionTimer >= isPerformingActionMaxTime)
    //    {
    //        isPerformingAction = false;
    //        isPerformingActionTimer = 0f;
    //    }
    //}

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            //reset any flags

            //if not grounded play falling death anim

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);

            }

            //play death sfx

            yield return new WaitForSeconds(5);

            //award players with runers

            gameObject.SetActive(false);

        }
    }


}
