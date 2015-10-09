using UnityEngine;
using System.Collections;

namespace SwissArmyKnife
{
    public class Triangle
    {
        public Vertex mA;
        public Vertex mB;
        public Vertex mC;
        public int miIndex;

        private float mfXMin;
        private float mfXMax;
        private float mfZMin;
        private float mfZMax;

        private Vector3 mvXZNormalA;
        private Vector3 mvXZNormalB;
        private Vector3 mvXZNormalC;


        public int CornersWithInsideFlag
        {
            get
            {
                return (mA.mbInside ? 1 : 0) + (mB.mbInside ? 1 : 0) + (mC.mbInside ? 1 : 0);
            }
        } 

        public Triangle(Vertex A, Vertex B, Vertex C, int i)
        {
            mA = A;
            mB = B;
            mC = C;
            miIndex = i;
            mfXMin = Mathf.Min(mA.mvPos.x, mB.mvPos.x, mC.mvPos.x);
            mfXMax = Mathf.Max(mA.mvPos.x, mB.mvPos.x, mC.mvPos.x);
            mfZMin = Mathf.Min(mA.mvPos.z, mB.mvPos.z, mC.mvPos.z);
            mfZMax = Mathf.Max(mA.mvPos.z, mB.mvPos.z, mC.mvPos.z);

            mvXZNormalA = new Vector3(mA.mvPos.z - mC.mvPos.z, 0.0f, mC.mvPos.x - mA.mvPos.x);
            mvXZNormalB = new Vector3(mB.mvPos.z - mA.mvPos.z, 0.0f, mA.mvPos.x - mB.mvPos.x);
            mvXZNormalC = new Vector3(mC.mvPos.z - mB.mvPos.z, 0.0f, mB.mvPos.x - mC.mvPos.x);
        }

        // For quick bounding box checks
        public bool IsFarFromXZ(Vector3 vPoint)
        {
            return (vPoint.x < mfXMin - 0.0001f || vPoint.x > mfXMax + 0.0001f || vPoint.z < mfZMin - 0.0001f || vPoint.z > mfZMax + 0.0001f);
        }

        public bool BorderContainsXZ(Vector3 vPoint, out Vertex P, out Vertex Q)
        {
            // Check AB
            if (GeometryToolbox.IsOverSegment(vPoint, mA.mvPos, mB.mvPos))
            {
                P = mA;
                Q = mB;
                return true;
            }

            // Check AC
            if (GeometryToolbox.IsOverSegment(vPoint, mA.mvPos, mC.mvPos))
            {
                P = mA;
                Q = mC;
                return true;
            }


            // Check BC
            if (GeometryToolbox.IsOverSegment(vPoint, mB.mvPos, mC.mvPos))
            {
                P = mB;
                Q = mC;
                return true;
            }

            P = null;
            Q = null;
            return false;

        }


        public bool ContainsXZ(Vector3 vPoint)
        {
            // Point is inside iff it's in the three half-planes
            return (Vector3.Dot(vPoint - mA.mvPos, mvXZNormalA) > 0
                   && Vector3.Dot(vPoint - mB.mvPos, mvXZNormalB) > 0
                   && Vector3.Dot(vPoint - mC.mvPos, mvXZNormalC) > 0);
        }

        public bool IntersectsXZ(Vector3 vP, Vector3 vQ)
        {
            Vector3 vA = mA.mvPos;
            Vector3 vB = mB.mvPos;
            Vector3 vC = mC.mvPos;

            return (GeometryToolbox.SegmentsIntersectXZ(vP, vQ, vA, vB)
                    || GeometryToolbox.SegmentsIntersectXZ(vP, vQ, vA, vC)
                    || GeometryToolbox.SegmentsIntersectXZ(vP, vQ, vB, vC));
        }

        public bool HasCorner(Vertex P)
        {
            return (P == mA || P == mB || P == mC);

        }

        public Vertex GetThirdCorner(Vertex notThisOne, Vertex notThisOneEither)
        {
            if (notThisOne == mA)
            {
                if (notThisOneEither == mB)
                {
                    return mC;
                }
                else
                {
                    return mB;
                }
            }
            else if (notThisOne == mB)
            {
                if (notThisOneEither == mA)
                {
                    return mC;
                }
                else
                {
                    return mA;
                }
            }
            else // notThisOne == mC
            {
                if (notThisOneEither == mB)
                {
                    return mA;
                }
                else
                {
                    return mB;
                }
            }

        }

        public Triangle GetNeighbourTriangleBehind(Vertex P, Vertex Q)
        {
            foreach (Triangle triangle in P.mlAdjacentTriangles)
            {
                if (triangle != this && triangle.HasCorner(Q))
                {
                    return triangle;
                }
            }

            return null;
        }


    }
}
