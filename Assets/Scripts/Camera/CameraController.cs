using System;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Components"), Space]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private CinemachineVirtualCamera normalCM;
    [SerializeField] private CinemachineVirtualCamera lockOnCM;
    [SerializeField] private Camera mainCam;
    private HUDManager _hudManager;

    [Header("FollowSetting"), Space] 
    [SerializeField] private LayerMask layerToSearch;
    [SerializeField] private float searchRadiusForTarget = 5f;
    private Transform _lockOnTargetTransform;

    private void Start()
    {
        _hudManager = HUDManager.Instance;
    }

    private void OnEnable()
    {
        playerData.inputManager.OnLockInput += HandleLockOnInput;
    }

    private void OnDisable()
    {
        playerData.inputManager.OnLockInput -= HandleLockOnInput;
    }

    private void HandleLockOnInput()
    {
        ChangeCinemachinePriority();
    }

    private void ChangeCinemachinePriority()
    {
        //Switch Between Lock And Normal Cinemachine
        if (playerData.isLockedOn)
        {
            normalCM.Priority = 1;
            lockOnCM.Priority = 0;
            _lockOnTargetTransform = null;

            _hudManager.SetLockOnUIState(false);
            
            playerData.isLockedOn = false;
        }
        else
        {
            FindNearestTarget();
            if(!_lockOnTargetTransform) return;

            lockOnCM.Priority = 1;
            normalCM.Priority = 0;

            lockOnCM.LookAt = _lockOnTargetTransform;

            _hudManager.SetLockOnUIState(true);

            playerData.isLockedOn = true;
        }
    }

    private void FindNearestTarget()
    {
        //This is Just For Now, Later We Can Change And Improve This Part
        //Find All Targets In Range
        var colliders = Physics.OverlapSphere(transform.position,searchRadiusForTarget,layerToSearch).ToList();
        
        if(colliders.Count == 0) return;
        //Find All Visible Targets
        foreach (var collider in colliders.ToList())
        {
            if(IsInCameraView(collider.transform.position) && !IsSomethingInBetween(playerData.transform.position,collider)) continue;
            colliders.Remove(collider);
        }
        
        if(colliders.Count == 0) return;
        //Find Nearest Visible In-range Target 
        _lockOnTargetTransform = colliders.OrderBy(x => Vector3.SqrMagnitude(x.transform.position - transform.position)).FirstOrDefault().transform;
    }

    private bool IsInCameraView(Vector3 position)
    {
        //renderer.isVisible() dont work properly because we have bug in low priority cinemachine 
        var viewportPoint = mainCam.WorldToViewportPoint(position);
        return viewportPoint.x is >=0 and <=1 && viewportPoint.y is >=0 and <=1;
    }

    private bool IsSomethingInBetween(Vector3 origin,Collider collider)
    {
        var rayDir = collider.transform.position - origin;
        Physics.Raycast(origin, rayDir, out var hitInfo);
        return hitInfo.collider != collider;
    }
}
