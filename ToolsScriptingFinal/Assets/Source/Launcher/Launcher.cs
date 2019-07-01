using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Launcher : MonoBehaviour
{
    public float firingRange;
    public float barrelRotationSpeed;
    [Range(0, 100)] public float velocity = 10;

    // Gameobjects needed to control rotation and motion and projectile 
    public Rigidbody projectile;
    public Transform target;
    public Transform baseRotation;
    public Transform gunBody;
    public Transform barrel;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;
    [HideInInspector] public Vector3 offset = Vector3.forward;

    private bool canFire = false; // If the turret is allowed to fire (target within range)
    private float currentRotationSpeed; // Temp variable to hold rotationspeed

    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;
    }

    void Update()
    {
        // Gun barrel rotation
        barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (canFire)
        {
            Debug.Log("Firing");
            Aim();
            Fire();
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);

            // stop the particle system
            if (muzzelFlash.isPlaying)
                muzzelFlash.Stop();
        }
    }

    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canFire = true;
        }
    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canFire = false;
        }
    }

    [ContextMenu("Fire")]
    public void Fire()
    {
        currentRotationSpeed = barrelRotationSpeed;

        Rigidbody body = Instantiate(projectile, gunBody.transform.TransformPoint(offset), gunBody.transform.rotation);
        body.velocity = gunBody.transform.forward * velocity;

        // start particle system 
        if (!muzzelFlash.isPlaying)
            muzzelFlash.Play();
    }

    [ContextMenu("Aim")]
    public void Aim()
    {
        // Rotate the canons
        Vector3 gunBodyTargetPostition = new Vector3(target.position.x, target.position.y, target.position.z);
        gunBody.transform.LookAt(gunBodyTargetPostition);

        // aim at enemy
        Vector3 baseTargetPostition = new Vector3(target.position.x, this.transform.position.y, target.position.z);
        baseRotation.transform.LookAt(baseTargetPostition);
    }
}