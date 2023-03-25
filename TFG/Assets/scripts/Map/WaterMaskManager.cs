using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WaterMaskManager : MonoBehaviour
{
    const int MASK_RENDER_QUEUE = 3001;

    class MaskedObject
    {
        public int maskedRefAmount = 1, originalRenderQueue;
        public MaskedObject(int _originalRenderQueue) { originalRenderQueue = _originalRenderQueue; }
    }
    Dictionary<Transform, MaskedObject> maskedObjects = new Dictionary<Transform, MaskedObject>();


    public void AddObjectMask(Transform _transform)
    {
        if (!maskedObjects.ContainsKey(_transform))
        {
            MeshRenderer renderer = _transform.GetComponent<MeshRenderer>();
            maskedObjects.Add(_transform, new MaskedObject(renderer.material.renderQueue));
            renderer.material.renderQueue = MASK_RENDER_QUEUE;
        }
        else
        {
            maskedObjects[_transform].maskedRefAmount++;
        }
    }

    public void RemoveObjectMask(Transform _transform)
    {
        if (maskedObjects.ContainsKey(_transform))
        {
            maskedObjects[_transform].maskedRefAmount--;
            if(maskedObjects[_transform].maskedRefAmount <= 0)
            {
                _transform.GetComponent<MeshRenderer>().material.renderQueue = maskedObjects[_transform].originalRenderQueue;
                maskedObjects.Remove(_transform);
            }
        }
    }


}
