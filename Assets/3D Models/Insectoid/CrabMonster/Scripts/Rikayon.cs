using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rikayon : MonoBehaviour {
    public enum AIState {
        Patrol,
        Follow,
        Attack
    };

    public enum CurrentScene
    {
        Menu,
        Mars,
        Moon
    };

    public AIState aiState;
    public float speed = 1.0f;
    public float attackDistance = 2.0f;
    public int attackDamage = 10;
    public float detectionDistance = 10.0f; // Distance to start following the player
    public Animator animator;
    public GameObject patrolPoint; // The point to patrol around
    private GameObject player;
    private float patrolRadius; // Distance to orbit the patrolPoint
    private float patrolAngle = 0f; // The current angle in the patrol orbit

    private AudioSource audioSrc;
    
    public AudioClip stepSound;
    public AudioClip attackSound;

    private CurrentScene currScene;
    
    private Rigidbody rb;

    void Start () {
        animator = GetComponent<Animator>();
        animator.SetBool("Walk_Cycle_1", true);

        player = GameObject.FindGameObjectWithTag("Player");
        
        // Calculate the initial distance between the enemy and the patrol point
        patrolRadius = Vector3.Distance(transform.position, patrolPoint.transform.position);

        aiState = AIState.Patrol;

        audioSrc = GetComponent<AudioSource>();

        //audioSrc.volume = 0.3f;
        rb = GetComponent<Rigidbody>();

        switch (SceneManager.GetActiveScene().name)
        {
            case "Mars Surface":
                currScene = CurrentScene.Mars;
                break;
            case "New Moon Surface":
                currScene = CurrentScene.Moon;
                break;
        }
    }
	
    void Update () {
        if (rb.velocity.magnitude > 0.05 && !audioSrc.isPlaying)
        {
            audioSrc.Play();
        }
        switch (aiState) {
            case AIState.Patrol:
                // If the player is close enough, start following
                if (Vector3.Distance(transform.position, player.transform.position) <= detectionDistance) {
                    aiState = AIState.Follow;
                    animator.SetBool("Walk_Cycle_1", true);
                } else {
                    Patrol();
                }
                break;

            case AIState.Follow:
                // If close enough to the player, start attacking
                if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance) {
                    aiState = AIState.Attack;
                    animator.SetBool("Attack_1", true);
                    animator.SetBool("Walk_Cycle_1", false);
                } else {
                    FollowPlayer();
                }
                break;

            case AIState.Attack:
                transform.LookAt(player.transform.position);
                
                // If the player moves away, start following
                if (Vector3.Distance(transform.position, player.transform.position) > attackDistance + 1) {
                    aiState = AIState.Follow;
                    animator.SetBool("Walk_Cycle_1", true);
                    animator.SetBool("Attack_1", false);
                }
                break;

            default:
                break;
        }
    }

    void FollowPlayer() {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);

        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
	
	void Patrol() {
		// Calculate the position on the patrol circle
		float xPos = patrolPoint.transform.position.x + patrolRadius * Mathf.Cos(patrolAngle);
		float zPos = patrolPoint.transform.position.z + patrolRadius * Mathf.Sin(patrolAngle);
		Vector3 nextPosition = new Vector3(xPos, transform.position.y, zPos);

        // Update the angle for the next frame
		patrolAngle += speed * Time.deltaTime / patrolRadius;

		// Face the next position
		transform.LookAt(nextPosition);

		// Smoothly move towards it
		transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
	}

    public void DealDamage()
    {
        // Check if player is within range
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance + 1)
        {
            // If player is within range, attack hits
            switch (currScene)
            {
                case CurrentScene.Mars:
                    player.GetComponent<ThirdPersonController>().TakeDamage(attackDamage);
                    break;
                case CurrentScene.Moon:
                    player.GetComponent<CharacterControllerCustom>().TakeDamage(attackDamage);
                    break;
            }

            audioSrc.PlayOneShot(attackSound, 1f);
        }
    }



}
