using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Cinemachine;
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : Singleton<CameraController>
{
    public CinemachineVirtualCamera cinemachineVirtualCamera { get; set; }
    public override void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        base.Awake();
    }

    public void SetTarget(Transform target)
    {
        cinemachineVirtualCamera.LookAt = target;
        cinemachineVirtualCamera.Follow = target;
    }

    public void ForceMove(Vector3 position)
    {
        cinemachineVirtualCamera.ForceCameraPosition(position, cinemachineVirtualCamera.transform.rotation);
    }
}
