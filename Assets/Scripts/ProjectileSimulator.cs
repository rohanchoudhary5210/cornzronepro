using UnityEngine;

public class ProjectileSimulator : MonoBehaviour
{
    public static ProjectileSimulator Instance;

    [Header("Trajectory Prediction Visuals")]
    public LineRenderer lineRenderer;
    public int resolution = 30;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PredictTrajectory(Vector3 startPos, float speed, Vector3 direction)
    {
        lineRenderer.positionCount = resolution + 1;
        float g = Mathf.Abs(Physics.gravity.y);

        Vector3 dirXZ = new Vector3(direction.x, 0f, direction.z).normalized;
        float angleRad = Mathf.Deg2Rad * Vector3.Angle(Vector3.forward, direction);

        float vH = speed * Mathf.Cos(angleRad);
        float vY = speed * Mathf.Sin(angleRad);

        Vector3 velocity = new Vector3(vH * dirXZ.x, vY, vH * dirXZ.z);
        float flightTime = (2f * vY) / g;
        float dt = flightTime / resolution;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i * dt;
            Vector3 pos = (startPos + velocity/7 * t + 0.5f * Physics.gravity * t * t);
            lineRenderer.SetPosition(i, pos);
        }

        lineRenderer.enabled = true;
    }

    public void ClearPath()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}
