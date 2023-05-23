using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Helper;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMesh : BaseMono
{
    [SerializeField] private int m_width;
    [SerializeField] private int m_height;
    [SerializeField] private float m_cellSize;
    
    private GridBase m_gridBase;
    private Vector3[] m_vertices;
    private MeshFilter m_meshFilter;
    private Mesh m_mesh;

    private void Awake()
    {
        m_gridBase = new GridBase(m_width, m_height, m_cellSize);
        m_meshFilter = GetComponent<MeshFilter>();
        Generate();
    }

    private void Generate()
    {
        m_vertices = new Vector3[(m_width) * (m_height)];
        m_meshFilter.mesh = m_mesh = new Mesh();
        m_mesh.name = "Grid Mesh";
        for (int i = 0, y = 0; y < m_height; ++y)
        {
            for (int x = 0; x < m_width; ++x, i++)
            {
                m_vertices[i] = m_gridBase.GetWorldPosition(x, y, RectTransform.Axis.Horizontal);
            }
        }

        m_mesh.vertices = m_vertices;

        int[] triangles = new int[(m_width - 1) * 6];
        for (int ti = 0, x = 0; x < m_width - 1; ++x, ti += 6)
        {
            triangles[ti] = x;
            triangles[ti + 1] = triangles[ti + 4] = x + m_width;
            triangles[ti + 2] = triangles[ti + 3] = x + 1;
            triangles[ti + 5] = x + m_width + 1;
        }

        m_mesh.triangles = triangles;
    }

    private void OnDrawGizmos()
    {
        if (m_width > 0 && m_height > 0 && m_cellSize > 0)
        {
            GridBase grid = new GridBase(m_width, m_height, m_cellSize);
            grid.DrawGizmos(transform);
        }
    }
}
