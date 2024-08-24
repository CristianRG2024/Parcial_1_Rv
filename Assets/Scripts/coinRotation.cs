using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    void Update()
    {
        transform.Rotate(0, rotationSpeed*Time.deltaTime,0);
    }
}
