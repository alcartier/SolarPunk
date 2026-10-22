using UnityEngine;
using UnityEngine.InputSystem;

public class Watering : MonoBehaviour
{
    private CropHolder cropHolder;
    
    [SerializeField] private InputActionReference waterCropAction;
    [SerializeField] private ParticleSystem waterParticles;
    
    private void OnEnable()
    {
        waterCropAction.action.performed += WaterCrop;
    }

    private void OnDisable()
    {
        waterCropAction.action.performed -= WaterCrop;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            cropHolder =  other.GetComponent<CropHolder>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            cropHolder = null;
        }
    }
    
    private void WaterCrop(InputAction.CallbackContext context)
    {
        waterParticles.Play();
        if (cropHolder != null && cropHolder.currentCrop != null)
        {
            cropHolder.currentCrop.watered = true;
        }
    }
}
