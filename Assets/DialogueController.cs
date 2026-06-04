using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueLine
{
    public GameObject speaker; // Drag the speaker GameObject here (Rabbit, Turtle, etc.)
    public string speakerName; // Name to display (Rabbit, Turtle, etc.)
    public string text; // The dialogue text
    public float displayTime; // Time when this line should appear (in seconds from start)
    public float duration = 3f; // How long to show the text
}

public class DialogueController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI speakerNameText; // Text for speaker name
    [SerializeField] private TextMeshProUGUI dialogueText; // Text for dialogue
    
    [Header("Dialogue List")]
    [SerializeField] private List<DialogueLine> dialogueLines;
    
    [Header("Settings")]
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private Vector3 speakerOffset = new Vector3(0, 1.5f, 0);
    
    [Header("Timeline Hiding")]
    [SerializeField] private PlayableDirector timeline; // Timeline reference
    [SerializeField] private float hideDialogueAtTime = 10f; // Time when dialogue UI should disappear
    [SerializeField] private bool stopDialogueWhenHidden = true; // Stop processing dialogue when hidden
    
    private bool isDialoguePlaying = false;
    private float dialogueStartTime;
    private GameObject currentSpeaker;
    private bool hasBeenHidden = false;
    private Coroutine dialogueCoroutine;
    
    void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
        
        if (timeline == null)
            timeline = FindObjectOfType<PlayableDirector>();
        
        StartCoroutine(StartDialogueAfterDelay());
    }
    
    void Update()
    {
        if (!isDialoguePlaying) return;
        
        // Check if timeline has reached hide time
        if (!hasBeenHidden && timeline != null && timeline.time >= hideDialogueAtTime)
        {
            HideDialoguePermanently();
            return;
        }
        
        if (dialogueUI != null && dialogueUI.activeSelf && currentSpeaker != null)
        {
            // Position UI above the speaker's head
            dialogueUI.transform.position = currentSpeaker.transform.position + speakerOffset;
            
            // Face the camera (billboard)
            FaceCamera(dialogueUI);
        }
    }
    
    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        
        // Don't start dialogue if already hidden by timeline
        if (!hasBeenHidden)
            StartDialogue();
    }
    
    void StartDialogue()
    {
        if (hasBeenHidden) return;
        
        isDialoguePlaying = true;
        dialogueStartTime = Time.time;
        
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);
        
        dialogueCoroutine = StartCoroutine(ProcessDialogueLines());
    }
    
    IEnumerator ProcessDialogueLines()
    {
        foreach (DialogueLine line in dialogueLines)
        {
            // Stop processing if hidden
            if (hasBeenHidden) yield break;
            
            float waitTime = line.displayTime - (Time.time - dialogueStartTime);
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            
            // Check again after waiting
            if (hasBeenHidden) yield break;
            
            currentSpeaker = line.speaker;
            ShowDialogue(line);
            
            yield return new WaitForSeconds(line.duration);
            
            // Check again before hiding
            if (!hasBeenHidden)
                HideDialogue();
        }
        
        isDialoguePlaying = false;
    }
    
    void ShowDialogue(DialogueLine line)
    {
        if (dialogueUI != null && !hasBeenHidden)
        {
            // Set speaker name text
            if (speakerNameText != null)
                speakerNameText.text = line.speakerName;
            
            // Set dialogue text
            if (dialogueText != null)
                dialogueText.text = line.text;
            
            // Position above speaker
            if (currentSpeaker != null)
            {
                dialogueUI.transform.position = currentSpeaker.transform.position + speakerOffset;
            }
            
            dialogueUI.SetActive(true);
            Debug.Log($"{line.speakerName}: {line.text}");
        }
    }
    
    void HideDialogue()
    {
        if (dialogueUI != null && !hasBeenHidden)
            dialogueUI.SetActive(false);
    }
    
    void HideDialoguePermanently()
    {
        hasBeenHidden = true;
        isDialoguePlaying = false;
        
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
        
        if (stopDialogueWhenHidden && dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);
        
        Debug.Log($"Dialogue UI hidden permanently at timeline time: {timeline?.time:F2}s / {hideDialogueAtTime}s");
    }
    
    void FaceCamera(GameObject ui)
    {
        if (Camera.main != null)
        {
            ui.transform.LookAt(ui.transform.position + Camera.main.transform.rotation * Vector3.forward,
                               Camera.main.transform.rotation * Vector3.up);
        }
    }
    
    // Public methods to control from other scripts
    public void StartDialogueFromScript()
    {
        if (!hasBeenHidden)
            StartDialogue();
    }
    
    public void SkipDialogue()
    {
        if (!hasBeenHidden)
        {
            if (dialogueCoroutine != null)
                StopCoroutine(dialogueCoroutine);
            isDialoguePlaying = false;
            HideDialogue();
        }
    }
    
    // Manually hide dialogue from other scripts
    public void ForceHideDialogue()
    {
        HideDialoguePermanently();
    }
    
    // Reset for replaying (optional)
    public void ResetDialogue()
    {
        hasBeenHidden = false;
        isDialoguePlaying = false;
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);
        
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
        
        StartDialogue();
    }
}