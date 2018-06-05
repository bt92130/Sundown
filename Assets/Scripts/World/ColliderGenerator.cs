using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderGenerator : MonoBehaviour
{
    private List<Edge> edges;
    private List<List<Vector2>> points;
    // private List<Vector2> points;
    private MapGenerator mapGen;

    public void Awake()
    {
        mapGen = GetComponent<MapGenerator>();
    }

    public void GenerateCollider(GameObject node)
    {
        //points is List<List<Vector2>> if using mapgen.edgepoints, otherwise just List<Vector2> if using getEdgePoints
        points = new List<List<Vector2>>();
        // points = new List<Vector2>();
        edges = new List<Edge>();
        MeshFilter meshFilter = node.GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            Debug.Log("[Error] No Mesh Filter!");
            return;
        }
        //use if using the getEdgePoints method below instead of mapgen.edgepoints
        // points = getEdgePoints(meshFilter.mesh.vertices, meshFilter.mesh.triangles);
        // foreach(Vector2 a in points)
        // {
        //     Debug.DrawLine(a, new Vector2(a.x+0.1f, a.y), Color.green, 100);
        // }
        //-------------------------------------------------------------------------------
        points = mapGen.edgePoints;
        foreach(List<Vector2> list in points)
        {
            foreach(Vector2 a in list)
            {
                Debug.DrawLine(a, new Vector2(a.x+0.1f, a.y), Color.green, 100);
            }
        }
    }

    //Does the same thing as mapgen.edgePoints attribute, but this doesn't differentiate between rooms
    //NOT FINISHED, DOESNT RETURN VECTOR2s
    //KEEP FOR NOW!!!
    private List<Vector2> getEdgePoints(Vector3[] vertices, int[] triangles)
    {
        //use the triangles to find which points are connected
        List<Vector2> edgePoints = new List<Vector2>();
        Dictionary<string, KeyValuePair<int, int>> unique = new Dictionary<string, KeyValuePair<int, int>>();
        for(int i = 0; i < triangles.Length; i += 3)
        {
            for(int e = 0; e < 3; e++)
            {
                int vertA = triangles[i+e];
                int vertB = i+e+1 > i+2 ? triangles[i] : triangles[i+e+1];
                string edge = Mathf.Min(vertA, vertB) + ":" + Mathf.Max(vertA, vertB);
                if(unique.ContainsKey(edge))
                    unique.Remove(edge);
                else
                    unique.Add(edge, new KeyValuePair<int, int>(vertA, vertB));
            }
        }
        Dictionary<int, int> edgeVerts = new Dictionary<int, int>();
        foreach(KeyValuePair<int, int> edge in unique.Values)
        {
            if(!edgeVerts.ContainsKey(edge.Key))
                edgeVerts.Add(edge.Key, edge.Value);
            else
                Debug.Log("hello");
        }
        Debug.Log(edgeVerts.Count);
        return edgePoints;
    }

    private void removeDuplicateEdges()
    {
        List<Edge> edgesToBeRemoved = new List<Edge>();
        foreach(Edge a in edges)
        {
            foreach(Edge b in edges)
            {
                if(a == b)
                    continue;
                if((a.a == b.a && a.b == b.b) || (a.b == b.a && a.a == b.b))
                    edgesToBeRemoved.Add(a);
            }
        }
        foreach(Edge removable in edgesToBeRemoved)
            edges.Remove(removable);
    }

    private class Edge
    {
        public int a, b;
        public Edge(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }
}