using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_BurnedFeedback : HealthStates_Feedback
{
    [SerializeField] ParticleSystem fireParticles;

    public override void ActivateFeedback(float _feedbackDuration)
    {
        base.ActivateFeedback(_feedbackDuration);
        fireParticles.Play();
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        fireParticles.Stop();
    }

}
