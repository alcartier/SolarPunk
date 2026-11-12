using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 1f;
    
    public float shakeMagnitude = 2f;

    public float dampingSpeed = 2f;

    private void OnEnable()
    {
        Crop.OnDeath += TriggerShake;
    }

    private void OnDisable()
    {
        Crop.OnDeath -= TriggerShake;
    }
    
    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }
    
    private IEnumerator Shake()
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.localPosition;

        while (elapsedTime < shakeDuration)
        {
            float magnitude = shakeMagnitude * Mathf.Exp(-dampingSpeed * elapsedTime);
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            
            transform.localPosition = new Vector3(initialPosition.x + xOffset, initialPosition.y +  yOffset, initialPosition.z);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        transform.localPosition = initialPosition;
    }
}
