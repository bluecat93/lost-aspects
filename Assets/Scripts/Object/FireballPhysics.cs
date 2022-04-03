using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballPhysics : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileLifeSpan = 5f;
    private Vector3 projectileDirection;
    //private Rigidbody rigidbody;

    void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
    }
    public void Setup(Transform target)
    {

        transform.LookAt(target);
        projectileDirection = transform.forward;
        Destroy(gameObject, projectileLifeSpan);
    }

    void update()
    {
        transform.position += projectileDirection * Time.deltaTime * projectileSpeed;
        //rigidbody.AddForce(transform.forward * projectileSpeed, ForceMode.Impulse);
    }
}
