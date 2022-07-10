using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    public int length;
    public float smoothSpeed;
    public float trailSpeed;

    [HideInInspector]public Vector3 trailDist;

    private Vector3[] segmentPoses;
    private Vector3[] segmentV;
    private Transform parent;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
        parent = transform.parent;
    }

    void Update()
    {
        segmentPoses[0] = parent.position;

        for(int i = 1; i < length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + trailDist / length, ref segmentV[i], smoothSpeed + 1 / trailSpeed);
        }
        lineRenderer.SetPositions(segmentPoses);
    }
}
