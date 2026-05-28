using UnityEngine;

public class CameraFollowPivot : MonoBehaviour
{
    [SerializeField] private Transform target;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position;
    }
}
