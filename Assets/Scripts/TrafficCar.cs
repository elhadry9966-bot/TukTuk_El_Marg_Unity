using UnityEngine;

/// <summary>
/// Individual AI traffic vehicle patrolling El Marg streets.
/// Moves along X or Z axis, checks building obstacle bounds, and reverses direction when needed.
/// </summary>
public class TrafficCar : MonoBehaviour
{
    public bool alongX = false;
    public float dir = 1.0f;
    public float speed = 9.0f;
    public float roadBound = 194.0f;

    void Update()
    {
        float step = dir * speed * Time.deltaTime;
        Vector3 pos = transform.position;

        if (alongX)
        {
            Vector2 nextPt = new Vector2(pos.x + step, pos.z);
            if (IsPointBlocked(nextPt, 3.0f))
            {
                dir = -dir;
                transform.rotation = Quaternion.Euler(0, dir > 0 ? 90f : -90f, 0);
                return;
            }

            pos.x += step;
            if (pos.x > roadBound) pos.x = -roadBound;
            else if (pos.x < -roadBound) pos.x = roadBound;
            transform.rotation = Quaternion.Euler(0, dir > 0 ? 90f : -90f, 0);
        }
        else
        {
            Vector2 nextPt = new Vector2(pos.x, pos.z + step);
            if (IsPointBlocked(nextPt, 3.0f))
            {
                dir = -dir;
                transform.rotation = Quaternion.Euler(0, dir > 0 ? 0f : 180f, 0);
                return;
            }

            pos.z += step;
            if (pos.z > roadBound) pos.z = -roadBound;
            else if (pos.z < -roadBound) pos.z = roadBound;
            transform.rotation = Quaternion.Euler(0, dir > 0 ? 0f : 180f, 0);
        }

        transform.position = pos;
    }

    private bool IsPointBlocked(Vector2 pt, float margin)
    {
        foreach (var r in CitySetup.BuildingRects)
        {
            Rect expanded = new Rect(r.x - margin, r.y - margin, r.width + margin * 2f, r.height + margin * 2f);
            if (expanded.Contains(pt))
                return true;
        }
        return false;
    }
}
