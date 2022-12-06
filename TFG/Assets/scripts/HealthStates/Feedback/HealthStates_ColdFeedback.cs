using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_ColdFeedback : HealthStates_Feedback
{
    [SerializeField] ParticleSystem iceParticles;

    public override void ActivateFeedback(HealthStates_FeedbackManager _manager, float _feedbackDuration)
    {
        base.ActivateFeedback(_manager, _feedbackDuration);
        iceParticles.Play();
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        iceParticles.Stop();
    }
}
