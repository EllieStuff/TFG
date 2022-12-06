using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_BurnedFeedback : HealthStates_Feedback
{
    [SerializeField] ParticleSystem fireParticles;

    public override void ActivateFeedback(HealthStates_FeedbackManager _manager, float _feedbackDuration)
    {
        base.ActivateFeedback(_manager, _feedbackDuration);
        fireParticles.Play();
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        fireParticles.Stop();
    }

}
