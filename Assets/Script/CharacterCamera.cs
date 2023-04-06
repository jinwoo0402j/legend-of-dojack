using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    public float FollowSpeed = 3.0f;
    public Transform TargetPivot;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPivot.position, FollowSpeed * Time.deltaTime);
    }
}
