using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;

public class RadioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject radioUI;
    [SerializeField] private TMP_Dropdown musicDropdown;
    [SerializeField] private AudioSource radioAudioSource;
    
    [Header("Music Tracks")]
    [SerializeField] private AudioClip[] musicTracks;
    
    [Header("Volume Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume = 0.7f;
    
    [Header("UI Position")]
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 0.5f, 0);
    
    private XRGrabInteractable grabInteractable;
    private bool isUIActive = false;
    private int currentTrackIndex = -1;
    
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        // Setup grab events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(_ => ShowUI());
            grabInteractable.selectExited.AddListener(_ => HideUI());
        }
        
        // Setup audio source
        if (radioAudioSource == null)
            radioAudioSource = GetComponent<AudioSource>();
        
        if (radioAudioSource != null)
        {
            radioAudioSource.loop = true;
            radioAudioSource.volume = defaultVolume;
        }
        
        // Setup dropdown with "Off" option
        if (musicDropdown != null)
        {
            musicDropdown.onValueChanged.AddListener(OnMusicSelected);
            
            // Clear and populate dropdown
            musicDropdown.ClearOptions();
            
            // Add "Off" option first
            musicDropdown.options.Add(new TMP_Dropdown.OptionData("Off"));
            
            // Add music tracks
            foreach (AudioClip track in musicTracks)
            {
                musicDropdown.options.Add(new TMP_Dropdown.OptionData(track.name));
            }
            
            musicDropdown.RefreshShownValue();
            
            // CHANGE THIS: Set to first song instead of Off
            musicDropdown.value = 1; // 1 = first song (since 0 is "Off")
            
            // Auto-play first song
            if (musicTracks.Length > 0 && musicTracks[0] != null)
            {
                radioAudioSource.clip = musicTracks[0];
                radioAudioSource.Play();
                currentTrackIndex = 0;
                Debug.Log($"Radio playing: {musicTracks[0].name}");
            }
        }
        
        // Setup volume slider
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = defaultVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        // Hide UI at start
        if (radioUI != null)
            radioUI.SetActive(false);
    }
    
    void Update()
    {
        if (isUIActive && radioUI != null)
        {
            // Position UI above radio and face camera
            radioUI.transform.position = transform.position + uiOffset;
            
            if (Camera.main != null)
            {
                radioUI.transform.LookAt(radioUI.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                        Camera.main.transform.rotation * Vector3.up);
            }
        }
    }
    
    void ShowUI()
    {
        radioUI.SetActive(true);
        isUIActive = true;
    }
    
    void HideUI()
    {
        radioUI.SetActive(false);
        isUIActive = false;
    }
    
    void OnMusicSelected(int index)
    {
        currentTrackIndex = index - 1;
        
        if (radioAudioSource == null)
        {
            Debug.LogError("Audio Source is missing on radio!");
            return;
        }
        
        // Stop current music
        radioAudioSource.Stop();
        
        // Check if "Off" is selected (index 0)
        if (index == 0)
        {
            Debug.Log("Music turned off");
            return;
        }
        
        // Play selected music
        int trackIndex = index - 1;
        if (trackIndex >= 0 && trackIndex < musicTracks.Length && musicTracks[trackIndex] != null)
        {
            radioAudioSource.clip = musicTracks[trackIndex];
            radioAudioSource.Play();
            Debug.Log($"Now playing: {musicTracks[trackIndex].name}");
        }
        else
        {
            Debug.LogError($"Music track at index {trackIndex} is missing!");
        }
    }
    
    void OnVolumeChanged(float volume)
    {
        if (radioAudioSource != null)
        {
            radioAudioSource.volume = volume;
            Debug.Log($"Volume set to: {volume}");
        }
    }
    
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(_ => ShowUI());
            grabInteractable.selectExited.RemoveListener(_ => HideUI());
        }
        
        if (musicDropdown != null)
            musicDropdown.onValueChanged.RemoveListener(OnMusicSelected);
        
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}