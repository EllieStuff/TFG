using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator ShakeCamera(float duration, float magnitude, float speed = 100)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float zOffset = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                transform.localPosition + new Vector3(xOffset, yOffset, zOffset),
                Time.deltaTime * speed
            );

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
        
    }
}