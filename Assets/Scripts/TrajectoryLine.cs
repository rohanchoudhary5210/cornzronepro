using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("Trajectory Line Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int segmentCount = 10;
    private Vector3[] linePositions;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        CreateLine();
    }

    private void CreateLine()
    {
        lineRenderer.positionCount = segmentCount;
        linePositions = new Vector3[segmentCount];
    }

    // Update is called once per frame
    void Update()
    {
        // Update the line positions if needed
        CreateLine();
    }
    
}
