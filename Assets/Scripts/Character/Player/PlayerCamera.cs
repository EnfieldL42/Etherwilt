using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cameraObject;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;


    [Header("Camera Settings")] //camera performance
    private float cameraSmoothSpeed = 1; //the bigger the number the longer it will take to move towards the player
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30; //lowest point you can look down
    [SerializeField] float maximumPivot = 60; //highest point you can look up
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")] //camera values
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;//for camera collision (moves the camera to this position)
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; //values for camera collision (moves camera forwards and backwards)
    private float targetCameraZPosition; //values for camera collision

    [Header("Lock On")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minimumViewableAngle = -50;
    [SerializeField] float maximumViewableAngle = 50;
    [SerializeField] float maximumLockOnDistance = 20;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;

    }

    private void HandleRotation()
    {
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;//rotate left and right
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;//rotate up and down
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot); //stops/clamps it to the max up and down look angle (cant look up and downt too far)

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        //rotate this gameobject left and right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        //rotate this gameobject up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;

        RaycastHit hit;
        //direction for collision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        //check if there is an object in front of direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            //if there is, we get the distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            //then equate the target z position
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        //if the target position is less than the collision radious, we subtract the collision radius(makint it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        //apply the position using lerp over time of 0.2f (can change to make faster or slower)
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    public void HandleLocatingLockOnTarget()
    {
        float shortestDistance = Mathf.Infinity; //used to find closest target to player
        float shortestDistanceOnRightTarget = Mathf.Infinity; //used to find shortest distance on one axis to the right of current target (closest target to the right of the current target)
        float shortestDistanceOnLeftTarget = -Mathf.Infinity; //same but for left

        //to do use a layermask
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for(int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                //Check if they are within field of view
                Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);

                if (lockOnTarget.isDead.Value)//if their dead, check next potential target
                {
                    continue;
                }

                if (lockOnTarget.transform.root == player.transform.root)//if target is us, check next potential target
                {
                    continue;
                }

                if (distanceFromTarget > maximumLockOnDistance)//if target is too far away, check the next potential target
                {
                    continue;
                }    

                if(viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    // TODO ADD LAYER MASK FOR ENVIRONMENT LAYERS ONLY
                    if (Physics.Linecast(player.PlayerCombatManager.lockOnTransform.position, lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        //we hit something, we cannot see our lock on target
                    }
                    else
                    {
                        Debug.Log("locked on");
                    }
                }

            }
        }
    }
}
