using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraFollow : MonoBehaviour {

    public Transform targetTransform;

    public Vector3 offset = new Vector3(0,0,-11);

    public float smoothSpeed = 0.125f;

    private void LateUpdate() {
        Vector3 desiredVector, smoothVector;
        desiredVector = targetTransform.position + offset;
        smoothVector = Vector3.Lerp(transform.position, desiredVector, smoothSpeed);
        transform.position = smoothVector;
    }
}
