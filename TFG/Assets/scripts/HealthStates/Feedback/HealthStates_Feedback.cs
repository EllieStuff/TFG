using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_Feedback : MonoBehaviour
{
    [SerializeField] internal HealthState.Effect relatedEffect;

    internal HealthStates_FeedbackManager manager;

    public virtual void ActivateFeedback(HealthStates_FeedbackManager _manager, float _feedbackDuration)
    {
        manager = _manager;
        StopAllCoroutines();
        StartCoroutine(EndFeedback_InTime(_feedbackDuration));
    }
    public virtual void EndFeedback()
    {
        manager.currActiveFeedbacks.Remove(relatedEffect);
    }
    
    internal IEnumerator EndFeedback_InTime(float _feedbackDuration)
    {
        float endEffectTimeStamp = Time.timeSinceLevelLoad + _feedbackDuration;
        while(Time.timeSinceLevelLoad < endEffectTimeStamp)
        {
            yield return new WaitForSeconds(0.3f);
            if (manager.lifeSystem.healthStates.Find(_effect => _effect.state == relatedEffect) == null)
                break;
        }
        EndFeedback();
    }

}
