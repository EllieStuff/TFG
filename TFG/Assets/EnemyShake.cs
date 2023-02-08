using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float posMagnitude, float sizeMagnitude, float speed = 100)
    {
        Vector3 originalPos = transform.localPosition;
        Vector3 originalScale = transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * posMagnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * posMagnitude;
            float zOffset = Random.Range(-0.5f, 0.5f) * posMagnitude;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                transform.localPosition + new Vector3(xOffset, yOffset, zOffset),
                Time.deltaTime * speed
            );

            xOffset = Random.Range(-0.5f, 0.5f) * sizeMagnitude;
            yOffset = Random.Range(-0.5f, 0.5f) * sizeMagnitude;
            zOffset = Random.Range(-0.5f, 0.5f) * sizeMagnitude;
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                transform.localScale + new Vector3(xOffset, yOffset, zOffset),
                Time.deltaTime * speed
            );

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
        transform.localScale = originalScale;

    }
}
