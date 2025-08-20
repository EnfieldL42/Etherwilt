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

    [Header("Slope Sliding")]
    [SerializeField] float slopeSlideStartPositionYOffset = 1;
    [SerializeField] float slopeSlideSphereCastMaxDistance = 2;
    private Vector3 slopeSlideVelocity;
    [SerializeField] float slopeSlideSpeed = -11;
    [SerializeField] float slopeSlideSpeedMultiplier = 3;
    [SerializeField] float slipperySurfaceMaxAngle = 15;
    private bool isSliding = false;
    private bool slideUntilGrounded = true;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
        SetGroundedVelocity();
        HandleSlopeSlideCheck();

        if (character.isGrounded)
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


    //SLOPES AND SLIDING
    private void HandleSlopeSlideCheck()
    {
        if(slopeSlideVelocity == Vector3.zero)
        {
            isSliding = false;
        }

        if (!character.isGrounded && slideUntilGrounded)
        {
            SetSlopeSlideVelocity(WorldUtilityManager.instance.GetEnviroLayers());
            return;
        }

        if (!character.isGrounded)
        {
            return;
        }

        SetSlopeSlideVelocity(WorldUtilityManager.instance.GetSlipperyEnviroLayers());
    }

    public void SetSlopeSlideVelocity(LayerMask layers)
    {
        Vector3 startPosition = new Vector3(transform.position.x, transform.position.y + slopeSlideStartPositionYOffset, transform.position.z);
        //use a sphere cast to determine the angle of what is bellow us, and if the angle is too greate, apply slope slide velocity
        if (Physics.SphereCast
            (startPosition, groundCheckSphereRadius, Vector3.down, out RaycastHit hitInfo, slopeSlideSphereCastMaxDistance, layers))
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, slopeSlideSpeed, 0), hitInfo.normal);

            if (angle >= slipperySurfaceMaxAngle)
            {
                slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, slopeSlideSpeed,0), hitInfo.normal);
            }
        }
        else
        {
            slopeSlideVelocity = Vector3.zero;
        }
    
        if (isSliding)
        {
            slopeSlideVelocity -= slopeSlideVelocity * Time.deltaTime * slopeSlideSpeedMultiplier;

            if (slopeSlideVelocity.magnitude > 1)
            {
                return;
            }
        }

        slopeSlideVelocity = Vector3.zero;
    }

    private void SetGroundedVelocity()
    {
        if (slopeSlideVelocity != Vector3.zero)
        {
            if (character.characterNetworkManager.isJumping.Value && yVelocity.y > 0)
            {
                isSliding = false;
            }
            else
            {
                isSliding = true;
            }
        }

        if (isSliding)
        {
            yVelocity.y += WorldUtilityManager.instance.slopeSlideForce * Time.deltaTime;
            Vector3 slideVelocity = slopeSlideVelocity;

            if (character.characterController.enabled)
            {
                character.characterController.Move(slideVelocity* Time.deltaTime);
            }
        }

        if (character.isGrounded)
        {
            if (yVelocity.y <= 0 && !isSliding)
            {
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            //handle sliding off a character
        }

        if (!character.characterController.enabled)
        {
            return;
        }

        //desync prevention measure
        if (!character.IsOwner)
        {
            float distance = Vector3.Distance(transform.position, character.characterNetworkManager.networkPosition.Value);

            if (distance > 2.5f)
            {
                yVelocity = Vector3.zero;
                character.transform.position = character.characterNetworkManager.networkPosition.Value;
            }
        }
    }
}
