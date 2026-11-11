using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Ease easeType = Ease.Linear;
    
    private MotionHandle currentMotion;
    
    // Update is called once per frame
    void Update()
    {
        var targetPosition = target.position + offset;
        
        if (currentMotion.IsActive()) currentMotion.Cancel();
        
        currentMotion = LMotion.Create(transform.position, targetPosition, smoothTime)
            .WithEase(easeType)
            .BindToPosition(transform)
            .AddTo(gameObject);
    }
}
