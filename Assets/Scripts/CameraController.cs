using UnityEngine;

/// <summary>
/// Smooth follow camera supporting 3 modes:
/// 0: Close 3rd person (قريبة)
/// 1: Far 3rd person (بعيدة)
/// 2: Top-down overview (علوية)
/// </summary>
public class CameraController : MonoBehaviour
{
    public Transform target;
    [Range(0, 2)] public int cameraMode = 0;
    public float smoothSpeed = 0.12f;

    private struct CamConfig
    {
        public float dist;
        public float height;
        public float look;
        public CamConfig(float d, float h, float l) { dist = d; height = h; look = l; }
    }

    private readonly CamConfig[] configs = new CamConfig[]
    {
        new CamConfig(12.5f, 7.0f, 5.0f),  // Close
        new CamConfig(19.0f, 11.0f, 6.0f), // Far
        new CamConfig(8.0f, 26.0f, 3.0f)   // Top-Down
    };

    void LateUpdate()
    {
        if (target == null) return;

        CamConfig cfg = configs[cameraMode];
        float headingRad = target.eulerAngles.y * Mathf.Deg2Rad;

        // Target camera position
        Vector3 offset = new Vector3(-Mathf.Sin(headingRad) * cfg.dist, cfg.height, -Mathf.Cos(headingRad) * cfg.dist);
        Vector3 targetPos = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        // Look at point ahead of player
        Vector3 lookPt = target.position + new Vector3(Mathf.Sin(headingRad) * cfg.look, 1.35f, Mathf.Cos(headingRad) * cfg.look);
        transform.LookAt(lookPt, Vector3.up);
    }

    public void CycleMode()
    {
        cameraMode = (cameraMode + 1) % 3;
    }

    public string GetModeName()
    {
        string[] names = new string[] { "🎥 قريبة", "🎥 بعيدة", "🎥 علوية" };
        return names[cameraMode];
    }
}
