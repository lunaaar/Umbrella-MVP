using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class SwingPoint_Guide : MonoBehaviour
{


    [Range(0.1f, 100f)]
    public float radius = 1.0f;

    [Range(3, 256)]
    public int numSegments = 128;

    void Start()
    {
        DoRenderer();
    }

    public void DoRenderer()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        Color c1 = GetComponentInParent<SpriteRenderer>().color;
        lineRenderer.startColor = c1; lineRenderer.endColor = c1;
        lineRenderer.startWidth = 0.2f; lineRenderer.startWidth = 0.2f;
        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}