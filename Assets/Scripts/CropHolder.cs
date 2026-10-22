using UnityEngine;

public class CropHolder : MonoBehaviour
{
    public Transform cropPosition;
    
    public GameObject currentCropGameObject;
    public Crop currentCrop;

    public void PlantSeed(GameObject cropPrefab)
    {
        if (currentCrop != null)
        {
            return;
        }
        currentCropGameObject = Instantiate(cropPrefab, cropPosition.position, cropPosition.rotation);
        currentCrop = currentCropGameObject.GetComponent<Crop>();
    }
}
