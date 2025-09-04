using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    [SerializeField] private GameObject _eToInteract;
    private IInteractable _currentInteractable;
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
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        _currentInteractable = closest;
        _eToInteract.SetActive(_currentInteractable != null);
    }

    private void RotateUIToCamera()
    {
        if (_currentInteractable?.InteractionUI == null || _mainCamera == null) return;

        Transform ui = _currentInteractable.InteractionUI;
        ui.LookAt(ui.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
    }
}