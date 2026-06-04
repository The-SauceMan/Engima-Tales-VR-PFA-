using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineUIHider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private GameObject uiToHide;
    [SerializeField] private float hideAtTime = 10f;
    
    private bool hasHidden = false;
    
    void Start()
    {
        if (timeline == null)
            timeline = FindObjectOfType<PlayableDirector>();
    }
    
    void Update()
    {
        if (!hasHidden && timeline != null && timeline.time >= hideAtTime)
        {
            if (uiToHide != null)
                uiToHide.SetActive(false);
            
            hasHidden = true;
            Debug.Log($"UI hidden at {timeline.time:F2} seconds");
        }
    }
}