using System;
using UnityEngine;

namespace BullBrukBruker
{
    public static class PhysicsUtils
    {        
        public static bool TryGetRayIntersectedInformation(Vector3 pos1, Vector3 dir, float rayLength, Vector3 pos2, float halfWidth, float halfHeight, out (Vector3 contactPoint, float distance, Vector3 normal) intersectedInfor, bool preventTunneling = true)
        {
            var minBounds = pos2 - new Vector3(halfWidth, halfHeight, 0);
            var maxBounds = pos2 + new Vector3(halfWidth, halfHeight, 0);

            dir.x = Mathf.Abs(dir.x) < Mathf.Epsilon ? 0.01f : dir.x;
            dir.y = Mathf.Abs(dir.y) < Mathf.Epsilon ? 0.01f : dir.y;

            float tMinX, tMaxX, tMinY, tMaxY;

            // Tính khoảng t theo trục X
            var t1x = (minBounds.x - pos1.x) / dir.x;
            var t2x = (maxBounds.x - pos1.x) / dir.x;
            tMinX = Mathf.Min(t1x, t2x);
            tMaxX = Mathf.Max(t1x, t2x);

            // Tính khoảng t theo trục Y
            var t1y = (minBounds.y - pos1.y) / dir.y;
            var t2y = (maxBounds.y - pos1.y) / dir.y;
            tMinY = Mathf.Min(t1y, t2y);
            tMaxY = Mathf.Max(t1y, t2y);

            var tMin = Mathf.Max(tMinX, tMinY);
            var tMax = Mathf.Min(tMaxX, tMaxY);

            if (!(tMin <= tMax && tMin <= rayLength && tMax >= 0))
            {
                intersectedInfor = (Vector3.zero, 0f, Vector3.zero);
                return false;
            }

            Vector3 contactPoint = pos1 + tMin * dir;
            float distance = tMin * dir.magnitude;
            Vector3 normal = Vector3.zero;

            if (tMin == tMinX)
                normal = dir.x > 0 ? Vector3.left : Vector3.right;
            else if (tMin == tMinY)
                normal = dir.y > 0 ? Vector3.down : Vector3.up;

            intersectedInfor = (contactPoint, distance, normal);
            return true;
        }
           
        public static bool TryGetRayIntersectedInformation(Transform obj, float rayLength, Vector3 pos2, float halfWidth, float halfHeight, out (Vector3 contactPoint, float distance, Vector3 normal) intersectedInfor)
        {
            var minBounds = pos2 - new Vector3(halfWidth, halfHeight, 0);
            var maxBounds = pos2 + new Vector3(halfWidth, halfHeight, 0);

            var pos1 = obj.position;
            var dir = obj.up;

            dir.x = Mathf.Abs(dir.x) < Mathf.Epsilon ? 0.01f : dir.x;
            dir.y = Mathf.Abs(dir.y) < Mathf.Epsilon ? 0.01f : dir.y;

            float tMinX, tMaxX, tMinY, tMaxY;

            // Tính khoảng t theo trục X
            var t1x = (minBounds.x - pos1.x) / dir.x;
            var t2x = (maxBounds.x - pos1.x) / dir.x;
            tMinX = Mathf.Min(t1x, t2x);
            tMaxX = Mathf.Max(t1x, t2x);

            // Tính khoảng t theo trục Y
            var t1y = (minBounds.y - pos1.y) / dir.y;
            var t2y = (maxBounds.y - pos1.y) / dir.y;
            tMinY = Mathf.Min(t1y, t2y);
            tMaxY = Mathf.Max(t1y, t2y);

            var tMin = Mathf.Max(tMinX, tMinY);
            var tMax = Mathf.Min(tMaxX, tMaxY);

            if (!(tMin <= tMax && tMin <= rayLength && tMax >= 0))
            {
                intersectedInfor = (Vector3.zero, 0f, Vector3.zero);
                return false;
            }

            Vector3 contactPoint = pos1 + tMin * dir;
            float distance = tMin * dir.magnitude;
            Vector3 normal = Vector3.zero;

            if (tMin == tMinX)
                normal = dir.x > 0 ? Vector3.left : Vector3.right;
            else if (tMin == tMinY)
                normal = dir.y > 0 ? Vector3.down : Vector3.up;

            intersectedInfor = (contactPoint, distance, normal);
            return true;
        }

        public static (Vector3 contactPoint, float distance, Vector3 normal) GetRayIntersectedInformation(Transform obj, Vector3 pos2, float halfWidth, float halfHeight)
        {
            var pos1 = obj.position;
            var dir = obj.up;

            Vector3 relativeVec = pos1 - pos2;

            dir.x = Mathf.Abs(dir.x) < Mathf.Epsilon ? 0.01f : dir.x;
            dir.y = Mathf.Abs(dir.y) < Mathf.Epsilon ? 0.01f : dir.y;

            float distToRight = halfWidth - relativeVec.x;
            float distToLeft = halfWidth + relativeVec.x;
            float distToTop = halfHeight - relativeVec.y;
            float distToBottom = halfHeight + relativeVec.y;

            float minDist = Mathf.Min(distToRight, distToLeft, distToTop, distToBottom);

            float t = Mathf.Infinity;
            Vector3 norDir = Vector3.zero;

            if (minDist == distToRight)
            {
                t = distToRight / dir.x;
                norDir = Vector3.right;
            }

            else if (minDist == distToLeft)
            {
                t = -distToLeft / dir.x;
                norDir = Vector3.left;
            }

            else if (minDist == distToTop)
            {
                t = distToTop / dir.y;
                norDir = Vector3.up;
            }

            else if (minDist == distToBottom)
            {
                t = -distToBottom / dir.y;
                norDir = Vector3.down;
            }

            return (pos1 + t * dir, (t * dir).magnitude, norDir);
        }
    }
}