using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_FeedbackManager : MonoBehaviour
{
    [SerializeField] HealthStates_BurnedFeedback burnedFeedbackRef;
    [SerializeField] HealthStates_ColdFeedback coldFeedbackRef;
    [SerializeField] HealthStates_FrozenFeedback frozenFeedbackRef;

    HealthStates_Feedback currFeedback = null;


    private void Start()
    {
        //ActivateFeedback(HealthState.Effect.BURNED, 10);
        //ActivateFeedback(HealthState.Effect.COLD, 10);
        //ActivateFeedback(HealthState.Effect.FROZEN, 10);
    }

    public void ActivateFeedback(HealthState.Effect _effect, float _effectDuration)
    {
        if (currFeedback != null)
            currFeedback.EndFeedback();

        switch (_effect)
        {
            case HealthState.Effect.BURNED:
                currFeedback = burnedFeedbackRef;
                break;

            case HealthState.Effect.COLD:
                currFeedback = coldFeedbackRef;
                break;

            case HealthState.Effect.FROZEN:
                currFeedback = frozenFeedbackRef;
                break;


            default:
                break;
        }

        currFeedback.ActivateFeedback(_effectDuration);
    }



}
