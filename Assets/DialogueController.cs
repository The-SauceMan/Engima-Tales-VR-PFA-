using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
    
    private bool isDialoguePlaying = false;
    private float dialogueStartTime;
    private GameObject currentSpeaker;
    
    void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
        
        StartCoroutine(StartDialogueAfterDelay());
    }
    
    void Update()
    {
        if (!isDialoguePlaying) return;
        
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
        StartDialogue();
    }
    
    void StartDialogue()
    {
        isDialoguePlaying = true;
        dialogueStartTime = Time.time;
        StartCoroutine(ProcessDialogueLines());
    }
    
    IEnumerator ProcessDialogueLines()
    {
        foreach (DialogueLine line in dialogueLines)
        {
            float waitTime = line.displayTime - (Time.time - dialogueStartTime);
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            
            currentSpeaker = line.speaker;
            ShowDialogue(line);
            yield return new WaitForSeconds(line.duration);
            HideDialogue();
        }
        
        isDialoguePlaying = false;
    }
    
    void ShowDialogue(DialogueLine line)
    {
        if (dialogueUI != null)
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
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
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
        StartDialogue();
    }
    
    public void SkipDialogue()
    {
        StopAllCoroutines();
        isDialoguePlaying = false;
        HideDialogue();
    }
}