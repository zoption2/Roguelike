using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlingShot
{
    [RequireComponent(typeof(LineRenderer))]
    public class SlingShotLine : MonoBehaviour
    {
        public LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void RenderLine(Vector2 startPoint, Vector2 endPoint)
        {
            _lineRenderer.positionCount = 2;
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(startPoint.x, startPoint.y, 0f);
            points[1] = new Vector3(endPoint.x, endPoint.y, 0f); 
            _lineRenderer.SetPositions(points);
        }


        public void EndLine() 
        {
            _lineRenderer.positionCount = 0;
        }
        
    }
}


