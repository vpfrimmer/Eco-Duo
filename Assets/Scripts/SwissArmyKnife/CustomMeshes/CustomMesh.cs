using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SwissArmyKnife
{
    // A class allowing creating custom meshes without the stress of keeping track of indices.

    // Use it as follows:
    // - Create a new CustomMesh
    // - Add vertices with AddVertex(...)
    // - Add triangles with Addtriangle or AddQuadrangle, referencing the vertices
    // - Use AttachMeshTo to create the actual MeshFilter and attach it to a game object
    // - If you want a mesh collider, use AddMeshCollider

    public class CustomMesh
    {
        protected List<Vertex> mlVertices;
        protected List<Triangle> mlTriangles;

        public CustomMesh()
        {
            mlVertices = new List<Vertex>();
            mlTriangles = new List<Triangle>();
        }

        // Constructor to clone existing mesh
        public CustomMesh(MeshFilter meshFilterOriginal)
        {
            // Retrieve data from original mesh
            Vector3[] aVertices = meshFilterOriginal.mesh.vertices;
            Vector2[] aUV = meshFilterOriginal.mesh.uv;
            int[] aTriangles = meshFilterOriginal.mesh.triangles;

            // Copy vertices
            mlVertices = new List<Vertex>();
            for (int i = 0; i < aVertices.Length; i++)
            {
                Vertex vertex = new Vertex(mlVertices.Count);
                vertex.mvPos = aVertices[i];
                vertex.mvUV = aUV[i];
#if UNITY_EDITOR
                vertex.DEBUG_INFO = "Vertex from original mesh";
#endif
                mlVertices.Add(vertex);
            }

            // Copy triangles
            mlTriangles = new List<Triangle>();
            for (int i = 0; i < aTriangles.Length; i += 3)
            {
                Vertex A = mlVertices[aTriangles[i]];
                Vertex B = mlVertices[aTriangles[i + 1]];
                Vertex C = mlVertices[aTriangles[i + 2]];
                AddTriangle(A, B, C);
            }
        }

        // Constructor to clone existing custom mesh
        public CustomMesh(CustomMesh customMeshOriginal)
        {
            // Copy vertices
            mlVertices = new List<Vertex>();
            for (int i = 0; i < customMeshOriginal.mlVertices.Count; i++)
            {
                Vertex vertex = new Vertex(mlVertices.Count);
                vertex.mvPos = customMeshOriginal.mlVertices[i].mvPos;
                vertex.mvUV = customMeshOriginal.mlVertices[i].mvUV;
                mlVertices.Add(vertex);
            }

            // Copy triangles
            mlTriangles = new List<Triangle>();
            for (int i = 0; i < customMeshOriginal.mlTriangles.Count; i += 3)
            {
                Triangle originalTriangle = customMeshOriginal.mlTriangles[i];
                Vertex A = AddVertex(originalTriangle.mA);
                Vertex B = AddVertex(originalTriangle.mB);
                Vertex C = AddVertex(originalTriangle.mC);
                AddTriangle(A, B, C);
            }
        }

        public Vertex FindVertexUnder(Vector3 vPosition)
        {
            for (int v = mlVertices.Count - 1; v >= 0; v--)
            {
                Vertex vertex = mlVertices[v];
                if (vertex.IsUnder(vPosition))
                {
                    return vertex;
                }
            }
            return null;
        }

        public Vertex AddVertex()
        {
            Vertex vertex = new Vertex(mlVertices.Count);
            mlVertices.Add(vertex);
            return vertex;
        }


        public Vertex AddVertex(Vector3 vPosition)
        {
            Vertex vertex = new Vertex(vPosition, mlVertices.Count);
            mlVertices.Add(vertex);
            return vertex;
        }

        public Vertex AddVertex(Vertex original)
        {
            Vertex vertex = new Vertex(original.mvPos, mlVertices.Count);
            mlVertices.Add(vertex);
            return vertex;
        }

        public virtual Triangle AddTriangle(Vertex A, Vertex B, Vertex C, bool bInverted = false)
        {
            if (bInverted)
            {
                return AddTriangle(A, C, B);
            }
            Triangle triangle = new Triangle(A, B, C, mlTriangles.Count);
            mlTriangles.Add(triangle);
            return triangle;
        }


        public void AddQuadrangle(Vertex A, Vertex B, Vertex C, Vertex D, bool bInverted = false)
        {
            AddTriangle(A, B, C, bInverted);
            AddTriangle(A, C, D, bInverted);
        }


        public void RemoveTriangle(Triangle triangle)
        {
            triangle.mA.mlAdjacentTriangles.Remove(triangle);
            triangle.mB.mlAdjacentTriangles.Remove(triangle);
            triangle.mC.mlAdjacentTriangles.Remove(triangle);
            mlTriangles.Remove(triangle);
        }

        public virtual void RemoveTriangle(int t)
        {
             mlTriangles.RemoveAt(t);
        }


        public List<Vertex> Vertices
        {
            get
            {
                return mlVertices;
            }
        }

        public List<Triangle> Triangles
        {
            get
            {
                return mlTriangles;
            }
        }


        public void AttachMeshTo(GameObject gObject, Material material)
        {
            // Create components
            if (gObject.GetComponent<MeshFilter>() == null)
            {
                gObject.AddComponent<MeshFilter>();
            }
            if (gObject.GetComponent<MeshRenderer>() == null)
            {
                gObject.AddComponent<MeshRenderer>();
            }
            if (gObject.GetComponent<MeshCollider>() == null)
            {
                gObject.AddComponent<MeshCollider>();
            }

            // Create arrays
            Vector3[] aVertices = new Vector3[mlVertices.Count];
            Color[] aVertexColors = new Color[mlVertices.Count];
            Vector2[] aUV = new Vector2[mlVertices.Count];
            int[] aTriangles = new int[mlTriangles.Count * 3];

            // Populate arrays
            for (int v = 0; v < mlVertices.Count; v++)
            {
                aVertices[v] = mlVertices[v].mvPos;
                aVertexColors[v] = mlVertices[v].mColor;
                aUV[v] = mlVertices[v].mvUV;
            }
            for (int t = 0; t < mlTriangles.Count; t++)
            {
                Triangle triangle = mlTriangles[t];
                aTriangles[3 * t + 0] = triangle.mA.miIndex;
                aTriangles[3 * t + 1] = triangle.mB.miIndex;
                aTriangles[3 * t + 2] = triangle.mC.miIndex;
            }

            // Assign arrays
            MeshFilter meshfilter = gObject.GetComponent<MeshFilter>();
            meshfilter.mesh.triangles = new int[0]; // Otherwise the next line may cause an error "not enough vertices"
            meshfilter.mesh.vertices = aVertices;
            meshfilter.mesh.colors = aVertexColors;
            meshfilter.mesh.uv = aUV;
            meshfilter.mesh.triangles = aTriangles;
            meshfilter.mesh.RecalculateNormals();
            meshfilter.mesh.RecalculateBounds();


            gObject.GetComponent<MeshRenderer>().material = material;
        }

        public void AddMeshCollider(GameObject gObject)
        {
            if (gObject.GetComponent<MeshCollider>() == null)
            {
                gObject.AddComponent<MeshCollider>();
            }

            gObject.GetComponent<MeshCollider>().sharedMesh = null;
            gObject.GetComponent<MeshCollider>().sharedMesh = gObject.GetComponent<MeshFilter>().mesh;
        }

    }

}
