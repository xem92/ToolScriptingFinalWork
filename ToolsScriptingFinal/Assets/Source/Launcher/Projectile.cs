using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for the projectile
// We can also make components require another component.
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    // Radius damage
    public float damageRadius = 1;

    [HideInInspector] new public Rigidbody rigidbody;

    // Reset function, available from context menu or first time component is launched.
    void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Better to handle this in the player
        // To do it fast, we handle it here
        Debug.Log("hit and destroy");
        Destroy(this.gameObject);
    }
}