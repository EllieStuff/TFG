using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_ColdFeedback : HealthStates_Feedback
{
    [SerializeField] ParticleSystem iceParticles;

    public override void ActivateFeedback(float _feedbackDuration)
    {
        base.ActivateFeedback(_feedbackDuration);
        iceParticles.Play();
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        iceParticles.Stop();
    }
}
