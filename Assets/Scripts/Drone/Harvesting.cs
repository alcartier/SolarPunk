using UnityEngine;
using UnityEngine.InputSystem;

public class Harvesting : MonoBehaviour
{
    [SerializeField] private CropHolder cropHolder;
    [SerializeField] private InputActionReference harvestCropAction;
    
    private void OnEnable()
    {
        harvestCropAction.action.performed += HarvestCrop;
    }

    private void OnDisable()
    {
        harvestCropAction.action.performed -= HarvestCrop;
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
        if (other.CompareTag("Dirt") && other.GetComponent<CropHolder>() == cropHolder)
        {
            cropHolder = null;
        }
    }

    private void HarvestCrop(InputAction.CallbackContext context)
    {
        if (cropHolder != null && cropHolder.currentCrop != null && cropHolder.currentCrop.currentState == CropState.Mature)
        {
            GameManager.Instance.ChangeScore(cropHolder.currentCrop.getCropScore());
            GameManager.Instance.Harvest();
            Destroy(cropHolder.currentCropGameObject);
            cropHolder.currentCrop = null;
            cropHolder.currentCropGameObject = null;
        }
    }
}
