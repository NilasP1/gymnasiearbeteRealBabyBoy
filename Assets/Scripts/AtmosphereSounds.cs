
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereSounds : MonoBehaviour
{
    [System.Serializable]
    public class SoundGroup
    {
        public string groupName;
        public AudioSource audioSource;
        public List<AudioClip> sounds = new List<AudioClip>();
    }

    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    private void Start()
    {
        foreach (SoundGroup group in soundGroups)
        {
            if (group.audioSource != null && group.sounds.Count > 0)
            {
                StartCoroutine(PlaySoundGroup(group));
            }
        }
    }

    IEnumerator PlaySoundGroup(SoundGroup group)
    {
        while (true)
        {
            AudioClip clip = group.sounds[
                Random.Range(0, group.sounds.Count)
            ];

            if (clip != null)
            {
                group.audioSource.clip = clip;
                group.audioSource.Play();

                yield return new WaitForSeconds(clip.length);
            }
            else
            {
                yield return null;
            }
        }
    }
}
