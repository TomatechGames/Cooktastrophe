using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    public BasePlatformInput PlatformInput { get; set; }
    [SerializeField]
    float moveSpeed = 3f;
    [SerializeField]
    Rigidbody rigRef;

    //TODO: teleport system
    private void Update()
    {
        rigRef.velocity = PlatformInput.MoveDir * moveSpeed;
    }
}

public abstract partial class BasePlatformInput : MonoBehaviour
{
    public abstract Vector3 MoveDir { get; }
}
