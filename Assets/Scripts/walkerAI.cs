
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class walkerAI : MonoBehaviour
{
    public Transform player;
    public CheckPlayerAgro checkPlayerAgro;
    public float roamingRange = 10f;

    [SerializeField] private float ChaseSpeed;
    [SerializeField] private float RoamSpeed;

    [Header("Ambient Sounds")]
    public AudioSource ambientSource;
    public AudioClip ambientSound1;
    public AudioClip ambientSound2;

    public float minAmbientInterval = 3f;
    public float maxAmbientInterval = 8f;

    private float ambientTimer;
    private bool playFirstAmbient = true;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public float footstepInterval = 0.5f;

    private float footstepTimer;
    private bool playFirstFootstep = true;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Randomize first ambient sound time
        ambientTimer = Random.Range(1f, maxAmbientInterval);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (checkPlayerAgro == null)
            checkPlayerAgro = GetComponentInChildren<CheckPlayerAgro>();
    }

    void Update()
    {
        HandleAmbientSounds();

        if (player == null || checkPlayerAgro == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        if (checkPlayerAgro.PlayerInAgroRange && CanSeePlayer())
        {
            agent.speed = ChaseSpeed;
            agent.SetDestination(player.position);
            animator.SetTrigger("PlayRun");
        }
        else if (!agent.pathPending &&
                 agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            agent.speed = RoamSpeed;

            Vector3 randomPoint =
                transform.position + Random.insideUnitSphere * roamingRange;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit,
                roamingRange, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            animator.SetTrigger("PlayWalk");
        }

        HandleFootsteps();
    }

    // AMBIENT SOUNDS

    void HandleAmbientSounds()
    {
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
        if (ambientSource == null)
            return;

        if (ambientSource.isPlaying)
            return;

        AudioClip clip = playFirstAmbient
            ? ambientSound1
            : ambientSound2;

        if (clip == null)
            return;

        ambientSource.clip = clip;

        if (ZombieSoundManager.Instance != null)
        {
            bool accepted =
                ZombieSoundManager.Instance.RequestSound(ambientSource);

            if (accepted)
                playFirstAmbient = !playFirstAmbient;
        }
        else
        {
            ambientSource.Play();
            playFirstAmbient = !playFirstAmbient;
        }
    }


    void OnDisable()
    {
        if (ZombieSoundManager.Instance != null &&
            ambientSource != null)
        {
            ZombieSoundManager.Instance.ReleaseSound(ambientSource);
        }
    }


    // FOOTSTEPS
    void HandleFootsteps()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepSource == null)
            return;

        AudioClip clip = playFirstFootstep ? footstep1 : footstep2;

        if (clip != null)
        {
            footstepSource.PlayOneShot(clip);
        }

        playFirstFootstep = !playFirstFootstep;
    }

    bool CanSeePlayer()
    {
        Vector3 dir = player.position - transform.position;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position,
            dir.normalized,
            dir.magnitude,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        System.Array.Sort(hits,
            (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform))
                continue;

            return hit.collider.transform.root.CompareTag("Player")
                || hit.collider.CompareTag("Player");
        }

        return false;
    }
}
