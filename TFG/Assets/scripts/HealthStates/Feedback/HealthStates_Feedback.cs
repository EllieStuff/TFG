using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_Feedback : MonoBehaviour
{


    public virtual void ActivateFeedback(float _feedbackDuration)
    {
        StartCoroutine(EndFeedback_InTime(_feedbackDuration));
    }
    public virtual void EndFeedback() { }
    
    internal IEnumerator EndFeedback_InTime(float _feedbackDuration)
    {
        yield return new WaitForSeconds(_feedbackDuration);
        EndFeedback();
    }

}
