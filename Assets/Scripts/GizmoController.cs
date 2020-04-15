using UnityEngine;

public class GizmoController : MonoBehaviour
{
    public Transform[] JawWaypoints;
    private Vector2[] _jawWaypointPositions;

    public Transform[] ThroatWaypoints;
    private Vector2[] _throatWaypointPositions;

    private bool _init = false;

    private void InitGizmos() {
        _jawWaypointPositions = new Vector2[JawWaypoints.Length];

        for (int i = 0; i < JawWaypoints.Length; i++) {
            _jawWaypointPositions[i] = JawWaypoints[i].position;
        }

        _throatWaypointPositions = new Vector2[ThroatWaypoints.Length];

        for (int i = 0; i < ThroatWaypoints.Length; i++) {
            _throatWaypointPositions[i] = ThroatWaypoints[i].position;
        }
    }

    private void OnDrawGizmos() {
        if (!_init) {
            InitGizmos();
        }

        Gizmos.color = Color.green;

        Vector2 old = Vector2.zero;
        bool startDrawLine = false;

        foreach (Vector2 pos in _jawWaypointPositions) {
            Gizmos.DrawWireSphere(pos, 0.1f);

            if (startDrawLine) {
                Gizmos.DrawLine(old, pos);
            }
            else {
                startDrawLine = true;
            }
            old = pos;
        }

        old = Vector2.zero;
        startDrawLine = false;

        foreach (Vector2 pos in _throatWaypointPositions) {
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
