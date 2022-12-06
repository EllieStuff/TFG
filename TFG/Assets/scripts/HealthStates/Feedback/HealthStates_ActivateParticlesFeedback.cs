using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_ActivateParticlesFeedback : HealthStates_Feedback
{
    [SerializeField] ParticleSystem particles;

    public override void ActivateFeedback(HealthStates_FeedbackManager _manager, float _feedbackDuration)
    {
        base.ActivateFeedback(_manager, _feedbackDuration);
        particles.Play();
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        particles.Stop();
    }

}
