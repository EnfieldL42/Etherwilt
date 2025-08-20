using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private bool isSlidingOffCharacter = false;
    private Coroutine slideOffCharacterCoroutine;
    private bool slideUntilGrounded = false;
    [SerializeField] float characterSlideOffHeadCollisionMaxDistanceCheck = 5;
    [SerializeField] float characterCollisionCheckSphereMultiplier = 1.5f;

    //[SerializeField] bool isTouchingWall = false;
    //Vector3 wallNormal;
    //[SerializeField] float wallSlideSpeed = 2f;
    //[SerializeField] float wallRepelForce = 0.05f;
    //public bool canWallSlide = true;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
        SetGroundedVelocity();
        HandleSlopeSlideCheck();
        //HandleWallRepel();

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

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if you hit a collider when in the air, you will slide until grounded on any steep collider
        if (!character.isGrounded)
        {
            slideUntilGrounded = true;
        }
    }

    protected void HandleGroundCheck()
    {
        if (character.isGrounded)
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position + groundCheckPlusOrMinusPosition, groundCheckSphereRadius, groundLayer, QueryTriggerInteraction.Ignore);

            if (!character.isGrounded)
            {
                OnIsNotGrounded();
            }
        }
        else
        {
            //can be usefull if wanting to change the check sphere radius while not grounded
            character.isGrounded = Physics.CheckSphere(character.transform.position + groundCheckPlusOrMinusPosition, groundCheckSphereRadius, groundLayer, QueryTriggerInteraction.Ignore);

            //if we are jumping or gaining altitute, we are not grounded
            if (yVelocity.y > 0)
            {
                character.isGrounded = false;
                return;
            }

            if (character.isGrounded)
            {
                OnIsGrounded();
            }
        }
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
                return;
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
                character.characterController.Move(slideVelocity * Time.deltaTime);
            }
        }

        if (character.isGrounded)
        {
            if (yVelocity.y <= 0 && !isSliding)
            {
                yVelocity.y = groundedYVelocity;
            }
        }
        else if (!character.isGrounded && !isSlidingOffCharacter)
        {
            //handle sliding off a character
            Collider[] characterColliders = 
                Physics.OverlapSphere(transform.position, groundCheckSphereRadius * characterCollisionCheckSphereMultiplier, WorldUtilityManager.instance.GetCharacterLayers());

            for (int i = 0; i < characterColliders.Length; i++)
            {
                if (characterColliders[i].gameObject.transform.root == character.gameObject.transform.root)
                {
                    continue;
                }

                CharacterController controller = characterColliders[i].GetComponent<CharacterController>();

                if (controller == null)
                {
                    continue;
                }

                if ((controller.collisionFlags & CollisionFlags.CollidedBelow) != 0)
                {
                    isSlidingOffCharacter = true;
                    SlideOffCharacter();
                }
            }

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

    //private void HandleWallRepel()
    //{
    //    if (isSliding && isTouchingWall)
    //    {

    //        Vector3 repel = wallNormal * wallRepelForce;
    //        character.characterController.Move((repel) * Time.deltaTime);
    //    }

    //}


    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    LayerMask enviroLayers = WorldUtilityManager.instance.GetEnviroLayers();

    //    // Check if the hit object's layer is in the environment layers
    //    if (((1 << hit.gameObject.layer) & enviroLayers) != 0)
    //    {
    //        // Check angle to determine if it's a wall
    //        if (Vector3.Angle(hit.normal, Vector3.up) > 65f)
    //        {
    //            isTouchingWall = true;
    //            wallNormal = hit.normal;
    //            Debug.Log("Hit Wall");
    //        }
    //        else
    //        {
    //            isTouchingWall = false;
    //        }
    //    }

    //}

    //CHARACTER SLIDING
    protected virtual void SlideOffCharacter()
    {
        if (slideOffCharacterCoroutine != null)
        {
            StopCoroutine(slideOffCharacterCoroutine);
        }

        slideOffCharacterCoroutine = StartCoroutine(SlideOffCharacterCoroutine());
    }

    protected virtual IEnumerator SlideOffCharacterCoroutine()
    {
        while (!character.isGrounded)
        {
            if (Physics.SphereCast
                (character.transform.position, groundCheckSphereRadius, Vector3.down, out RaycastHit hitInfo, characterSlideOffHeadCollisionMaxDistanceCheck, WorldUtilityManager.instance.GetCharacterLayers()))
            {
                Vector3 characterSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, yVelocity.y, 0), hitInfo.normal);
                yVelocity.y += WorldUtilityManager.instance.slopeSlideForce * Time.deltaTime;
                Vector3 slideVelocity = characterSlideVelocity;

                if (character.characterController.enabled)
                {
                    character.characterController.Move(slideVelocity * Time.deltaTime);
                }

                yield return null;
            }

            yield return null;
        }

        isSlidingOffCharacter = false;

        yield return null;
    }

    //ON IS/NOT GROUNDED
    protected virtual void OnIsGrounded()
    {
        //FALL DAMAGE
        //DETERMINE HOW HIGH YOU FELL BY SAVING A POSITION WHEN YOU LEAVE THE GROUND, AND SAVINGG ONE WHEN YOU LAND
        //COMPARE THE Y LEVEL OF THESE POSITION AND IF YOU ARE IGNORING GRAVITY OR NOT
        //IF Y LEVEL IS TOO GREAT, APPLY DAMAGE

        //APPLY IMPACT/LANDING ANIMATION
        //UPON LEAVING THE GROUND, USE A RAYCAST, OR AGAIN USE THE Y VALUE OF YOUR FALL. DEPENDING ON ITS LEVEL, PLAY A DIFFERENT LANDING ANIMATION

         
    }

    protected virtual void OnIsNotGrounded()
    {

    }
}
