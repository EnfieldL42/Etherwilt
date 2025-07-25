using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;


    [Header("Ground & Jumping")]
    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 1;
    [SerializeField] Vector3 groundCheckPlusOrMinusPosition;
    [SerializeField] protected Vector3 yVelocity; //force which character is pulled up or down
    [SerializeField] protected float groundedYVelocity = -20; //force which the character s sicking to the ground when grounded
    [SerializeField] protected float fallStartYVelocity = -5; //force which the character begins to fall when they become ungrounded(will increase the longer they fall)
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;

    [Header("Flags")]
    public bool isRolling = false;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();

        if(character.isGrounded)
        {
            //if we are not trying to jump or go upwards
            if(yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            //if we are not jumping, and our falling velocitty has nbot been set
            if(!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer += Time.deltaTime;

            character.animator.SetFloat("InAirTimer", inAirTimer);

            yVelocity.y += gravityForce * Time.deltaTime;
        }

        character.characterController.Move(yVelocity * Time.deltaTime);

    }

    protected void HandleGroundCheck()
    {
        character.isGrounded = Physics.CheckSphere(character.transform.position + groundCheckPlusOrMinusPosition, groundCheckSphereRadius, groundLayer);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(character.transform.position + groundCheckPlusOrMinusPosition, groundCheckSphereRadius);
    }

    public void EnableCanRotate()
    {
       character.canRotate = true;
    }

    public void DisableCanRotate()
    {
        character.canRotate = false;
    }



}
