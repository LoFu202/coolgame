using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public Camera playerCamera;
    public Camera carCamera;
    public GameObject playerVisual;
    public PlayerMovement playerMovement;
    public Mouselook mouseLook;
    public CarController carController;
    public Transform carExitPoint;

    private bool isInCar = false;

    void Start()
    {
        carCamera.gameObject.SetActive(false); 
        playerCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isInCar)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.CompareTag("Car") && Input.GetKeyDown(interactKey))
                {
                    Debug.Log("Car detected, entering car...");
                    EnterCar();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Exiting car...");
                ExitCar();
            }
        }
    }

    void EnterCar()
    {
        isInCar = true;

        carCamera.gameObject.SetActive(true);
        carCamera.enabled = true;
        playerCamera.gameObject.SetActive(false);
        playerCamera.enabled = false;

        playerMovement.enabled = false;
        mouseLook.enabled = false;
        playerVisual.SetActive(false);

        carController.EnableDriving(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        Debug.Log("Car camera enabled: " + carCamera.enabled + " | GameObject active: " + carCamera.gameObject.activeSelf);
    }

    void ExitCar()
    {
        isInCar = false;

        carCamera.gameObject.SetActive(false);
        carCamera.enabled = false;
        playerCamera.gameObject.SetActive(true);
        playerCamera.enabled = true;

        playerMovement.enabled = true;
        mouseLook.enabled = true;
        playerVisual.SetActive(true);

        transform.position = carExitPoint.position;
        transform.rotation = carExitPoint.rotation;

        carController.EnableDriving(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Switched back to player camera");
    }
}
