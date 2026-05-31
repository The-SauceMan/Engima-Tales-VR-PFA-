using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip soundClip;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool playOnce = true;
    [SerializeField] private float cooldown = 1f;
    
    private bool hasPlayed = false;
    private float lastPlayTime = 0f;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnce && hasPlayed) return;
            if (Time.time - lastPlayTime < cooldown) return;
            
            PlaySound();
            hasPlayed = true;
            lastPlayTime = Time.time;
        }
    }
    
    void PlaySound()
    {
        if (soundClip == null) return;
        
        GameObject soundObject = new GameObject("TempSound");
        soundObject.transform.position = transform.position;
        
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        
        audioSource.Play();
        Destroy(soundObject, soundClip.length);
    }
}