using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace SwissArmyKnife
{
    public class Vertex
    {
        public Vector3 mvPos;
        public int miIndex;
        public Color mColor = Color.white;
        public Vector2 mvUV;

        public List<Triangle> mlAdjacentTriangles; 

#if UNITY_EDITOR
        public string DEBUG_INFO = "n/a";
#endif

        // Temporarily used
        public bool mbInside;

        public Vertex()
        {
            // Only exists to make compiler happy ("Use of unassignes...")
            mlAdjacentTriangles = new List<Triangle>();
        }

        public Vertex(int i)
        {
            miIndex = i;
            mlAdjacentTriangles = new List<Triangle>();
        }


        public Vertex(Vector3 vPosition, int i)
        {
            mvPos = vPosition;
            miIndex = i;
            mlAdjacentTriangles = new List<Triangle>();
        }

        public void SetUV(float u, float v)
        {
            mvUV.x = u;
            mvUV.y = v;
        }

        public bool IsUnder(Vector3 vPosition)
        {
            return Mathf.Abs(vPosition.x - mvPos.x) + Mathf.Abs(vPosition.z - mvPos.z) < 0.0001f;
        }

        public bool IsNeighbourOf(Vertex vOtherVertex)
        {
            foreach (Triangle triangle in mlAdjacentTriangles)
            {
                if (vOtherVertex == triangle.mA || vOtherVertex == triangle.mB || vOtherVertex == triangle.mC)
                {
                    return true;
                }
            }
            return false;
        }



        public Vertex GetNeighbourVertexInDirection(Vector3 vDirection)
        {
            vDirection.y = 0.0f;
            Vertex neighbour1 = null;
            Vertex neighbour2 = null;

            foreach (Triangle triangle in mlAdjacentTriangles)
            {
                if (triangle == null)
                {
                    continue;
                }
                // Which are the neighbor points ?
                if (this == triangle.mA)
                {
                    neighbour1 = triangle.mB;
                    neighbour2 = triangle.mC;
                }
                else if (this == triangle.mB)
                {
                    neighbour1 = triangle.mC;
                    neighbour2 = triangle.mA;
                }
                else // this == triangle.mC
                {
                    neighbour1 = triangle.mA;
                    neighbour2 = triangle.mB;
                    // Note that we don't risk to overwrite the right neighbors, as we return as soon as we have found the right ones.
                }


                // Get the arms of the triangle
                Vector3 vArm1 = (neighbour1.mvPos - mvPos);
                Vector3 vArm2 = (neighbour2.mvPos - mvPos);

                // Everything happens in the x-z-plane
                vArm1.y = 0.0f;
                vArm2.y = 0.0f;
                vDirection.y = 0.0f;

                // Check the shortest arm first (in case both are roughly into the same direction)
                if (vArm1.sqrMagnitude > vArm2.sqrMagnitude)
                {
                    // Switch the neighbours and arms
                    Vertex vertexTemp = neighbour1;
                    neighbour1 = neighbour2;
                    neighbour2 = vertexTemp;
                    Vector3 vTemp = vArm1;
                    vArm1 = vArm2;
                    vArm2 = vTemp;
                }

                // Normalize arms and direction
                vArm1.Normalize();
                vArm2.Normalize();
                vDirection.Normalize();

                // Check if the first neighbor is the correct one
                if (Vector3.Dot(vDirection, vArm1) > 0
                   && Mathf.Abs(vArm1.x * vDirection.z - vArm1.z * vDirection.x) <= 0.0001f)
                {
                    return neighbour1;
                }

                // Check if the second neighbor is the correct one
                if (Vector3.Dot(vDirection, vArm2) > 0
                   && Mathf.Abs(vArm2.x * vDirection.z - vArm2.z * vDirection.x) <= 0.0001f)
                {
                    return neighbour2;
                }
            }

            // Sorry, nothing found
            return null;
        }


        public void DebugDrawAdjacentTriangles(Color color)
        {
            Debug.DrawLine(mvPos - Vector3.up, mvPos + Vector3.up, color, 1000.0f);
            foreach (Triangle triangle in mlAdjacentTriangles)
            {
                Debug.Log("Adjacent triangle :");
                triangle.mA.DebugLogInfo("A ", mvPos);
                triangle.mB.DebugLogInfo("B ", mvPos);
                triangle.mC.DebugLogInfo("C ", mvPos);

                Debug.DrawLine(triangle.mA.mvPos, triangle.mB.mvPos, color, 1000.0f);
                Debug.DrawLine(triangle.mB.mvPos, triangle.mC.mvPos, color, 1000.0f);
                Debug.DrawLine(triangle.mC.mvPos, triangle.mA.mvPos, color, 1000.0f);
            }
        }



        public void DebugLogInfo(string strName, Vector3 vOffset)
        {
#if UNITY_EDITOR
            Debug.Log(strName + " [" + miIndex + "] " + DEBUG_INFO + " at " + (mvPos - vOffset) * 10000000);
#endif
        }

        public Triangle GetAdjacentTriangleInDirection(Vector3 vDirection, out Vertex neighbour1, out Vertex neighbour2)
        {
            vDirection.y = 0.0f;

            Vector3 vNormal = new Vector3(vDirection.z, 0.0f, -vDirection.x);
            foreach (Triangle triangle in mlAdjacentTriangles)
            {
                if (triangle == null)
                {
                    continue;
                }

                // Which are the neighbor points ?
                if (this == triangle.mA)
                {
                    neighbour1 = triangle.mB;
                    neighbour2 = triangle.mC;
                }
                else if (this == triangle.mB)
                {
                    neighbour1 = triangle.mC;
                    neighbour2 = triangle.mA;
                }
                else // this == triangle.mC
                {
                    neighbour1 = triangle.mA;
                    neighbour2 = triangle.mB;
                    // Note that we don't risk to overwrite the right neighbors, as we return as soon as we have found the right ones.
                }

                // Compute the two "arms" parting from this point
                Vector3 vArm1 = (neighbour1.mvPos - mvPos);
                Vector3 vArm2 = (neighbour2.mvPos - mvPos);

                // Flatten and normalize
                vArm1.y = 0.0f;
                vArm1.Normalize();
                vArm2.y = 0.0f;
                vArm2.Normalize();

                // Is the vector on the right halfplane?
                // Are the two arms on different sides of the vector? 
                // (The latter ones means that Dot( vNormal, vArm1) has a different sign than Dot( vNormal, vArmZ). In other words, their product is < 0. We include the case "=0" out of kindness.)
                if (Vector3.Dot(vDirection, vArm1 + vArm2) > 0
                   && Vector3.Dot(vNormal, vArm1) * Vector3.Dot(vNormal, vArm2) <= 0)
                {
                    // We have found the correct triangle
                    if (neighbour1 == neighbour2)
                    {
                        Debug.LogError("neighbour1 == neighbour2");
                    }
                    else if (neighbour1.mvPos == neighbour2.mvPos)
                    {
                        Debug.LogError("neighbour1.mvPos == neighbour2.mvPos");
                    }

                    if (Vector3.Dot(vNormal, vArm1) < 0)
                    {
                        Vertex temp = neighbour1;
                        neighbour1 = neighbour2;
                        neighbour2 = temp;
                    }
                    return triangle;
                }
            }

            // Sorry, nothing found
            neighbour1 = null;
            neighbour2 = null;
            return null;
        }


    }
}