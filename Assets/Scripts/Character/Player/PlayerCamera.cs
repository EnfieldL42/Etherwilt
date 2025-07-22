using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cameraObject;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;


    [Header("Camera Settings")] //camera performance
    [SerializeField] float cameraSmoothSpeed = 1; //the bigger the number the longer it will take to move towards the player
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float leftAndRightRotationSpeed = 220;

    [Header("Gamepad and Mouse Sensitivity")]
    [SerializeField] float gamepadUDSensitivity = 220;
    [SerializeField] float gamepadLRSensitivity = 220;
    [SerializeField] float mouseUDSensitivity = 20;
    [SerializeField] float mouseLRSensitivity = 20;

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
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] float setCameraHeightSpeed = 0.5f;
    [SerializeField] float unlockedCamereHeight = 0.5f;
    [SerializeField] float lockedCameraHeight = 1.0f;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    private Coroutine cameraLockOnHeightCoroutine;


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
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed);
        transform.position = targetCameraPosition;

    }

    private void HandleRotation()
    {
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            //main player camera object, this rotates this gameobject
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            //this rotates the pivot object
            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            //Save our rotation to our look angles, so we unlock it doent snap too far away
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        else
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
        float shortestDistanceOfRightTarget = Mathf.Infinity; //used to find shortest distance on one axis to the right of current target (closest target to the right of the current target)
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; //same but for left

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


                if(viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)//lastly if target is outside field of view or blocked by envoronment, check next potential
                {
                    RaycastHit hit;

                    // TODO ADD LAYER MASK FOR ENVIRONMENT LAYERS ONLY
                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        //we hit something, we cannot see our lock on target
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);//add to potential lock on targets
                    }
                }

            }

        }

        for(int k = 0; k < availableTargets.Count; k++)//sort through potential targets to see which one is the closest
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);
                Vector3 lockTargetsDirection = availableTargets[k].transform.position - player.transform.position;

                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                //if already locked on, look for nearest left or right target
                if(player.playerNetworkManager.isLockedOn.Value)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[k] == player.playerCombatManager.currentTarget)
                    {
                        continue;
                    }
                    //check for left targets
                    if(relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    }
                    //check for right targets
                    else if(relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[k];
                    }
                }

            }
            else
            {
                ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if(cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public void SwitchToMouseSensitivity()
    {
        upAndDownRotationSpeed = mouseUDSensitivity;
        leftAndRightRotationSpeed = mouseLRSensitivity;

    }

    public void SwitchToGamePadSensitivity()
    {
        upAndDownRotationSpeed = gamepadUDSensitivity;
        leftAndRightRotationSpeed = gamepadLRSensitivity;
    }


    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;

        }

        ClearLockOnTargets();
        HandleLocatingLockOnTarget();

        if(nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }

        yield return null;
    }

    public IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCamereHeight);

        while(timer < duration)
        {
            timer += Time.deltaTime;

            if(player != null)
            {
                //if(player.playerCombatManager.currentTarget != null)
                //{
                //    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                //    //cameraPivotTransform.transform.localRotation = Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                //}
                //else
                //{
                //    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                //}
                if (player.playerCombatManager.currentTarget != null)
                {
                    // ONLY adjust height, NOT rotation
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                        cameraPivotTransform.transform.localPosition,
                        newLockedCameraHeight,
                        ref velocity,
                        setCameraHeightSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                        cameraPivotTransform.transform.localPosition,
                        newUnlockedCameraHeight,
                        ref velocity,
                        setCameraHeightSpeed);
                }

            }

            yield return null;

        }


        if (player != null)
        {
            if (player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                //cameraPivotTransform.transform.localRotation = Quaternion.Euler(0,0,0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }

}
