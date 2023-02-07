using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_FeedbackManager : MonoBehaviour
{
    [SerializeField] internal LifeSystem lifeSystem;

    Dictionary<HealthState.Effect, HealthStates_Feedback> healthStatesFeedbacks = new Dictionary<HealthState.Effect, HealthStates_Feedback>();
    internal Dictionary<HealthState.Effect, HealthStates_Feedback> currActiveFeedbacks = new Dictionary<HealthState.Effect, HealthStates_Feedback>();


    /*private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            HealthStates_Feedback childFeedback = transform.GetChild(i).GetComponent<HealthStates_Feedback>();
            healthStatesFeedbacks.Add(childFeedback.relatedEffect, childFeedback);
        }
    }*/

    public void ActivateFeedback(HealthState.Effect _effect, float _effectDuration)
    {
        if (!healthStatesFeedbacks.ContainsKey(_effect))
        {
            Debug.LogWarning("HealthState Feedback was not found.");
            return;
        }

        if (!currActiveFeedbacks.ContainsKey(_effect))
            currActiveFeedbacks.Add(_effect, healthStatesFeedbacks[_effect]);
        currActiveFeedbacks[_effect].ActivateFeedback(this, _effectDuration);
    }



}
