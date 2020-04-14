using UnityEngine;

public class GizmoController : MonoBehaviour
{
    public Transform[] JawWaypoints;

    private Vector2[] jawWaypointPositions;

    private void InitGizmos() {
        jawWaypointPositions = new Vector2[JawWaypoints.Length];

        for (int i = 0; i < JawWaypoints.Length; i++) {
            jawWaypointPositions[i] = JawWaypoints[i].position;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        if (jawWaypointPositions == null) InitGizmos();

        Vector2 old = Vector2.zero;
        bool startDrawLine = false;

        foreach (Vector2 pos in jawWaypointPositions) {
            Gizmos.DrawWireSphere(pos, 0.1f);

            if (startDrawLine) {
                Gizmos.DrawLine(old, pos);
            }
            else {
                startDrawLine = true;
            }
            old = pos;
        }
    }
}
