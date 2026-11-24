
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class walkerAI : MonoBehaviour
{
    public Transform player;
    public CheckPlayerAgro checkPlayerAgro;
    public float roamingRange = 10f;
    [SerializeField] private float ChaseSpeed;
    [SerializeField] private float RoamSpeed;
    [SerializeField] private bool isTouchingCar = false;

    private NavMeshAgent agent;
    private Animator animator;

    // AMBIENT SOUNDS
    [Header("Ambient Sounds")]
    public AudioSource ambientSource;
    public List<AudioClip> ambientSounds = new List<AudioClip>();

    public float minAmbientInterval = 3f;
    public float maxAmbientInterval = 8f;

    private float ambientTimer;
    private int nextAmbientSound = 0;

    // FOOTSTEP SOUNDS
    [Header("Footstep Sounds")]
    public AudioSource footstepSource;
    public AudioClip footstepSound1;
    public AudioClip footstepSound2;

    public float footstepInterval = 0.5f;

    private float footstepTimer;
    private bool playFirstFootstep = true;

    void Start()
    {
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (checkPlayerAgro == null)
            checkPlayerAgro = GetComponentInChildren<CheckPlayerAgro>();

        ambientTimer = Random.Range(
            minAmbientInterval,
            maxAmbientInterval
        );

        footstepTimer = footstepInterval;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyStop"))
        {
            Debug.Log("Touched Car " + other.name);
            isTouchingCar = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyStop"))
        {
            Debug.Log("Exited car touch");
            isTouchingCar = false;
        }
    }

    void Update()
    {
        // Sound systems run independently of player detection
        HandleAmbientSounds();
        HandleFootsteps();

        if (player == null || checkPlayerAgro == null)
            return;

        if (!agent.isOnNavMesh) // maybe prone to error, remove if no work later
            return;

        // Stop if the enemy is touching the players car 
        if (isTouchingCar)
        {
            agent.isStopped = true;
            agent.ResetPath();

            AttackCar();
            return;
        }
        else
        {
            agent.isStopped = false;
        }

        if (checkPlayerAgro.PlayerInAgroRange && CanSeePlayer()) // Is agitated and running towards player
        {
            agent.speed = ChaseSpeed;
            agent.SetDestination(player.position);
            animator.SetTrigger("PlayRun");
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f) // Random roam, no aggro
        {
            agent.speed = RoamSpeed;

            Vector3 randomPoint = transform.position + Random.insideUnitSphere * roamingRange;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, roamingRange, NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            animator.SetTrigger("PlayWalk");
        }
    }

    public void AttackCar()
    {

    }

    bool CanSeePlayer()
    {
        Vector3 dir = player.position - transform.position;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, dir.normalized, dir.magnitude,
            ~0, QueryTriggerInteraction.Ignore);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // Skip the zombie itself
            if (hit.collider.transform.IsChildOf(transform))
                continue;

            // First thing that isn't the zombie must be the player
            return hit.collider.transform.root.CompareTag("Player")
                || hit.collider.CompareTag("Player");
        }

        return false;
    }

    // AMBIENT SOUND SYSTEM

    void HandleAmbientSounds()
    {
        if (ambientSource == null || ambientSounds.Count == 0)
            return;

        ambientTimer -= Time.deltaTime;

        if (ambientTimer <= 0f)
        {
            PlayAmbientSound();

            ambientTimer = Random.Range(
                minAmbientInterval,
                maxAmbientInterval
            );
        }
    }

    void PlayAmbientSound()
    {
        if (ambientSource == null || ambientSource.isPlaying)
            return;

        if (ambientSounds == null || ambientSounds.Count == 0)
            return;

        AudioClip clip = ambientSounds[
            Random.Range(0, ambientSounds.Count)
        ];

        if (clip == null)
            return;

        ambientSource.PlayOneShot(clip);
    }

    // FOOTSTEP SOUND SYSTEM

    void HandleFootsteps()
    {
        if (footstepSource == null || agent == null)
            return;

        if (agent.velocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep();

                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = footstepInterval;
        }
    }

    void PlayFootstep()
    {
        AudioClip clip = playFirstFootstep
            ? footstepSound1
            : footstepSound2;

        if (clip != null)
            footstepSource.PlayOneShot(clip);

        playFirstFootstep = !playFirstFootstep;
    }
}
