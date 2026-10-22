using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropSeed : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private InputActionReference plantSeedAction;

    private void OnEnable()
    {
        plantSeedAction.action.performed += PlantSeed;
    }

    private void OnDisable()
    {
        plantSeedAction.action.performed -= PlantSeed;
    }

    private void PlantSeed(InputAction.CallbackContext context)
    {
        Instantiate(seedPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
