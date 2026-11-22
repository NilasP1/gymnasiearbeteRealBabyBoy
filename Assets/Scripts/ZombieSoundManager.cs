
using UnityEngine;
using System.Collections.Generic;

public class ZombieSoundManager : MonoBehaviour
{
    public static ZombieSoundManager Instance;

    [Header("Settings")]
    [Range(1, 2)]
    public int maxPlayingSounds = 2;

    private List<AudioSource> playingSources = new List<AudioSource>();
    private List<AudioSource> waitingSources = new List<AudioSource>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool RequestSound(AudioSource source)
    {
        if (source == null)
            return false;

        CleanUp();

        if (playingSources.Contains(source) ||
            waitingSources.Contains(source))
            return false;

        if (playingSources.Count < maxPlayingSounds)
        {
            playingSources.Add(source);
            source.Play();
        }
        else
        {
            waitingSources.Add(source);
        }

        return true;
    }

    public void ReleaseSound(AudioSource source)
    {
        if (source == null)
            return;

        playingSources.Remove(source);
        waitingSources.Remove(source);

        source.Stop();

        StartWaitingSounds();
    }

    void Update()
    {
        CleanUp();
        StartWaitingSounds();
    }

    void CleanUp()
    {
        playingSources.RemoveAll(source =>
            source == null ||
            !source.isPlaying ||
            !source.gameObject.activeInHierarchy
        );

        waitingSources.RemoveAll(source =>
            source == null ||
            !source.gameObject.activeInHierarchy
        );
    }

    void StartWaitingSounds()
    {
        while (playingSources.Count < maxPlayingSounds &&
               waitingSources.Count > 0)
        {
            AudioSource next = waitingSources[0];
            waitingSources.RemoveAt(0);

            if (next != null && next.gameObject.activeInHierarchy)
            {
                playingSources.Add(next);
                next.Play();
            }
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
