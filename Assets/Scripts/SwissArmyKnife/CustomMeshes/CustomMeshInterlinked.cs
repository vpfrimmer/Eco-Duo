using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SwissArmyKnife
{
    // A custom mesh with additional information on which triangle is adjacent to which vertex.
    // Allows "punching out" horizontal polygons (XZPolygon)
    public class CustomMeshInterlinked : CustomMesh
    {
        private const float MAX_TIME_CHUNK = 0.03f;
        private Vertex[] maVerticesUnderPolygonPoints;

        public CustomMeshInterlinked(MeshFilter meshFilterOriginal)
            : base(meshFilterOriginal)
        {
        }

        public CustomMeshInterlinked(CustomMesh customMeshOriginal)
            : base(customMeshOriginal)
        {
        }

        public override Triangle AddTriangle(Vertex A, Vertex B, Vertex C, bool bInverted = false)
        {
            if (bInverted)
            {
                return AddTriangle(A, C, B);
            }
            Triangle triangle = new Triangle(A, B, C, mlTriangles.Count);
            mlTriangles.Add(triangle);

            // The following three lines are the very reason CustomMeshInterlinked exists
            // We don't need this in most meshes, only when we want to do a PunchOut operation.
            // Also, it requires unnecessary CPU time and memory, which may lead to a nasty crash in iOS due to a UNITY bug. 
            A.mlAdjacentTriangles.Add(triangle);
            B.mlAdjacentTriangles.Add(triangle);
            C.mlAdjacentTriangles.Add(triangle);
            return triangle;
        }

        public override void RemoveTriangle(int t)
        {
            Triangle triangle = mlTriangles[t];
            triangle.mA.mlAdjacentTriangles.Remove(triangle);
            triangle.mB.mlAdjacentTriangles.Remove(triangle);
            triangle.mC.mlAdjacentTriangles.Remove(triangle);

            mlTriangles.RemoveAt(t);
        }


        //==============================================================================================
        // 
        // Punch out methods
        //
        //==============================================================================================

        // Note on terminology:
        // In the following algorithms, we will say that
        // - A grid has VERTICES and TRIANGLES
        // - A polygon has POINTS and SEGMENTS
        // - A triangle has CORNERS and SIDES

        public void PunchOut(XZPolygon polygon, bool bKeepInterior)
        {
            PunchOutStep1(polygon);
            PunchOutStep2(polygon);
            PunchOutStep3(polygon, bKeepInterior);
        }


        private IEnumerator PunchOutStep1(XZPolygon polygon)
        {
            Profiler.BeginSample("PunchOutStep1");

            float fTimeWhenStartedChunk = Time.realtimeSinceStartup;
            maVerticesUnderPolygonPoints = new Vertex[polygon.maPoints.Length - 1];

            // (1) For each polygon point, determine which triangle it's in, and split that triangle into three such that the point is on a new vertex
            for (int p = 0; p < polygon.maPoints.Length - 1; p++)  //Length-1 because it's a closed polyline
            {
                // Coroutine time management
                if (Time.realtimeSinceStartup - fTimeWhenStartedChunk > MAX_TIME_CHUNK)
                {

                    // Enough done this frame, go on holiday
                    yield return null;

                    // Back from holyday
                    fTimeWhenStartedChunk = Time.realtimeSinceStartup;
                }



                Vector3 vPoint = polygon.maPoints[p];

                if (p > 0 && (vPoint - polygon.maPoints[p - 1]).magnitude < 0.0001f)
                {
                    Debug.Log("Polygon has two very near points. Distance is " + (vPoint - polygon.maPoints[p - 1]).magnitude);
                }

                if (p > 0)
                {
                    // Debug.DrawLine(polygon.maPoints[p - 1], polygon.maPoints[p], Color.yellow, 1000.0f);
                }

                for (int t = 0; t < mlTriangles.Count; t++)
                {
                    Triangle triangle = mlTriangles[t];

                    Vertex P = null;
                    Vertex Q = null;
                    if (triangle.IsFarFromXZ(vPoint))
                    {
                        // NOP - Continue
                    }
                    else if (triangle.mA.IsUnder(vPoint) || triangle.mB.IsUnder(vPoint) || triangle.mC.IsUnder(vPoint))
                    {
                        // We are already over a vertex, yay
                        if (triangle.mA.IsUnder(vPoint))
                        {
                            maVerticesUnderPolygonPoints[p] = triangle.mA;
                            // Debug.DrawLine(triangle.mA.mvPos - 0.01f * Vector3.up, triangle.mA.mvPos + 0.01f * Vector3.up, Color.grey, 1000.0f);
                        }
                        else if (triangle.mB.IsUnder(vPoint))
                        {
                            maVerticesUnderPolygonPoints[p] = triangle.mB;
                            // Debug.DrawLine(triangle.mB.mvPos - 0.01f * Vector3.up, triangle.mB.mvPos + 0.01f * Vector3.up, Color.grey, 1000.0f);
                        }
                        else if (triangle.mC.IsUnder(vPoint))
                        {
                            maVerticesUnderPolygonPoints[p] = triangle.mC;
                            // Debug.DrawLine(triangle.mC.mvPos - 0.01f * Vector3.up, triangle.mC.mvPos + 0.01f * Vector3.up, Color.grey, 1000.0f);
                        }
                        break;


                    }
                    else if (triangle.BorderContainsXZ(vPoint, out P, out Q))
                    {
                        // We are on an edge
                        //             P---------------R
                        //           /  \ triangleBis//
                        //         /     \         / /
                        //       /        \     /   /
                        //     / triangle  \  /    /
                        //   S -_- - - - - -X     /
                        //        -  _       \   /
                        //              -  _  \ /
                        //                   - Q
                        //

                        //Debug.Log ("p="+p+": Splitting edge");

                        // Compute where X is
                        Vector3 vPQ = Q.mvPos - P.mvPos;
                        vPQ.y = 0.0f;
                        Vector3 vPX = vPoint - P.mvPos;
                        vPX.y = 0.0f;
                        float fCutRatio = vPX.magnitude / vPQ.magnitude;

                        // Add new vertex
                        Vertex X = AddVertex(Vector3.Lerp(P.mvPos, Q.mvPos, fCutRatio));
#if UNITY_EDITOR
                        X.DEBUG_INFO = "added on edge in step1";
#endif
                        maVerticesUnderPolygonPoints[p] = X;

                        // Debug.DrawLine(X.mvPos - 0.01f * Vector3.up, X.mvPos + 0.01f * Vector3.up, Color.white, 1000.0f);

                        // Interpolate UV coordinates
                        X.mvUV = Vector2.Lerp(P.mvUV, Q.mvUV, fCutRatio);

                        //Get the point S
                        Vertex S = triangle.GetThirdCorner(P, Q);

                        // Before we kill triangle, check if there's a second triangle to subdivide
                        Triangle triangleBis = triangle.GetNeighbourTriangleBehind(P, Q);

                        // Supdivide triangle
                        RemoveTriangle(t);
                        bool bFlip = (Vector3.Cross(P.mvPos - S.mvPos, Q.mvPos - S.mvPos).y > 0);
                        AddTriangle(S, X, P, bFlip);
                        AddTriangle(S, Q, X, bFlip);

                        if (triangleBis != null)
                        {
                            Vertex R = triangleBis.GetThirdCorner(P, Q);
                            RemoveTriangle(triangleBis);
                            AddTriangle(P, X, R, bFlip);
                            AddTriangle(X, Q, R, bFlip);
                        }
                        break;
                    }
                    else if (triangle.ContainsXZ(vPoint))
                    {
                        //Debug.Log ("p="+p+": Splitting triangle");

                        // We have found the triangle
                        //               B     
                        //              / \
                        //            /  | \
                        //          /   |   \
                        //        /    |     \      
                        //      /    _ P' _   \    
                        //    /_ - -       -  _\    
                        //  A-------------------C   

                        // Retrieve the existing vertices
                        Vertex A = triangle.mA;
                        Vertex B = triangle.mB;
                        Vertex C = triangle.mC;

                        // We want to project P vertically (not orthogonally) onto a point P' on the ABC plane.
                        // The problem is choosing the right altitude.
                        // If we set U = P-A, V = C-A, W = B-A, we are looking for lambda, my, such that P' = A + lambda V + my W has the same x and z coordinates as P.
                        // This comes back to resolving the linear equation system
                        //
                        //   lambda V.x + my * W.x == U.x
                        //   lambda V.z + my * W.z == U.z  , or in matrix form
                        // 
                        //   [V.x   W.x]   ( lambda )   ( U.x )  
                        //   [V.z   W.z] * (   my   ) = ( U.z )
                        //
                        //   We resolve for the vector on the left:
                        //
                        //    ( lambda )    [V.x   W.x]-1   ( U.x )           [ W.z  -W.x]   ( U.x ) 
                        //    (   my   ) =  [V.z   W.z]   * ( U.z ) = 1/det * [-V.z   V.x] * ( U.z )
                        //
                        // So all we have to do is invert a 2x2 matrix. That should not be a problem.

                        // Initialize U, V, W
                        Vector3 vU = vPoint - A.mvPos;
                        Vector3 vV = C.mvPos - A.mvPos;
                        Vector3 vW = B.mvPos - A.mvPos;

                        // Compute the determinant
                        float fDet = vV.x * vW.z - vV.z * vW.x;
                        if (fDet == 0.0f)
                        {
                            // Degenerated triangle
                            Debug.LogError("Degenerated triangle");
                            continue;
                        }

                        // Resolve for lambda and my
                        float fLambda = (1.0f / fDet) * (vW.z * vU.x - vW.x * vU.z);
                        float fMy = (1.0f / fDet) * (-vV.z * vU.x + vV.x * vU.z);

                        // Now we can apply lambda and my
                        Vector3 vPprime = A.mvPos + fLambda * vV + fMy * vW;

                        // Add a vertex at P'
                        Vertex newVertex = AddVertex(vPprime);
#if UNITY_EDITOR
                        newVertex.DEBUG_INFO = "added in triangle in step 1";
#endif
                        maVerticesUnderPolygonPoints[p] = newVertex;

                        // Debug.DrawLine(newVertex.mvPos - 0.01f * Vector3.up, newVertex.mvPos + 0.01f * Vector3.up, Color.black, 1000.0f);

                        // Interpolate UV coordinates
                        newVertex.mvUV = A.mvUV + fLambda * (C.mvUV - A.mvUV) + fMy * (B.mvUV - A.mvUV);

                        // Replace the triangle
                        RemoveTriangle(t);
                        AddTriangle(C, A, newVertex);
                        AddTriangle(A, B, newVertex);
                        AddTriangle(B, C, newVertex);

                        // Stop the inner loop
                        break;
                    }


                }
            }
            Profiler.EndSample();
        }

        private IEnumerator PunchOutStep2(XZPolygon polygon)
        {
            Profiler.BeginSample("PunchOutStep2");

            float fTimeWhenStartedChunk = Time.realtimeSinceStartup;

            // (2) Create additional mesh lines corresponding to the polygon lines
            int iSegmentIndex = 0;
            Vertex currentVertex = FindVertexUnder(polygon.maPoints[0]);
            int iSecurityCounter = 0;

            while (iSegmentIndex < polygon.maPoints.Length - 1 && iSecurityCounter++ < 10000)
            {
                // Coroutine time management
                if (Time.realtimeSinceStartup - fTimeWhenStartedChunk > MAX_TIME_CHUNK)
                {

                    // Enough done this frame, go on holiday
                    yield return null;

                    // Back from holyday
                    fTimeWhenStartedChunk = Time.realtimeSinceStartup;
                }


                //                            P---------------R
                //                          /  \ triangle2  //
                //                        /     \         / /
                //                \     /        \     /   /
                //                 \  / triangle1 \  /    /
                //    currentVertex * -------------X--   /
                //                  |\ ' -  _       \   /
                //                  | \        -  _  \ /
                //                                  - Q
                //



                Vector3 vPosition = currentVertex.mvPos;
                Vector3 vDirection = polygon.maPoints[iSegmentIndex + 1] - vPosition;

                // A saveguard for some nasty infinitely-go-and-fro-between-two-vertices cases
                while (Vector3.Dot(vDirection, polygon.maPoints[iSegmentIndex + 1] - polygon.maPoints[iSegmentIndex]) < 0)
                {
                    Debug.Log("Wait, wait, we're going into the wrong direction");
                    iSegmentIndex++;
                    vDirection = polygon.maPoints[iSegmentIndex + 1] - vPosition;
                }
                vDirection.y = 0.0f;

                if (iSecurityCounter == 9999)
                {
                    Debug.DrawLine(vPosition - Vector3.up, vPosition + Vector3.up, Color.magenta, 1000.0f);
                    Debug.Log("Endless loop?");
                    Debug.Break();
                }

                if (iSecurityCounter > 9980)
                {
                    Debug.DrawLine(vPosition - Vector3.up * 0.5f, vPosition + Vector3.up * 0.5f, Color.white, 1000.0f);
                    currentVertex.DebugLogInfo("Current vertex", Vector3.zero);
                }

                if (vDirection * 1.0e6f == Vector3.zero)
                {
                    Debug.LogError("vDirection is zero. ");
                }


                if (iSegmentIndex + 1 < maVerticesUnderPolygonPoints.Length && maVerticesUnderPolygonPoints[iSegmentIndex + 1].IsNeighbourOf(currentVertex))
                {
                    currentVertex = maVerticesUnderPolygonPoints[iSegmentIndex + 1];
                    iSegmentIndex++;
                }
                else
                {
                    // Check if there is a neighbor vertex just in this direction
                    Vertex N = currentVertex.GetNeighbourVertexInDirection(vDirection);


                    if (N != null)
                    {
                        // No subdivision required, just procede to vertex N

                        if (N.IsUnder(polygon.maPoints[iSegmentIndex + 1]))
                        {
                            // We reach a corner
                            iSegmentIndex++;

                        }

                        currentVertex = N;

                    }
                    else
                    {

                        // Search the vertices P, Q and the triangles triangle1 and triangle2
                        Vertex P;
                        Vertex Q;
                        Triangle triangle1 = currentVertex.GetAdjacentTriangleInDirection(vDirection, out P, out Q);
                        if (triangle1 == null)
                        {
                            Debug.LogError("Couldn't find triangle under polygon.");
                            currentVertex.DebugLogInfo("Current vertex ", Vector3.zero);
                            currentVertex.DebugDrawAdjacentTriangles(Color.red);
                            iSegmentIndex = polygon.maPoints.Length;
                            break;
                        }
                        Triangle triangle2 = triangle1.GetNeighbourTriangleBehind(P, Q);
                        if (triangle2 == null)
                        {
                            Debug.LogError("Couldn't find triangle 'behind' the one under polygon");
                            currentVertex.DebugDrawAdjacentTriangles(Color.yellow);
                            iSegmentIndex = polygon.maPoints.Length;
                            break;
                        }
                        Vertex R = triangle2.GetThirdCorner(P, Q);

                        //Compute X
                        float fCutRatio;
                        Vector3 vX = GeometryToolbox.CutSegmentByLine(P.mvPos, Q.mvPos, vPosition, vPosition + vDirection, out fCutRatio);


                        // Sometimes the cut ratio is very close to 0 or 1, even if this should theoretically not happen. In this case we do the same thing as above in the case "N != null"
                        if (fCutRatio < 0.001f)
                        {
                            // Don't need a new point, P is fine
                            if (P.IsUnder(polygon.maPoints[iSegmentIndex + 1]))
                            {
                                // We reach a corner
                                iSegmentIndex++;
                            }
                            currentVertex = P;
                        }
                        else if (fCutRatio > 0.999f)
                        {
                            // Same for Q
                            if (Q.IsUnder(polygon.maPoints[iSegmentIndex + 1]))
                            {
                                // We reach a corner
                                iSegmentIndex++;
                            }
                            currentVertex = Q;
                        }
                        else
                        {


                            // Add vertex
                            Vertex X = AddVertex(vX);
                            X.mvUV = Vector2.Lerp(P.mvUV, Q.mvUV, fCutRatio);

#if UNITY_EDITOR
                            X.DEBUG_INFO = "added on line in step 2, fCutRatio = " + fCutRatio;
#endif

                            // Replace triangle1
                            RemoveTriangle(triangle1);
                            AddTriangle(currentVertex, X, P);
                            AddTriangle(currentVertex, Q, X);

                            // Replace triangle2
                            RemoveTriangle(triangle2);
                            AddTriangle(X, Q, R);
                            AddTriangle(X, R, P);


                            // Go on
                            if ((currentVertex.mvPos - P.mvPos).magnitude < 0.00001f)
                            {
                                //currentVertex vertex is very near to P - goto on.
                                currentVertex = P;
                            }
                            else if ((currentVertex.mvPos - Q.mvPos).magnitude < 0.00001f)
                            {
                                // Idem for Q
                                currentVertex = Q;
                            }
                            else if (R.IsUnder(polygon.maPoints[iSegmentIndex + 1]))
                            {
                                // We reach a corner
                                currentVertex = R;
                                iSegmentIndex++;
                            }
                            else
                            {
                                currentVertex = X;
                            }

                            while (iSegmentIndex + 1 < polygon.maPoints.Length && currentVertex.IsUnder(polygon.maPoints[iSegmentIndex + 1]))
                            {
                                iSegmentIndex++;
                            }
                        }
                    }
                }
                //  Debug.DrawLine( vPosition, currentVertex.mvPos, Color.red, 1000.0f);
            }

            // maVerticesUnderPolygonPoints is not needed anymore
            maVerticesUnderPolygonPoints = new Vertex[0];


            Profiler.EndSample();
        }

        private IEnumerator PunchOutStep3(XZPolygon polygon, bool bKeepInterior)
        {
            Profiler.BeginSample("PunchOutStep3");

            float fTimeWhenStartedChunk = Time.realtimeSinceStartup;

            //*/
            // (3) Now there are no more "border-crossing" triangles, they are all wholly outside or wholly inside. 
            //  Remove the triangles inside (or ouside, depending on  bKeepInterior )
            for (int t = 0; t < mlTriangles.Count; t++)
            {
                // Coroutine time management
                if (Time.realtimeSinceStartup - fTimeWhenStartedChunk > MAX_TIME_CHUNK)
                {

                    // Enough done this frame, go on holiday
                    yield return null;

                    // Back from holyday
                    fTimeWhenStartedChunk = Time.realtimeSinceStartup;
                }

                Triangle triangle = mlTriangles[t];
                Vector3 vCenter = (triangle.mA.mvPos + triangle.mB.mvPos + triangle.mC.mvPos) / 3;

                Profiler.BeginSample("IsInside");
                bool bInside = polygon.Contains(vCenter);
                Profiler.EndSample();

                if (bInside != bKeepInterior)
                {
                    // Cut out this triangle
                    Profiler.BeginSample("RemoveTriangle");
                    RemoveTriangle(t);
                    t--;
                    Profiler.EndSample();
                }
            }
            //*/	

            Profiler.EndSample();
        }


        public void PunchOutAndInitialize(MonoBehaviour caller, XZPolygon polygon, bool bKeepInterior, GameObject zeObject, string strName, string strLayer, CustomMesh customMesh, Material material, bool bCollider)
        {
            caller.StartCoroutine(CoroutinePunchOutAndInitialize(caller, polygon, bKeepInterior, zeObject, strName, strLayer, customMesh, material, bCollider));

        }

        public IEnumerator CoroutinePunchOutAndInitialize(MonoBehaviour caller, XZPolygon polygon, bool bKeepInterior, GameObject zeObject, string strName, string strLayer, CustomMesh customMesh, Material material, bool bCollider)
        {
            float T = Time.time;


            yield return null;

            yield return caller.StartCoroutine(PunchOutStep1(polygon));

            Debug.Log("(1)");

            yield return null;

            Debug.Log("(2) Dt=" + (Time.time - T));
            T = Time.time;

            yield return caller.StartCoroutine(PunchOutStep2(polygon));

            yield return null;

            Debug.Log("(3) Dt=" + (Time.time - T));
            T = Time.time;

            yield return caller.StartCoroutine(PunchOutStep3(polygon, bKeepInterior));

            yield return null;

            Debug.Log("(4) Dt=" + (Time.time - T));
            T = Time.time;

            if (zeObject == null)
            {
                zeObject = new GameObject(strName);
                zeObject.transform.parent = caller.transform;
                zeObject.transform.localPosition = Vector3.zero;
                zeObject.transform.localScale = Vector3.one;
                zeObject.layer = LayerMask.NameToLayer(strLayer);
            }
            customMesh.AttachMeshTo(zeObject, material);
            if (bCollider)
            {
                customMesh.AddMeshCollider(zeObject);
            }

            yield return null;

            Debug.Log("(5) Dt=" + (Time.time - T));
            T = Time.time;

            caller.gameObject.SendMessage("OnPunchOutDone", zeObject);
        }


    }
}
