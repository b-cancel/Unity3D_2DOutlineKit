/*
**	OutlineMesh.cs
*/

//#define DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutlineMesh : Script
{
	public Camera m_Camera;
	public Transform m_Target;
	public Material m_Material;

	protected Mesh m_Mesh;
	protected Vector3[] m_Vertices;
	protected int[] m_Triangles;

	protected Matrix4x4 m_ViewMatrix;

#if DEBUG
	protected string m_Errors;
#endif

	protected void Start()
	{
		m_ViewMatrix = Matrix4x4.TRS(
			new Vector3(0.5f, 0.5f, 0.0f),
			Quaternion.identity,
			new Vector3(0.5f, 0.5f, 1.0f)) * m_Camera.projectionMatrix;

		if(m_Target)
		{
			MeshFilter meshFilter = m_Target.GetComponent<MeshFilter>();
			m_Mesh = meshFilter.mesh;
			m_Vertices = m_Mesh.vertices;
			m_Triangles = m_Mesh.triangles;
		}
	}

	protected void OnPostRender()
	{
#if DEBUG
		m_Errors = "";
#endif

		if(m_Target == null)
		{
#if DEBUG
			m_Errors = "No target";
#endif
			return;
		}

		int rightmostVertexIndex;
		List<Vector3> transformedVertices = TransformVertices(out rightmostVertexIndex);
		int currentIndex = rightmostVertexIndex;

//System.Console.WriteLine("New iteration");

		List<Vector3> outlineVertices = new List<Vector3>();

		const float k2PI = Mathf.PI * 2.0f;

		float currentAngle = 0.0f;
		int safetyCount = 0;
		do
		{
//System.Console.WriteLine("Next vertex");

			float leastAngle = k2PI;
			float leastDifference = k2PI;
			int leastAngleIndex = -1;
			for(int i = 0; i < transformedVertices.Count; ++i)
			{
				if(i != currentIndex)						// Don't compare this vertex with itself
				{
					Vector3 direction = transformedVertices[i] - transformedVertices[currentIndex];
					if(direction.sqrMagnitude > 0.0001f)	// Skip vertices that are right on top of this one
					{
						float angle = Mathf.Atan2(direction.y, direction.x);
						float difference = (angle - currentAngle + k2PI * 2.0f) % k2PI;

						if(difference < leastDifference)
						{
							leastAngle = angle;
							leastDifference = difference;
							leastAngleIndex = i;
//System.Console.WriteLine("i = " + i + ", leastAngleDifference = " + leastAngleDifference);
						}
					}
				}
			}

			outlineVertices.Add(transformedVertices[currentIndex]);

			currentIndex = leastAngleIndex;
			currentAngle = leastAngle;
//System.Console.WriteLine("currentIndex = " + currentIndex + ", currentAngle = " + currentAngle * Mathf.Rad2Deg);

			if(++safetyCount >= transformedVertices.Count)
			{
#if DEBUG
				m_Errors += "Iterated too many times";
#endif
				break;
			}
		}
		while(currentIndex != rightmostVertexIndex);

		if(outlineVertices.Count > 1)
		{
			RenderOutline(outlineVertices);
			RenderOutline(OffsetOutline(outlineVertices, -0.02f));
			RenderOutline(OffsetOutline(outlineVertices, -0.04f));
			RenderOutline(OffsetOutline(outlineVertices, -0.06f));
		}
	}

#if DEBUG
	protected void OnGUI()
	{
		GUILayout.Label(m_Errors);
	}
#endif

	protected List<Vector3> TransformVertices(out int rightmostVertexIndex)
	{
		List<Vector3> transformedVertices = new List<Vector3>();

		Matrix4x4 modelMatrix = m_Camera.worldToCameraMatrix * m_Target.localToWorldMatrix;
		Vector3 rightmostVertex = new Vector3(-Mathf.Infinity, 0.0f, 0.0f);
		rightmostVertexIndex = -1;

		for(int i = 0; i < m_Vertices.Length; ++i)
		{
			Vector3 vertex = modelMatrix.MultiplyPoint3x4(m_Vertices[i]);
			if(vertex.z <= 0.0f)
			{
				Vector3 transformedVertex = Vector3.Scale(m_ViewMatrix.MultiplyPoint(vertex), new Vector3(1.0f, 1.0f, 0.0f));
				transformedVertices.Add(transformedVertex);

				if(transformedVertex.x > rightmostVertex.x)
				{
					rightmostVertex = transformedVertex;
					rightmostVertexIndex = i;
				}
			}
		}

		return transformedVertices;
	}

	protected List<Vector3> OffsetOutline(List<Vector3> outline, float width)
	{
		int numVertices = outline.Count;

 		List<Vector3> edges = new List<Vector3>(numVertices * 2);
  		for(int i = 0; i < numVertices; ++i)
 		{
			Vector3 start = outline[Index(i - 1, numVertices)];
			Vector3 end = outline[i];
			Vector3 perpendicular = Perpendicular(end - start) * width;

 			edges.Add(start + perpendicular);
 			edges.Add(end + perpendicular);
 		}

		List<Vector3> offsetOutline = new List<Vector3>(numVertices);
		for(int i = 0; i < numVertices * 2; i += 2)
		{
			int previousEdge = Index(i - 2, numVertices * 2);
			Vector3 intersection;
			if(Intersection(edges[i], edges[i + 1], edges[previousEdge], edges[previousEdge + 1], out intersection))
				offsetOutline.Add(intersection);
		}

		return offsetOutline;
	}

	protected int Index(int index, int length)
	{
		return (index + length + length) % length;
	}

	protected Vector3 Perpendicular(Vector3 edge)
	{
		return Vector3.Scale(new Vector3(-edge.y, edge.x, 0.0f).normalized, new Vector3(1.0f, m_Camera.aspect, 1.0f));
	}

	protected bool Intersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 result)
	{
		// Code butchered from http://flassari.is/2008/11/line-line-intersection-in-cplusplus/

		result = Vector3.zero;

		// Store the values for fast access and easy equations-to-code conversion:

		float x1 = p1.x, x2 = p2.x, x3 = p3.x, x4 = p4.x;
		float y1 = p1.y, y2 = p2.y, y3 = p3.y, y4 = p4.y;

		// Get the denominator:

		float denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

		// If denominator is zero, the lines are parallel:

		if(denominator == 0.0f)
			return false;

		float pre = (x1 * y2 - y1 * x2), post = (x3 * y4 - y3 * x4);
		float x = (pre * (x3 - x4) - (x1 - x2) * post) / denominator;
		float y = (pre * (y3 - y4) - (y1 - y2) * post) / denominator;

		result = new Vector3(x, y, 0.0f);
		return true;
	}

	protected void RenderOutline(List<Vector3> vertices)
	{
		m_Material.SetPass(0);

		bool even = true;

		GL.LoadOrtho();
		GL.Begin(GL.LINES);
		GL.Color(Color.white);

		if(vertices.Count > 1)
		{
			for(int i = 1; i < vertices.Count; ++i)
			{
				GL.Vertex(vertices[i - 1]);
				GL.Vertex(vertices[i]);

				GL.Color(even ? Color.yellow : Color.red);
				even = !even;
			}

			GL.Vertex(vertices[vertices.Count - 1]);
			GL.Vertex(vertices[0]);
		}
		GL.End();
	}
}
