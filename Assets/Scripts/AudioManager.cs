using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject placeSoundStore;

    private AudioSource[] _placeSounds;

    public void Start()
    {
        _placeSounds = placeSoundStore.GetComponents<AudioSource>();
    }

    public void PlayPlace()
    {
        var placeSound = RandomSound(_placeSounds);
        placeSound.Play();
    }

    private static AudioSource RandomSound(IReadOnlyList<AudioSource> sounds)
    {
        var random = new System.Random();
        var index = random.Next(sounds.Count);

        return sounds[index];
    }
}