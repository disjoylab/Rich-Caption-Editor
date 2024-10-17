using UnityEngine;
using UnityEngine.EventSystems;

public class CreateNewCue : MonoBehaviour, IPointerDownHandler
{
    //Create a new cue in the Timeline
    public void OnPointerDown(PointerEventData eventData)
    {
        TimelineManager.Instance.NewCue();
    }
}
