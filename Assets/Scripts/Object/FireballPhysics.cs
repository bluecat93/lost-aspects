using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballPhysics : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 5f;
    public void Setup(Vector3 shootDirection)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(shootDirection * projectileSpeed, ForceMode.Impulse);
    }
}
