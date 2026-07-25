using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceWrapper : MonoBehaviour
{
    public AudioSource Source { get; private set; }

    private void Awake()
    {
        Source = GetComponent<AudioSource>();
    }
}
