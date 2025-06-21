using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private IInteractable _currentInteractable;

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        FindClosestInteractable();
        RotateUIToCamera();
        
        if (_currentInteractable != null && Input.GetKeyDown(_interactKey))
        {
            _currentInteractable.PerformAction(gameObject);
        }
    }

    private void FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _interactRange);

        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        if (_currentInteractable != closest)
        {
            HideCurrentUI();
            _currentInteractable = closest;
            ShowCurrentUI();
        }
    }

    private void ShowCurrentUI()
    {
        if (_currentInteractable != null)
            _currentInteractable.InteractionUI?.gameObject.SetActive(true);
    }

    private void HideCurrentUI()
    {
        if (_currentInteractable != null)
            _currentInteractable.InteractionUI?.gameObject.SetActive(false);
    }

    private void RotateUIToCamera()
    {
        if (_currentInteractable?.InteractionUI != null)
        {
            Transform ui = _currentInteractable.InteractionUI;
            ui.LookAt(ui.position + _mainCamera.transform.rotation * Vector3.forward,
                      _mainCamera.transform.rotation * Vector3.up);
        }
    }
}