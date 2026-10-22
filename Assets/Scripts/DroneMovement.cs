using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;

public class DroneMovement : MonoBehaviour
{
    
    public InputActionReference moveAction;
    public float speed = 5f;
    
    public Transform visualTransform;
    public float maxTiltAngle = 30f;
    public float tiltSmoothing = 0.05f;
    
    void Update()
    {
        Vector2 inputDir = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y);
        transform.Translate(moveDir * (Time.deltaTime * speed));
        
        // Rotation du drone
        Quaternion target = Quaternion.Euler(
            inputDir.y * maxTiltAngle,
            0f,
            -inputDir.x * maxTiltAngle
        );
        
        visualTransform.localRotation = Quaternion.Slerp(visualTransform.localRotation, target, tiltSmoothing);
        
    }
}