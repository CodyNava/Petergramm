using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles
{
    public class ProjectileTrail : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        
        [SerializeField] private float pointSpacing = 0.1f;
        [SerializeField] private int maxPoints = 10;

        private readonly List<Vector3> points = new();

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            points.Clear();
            lineRenderer.positionCount = 0;

            AddPoint(transform.position);
        }

        private void Update()
        {
            if (points.Count == 0)
                return;

            if (Vector3.Distance(transform.position, points[0]) >= pointSpacing)
            {
                AddPoint(transform.position);
            }
        }

        private void AddPoint(Vector3 position)
        {
            points.Insert(0, position);
            if (points.Count > maxPoints)
            {
                points.RemoveAt(points.Count - 1);
            }

            lineRenderer.positionCount = points.Count;


            for (var i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }
    }
}