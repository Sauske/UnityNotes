using UnityEngine;

namespace UMI.FrameCommand
{
    public class IntMath
    {
        public static long Divide(long a, long b)
        {
            long neg = (long)(((ulong)(a ^ b) & 0x8000000000000000) >> 63);
            long sign = neg * -2 + 1;

            return (a + (b / 2 * sign)) / b;
        }

        public static int Divide(int a, int b)
        {
            int neg = (int)(((uint)(a ^ b) & 0x80000000) >> 31);
            int sign = neg * -2 + 1;

            return (a + (b / 2 * sign)) / b;
        }

        public static VInt3 Divide(VInt3 a, long m, long b)
        {
            a.x = (int)Divide(a.x * m, b);
            a.y = (int)Divide(a.y * m, b);
            a.z = (int)Divide(a.z * m, b);
            return a;
        }

        public static VInt2 Divide(VInt2 a, long m, long b)
        {
            a.x = (int)Divide(a.x * m, b);
            a.y = (int)Divide(a.y * m, b);
            return a;
        }

        public static VInt3 Divide(VInt3 a, int b)
        {
            a.x = Divide(a.x, b);
            a.y = Divide(a.y, b);
            a.z = Divide(a.z, b);
            return a;
        }

        public static VInt3 Divide(VInt3 a, long b)
        {
            a.x = (int)Divide(a.x, b);
            a.y = (int)Divide(a.y, b);
            a.z = (int)Divide(a.z, b);
            return a;
        }

        public static VInt2 Divide(VInt2 a, long b)
        {
            a.x = (int)Divide(a.x, b);
            a.y = (int)Divide(a.y, b);
            return a;
        }

        public static uint Sqrt32(uint a)
        {
            uint rem = 0;
            uint root = 0;
            int i;

            for (i = 0; i < 16; i++)
            {
                root <<= 1;
                rem <<= 2;
                rem += a >> 30;
                a <<= 2;

                if (root < rem)
                {
                    root++;
                    rem -= root;
                    root++;
                }
            }

            return (root >> 1) & 0xffff;
        }

        public static ulong Sqrt64(ulong a)
        {
            ulong rem = 0;
            ulong root = 0;
            int i;

            for (i = 0; i < 32; i++)
            {
                root <<= 1;
                rem <<= 2;
                rem += a >> 62;
                a <<= 2;

                if (root < rem)
                {
                    root++;
                    rem -= root;
                    root++;
                }
            }

            return (root >> 1) & 0xffffffff;
        }

        public static long SqrtLong(long a)
        {
            if (a <= 0)
                return 0;

            if (a <= uint.MaxValue)
            {
                return Sqrt32((uint)a);
            }
            else
            {
                return (long)Sqrt64((ulong)a);
            }
        }

        public static int Sqrt(long a)
        {
            if (a <= 0)
                return 0;

            if (a <= uint.MaxValue)
            {
                return (int)Sqrt32((uint)a);
            }
            else
            {
                return (int)Sqrt64((ulong)a);
            }
        }

        public static long Clamp(long a, long min, long max)
        {
            if (a < min)
                return min;
            else if (a > max)
                return max;
            else
                return a;
        }

        public static long Max(long a, long b)
        {
            return a > b ? a : b;
        }

        public static VInt3 Transform(ref VInt3 point, ref VInt3 axis_x, ref VInt3 axis_y, ref VInt3 axis_z, ref VInt3 trans)
        {
            return new VInt3(
                IntMath.Divide(axis_x.x * point.x + axis_y.x * point.y + axis_z.x * point.z, 1000) + trans.x,
                IntMath.Divide(axis_x.y * point.x + axis_y.y * point.y + axis_z.y * point.z, 1000) + trans.y,
                IntMath.Divide(axis_x.z * point.x + axis_y.z * point.y + axis_z.z * point.z, 1000) + trans.z
                );
        }

        public static VInt3 Transform(VInt3 point, ref VInt3 axis_x, ref VInt3 axis_y, ref VInt3 axis_z, ref VInt3 trans)
        {
            return new VInt3(
                IntMath.Divide(axis_x.x * point.x + axis_y.x * point.y + axis_z.x * point.z, 1000) + trans.x,
                IntMath.Divide(axis_x.y * point.x + axis_y.y * point.y + axis_z.y * point.z, 1000) + trans.y,
                IntMath.Divide(axis_x.z * point.x + axis_y.z * point.y + axis_z.z * point.z, 1000) + trans.z
                );
        }

        public static VInt3 Transform(ref VInt3 point, ref VInt3 axis_x, ref VInt3 axis_y, ref VInt3 axis_z, ref VInt3 trans, ref VInt3 scale)
        {
            long x = (long)point.x * scale.x;
            long y = (long)point.y * scale.x;
            long z = (long)point.z * scale.x;

            return new VInt3(
                (int)IntMath.Divide(axis_x.x * x + axis_y.x * y + axis_z.x * z, 1000000L) + trans.x,
                (int)IntMath.Divide(axis_x.y * x + axis_y.y * y + axis_z.y * z, 1000000L) + trans.y,
                (int)IntMath.Divide(axis_x.z * x + axis_y.z * y + axis_z.z * z, 1000000L) + trans.z
                );
        }

        public static VInt3 Transform(ref VInt3 point, ref VInt3 forward, ref VInt3 trans)
        {
            VInt3 up = VInt3.up;
            VInt3 right = VInt3.Cross(VInt3.up, forward);

            return Transform(ref point, ref right, ref up, ref forward, ref trans);
        }

        public static VInt3 Transform(VInt3 point, VInt3 forward, VInt3 trans)
        {
            VInt3 up = VInt3.up;
            VInt3 right = VInt3.Cross(VInt3.up, forward);

            return Transform(ref point, ref right, ref up, ref forward, ref trans);
        }

        public static VInt3 Transform(VInt3 point, VInt3 forward, VInt3 trans, VInt3 scale)
        {
            VInt3 up = VInt3.up;
            VInt3 right = VInt3.Cross(VInt3.up, forward);

            return Transform(ref point, ref right, ref up, ref forward, ref trans, ref scale);
        }

        public static int Lerp(int src, int dest, int nom, int den)
        {
            // src + (dest-src) * (nom / den)

            return IntMath.Divide(src * den + (dest - src) * nom, den);
        }

        public static long Lerp(long src, long dest, long nom, long den)
        {
            // src + (dest-src) * (nom / den)

            return IntMath.Divide(src * den + (dest - src) * nom, den);
        }

        public static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public static int CeilPowerOfTwo(int x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;
            return x;
        }

        /// <summary>
        /// 直线的点斜式转化为一般式
        /// </summary>
        /// <param name="segSrc">Seg source.</param>
        /// <param name="segVec">Seg vec.</param>
        public static void SegvecToLinegen(ref VInt2 segSrc, ref VInt2 segVec,
                                          out long a, out long b, out long c)
        {
            a = segVec.y;
            b = -segVec.x;
            c = (long)segVec.x * (long)segSrc.y - (long)segSrc.x * (long)segVec.y;
        }

        /// <summary>
        /// 判断【同一直线】上的点和线段是否重叠
        /// </summary>
        /// <returns><c>true</c> if is point on segment the specified segSrc segVec x y; otherwise, <c>false</c>.</returns>
        /// <param name="segSrc">Seg source.</param>
        /// <param name="segVec">Seg vec.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        private static bool IsPointOnSegment(ref VInt2 segSrc, ref VInt2 segVec, long x, long y)
        {
            long tstVecX = x - segSrc.x;
            long tstVecY = y - segSrc.y;
            return ((segVec.x * tstVecX + segVec.y * tstVecY) >= 0 &&
                    (tstVecX * tstVecX + tstVecY * tstVecY) <= segVec.sqrMagnitudeLong);
        }

        /// <summary>
        /// 计算两条线段的交点
        /// </summary>
        /// <returns><c>true</c>, if segment was intersected, <c>false</c> otherwise.</returns>
        /// <param name="seg1Src">Seg1 source.</param>
        /// <param name="seg1Vec">Seg1 vec.</param>
        /// <param name="seg2Src">Seg2 source.</param>
        /// <param name="seg2Vec">Seg2 vec.</param>
        /// <param name="interPoint">Inter point.</param>
        public static bool IntersectSegment(ref VInt2 seg1Src, ref VInt2 seg1Vec,
                                            ref VInt2 seg2Src, ref VInt2 seg2Vec,
                                            out VInt2 interPoint)
        {
            long a1, b1, c1;
            long a2, b2, c2;
            SegvecToLinegen(ref seg1Src, ref seg1Vec, out a1, out b1, out c1);
            SegvecToLinegen(ref seg2Src, ref seg2Vec, out a2, out b2, out c2);
            long d = a1 * b2 - a2 * b1;
            if (d != 0)
            {
                long ipx = Divide(b1 * c2 - b2 * c1, d);
                long ipy = Divide(a2 * c1 - a1 * c2, d);
                bool isIntersect = IsPointOnSegment(ref seg1Src, ref seg1Vec, ipx, ipy) &&
                    IsPointOnSegment(ref seg2Src, ref seg2Vec, ipx, ipy);
                interPoint.x = (int)ipx;
                interPoint.y = (int)ipy;
                return isIntersect;
            }
            else
            {
                interPoint = VInt2.zero;
                return false;
            }
        }

        public static bool PointInPolygon(ref VInt2 pnt, VInt2[] plg)
        {
            if (null == plg || plg.Length < 3)
                return false;

            bool isIn = false;
            for (int i = 0, j = plg.Length - 1; i < plg.Length; j = i++)
            {
                VInt2 V1 = plg[i];
                VInt2 V2 = plg[j];
                if (V1.y <= pnt.y && pnt.y < V2.y || V2.y <= pnt.y && pnt.y < V1.y)
                {
                    int dY = V2.y - V1.y;
                    long val = (long)(pnt.y - V1.y) * (long)(V2.x - V1.x) - (long)(pnt.x - V1.x) * dY;
                    if (dY > 0)
                    {
                        if (val > 0) isIn = !isIn;
                    }
                    else
                    {
                        if (val < 0) isIn = !isIn;
                    }
                }
            }
            return isIn;
        }

        public static bool SegIntersectPlg(ref VInt2 segSrc, ref VInt2 segVec,
                                                   VInt2[] plg, out VInt2 nearPoint, out VInt2 projectVec)
        {
            nearPoint = VInt2.zero;
            projectVec = VInt2.zero;
            if (null == plg || plg.Length < 2)
                return false;

            bool intersected = false;
            VInt2 tmpPnt;
            long nearDistSqr = -1;
            int nearEdgeIdx = -1;
            for (int i = 0; i < plg.Length; ++i)
            {
                VInt2 edgVec = plg[(i + 1) % plg.Length] - plg[i];
                if (IntersectSegment(ref segSrc, ref segVec, ref plg[i], ref edgVec, out tmpPnt))
                {
                    long tmpDistSqr = (tmpPnt - segSrc).sqrMagnitudeLong;
                    if (nearDistSqr < 0 || tmpDistSqr < nearDistSqr)
                    {
                        nearPoint = tmpPnt;
                        nearDistSqr = tmpDistSqr;
                        nearEdgeIdx = i;
                        intersected = true;
                    }
                }
            }

            if (nearEdgeIdx >= 0)
            {
                VInt2 prjVec = plg[(nearEdgeIdx + 1) % plg.Length] - plg[nearEdgeIdx];
                VInt2 bydVec = segSrc + segVec - nearPoint;
                long dotVal = (long)bydVec.x * (long)prjVec.x + (long)bydVec.y * (long)prjVec.y;
                if (dotVal < 0)
                {
                    dotVal = -dotVal;
                    prjVec = -prjVec;
                }
                long prjMgn = prjVec.sqrMagnitudeLong;
                projectVec.x = (int)Divide(prjVec.x * dotVal, prjMgn);
                projectVec.y = (int)Divide(prjVec.y * dotVal, prjMgn);
            }

            return intersected;
        }

        public static VFactor atan2(int y, int x)
        {
            int add;
            int mul;

            if (x < 0)
            {
                if (y < 0)
                {
                    x = -x;
                    y = -y;
                    mul = 1;
                }
                else
                {
                    x = -x;
                    mul = -1;
                }

                add = -31416;
            }
            else
            {
                if (y < 0)
                {
                    y = -y;
                    mul = -1;
                }
                else
                {
                    mul = 1;
                }

                add = 0;
            }

            int dim = Atan2LookupTable.DIM;

            long nom = (dim - 1);
            long den = (x < y) ? y : x;

            int xi = (int)IntMath.Divide(x * nom, den);
            int yi = (int)IntMath.Divide(y * nom, den);

            int t = Atan2LookupTable.table[yi * dim + xi];

            VFactor result = new VFactor();

            result.nom = (t + add) * mul;
            result.den = 10000;

            return result;
        }

        public static VFactor acos(long nom, long den)
        {
            int index = (int)IntMath.Divide((long)nom * AcosLookupTable.HALF_COUNT, den) + AcosLookupTable.HALF_COUNT;

            index = Mathf.Clamp(index, 0, AcosLookupTable.COUNT);

            VFactor result = new VFactor();
            result.nom = AcosLookupTable.table[index];
            result.den = 10000;

            return result;
        }

        public static VFactor sin(long nom, long den)
        {
            int index = SinCosLookupTable.getIndex(nom, den);

            return new VFactor(SinCosLookupTable.sin_table[index], SinCosLookupTable.FACTOR);
        }

        public static VFactor cos(long nom, long den)
        {
            int index = SinCosLookupTable.getIndex(nom, den);

            return new VFactor(SinCosLookupTable.cos_table[index], SinCosLookupTable.FACTOR);
        }

        public static void sincos(out VFactor s, out VFactor c, long nom, long den)
        {
            int index = SinCosLookupTable.getIndex(nom, den);

            s = new VFactor(SinCosLookupTable.sin_table[index], SinCosLookupTable.FACTOR);
            c = new VFactor(SinCosLookupTable.cos_table[index], SinCosLookupTable.FACTOR);
        }

        public static void sincos(out VFactor s, out VFactor c, VFactor angle)
        {
            int index = SinCosLookupTable.getIndex(angle.nom, angle.den);

            s = new VFactor(SinCosLookupTable.sin_table[index], SinCosLookupTable.FACTOR);
            c = new VFactor(SinCosLookupTable.cos_table[index], SinCosLookupTable.FACTOR);
        }
    }

}
