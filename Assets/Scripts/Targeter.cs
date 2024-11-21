using UnityEngine;

//Note: this is a terrible name for this class; please feel free to change it
public class Targeter : MonoBehaviour
{
    [SerializeField] private Transform targetOrigin;
    [SerializeField] private float targetDistance = 1000f;
    [SerializeField] private LayerMask hitLayer;
    private AimTarget _currentTarget;
    void Update()
    {
        Vector3 origin = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(origin, targetOrigin.transform.forward, out hit, targetDistance, hitLayer))
        {
            if (hit.collider.gameObject.TryGetComponent<AimTarget>(out AimTarget target))
            {
                SwapTarget(target);
                return;
            }
        }
        _currentTarget?.StopTarget();
        _currentTarget = null;

    }

    private void SwapTarget(AimTarget target)
    {
        if (target != _currentTarget)
        {
            _currentTarget?.StopTarget();
            _currentTarget = target;
            _currentTarget.Target();
        }
    }
    
    
}
