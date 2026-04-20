using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._02_Grid.GridRendering
{
   [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
   public class GridBase : MonoBehaviour
   {
      [Header("Grid Settings")]
      [SerializeField] [Range(0f,100f)] private int xSize;
      [SerializeField] [Range(0f,100f)] private int ySize;
      [Header("Debugging")]
      [SerializeField] [Range(0f,1f)] private float squareSize;
      [SerializeField] private bool debuggingOn = true;
      [SerializeField] private Color32 squareColor;
      [SerializeField] private Mesh gridMesh;
      
      private List<Vector3> _squareCoord;
      private Vector3[] _vertices;
      private int[] _triangles;
      private Vector3 _gridStartPos;
      
      //Getter
      //these are the coords where tower and other placeable object are to be placed on
      public IReadOnlyList<Vector3> SquareCoords => _squareCoord;

      private void OnValidate()
      {
         RenderGrid();
         CalculateSquareCoords();
      }

      public void OnDrawGizmos()
      {
         if (!debuggingOn) return;
         
         Gizmos.color = Color.red;
         foreach (Vector3 t in _vertices) 
         { Gizmos.DrawSphere(t + _gridStartPos, 0.1f); }
         
         Gizmos.color = squareColor;
         foreach (Vector3 t in _squareCoord) 
         { Gizmos.DrawCube(t + _gridStartPos, new Vector3(squareSize, 0.1f, squareSize)); }
         
      }

      [Button]
      private void RenderGrid()
      {
         _gridStartPos = this.transform.position;
         GetComponent<MeshFilter>().mesh = gridMesh = new Mesh();
         gridMesh.name = "Grid_Mesh";
         _vertices = new Vector3[(xSize + 1) * (ySize + 1)];
         Vector2[] uvs = new Vector2[_vertices.Length];
         Vector4[] tangents = new Vector4[_vertices.Length];
         Vector4 tangent = new Vector4(1f,0f,0f,-1f);
         
         for (int i = 0, y = 0; y <= ySize; y++)
         {
            for (int x = 0; x <= xSize; x++, i++)
            {
               _vertices[i] = new Vector3(x, 0, y);
               uvs[i] = new Vector2((float)x / xSize, (float)y / ySize);
               tangents[i] = tangent;
            }
         }

         gridMesh.vertices = _vertices;
         gridMesh.uv = uvs;
         gridMesh.tangents = tangents;

         _triangles = new int[xSize * ySize * 6];
         for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
         {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
               _triangles[ti] = vi;
               _triangles[ti + 3] = _triangles[ti + 2] = vi + 1;
               _triangles[ti + 4] = _triangles[ti + 1] = vi + xSize + 1;
               _triangles[ti + 5] = vi + xSize + 2;
            }
         }

         gridMesh.triangles = _triangles;
         gridMesh.RecalculateNormals();
      }
      [Button]
      private void CalculateSquareCoords()
      {
         _squareCoord = new List<Vector3>();
         for (int y = 0; y < ySize; y++)
         {
            for (int x = 0; x < xSize; x++)
            {
               Vector3 center = new Vector3(x + 0.5f, 0f, y + 0.5f);
               _squareCoord.Add(center);
               
            }
         }
      }
   }
}