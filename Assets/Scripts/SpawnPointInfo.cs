using UnityEngine;

public class SpawnPointInfo : MonoBehaviour
{
    public enum PointRole
    {
        Any,
        Patient,
        Doctor
    }

    [Header("Role")]
    public PointRole assignedRole = PointRole.Any;

    [Header("Animation Type")]
    public bool isSittingPoint = false;

    private void OnDrawGizmos()
    {
        switch (assignedRole)
        {
            case PointRole.Patient:
                Gizmos.color = Color.red;
                break;
            case PointRole.Doctor:
                Gizmos.color = Color.blue;
                break;
            default:
                Gizmos.color = Color.green;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 0.3f);

        Gizmos.DrawRay(transform.position, transform.forward * 0.5f);
    }
}