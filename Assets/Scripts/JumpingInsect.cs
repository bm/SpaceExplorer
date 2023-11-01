using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingInsect : MonoBehaviour
{
    public float jumpForce = 5f;
    public float torqueForce = 100f; // Adjust the torque force as needed
    public float minGroundTime = 0.5f; // Minimum time to stay on the ground
    public float maxGroundTime = 3f; // Maximum time to stay on the ground

    private Rigidbody rb;
    private bool isGrounded = false;
    private bool canJump = true;

    private Coroutine jumpCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Jump control
        if (isGrounded && canJump)
        {
            if (jumpCoroutine != null)
                StopCoroutine(jumpCoroutine);

            jumpCoroutine = StartCoroutine(Jump());
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the capsule is on the ground
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    IEnumerator Jump()
    {
        isGrounded = false;
        canJump = false;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        float randomTorqueX = Random.Range(-torqueForce, torqueForce);
        float randomTorqueY = Random.Range(-torqueForce, torqueForce);
        float randomTorqueZ = Random.Range(-torqueForce, torqueForce);
        rb.AddTorque(new Vector3(randomTorqueX, randomTorqueY, randomTorqueZ), ForceMode.Impulse); // Apply torque

        yield return new WaitForSeconds(Random.Range(minGroundTime, maxGroundTime));

        isGrounded = true;
        canJump = true;
        jumpCoroutine = null;
    }
}