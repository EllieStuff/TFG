using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeEventByIndex : MonoBehaviour
{
    [SerializeField] int currIdx = 0;

    [System.Serializable]
    public class CustomEvent
    {
        public string name = "Name";
        public UnityEvent _event;
    }
    public CustomEvent[] orderedEvents;
    //public UnityEvent[] orderedEvents;

    int firstIdx = 0;


    private void Awake()
    {
        firstIdx = currIdx;
    }


    public void InvokeCurrentEvent()
    {
        if(currIdx < 0 || currIdx >= orderedEvents.Length)
        {
            Debug.LogWarning("Index was out of range");
            return;
        }

        orderedEvents[currIdx]._event.Invoke();
    }
    public void InvokeCurrentEvent_IncreaseIdx()
    {
        InvokeCurrentEvent();
        if (orderedEvents[currIdx]._event.GetPersistentEventCount() > 0)
            currIdx++;
    }
    public void InvokeCurrentEvent_DecreaseIdx()
    {
        InvokeCurrentEvent();
        if (orderedEvents[currIdx]._event.GetPersistentEventCount() > 0)
            currIdx--;
    }

    public void IncreaseIndex()
    {
        currIdx++;
    }
    public void DecreaseIndex()
    {
        currIdx--;
    }
    public void ResetIndex()
    {
        currIdx = firstIdx;
    }
    public void SetIndex(int _idx)
    {
        currIdx = _idx;
    }

}
