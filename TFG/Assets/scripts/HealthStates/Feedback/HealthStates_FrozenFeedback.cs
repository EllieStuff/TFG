using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStates_FrozenFeedback : HealthStates_Feedback
{
    [SerializeField] MeshRenderer targetRenderer;
    [SerializeField] Material iceMat;

    Material[] originalMats;


    public override void ActivateFeedback(HealthStates_FeedbackManager _manager, float _feedbackDuration)
    {
        base.ActivateFeedback(_manager, _feedbackDuration);
        originalMats = targetRenderer.materials;
        targetRenderer.materials = new Material[1] { iceMat };
    }

    public override void EndFeedback()
    {
        base.EndFeedback();
        targetRenderer.materials = originalMats;
    }


}
