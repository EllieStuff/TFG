using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_ChangeMaterialFeedback : HealthStates_Feedback
{
    [SerializeField] SkinnedMeshRenderer targetRenderer;
    [SerializeField] Animator targetAnimator;
    [SerializeField] Material mat;

    Material[] originalMats;


    public override void ActivateFeedback(HealthStates_FeedbackManager _manager, float _feedbackDuration)
    {
        base.ActivateFeedback(_manager, _feedbackDuration);
        originalMats = targetRenderer.materials;
        targetAnimator.speed = 0;
        targetRenderer.materials = new Material[1] { mat };
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        targetAnimator.speed = 1;
        targetRenderer.materials = originalMats;
    }


}
