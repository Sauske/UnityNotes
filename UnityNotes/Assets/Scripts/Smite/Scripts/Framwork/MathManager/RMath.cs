﻿using System;
using UnityEngine;
using System.Text;

namespace Framework
{

    [Serializable]
    public struct VInt3
    {
        public int x;
        public int y;
        public int z;

        //These should be set to the same value (only PrecisionFactor should be 1 divided by Precision)

        /** Precision for the integer coordinates.
         * One world unit is divided into [value] pieces. A value of 1000 would mean millimeter precision, a value of 1 would mean meter precision (assuming 1 world unit = 1 meter).
         * This value affects the maximum coordinates for nodes as well as how large the cost values are for moving between two nodes.
         * A higher value means that you also have to set all penalty values to a higher value to compensate since the normal cost of moving will be higher.
         */
        public const int Precision = 1000;

        /** #Precision as a float */
        public const float FloatPrecision = 1000F;

        /** 1 divided by #Precision */
        public const float PrecisionFactor = 0.001F;

        /* Factor to multiply cost with */
        //public const float CostFactor = 0.01F;

        public static readonly VInt3 zero = new VInt3(0, 0, 0);
        public static readonly VInt3 one = new VInt3(1000, 1000, 1000);
        public static readonly VInt3 half = new VInt3(500, 500, 500);
        public static readonly VInt3 forward = new VInt3(0, 0, 1000);
        public static readonly VInt3 up = new VInt3(0, 1000, 0);
        public static readonly VInt3 right = new VInt3(1000, 0, 0);

        public VInt3(Vector3 position)
        {
            x = MMGame_Math.RoundToInt(position.x * FloatPrecision);
            y = MMGame_Math.RoundToInt(position.y * FloatPrecision);
            z = MMGame_Math.RoundToInt(position.z * FloatPrecision);
            //             x = MMGame_Math.RoundToInt(position.x * FloatPrecision);
            //             y = MMGame_Math.RoundToInt(position.y * FloatPrecision);
            //             z = MMGame_Math.RoundToInt(position.z * FloatPrecision);
        }


        public VInt3(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static bool operator ==(VInt3 lhs, VInt3 rhs)
        {
            return lhs.x == rhs.x &&
                    lhs.y == rhs.y &&
                    lhs.z == rhs.z;
        }

        public static bool operator !=(VInt3 lhs, VInt3 rhs)
        {
            return lhs.x != rhs.x ||
                    lhs.y != rhs.y ||
                    lhs.z != rhs.z;
        }

        public static explicit operator VInt3(Vector3 ob)
        {
            return new VInt3(
                MMGame_Math.RoundToInt(ob.x * FloatPrecision),
                MMGame_Math.RoundToInt(ob.y * FloatPrecision),
                MMGame_Math.RoundToInt(ob.z * FloatPrecision)
                );
        }

        public static explicit operator Vector3(VInt3 ob)
        {
            return new Vector3(ob.x * PrecisionFactor, ob.y * PrecisionFactor, ob.z * PrecisionFactor);
        }

        public static VInt3 operator -(VInt3 lhs, VInt3 rhs)
        {
            lhs.x -= rhs.x;
            lhs.y -= rhs.y;
            lhs.z -= rhs.z;
            return lhs;
        }

        public static VInt3 operator -(VInt3 lhs)
        {
            lhs.x = -lhs.x;
            lhs.y = -lhs.y;
            lhs.z = -lhs.z;
            return lhs;
        }

        public static VInt3 operator +(VInt3 lhs, VInt3 rhs)
        {
            lhs.x += rhs.x;
            lhs.y += rhs.y;
            lhs.z += rhs.z;
            return lhs;
        }

        public static VInt3 operator *(VInt3 lhs, int rhs)
        {
            lhs.x *= rhs;
            lhs.y *= rhs;
            lhs.z *= rhs;

            return lhs;
        }

        public static VInt3 operator *(VInt3 lhs, float rhs)
        {
            lhs.x = MMGame_Math.RoundToInt(lhs.x * rhs);
            lhs.y = MMGame_Math.RoundToInt(lhs.y * rhs);
            lhs.z = MMGame_Math.RoundToInt(lhs.z * rhs);

            return lhs;
        }

        public static VInt3 operator *(VInt3 lhs, double rhs)
        {
            lhs.x = MMGame_Math.RoundToInt(lhs.x * rhs);
            lhs.y = MMGame_Math.RoundToInt(lhs.y * rhs);
            lhs.z = MMGame_Math.RoundToInt(lhs.z * rhs);

            return lhs;
        }

        public static VInt3 operator *(VInt3 lhs, Vector3 rhs)
        {
            lhs.x = MMGame_Math.RoundToInt(lhs.x * rhs.x);
            lhs.y = MMGame_Math.RoundToInt(lhs.y * rhs.y);
            lhs.z = MMGame_Math.RoundToInt(lhs.z * rhs.z);

            return lhs;
        }

        public static VInt3 operator *(VInt3 lhs, VInt3 rhs)
        {
            lhs.x = lhs.x * rhs.x;
            lhs.y = lhs.y * rhs.y;
            lhs.z = lhs.z * rhs.z;

            return lhs;
        }

        public static VInt3 operator /(VInt3 lhs, float rhs)
        {
            lhs.x = MMGame_Math.RoundToInt(lhs.x / rhs);
            lhs.y = MMGame_Math.RoundToInt(lhs.y / rhs);
            lhs.z = MMGame_Math.RoundToInt(lhs.z / rhs);
            return lhs;
        }

        public VInt3 DivBy2()
        {
            x >>= 1;
            y >>= 1;
            z >>= 1;
            return this;
        }

        public int this[int i]
        {
            get
            {
                return i == 0 ? x : (i == 1 ? y : z);
            }
            set
            {
                if (i == 0) x = value;
                else if (i == 1) y = value;
                else z = value;
            }
        }

        /** Angle between the vectors in radians */
        public static float Angle(VInt3 lhs, VInt3 rhs)
        {
            double cos = Dot(lhs, rhs) / ((double)lhs.magnitude * (double)rhs.magnitude);
            cos = cos < -1 ? -1 : (cos > 1 ? 1 : cos);
            return (float)System.Math.Acos(cos);
        }

        public static VFactor AngleInt(VInt3 lhs, VInt3 rhs)
        {
            long magnitude = (long)lhs.magnitude * (long)rhs.magnitude;

            return IntMath.acos(Dot(ref lhs, ref rhs), magnitude);
        }

        public static int Dot(ref VInt3 lhs, ref VInt3 rhs)
        {
            return
                    lhs.x * rhs.x +
                    lhs.y * rhs.y +
                    lhs.z * rhs.z;
        }

        public static int Dot(VInt3 lhs, VInt3 rhs)
        {
            return
                    lhs.x * rhs.x +
                    lhs.y * rhs.y +
                    lhs.z * rhs.z;
        }

        public static long DotLong(VInt3 lhs, VInt3 rhs)
        {
            return
                    (long)lhs.x * (long)rhs.x +
                    (long)lhs.y * (long)rhs.y +
                    (long)lhs.z * (long)rhs.z;
        }

        public static long DotLong(ref VInt3 lhs, ref VInt3 rhs)
        {
            return
                    (long)lhs.x * (long)rhs.x +
                    (long)lhs.y * (long)rhs.y +
                    (long)lhs.z * (long)rhs.z;
        }

        public static long DotXZLong(ref VInt3 lhs, ref VInt3 rhs)
        {
            return
                    (long)lhs.x * (long)rhs.x +
                    (long)lhs.z * (long)rhs.z;
        }

        public static long DotXZLong(VInt3 lhs, VInt3 rhs)
        {
            return
                    (long)lhs.x * (long)rhs.x +
                    (long)lhs.z * (long)rhs.z;
        }


        public static VInt3 Cross(ref VInt3 lhs, ref VInt3 rhs)
        {
            return new VInt3(
                IntMath.Divide((lhs.y * rhs.z) - (lhs.z * rhs.y), 1000),
                IntMath.Divide((lhs.z * rhs.x) - (lhs.x * rhs.z), 1000),
                IntMath.Divide((lhs.x * rhs.y) - (lhs.y * rhs.x), 1000)
                );
        }

        public static VInt3 Cross(VInt3 lhs, VInt3 rhs)
        {
            return new VInt3(
                IntMath.Divide((lhs.y * rhs.z) - (lhs.z * rhs.y), 1000),
                IntMath.Divide((lhs.z * rhs.x) - (lhs.x * rhs.z), 1000),
                IntMath.Divide((lhs.x * rhs.y) - (lhs.y * rhs.x), 1000)
                );
        }

        public static VInt3 MoveTowards(VInt3 from, VInt3 to, int dt)
        {
            if ((to - from).sqrMagnitudeLong <= dt * dt)
            {
                return to;
            }
            else
            {
                VInt3 dir = to - from;
                return from + dir.NormalizeTo(dt);
            }
        }

        /** Normal in 2D space (XZ).
         * Equivalent to Cross(this, Int3(0,1,0) )
         * except that the Y coordinate is left unchanged with this operation.
         */
        public VInt3 Normal2D()
        {
            return new VInt3(z, y, -x);
        }

        public VInt3 NormalizeTo(int newMagn)
        {
            long _x = x * 100;
            long _y = y * 100;
            long _z = z * 100;

            long sqrMagn = _x * _x + _y * _y + _z * _z;
            if (sqrMagn == 0)
                return this;

            long magn = IntMath.Sqrt(sqrMagn);
            long newMagnLong = (long)newMagn;

            x = (int)IntMath.Divide(_x * newMagnLong, magn);
            y = (int)IntMath.Divide(_y * newMagnLong, magn);
            z = (int)IntMath.Divide(_z * newMagnLong, magn);

            return this;
        }

        public long Normalize()
        {
            long _x = x << 7;
            long _y = y << 7;
            long _z = z << 7;

            long sqrMagn = _x * _x + _y * _y + _z * _z;
            if (sqrMagn == 0)
                return 0;

            long magn = IntMath.Sqrt(sqrMagn);
            long newMagnLong = 1000;

            x = (int)IntMath.Divide(_x * newMagnLong, magn);
            y = (int)IntMath.Divide(_y * newMagnLong, magn);
            z = (int)IntMath.Divide(_z * newMagnLong, magn);

            return magn >> 7;
        }

        public Vector3 vec3
        {
            get
            {
                return new Vector3(x * PrecisionFactor, y * PrecisionFactor, z * PrecisionFactor);
            }
        }

        public VInt2 xz
        {
            get
            {
                return new VInt2(x, z);
            }
        }

        public int magnitude
        {
            get
            {
                long _x = x;
                long _y = y;
                long _z = z;

                return IntMath.Sqrt(_x * _x + _y * _y + _z * _z);
            }
        }

        public int magnitude2D
        {
            get
            {
                long _x = x;
                long _z = z;

                return IntMath.Sqrt(_x * _x + _z * _z);
            }
        }

        public VInt3 RotateY(ref VFactor radians)
        {
            VInt3 outDir;
            VFactor s;
            VFactor c;

            IntMath.sincos(out s, out c, radians.nom, radians.den);

            long n1 = c.nom * s.den;
            long n2 = c.den * s.nom;
            long d = c.den * s.den;

            //outDir.x = x * c + z * s;
            //outDir.z = -x * s + z * c;
            outDir.x = (int)IntMath.Divide(x * n1 + z * n2, d);
            outDir.z = (int)IntMath.Divide(-x * n2 + z * n1, d);
            outDir.y = 0;

            return outDir.NormalizeTo(1000);
        }

        public VInt3 RotateY(int degree)
        {
            VInt3 outDir;
            VFactor s;
            VFactor c;
            IntMath.sincos(out s, out c, 31416 * degree, 180 * 10000);

            long n1 = c.nom * s.den;
            long n2 = c.den * s.nom;
            long d = c.den * s.den;

            //outDir.x = x * c + z * s;
            //outDir.z = -x * s + z * c;
            outDir.x = (int)IntMath.Divide(x * n1 + z * n2, d);
            outDir.z = (int)IntMath.Divide(-x * n2 + z * n1, d);
            outDir.y = 0;

            return outDir.NormalizeTo(1000);
        }

        /** Magnitude used for the cost between two nodes. The default cost between two nodes can be calculated like this:
          * \code int cost = (node1.position-node2.position).costMagnitude; \endcode
          * 
          * This is simply the magnitude, rounded to the nearest integer
          */
        public int costMagnitude
        {
            get
            {
                //return MMGame_Math.RoundToInt(magnitude);

                return magnitude;
            }
        }

        /** The magnitude in world units */
        public float worldMagnitude
        {
            get
            {
                double _x = x;
                double _y = y;
                double _z = z;

                return (float)System.Math.Sqrt(_x * _x + _y * _y + _z * _z) * PrecisionFactor;

                //Scale numbers down
                /*float _x = x*PrecisionFactor;
                float _y = y*PrecisionFactor;
                float _z = z*PrecisionFactor;
                return Mathf.Sqrt (_x*_x+_y*_y+_z*_z);*/
            }
        }

        /** The squared magnitude of the vector */
        public double sqrMagnitude
        {
            get
            {
                double _x = x;
                double _y = y;
                double _z = z;
                return (_x * _x + _y * _y + _z * _z);
                //return x*x+y*y+z*z;
            }
        }

        /** The squared magnitude of the vector */
        public long sqrMagnitudeLong
        {
            get
            {
                long _x = x;
                long _y = y;
                long _z = z;
                return (_x * _x + _y * _y + _z * _z);
                //return x*x+y*y+z*z;
            }
        }

        public long sqrMagnitudeLong2D
        {
            get
            {
                long _x = x;
                long _z = z;
                return (_x * _x + _z * _z);
            }
        }

        /** \warning Can cause number overflows if the magnitude is too large */
        public int unsafeSqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        public VInt3 abs
        {
            get
            {
                return new VInt3(Math.Abs(x), Math.Abs(y), Math.Abs(z));
            }
        }

        /** To avoid number overflows. \deprecated Int3.magnitude now uses the same implementation */
        [System.Obsolete("Same implementation as .magnitude")]
        public float safeMagnitude
        {
            get
            {
                //Of some reason, it is faster to use doubles (almost 40% faster)
                double _x = x;
                double _y = y;
                double _z = z;

                return (float)System.Math.Sqrt(_x * _x + _y * _y + _z * _z);

                //Scale numbers down
                /*float _x = x*PrecisionFactor;
                float _y = y*PrecisionFactor;
                float _z = z*PrecisionFactor;
                //Find the root and scale it up again
                return Mathf.Sqrt (_x*_x+_y*_y+_z*_z)*FloatPrecision;*/
            }
        }

        /** To avoid number overflows. The returned value is the squared magnitude of the world distance (i.e divided by Precision) 
         * \deprecated .sqrMagnitude is now per default safe (Int3.unsafeSqrMagnitude can be used for unsafe operations) */
        [System.Obsolete(".sqrMagnitude is now per default safe (.unsafeSqrMagnitude can be used for unsafe operations)")]
        public float safeSqrMagnitude
        {
            get
            {
                float _x = x * PrecisionFactor;
                float _y = y * PrecisionFactor;
                float _z = z * PrecisionFactor;
                return _x * _x + _y * _y + _z * _z;
            }
        }

        public static implicit operator string(VInt3 ob)
        {
            return ob.ToString();
        }

        /** Returns a nicely formatted string representing the vector */
        public override string ToString()
        {
            return "( " + x + ", " + y + ", " + z + ")";
        }

        public override bool Equals(System.Object o)
        {

            if (o == null) return false;

            VInt3 rhs = (VInt3)o;

            return x == rhs.x &&
                    y == rhs.y &&
                    z == rhs.z;
        }

        public override int GetHashCode()
        {
            return x * 73856093 ^ y * 19349663 ^ z * 83492791;
        }

        public static VInt3 Lerp(VInt3 a, VInt3 b, float f)
        {
            return new VInt3(
                Mathf.RoundToInt(a.x * (1f - f)) + Mathf.RoundToInt(b.x * f),
                Mathf.RoundToInt(a.y * (1f - f)) + Mathf.RoundToInt(b.y * f),
                Mathf.RoundToInt(a.z * (1f - f)) + Mathf.RoundToInt(b.z * f)
                );
        }

        public static VInt3 Lerp(VInt3 a, VInt3 b, VFactor f)
        {
            return new VInt3(
                (int)IntMath.Divide((b.x - a.x) * f.nom, f.den) + a.x,
                (int)IntMath.Divide((b.y - a.y) * f.nom, f.den) + a.y,
                (int)IntMath.Divide((b.z - a.z) * f.nom, f.den) + a.z
                );
        }

        public static VInt3 Lerp(VInt3 a, VInt3 b, int factorNom, int factorDen)
        {
            return new VInt3(
                (int)IntMath.Divide((b.x - a.x) * factorNom, factorDen) + a.x,
                (int)IntMath.Divide((b.y - a.y) * factorNom, factorDen) + a.y,
                (int)IntMath.Divide((b.z - a.z) * factorNom, factorDen) + a.z
                );
        }

        public long XZSqrMagnitude(VInt3 rhs)
        {
            long a = x - rhs.x;
            long b = z - rhs.z;

            return (a * a) + (b * b);
        }

        public long XZSqrMagnitude(ref VInt3 rhs)
        {
            long a = x - rhs.x;
            long b = z - rhs.z;

            return (a * a) + (b * b);
        }

        public bool IsEqualXZ(VInt3 rhs)
        {
            return x == rhs.x && z == rhs.z;
        }

        public bool IsEqualXZ(ref VInt3 rhs)
        {
            return x == rhs.x && z == rhs.z;
        }
    }

    /** Two Dimensional Integer Coordinate Pair */
    [Serializable]
    public struct VInt2
    {
        public int x;
        public int y;

        public static VInt2 zero = new VInt2()
        {
            x = 0,
            y = 0,
        };

        public VInt2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static explicit operator Vector2(VInt2 ob)
        {
            return new Vector2(ob.x * VInt3.PrecisionFactor, ob.y * VInt3.PrecisionFactor);
        }

        public static explicit operator VInt2(Vector2 ob)
        {
            return new VInt2(
                MMGame_Math.RoundToInt(ob.x * VInt3.FloatPrecision),
                MMGame_Math.RoundToInt(ob.y * VInt3.FloatPrecision)
                );
        }

        public int sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        public long sqrMagnitudeLong
        {
            get
            {
                long _x = x;
                long _y = y;

                return _x * _x + _y * _y;
            }
        }

        public int magnitude
        {
            get
            {
                long _x = x;
                long _y = y;

                return IntMath.Sqrt(_x * _x + _y * _y);
            }
        }

        public static VInt2 operator +(VInt2 a, VInt2 b)
        {
            return new VInt2(a.x + b.x, a.y + b.y);
        }

        public static VInt2 operator -(VInt2 a, VInt2 b)
        {
            return new VInt2(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(VInt2 a, VInt2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(VInt2 a, VInt2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static int Dot(VInt2 a, VInt2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static long DotLong(ref VInt2 a, ref VInt2 b)
        {
            return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
        }

        public static long DotLong(VInt2 a, VInt2 b)
        {
            return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
        }

        public static long DetLong(ref VInt2 a, ref VInt2 b)
        {
            return (long)a.x * (long)b.y - (long)a.y * (long)b.x;
        }

        public static long DetLong(VInt2 a, VInt2 b)
        {
            return (long)a.x * (long)b.y - (long)a.y * (long)b.x;
        }

        public override bool Equals(System.Object o)
        {
            if (o == null) return false;
            VInt2 rhs = (VInt2)o;

            return x == rhs.x && y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x * 49157 + y * 98317;
        }

        /** Matrices for rotation.
         * Each group of 4 elements is a 2x2 matrix.
         * The XZ position is multiplied by this.
         * So
         * \code
         * //A rotation by 90 degrees clockwise, second matrix in the array
         * (5,2) * ((0, 1), (-1, 0)) = (2,-5)
         * \endcode
         */
        private static readonly int[] Rotations = {
			 1, 0, //Identity matrix
			 0, 1,
			
			 0, 1,
			-1, 0,
			
			-1, 0,
			 0,-1,
			
			 0,-1,
			 1, 0
		};

        /** Returns a new Int2 rotated 90*r degrees around the origin. */
        public static VInt2 Rotate(VInt2 v, int r)
        {
            r = r % 4;
            return new VInt2(v.x * Rotations[r * 4 + 0] + v.y * Rotations[r * 4 + 1], v.x * Rotations[r * 4 + 2] + v.y * Rotations[r * 4 + 3]);
        }

        public static VInt2 Min(VInt2 a, VInt2 b)
        {
            return new VInt2(System.Math.Min(a.x, b.x), System.Math.Min(a.y, b.y));
        }

        public static VInt2 Max(VInt2 a, VInt2 b)
        {
            return new VInt2(System.Math.Max(a.x, b.x), System.Math.Max(a.y, b.y));
        }

        public static VInt2 FromInt3XZ(VInt3 o)
        {
            return new VInt2(o.x, o.z);
        }

        public static VInt3 ToInt3XZ(VInt2 o)
        {
            return new VInt3(o.x, 0, o.y);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public void Min(ref VInt2 r)
        {
            x = Mathf.Min(x, r.x);
            y = Mathf.Min(y, r.y);
        }

        public void Max(ref VInt2 r)
        {
            x = Mathf.Max(x, r.x);
            y = Mathf.Max(y, r.y);
        }

        public void Normalize()
        {
            long _x = x * 100;
            long _y = y * 100;

            long sqrMagn = _x * _x + _y * _y;
            if (sqrMagn == 0)
                return;

            long magn = IntMath.Sqrt(sqrMagn);

            x = (int)IntMath.Divide(_x * 1000L, magn);
            y = (int)IntMath.Divide(_y * 1000L, magn);
        }

        public static VInt2 operator -(VInt2 lhs)
        {
            lhs.x = -lhs.x;
            lhs.y = -lhs.y;
            return lhs;
        }

        public VInt2 normalized
        {
            get
            {
                VInt2 r = new VInt2(x, y);
                r.Normalize();
                return r;
            }
        }

        public static VInt2 operator *(VInt2 lhs, int rhs)
        {
            lhs.x *= rhs;
            lhs.y *= rhs;

            return lhs;
        }

        public static VInt2 ClampMagnitude(VInt2 v, int maxLength)
        {
            long sqrMagn = v.sqrMagnitudeLong;
            long maxLen = maxLength;

            if (sqrMagn > (maxLen * maxLen))
            {
                long magn = IntMath.Sqrt(sqrMagn);

                int x = (int)IntMath.Divide(v.x * maxLength, magn);
                int y = (int)IntMath.Divide(v.x * maxLength, magn);

                return new VInt2(x, y);
            }
            return v;
        }
    }

    [Serializable]
    public struct VInt
    {
        public int i;

        public VInt(int i)
        {
            this.i = i;
        }

        public VInt(float f)
        {
            i = MMGame_Math.RoundToInt(f * VInt3.FloatPrecision);
        }

        public static explicit operator VInt(float f)
        {
            return new VInt(MMGame_Math.RoundToInt(f * VInt3.FloatPrecision));
        }

        public static implicit operator VInt(int i)
        {
            return new VInt(i);
        }

        public static explicit operator float(VInt ob)
        {
            return ob.i * VInt3.PrecisionFactor;
        }

        public static explicit operator long(VInt ob)
        {
            return ob.i;
        }

        //public static implicit operator int(VInt ob)
        //{
        //    return ob.i;
        //}

        public static VInt operator +(VInt a, VInt b)
        {
            return new VInt(a.i + b.i);
        }

        public static VInt operator -(VInt a, VInt b)
        {
            return new VInt(a.i - b.i);
        }

        public static bool operator ==(VInt a, VInt b)
        {
            return a.i == b.i;
        }

        public static bool operator !=(VInt a, VInt b)
        {
            return a.i != b.i;
        }

        public override bool Equals(System.Object o)
        {
            if (o == null) return false;
            VInt rhs = (VInt)o;

            return i == rhs.i;
        }

        public override int GetHashCode()
        {
            return i.GetHashCode();
        }

        public static VInt Min(VInt a, VInt b)
        {
            return new VInt(System.Math.Min(a.i, b.i));
        }

        public static VInt Max(VInt a, VInt b)
        {
            return new VInt(System.Math.Max(a.i, b.i));
        }

        public override string ToString()
        {
            return scalar.ToString();
        }

        public float scalar
        {
            get { return i * VInt3.PrecisionFactor; }
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VRect
    {
        private int m_XMin;
        private int m_YMin;
        private int m_Width;
        private int m_Height;
        public VRect(int left, int top, int width, int height)
        {
            this.m_XMin = left;
            this.m_YMin = top;
            this.m_Width = width;
            this.m_Height = height;
        }

        public VRect(VRect source)
        {
            this.m_XMin = source.m_XMin;
            this.m_YMin = source.m_YMin;
            this.m_Width = source.m_Width;
            this.m_Height = source.m_Height;
        }

        public static VRect MinMaxRect(int left, int top, int right, int bottom)
        {
            return new VRect(left, top, right - left, bottom - top);
        }

        public void Set(int left, int top, int width, int height)
        {
            this.m_XMin = left;
            this.m_YMin = top;
            this.m_Width = width;
            this.m_Height = height;
        }

        public int x
        {
            get
            {
                return this.m_XMin;
            }
            set
            {
                this.m_XMin = value;
            }
        }
        public int y
        {
            get
            {
                return this.m_YMin;
            }
            set
            {
                this.m_YMin = value;
            }
        }
        public VInt2 position
        {
            get
            {
                return new VInt2(this.m_XMin, this.m_YMin);
            }
            set
            {
                this.m_XMin = value.x;
                this.m_YMin = value.y;
            }
        }
        public VInt2 center
        {
            get
            {
                return new VInt2(this.x + (this.m_Width >> 1), this.y + (this.m_Height >> 1));
            }
            set
            {
                this.m_XMin = value.x - (this.m_Width >> 1);
                this.m_YMin = value.y - (this.m_Height >> 1);
            }
        }
        public VInt2 min
        {
            get
            {
                return new VInt2(this.xMin, this.yMin);
            }
            set
            {
                this.xMin = value.x;
                this.yMin = value.y;
            }
        }
        public VInt2 max
        {
            get
            {
                return new VInt2(this.xMax, this.yMax);
            }
            set
            {
                this.xMax = value.x;
                this.yMax = value.y;
            }
        }
        public int width
        {
            get
            {
                return this.m_Width;
            }
            set
            {
                this.m_Width = value;
            }
        }
        public int height
        {
            get
            {
                return this.m_Height;
            }
            set
            {
                this.m_Height = value;
            }
        }
        public VInt2 size
        {
            get
            {
                return new VInt2(this.m_Width, this.m_Height);
            }
            set
            {
                this.m_Width = value.x;
                this.m_Height = value.y;
            }
        }
        public int xMin
        {
            get
            {
                return this.m_XMin;
            }
            set
            {
                int xMax = this.xMax;
                this.m_XMin = value;
                this.m_Width = xMax - this.m_XMin;
            }
        }
        public int yMin
        {
            get
            {
                return this.m_YMin;
            }
            set
            {
                int yMax = this.yMax;
                this.m_YMin = value;
                this.m_Height = yMax - this.m_YMin;
            }
        }
        public int xMax
        {
            get
            {
                return (this.m_Width + this.m_XMin);
            }
            set
            {
                this.m_Width = value - this.m_XMin;
            }
        }
        public int yMax
        {
            get
            {
                return (this.m_Height + this.m_YMin);
            }
            set
            {
                this.m_Height = value - this.m_YMin;
            }
        }
        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.width, this.height };
            return string.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", args);
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.width.ToString(format), this.height.ToString(format) };
            return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", args);
        }

        public bool Contains(VInt2 point)
        {
            return ((((point.x >= this.xMin) && (point.x < this.xMax)) && (point.y >= this.yMin)) && (point.y < this.yMax));
        }

        public bool Contains(VInt3 point)
        {
            return ((((point.x >= this.xMin) && (point.x < this.xMax)) && (point.y >= this.yMin)) && (point.y < this.yMax));
        }

        public bool Contains(VInt3 point, bool allowInverse)
        {
            if (!allowInverse)
            {
                return this.Contains(point);
            }
            bool flag = false;
            if ((((this.width < 0f) && (point.x <= this.xMin)) && (point.x > this.xMax)) || (((this.width >= 0f) && (point.x >= this.xMin)) && (point.x < this.xMax)))
            {
                flag = true;
            }
            if (!flag || ((((this.height >= 0f) || (point.y > this.yMin)) || (point.y <= this.yMax)) && (((this.height < 0f) || (point.y < this.yMin)) || (point.y >= this.yMax))))
            {
                return false;
            }
            return true;
        }

        private static VRect OrderMinMax(VRect rect)
        {
            if (rect.xMin > rect.xMax)
            {
                int xMin = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = xMin;
            }
            if (rect.yMin > rect.yMax)
            {
                int yMin = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = yMin;
            }
            return rect;
        }

        public bool Overlaps(VRect other)
        {
            return ((((other.xMax > this.xMin) && (other.xMin < this.xMax)) && (other.yMax > this.yMin)) && (other.yMin < this.yMax));
        }

        public bool Overlaps(VRect other, bool allowInverse)
        {
            VRect rect = this;
            if (allowInverse)
            {
                rect = OrderMinMax(rect);
                other = OrderMinMax(other);
            }
            return rect.Overlaps(other);
        }

        public override int GetHashCode()
        {
            return (((this.x.GetHashCode() ^ (this.width.GetHashCode() << 2)) ^ (this.y.GetHashCode() >> 2)) ^ (this.height.GetHashCode() >> 1));
        }

        public override bool Equals(object other)
        {
            if (!(other is VRect))
            {
                return false;
            }
            VRect rect = (VRect)other;
            return (((this.x.Equals(rect.x) && this.y.Equals(rect.y)) && this.width.Equals(rect.width)) && this.height.Equals(rect.height));
        }

        public static bool operator !=(VRect lhs, VRect rhs)
        {
            return ((((lhs.x != rhs.x) || (lhs.y != rhs.y)) || (lhs.width != rhs.width)) || (lhs.height != rhs.height));
        }

        public static bool operator ==(VRect lhs, VRect rhs)
        {
            return ((((lhs.x == rhs.x) && (lhs.y == rhs.y)) && (lhs.width == rhs.width)) && (lhs.height == rhs.height));
        }
    }

    [Serializable]
    public struct VFactor
    {
        public long nom;
        public long den;

        public VFactor(long n, long d)
        {
            nom = n;
            den = d;
        }

        public int roundInt
        {
            get
            {
                return (int)IntMath.Divide(nom, den);
            }
        }

        public int integer
        {
            get
            {
                return (int)(nom / den);
            }
        }

        public float single
        {
            get
            {
                double d = nom / (double)den;
                return (float)d;
            }
        }

        public bool IsPositive
        {
            get
            {
              //  DebugHelper.Assert(den != 0, "VFactor: denominator is zero !");

                if (nom == 0)
                    return false;

                bool b1 = nom > 0;
                bool b2 = den > 0;

                return !(b1 ^ b2);
            }
        }

        public bool IsNegative
        {
            get
            {
             //   DebugHelper.Assert(den != 0, "VFactor: denominator is zero !");

                if (nom == 0)
                    return false;

                bool b1 = nom > 0;
                bool b2 = den > 0;

                return (b1 ^ b2);
            }
        }

        public bool IsZero
        {
            get
            {
                return nom == 0;
            }
        }

        [NonSerialized]
        public static VFactor zero = new VFactor(0, 1);

        [NonSerialized]
        public static VFactor one = new VFactor(1, 1);

        [NonSerialized]
        public static VFactor pi = new VFactor(31416, 10000);

        [NonSerialized]
        public static VFactor twoPi = new VFactor(62832, 10000);

        public static bool operator <(VFactor a, VFactor b)
        {
            long l = a.nom * b.den;
            long r = b.nom * a.den;
            bool sign = (b.den > 0) ^ (a.den > 0);
            return sign ? (l > r) : (l < r);
        }

        public static bool operator >(VFactor a, VFactor b)
        {
            long l = a.nom * b.den;
            long r = b.nom * a.den;
            bool sign = (b.den > 0) ^ (a.den > 0);
            return sign ? (l < r) : (l > r);
        }

        public static bool operator <=(VFactor a, VFactor b)
        {
            long l = a.nom * b.den;
            long r = b.nom * a.den;
            bool sign = (b.den > 0) ^ (a.den > 0);
            return sign ? (l >= r) : (l <= r);
        }

        public static bool operator >=(VFactor a, VFactor b)
        {
            long l = a.nom * b.den;
            long r = b.nom * a.den;
            bool sign = (b.den > 0) ^ (a.den > 0);
            return sign ? (l <= r) : (l >= r);
        }

        public static bool operator ==(VFactor a, VFactor b)
        {
            return (a.nom * b.den) == (b.nom * a.den);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                this.GetType() == obj.GetType() &&
                    this == (VFactor)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(VFactor a, VFactor b)
        {
            return (a.nom * b.den) != (b.nom * a.den);
        }

        public static bool operator <(VFactor a, long b)
        {
            long l = a.nom;
            long r = b * a.den;
            return (a.den > 0) ? l < r : l > r;
        }

        public static bool operator >(VFactor a, long b)
        {
            long l = a.nom;
            long r = b * a.den;
            return (a.den > 0) ? l > r : l < r;
        }

        public static bool operator <=(VFactor a, long b)
        {
            long l = a.nom;
            long r = b * a.den;
            return (a.den > 0) ? l <= r : l >= r;
        }

        public static bool operator >=(VFactor a, long b)
        {
            long l = a.nom;
            long r = b * a.den;
            return (a.den > 0) ? l >= r : l <= r;
        }

        public static bool operator ==(VFactor a, long b)
        {
            return a.nom == b * a.den;
        }

        public static bool operator !=(VFactor a, long b)
        {
            return a.nom != b * a.den;
        }

        public static VFactor operator +(VFactor a, VFactor b)
        {
            return new VFactor()
            {
                nom = a.nom * b.den + b.nom * a.den,
                den = a.den * b.den,
            };
        }

        public static VFactor operator +(VFactor a, long b)
        {
            a.nom += b * a.den;
            return a;
        }

        public static VFactor operator -(VFactor a, VFactor b)
        {
            return new VFactor()
            {
                nom = a.nom * b.den - b.nom * a.den,
                den = a.den * b.den,
            };
        }

        public static VFactor operator -(VFactor a, long b)
        {
            a.nom -= b * a.den;
            return a;
        }

        public static VFactor operator *(VFactor a, long b)
        {
            a.nom *= b;
            return a;
        }

        public static VFactor operator /(VFactor a, long b)
        {
            a.den *= b;
            return a;
        }

        public static VInt3 operator *(VInt3 v, VFactor f)
        {
            return IntMath.Divide(v, f.nom, f.den);
        }

        public static VInt2 operator *(VInt2 v, VFactor f)
        {
            return IntMath.Divide(v, f.nom, f.den);
        }

        public static VInt3 operator /(VInt3 v, VFactor f)
        {
            return IntMath.Divide(v, f.den, f.nom);
        }

        public static VInt2 operator /(VInt2 v, VFactor f)
        {
            return IntMath.Divide(v, f.den, f.nom);
        }

        public static int operator *(int i, VFactor f)
        {
            return (int)IntMath.Divide(i * f.nom, f.den);
        }

        public static VFactor operator -(VFactor a)
        {
            a.nom = -a.nom;
            return a;
        }

        public VFactor Inverse
        {
            get
            {
                return new VFactor(den, nom);
            }
        }

        public override string ToString()
        {
            return single.ToString();
        }

        static long mask_ = 0x7FFFFFFFFFFFFFFF;
        static long upper_ = (1L << 24) - 1;

        public void strip()
        {
            while ((nom & mask_) > upper_ && (den & mask_) > upper_)
            {
                nom >>= 1;
                den >>= 1;
            }
        }
    }

    public struct VLine
    {
        public VInt2 point;
        public VInt2 direction;
    }

    public partial class IntMath
    {
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
    }

    public static class MMGame_Math
    {
        public static float Dot3(this Vector3 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot3(this Vector3 a, ref Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot3(this Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot3(this Vector3 a, ref Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float DotXZ(this Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.z * b.z;
        }

        public static float DotXZ(this Vector3 a, ref Vector3 b)
        {
            return a.x * b.x + a.z * b.z;
        }

        public static Vector3 Mul(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Mul(this Vector3 a, ref Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Mul(this Vector3 a, Vector3 b, float f)
        {
            return new Vector3(a.x * b.x * f, a.y * b.y * f, a.z * b.z * f);
        }

        public static Vector3 Mul(this Vector3 a, ref Vector3 b, float f)
        {
            return new Vector3(a.x * b.x * f, a.y * b.y * f, a.z * b.z * f);
        }

        public static float XZSqrMagnitude(this Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;

            return dx * dx + dz * dz;
        }

        public static float XZSqrMagnitude(this Vector3 a, ref Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;

            return dx * dx + dz * dz;
        }

        public static Vector2 xz(this Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }

        public static string ToString2(this Vector3 a)
        {
            return string.Format("({0},{1},{2})", a.x, a.y, a.z);
        }

        public static Vector3 toVec3(this Vector4 a)
        {
            return new Vector3(a.x, a.y, a.z);
        }

        public static Vector4 toVec4(this Vector3 v, float a)
        {
            return new Vector4(v.x, v.y, v.z, a);
        }

        public static Vector3 RotateY(this Vector3 v, float angle)
        {
            float s = Mathf.Sin(angle);
            float c = Mathf.Cos(angle);

            Vector3 outV;
            outV.x = v.x * c + v.z * s;
            outV.z = -v.x * s + v.z * c;
            outV.y = v.y;
            return outV;
        }

        public static bool isMirror(Matrix4x4 m)
        {
            Vector3 x = m.GetColumn(0).toVec3();
            Vector3 y = m.GetColumn(1).toVec3();
            Vector3 z = m.GetColumn(2).toVec3();

            Vector3 zz = Vector3.Cross(x, y);

            z.Normalize();
            zz.Normalize();

            float f = z.Dot3(ref zz);

            if (f < 0)
            {
                return true;
            }

            return false;
        }

        public static void SetLayer(this GameObject go, string layerName, bool bFileSkillIndicator = false)
        {
            int layer = LayerMask.NameToLayer(layerName);

            SetLayerRecursively(go, layer, bFileSkillIndicator);
        }

        public static void SetLayer(this GameObject go, int layer, bool bFileSkillIndicator)
        {
            SetLayerRecursively(go, layer, bFileSkillIndicator);
        }

        public static void SetLayerRecursively(GameObject go, int layer, bool bFileSkillIndicator)
        {
            if (bFileSkillIndicator == true && go.CompareTag("SCI") == true)
                return;

            go.layer = layer;

            int count = go.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
            }
        }

        public static void SetGameObjVisible(this GameObject go, bool bVisible)
        {
            if (go.IsGameObjHidden() == !bVisible)
            {
                return;
            }

            if (bVisible)
            {
                go.SetLayer("Actor", "Particles", true);
            }
            else
            {
                go.SetLayer("Hide", true);
            }
        }

        public static bool IsGameObjHidden(this GameObject go)
        {
            string layerName = LayerMask.LayerToName(go.layer);

            return layerName == "Hide";
        }

        public static void SetVisibleSameAs(this GameObject go, GameObject tarGo)
        {
            if (IsGameObjHidden(tarGo))
            {
                go.SetGameObjVisible(false);
            }
            else
            {
                go.SetGameObjVisible(true);
            }
        }

        public static void SetLayer(this GameObject go, string layerName, string layerNameParticles, bool bFileSkillIndicator = false)
        {
            int layer = LayerMask.NameToLayer(layerName);
            int layerParticles = LayerMask.NameToLayer(layerNameParticles);

            SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
        }

        public static void SetLayer(this GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
        {
            SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
        }

        public static void SetLayerRecursively(GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
        {
            if (bFileSkillIndicator == true && go.CompareTag("SCI") == true)
                return;

            if (go.GetComponent<ParticleSystem>() != null)
                go.layer = layerParticles;
            else
                go.layer = layer;

            int count = go.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
            }
        }

        public static Renderer GetRendererInChildren(this GameObject go)
        {
            if (go.GetComponent<Renderer>())
                return go.GetComponent<Renderer>();

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform childTrans = go.transform.GetChild(i);
                if (childTrans && childTrans.gameObject)
                {
                    Renderer r = childTrans.gameObject.GetRendererInChildren();
                    if (r)
                        return r;
                }
            }

            return null;
        }

        public static SkinnedMeshRenderer GetSkinnedMeshRendererInChildren(this GameObject go)
        {
            var r = go.GetComponent<Renderer>() as SkinnedMeshRenderer;
            if (r)
                return r;

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform childTrans = go.transform.GetChild(i);
                if (childTrans && childTrans.gameObject)
                {
                    r = childTrans.gameObject.GetSkinnedMeshRendererInChildren();
                    if (r)
                        return r;
                }
            }

            return null;
        }

        public static MeshRenderer GetMeshRendererInChildren(this GameObject go)
        {
            var r = go.GetComponent<Renderer>() as MeshRenderer;
            if (r)
                return r;

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform childTrans = go.transform.GetChild(i);
                if (childTrans && childTrans.gameObject)
                {
                    r = childTrans.gameObject.GetMeshRendererInChildren();
                    if (r)
                        return r;
                }
            }

            return null;
        }

        public static void SetOffsetX(this Camera camera, float offsetX)
        {
            float height = 2f * Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f) * camera.nearClipPlane;
            float width = height * camera.aspect;

            float centerX = -Mathf.Clamp(offsetX, -1f, 1f) * width;

            float right = (width + centerX) * 0.5f;
            float left = right - width;

            camera.SetPerspectiveOffCenter(left, right, -height * 0.5f, height * 0.5f, camera.nearClipPlane, camera.farClipPlane);
        }

        public static void SetPerspectiveOffCenter(this Camera camera, float left, float right, float bottom, float top, float near, float far)
        {
            float invW = 1f / (right - left);
            float invH = 1f / (top - bottom);
            float invL = 1f / (near - far);

            Matrix4x4 m = new Matrix4x4();

            m.m00 = 2f * near * invW;
            m.m10 = 0f;
            m.m20 = 0f;
            m.m30 = 0f;

            m.m01 = 0f;
            m.m11 = 2f * near * invH;
            m.m21 = 0f;
            m.m31 = 0f;

            m.m02 = (right + left) * invW;
            m.m12 = (top + bottom) * invH;
            m.m22 = far * invL;
            m.m32 = -1f;

            m.m03 = 0f;
            m.m13 = 0f;
            m.m23 = (2f * far * near) * invL;
            m.m33 = 0f;

            camera.projectionMatrix = m;
        }



        public static Vector2 Lerp(this Vector2 left, Vector2 right, float lerp)
        {
            return new Vector2(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp));
        }

        public static Vector3 Lerp(this Vector3 left, Vector3 right, float lerp)
        {
            return new Vector3(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp));
        }

        public static Vector4 Lerp(this Vector4 left, Vector4 right, float lerp)
        {
            return new Vector4(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp), Mathf.Lerp(left.w, right.w, lerp));
        }

        public static int RoundToInt(double x)
        {
            if (x >= 0.0)
            {
                return (int)(x + 0.5);
            }
            else
            {
                return (int)(x - 0.5);
            }
        }

        public static double Round(double x)
        {
            return (double)RoundToInt(x);
        }
    }

    public static class Atan2LookupTable
    {
        public static readonly int BITS = 7;
        public static readonly int BITS2 = BITS << 1;
        public static readonly int MASK = ~(-1 << BITS2);
        public static readonly int COUNT = MASK + 1;
        public static readonly int DIM = IntMath.Sqrt(COUNT);

        #region Atan2 lookup table

        public static readonly int[] table = new int[]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
		15708, 7854, 4636, 3218, 2450, 1974, 1651, 1419, 1244, 1107, 997, 907, 831, 768, 713, 666, 624, 588, 555, 526, 500, 476, 454, 435, 416, 400, 384, 370, 357, 345, 333, 322, 312, 303, 294, 286, 278, 270, 263, 256, 250, 244, 238, 233, 227, 222, 217, 213, 208, 204, 200, 196, 192, 189, 185, 182, 179, 175, 172, 169, 167, 164, 161, 159, 156, 154, 152, 149, 147, 145, 143, 141, 139, 137, 135, 133, 132, 130, 128, 127, 125, 123, 122, 120, 119, 118, 116, 115, 114, 112, 111, 110, 109, 108, 106, 105, 104, 103, 102, 101, 100, 99, 98, 97, 96, 95, 94, 93, 93, 92, 91, 90, 89, 88, 88, 87, 86, 85, 85, 84, 83, 83, 82, 81, 81, 80, 79, 79, 
		15708, 11071, 7854, 5880, 4636, 3805, 3218, 2783, 2450, 2187, 1974, 1799, 1651, 1526, 1419, 1326, 1244, 1171, 1107, 1049, 997, 950, 907, 867, 831, 798, 768, 739, 713, 689, 666, 644, 624, 605, 588, 571, 555, 540, 526, 512, 500, 487, 476, 465, 454, 444, 435, 425, 416, 408, 400, 392, 384, 377, 370, 363, 357, 351, 345, 339, 333, 328, 322, 317, 312, 308, 303, 298, 294, 290, 286, 282, 278, 274, 270, 267, 263, 260, 256, 253, 250, 247, 244, 241, 238, 235, 233, 230, 227, 225, 222, 220, 217, 215, 213, 210, 208, 206, 204, 202, 200, 198, 196, 194, 192, 190, 189, 187, 185, 183, 182, 180, 179, 177, 175, 174, 172, 171, 169, 168, 167, 165, 164, 163, 161, 160, 159, 157, 
		15708, 12490, 9828, 7854, 6435, 5404, 4636, 4049, 3588, 3218, 2915, 2663, 2450, 2268, 2111, 1974, 1853, 1747, 1651, 1566, 1489, 1419, 1355, 1297, 1244, 1194, 1149, 1107, 1067, 1031, 997, 965, 935, 907, 880, 855, 831, 809, 788, 768, 749, 730, 713, 697, 681, 666, 651, 637, 624, 611, 599, 588, 576, 565, 555, 545, 535, 526, 517, 508, 500, 491, 483, 476, 468, 461, 454, 447, 441, 435, 428, 422, 416, 411, 405, 400, 395, 389, 384, 380, 375, 370, 366, 361, 357, 353, 349, 345, 341, 337, 333, 330, 326, 322, 319, 316, 312, 309, 306, 303, 300, 297, 294, 291, 288, 286, 283, 280, 278, 275, 273, 270, 268, 265, 263, 261, 259, 256, 254, 252, 250, 248, 246, 244, 242, 240, 238, 236, 
		15708, 13258, 11071, 9273, 7854, 6747, 5880, 5191, 4636, 4182, 3805, 3488, 3218, 2985, 2783, 2606, 2450, 2311, 2187, 2075, 1974, 1882, 1799, 1722, 1651, 1587, 1526, 1471, 1419, 1371, 1326, 1283, 1244, 1206, 1171, 1138, 1107, 1077, 1049, 1022, 997, 973, 950, 928, 907, 887, 867, 849, 831, 815, 798, 783, 768, 753, 739, 726, 713, 701, 689, 677, 666, 655, 644, 634, 624, 615, 605, 596, 588, 579, 571, 563, 555, 547, 540, 533, 526, 519, 512, 506, 500, 493, 487, 482, 476, 470, 465, 459, 454, 449, 444, 439, 435, 430, 425, 421, 416, 412, 408, 404, 400, 396, 392, 388, 384, 381, 377, 374, 370, 367, 363, 360, 357, 354, 351, 348, 345, 342, 339, 336, 333, 330, 328, 325, 322, 320, 317, 315, 
		15708, 13734, 11903, 10304, 8961, 7854, 6947, 6202, 5586, 5071, 4636, 4266, 3948, 3672, 3430, 3218, 3029, 2861, 2709, 2573, 2450, 2337, 2235, 2141, 2054, 1974, 1900, 1831, 1767, 1707, 1651, 1599, 1550, 1504, 1460, 1419, 1380, 1343, 1308, 1275, 1244, 1214, 1185, 1158, 1132, 1107, 1083, 1060, 1038, 1017, 997, 977, 959, 941, 923, 907, 890, 875, 860, 845, 831, 818, 805, 792, 780, 768, 756, 745, 734, 723, 713, 703, 693, 684, 675, 666, 657, 648, 640, 632, 624, 617, 609, 602, 595, 588, 581, 574, 568, 561, 555, 549, 543, 537, 531, 526, 520, 515, 510, 505, 500, 495, 490, 485, 480, 476, 471, 467, 463, 458, 454, 450, 446, 442, 438, 435, 431, 427, 423, 420, 416, 413, 410, 406, 403, 400, 397, 393, 
		15708, 14056, 12490, 11071, 9828, 8761, 7854, 7086, 6435, 5880, 5404, 4993, 4636, 4324, 4049, 3805, 3588, 3393, 3218, 3059, 2915, 2783, 2663, 2552, 2450, 2355, 2268, 2187, 2111, 2040, 1974, 1912, 1853, 1799, 1747, 1698, 1651, 1608, 1566, 1526, 1489, 1453, 1419, 1386, 1355, 1326, 1297, 1270, 1244, 1218, 1194, 1171, 1149, 1127, 1107, 1087, 1067, 1049, 1031, 1013, 997, 980, 965, 950, 935, 920, 907, 893, 880, 867, 855, 843, 831, 820, 809, 798, 788, 778, 768, 758, 749, 739, 730, 722, 713, 705, 697, 689, 681, 673, 666, 658, 651, 644, 637, 631, 624, 618, 611, 605, 599, 593, 588, 582, 576, 571, 565, 560, 555, 550, 545, 540, 535, 530, 526, 521, 517, 512, 508, 504, 500, 495, 491, 487, 483, 480, 476, 472, 
		15708, 14289, 12925, 11659, 10517, 9505, 8622, 7854, 7188, 6610, 6107, 5667, 5281, 4939, 4636, 4366, 4124, 3906, 3709, 3530, 3367, 3218, 3081, 2954, 2838, 2730, 2630, 2537, 2450, 2368, 2292, 2221, 2154, 2090, 2030, 1974, 1920, 1870, 1822, 1776, 1732, 1691, 1651, 1614, 1578, 1543, 1510, 1478, 1448, 1419, 1391, 1364, 1338, 1313, 1289, 1266, 1244, 1222, 1201, 1181, 1161, 1143, 1124, 1107, 1089, 1073, 1057, 1041, 1026, 1011, 997, 983, 969, 956, 943, 931, 918, 907, 895, 884, 873, 862, 852, 841, 831, 822, 812, 803, 794, 785, 776, 768, 759, 751, 743, 736, 728, 720, 713, 706, 699, 692, 685, 679, 672, 666, 659, 653, 647, 641, 636, 630, 624, 619, 613, 608, 603, 598, 593, 588, 583, 578, 573, 568, 564, 559, 555, 551, 
		15708, 14464, 13258, 12120, 11071, 10122, 9273, 8520, 7854, 7266, 6747, 6288, 5880, 5517, 5191, 4900, 4636, 4398, 4182, 3985, 3805, 3640, 3488, 3347, 3218, 3097, 2985, 2881, 2783, 2692, 2606, 2526, 2450, 2378, 2311, 2247, 2187, 2129, 2075, 2023, 1974, 1927, 1882, 1839, 1799, 1759, 1722, 1686, 1651, 1618, 1587, 1556, 1526, 1498, 1471, 1444, 1419, 1394, 1371, 1348, 1326, 1304, 1283, 1263, 1244, 1225, 1206, 1188, 1171, 1154, 1138, 1122, 1107, 1092, 1077, 1063, 1049, 1035, 1022, 1009, 997, 984, 973, 961, 950, 938, 928, 917, 907, 896, 887, 877, 867, 858, 849, 840, 831, 823, 815, 806, 798, 790, 783, 775, 768, 760, 753, 746, 739, 733, 726, 719, 713, 707, 701, 695, 689, 683, 677, 671, 666, 660, 655, 649, 644, 639, 634, 629, 
		15708, 14601, 13521, 12490, 11526, 10637, 9828, 9098, 8442, 7854, 7328, 6857, 6435, 6055, 5713, 5404, 5124, 4869, 4636, 4424, 4229, 4049, 3883, 3730, 3588, 3456, 3332, 3218, 3110, 3009, 2915, 2826, 2742, 2663, 2588, 2517, 2450, 2386, 2326, 2268, 2213, 2161, 2111, 2063, 2018, 1974, 1932, 1892, 1853, 1816, 1781, 1747, 1714, 1682, 1651, 1622, 1594, 1566, 1539, 1514, 1489, 1465, 1442, 1419, 1397, 1376, 1355, 1335, 1316, 1297, 1279, 1261, 1244, 1227, 1210, 1194, 1179, 1164, 1149, 1134, 1120, 1107, 1093, 1080, 1067, 1055, 1043, 1031, 1019, 1008, 997, 986, 975, 965, 955, 945, 935, 925, 916, 907, 898, 889, 880, 872, 863, 855, 847, 839, 831, 824, 816, 809, 802, 795, 788, 781, 774, 768, 761, 755, 749, 742, 736, 730, 725, 719, 713, 707, 
		15708, 14711, 13734, 12793, 11903, 11071, 10304, 9601, 8961, 8380, 7854, 7378, 6947, 6557, 6202, 5880, 5586, 5317, 5071, 4845, 4636, 4444, 4266, 4101, 3948, 3805, 3672, 3547, 3430, 3321, 3218, 3120, 3029, 2942, 2861, 2783, 2709, 2640, 2573, 2510, 2450, 2392, 2337, 2285, 2235, 2187, 2141, 2096, 2054, 2013, 1974, 1936, 1900, 1865, 1831, 1799, 1767, 1737, 1707, 1679, 1651, 1625, 1599, 1574, 1550, 1526, 1504, 1482, 1460, 1439, 1419, 1399, 1380, 1361, 1343, 1326, 1308, 1291, 1275, 1259, 1244, 1228, 1214, 1199, 1185, 1171, 1158, 1144, 1132, 1119, 1107, 1095, 1083, 1071, 1060, 1049, 1038, 1027, 1017, 1007, 997, 987, 977, 968, 959, 950, 941, 932, 923, 915, 907, 898, 890, 883, 875, 867, 860, 853, 845, 838, 831, 825, 818, 811, 805, 798, 792, 786, 
		15708, 14801, 13909, 13045, 12220, 11442, 10714, 10041, 9420, 8851, 8330, 7854, 7419, 7023, 6660, 6327, 6023, 5743, 5485, 5248, 5028, 4825, 4636, 4461, 4298, 4145, 4002, 3869, 3743, 3625, 3514, 3410, 3311, 3218, 3129, 3045, 2965, 2890, 2818, 2749, 2684, 2621, 2562, 2504, 2450, 2397, 2347, 2299, 2253, 2208, 2166, 2124, 2085, 2046, 2010, 1974, 1940, 1906, 1874, 1843, 1813, 1784, 1756, 1729, 1702, 1676, 1651, 1627, 1604, 1581, 1559, 1537, 1516, 1496, 1476, 1456, 1437, 1419, 1401, 1384, 1366, 1350, 1334, 1318, 1302, 1287, 1272, 1258, 1244, 1230, 1216, 1203, 1190, 1177, 1165, 1153, 1141, 1129, 1118, 1107, 1096, 1085, 1074, 1064, 1054, 1044, 1034, 1024, 1015, 1006, 997, 988, 979, 970, 962, 954, 945, 937, 930, 922, 914, 907, 899, 892, 885, 878, 871, 864, 
		15708, 14877, 14056, 13258, 12490, 11760, 11071, 10427, 9828, 9273, 8761, 8288, 7854, 7454, 7086, 6747, 6435, 6147, 5880, 5633, 5404, 5191, 4993, 4809, 4636, 4475, 4324, 4182, 4049, 3923, 3805, 3693, 3588, 3488, 3393, 3303, 3218, 3136, 3059, 2985, 2915, 2847, 2783, 2721, 2663, 2606, 2552, 2500, 2450, 2402, 2355, 2311, 2268, 2227, 2187, 2148, 2111, 2075, 2040, 2007, 1974, 1942, 1912, 1882, 1853, 1826, 1799, 1772, 1747, 1722, 1698, 1674, 1651, 1629, 1608, 1587, 1566, 1546, 1526, 1507, 1489, 1471, 1453, 1436, 1419, 1402, 1386, 1371, 1355, 1340, 1326, 1311, 1297, 1283, 1270, 1257, 1244, 1231, 1218, 1206, 1194, 1183, 1171, 1160, 1149, 1138, 1127, 1117, 1107, 1097, 1087, 1077, 1067, 1058, 1049, 1040, 1031, 1022, 1013, 1005, 997, 989, 980, 973, 965, 957, 950, 942, 
		15708, 14940, 14181, 13440, 12723, 12036, 11384, 10769, 10191, 9653, 9151, 8685, 8254, 7854, 7484, 7141, 6823, 6528, 6255, 6001, 5764, 5543, 5337, 5145, 4964, 4795, 4636, 4487, 4347, 4214, 4089, 3971, 3859, 3753, 3652, 3556, 3465, 3379, 3296, 3218, 3142, 3070, 3002, 2936, 2873, 2812, 2754, 2698, 2645, 2593, 2544, 2496, 2450, 2405, 2362, 2321, 2281, 2242, 2205, 2169, 2134, 2100, 2067, 2035, 2004, 1974, 1945, 1916, 1889, 1862, 1836, 1811, 1786, 1762, 1739, 1716, 1694, 1673, 1651, 1631, 1611, 1591, 1572, 1554, 1535, 1518, 1500, 1483, 1467, 1450, 1435, 1419, 1404, 1389, 1374, 1360, 1346, 1332, 1319, 1306, 1293, 1280, 1268, 1255, 1244, 1232, 1220, 1209, 1198, 1187, 1176, 1166, 1156, 1145, 1135, 1126, 1116, 1107, 1097, 1088, 1079, 1070, 1062, 1053, 1045, 1036, 1028, 1020, 
		15708, 14995, 14289, 13597, 12925, 12278, 11659, 11071, 10517, 9995, 9505, 9048, 8622, 8224, 7854, 7509, 7188, 6889, 6610, 6350, 6107, 5880, 5667, 5468, 5281, 5105, 4939, 4784, 4636, 4498, 4366, 4242, 4124, 4012, 3906, 3805, 3709, 3617, 3530, 3446, 3367, 3290, 3218, 3148, 3081, 3016, 2954, 2895, 2838, 2783, 2730, 2679, 2630, 2583, 2537, 2493, 2450, 2408, 2368, 2330, 2292, 2256, 2221, 2187, 2154, 2121, 2090, 2060, 2030, 2002, 1974, 1947, 1920, 1895, 1870, 1845, 1822, 1799, 1776, 1754, 1732, 1711, 1691, 1671, 1651, 1632, 1614, 1596, 1578, 1560, 1543, 1526, 1510, 1494, 1478, 1463, 1448, 1433, 1419, 1405, 1391, 1377, 1364, 1351, 1338, 1326, 1313, 1301, 1289, 1277, 1266, 1255, 1244, 1233, 1222, 1211, 1201, 1191, 1181, 1171, 1161, 1152, 1143, 1133, 1124, 1115, 1107, 1098, 
		15708, 15042, 14382, 13734, 13102, 12490, 11903, 11342, 10808, 10304, 9828, 9380, 8961, 8567, 8199, 7854, 7532, 7230, 6947, 6683, 6435, 6202, 5984, 5779, 5586, 5404, 5233, 5071, 4918, 4773, 4636, 4507, 4383, 4266, 4155, 4049, 3948, 3852, 3760, 3672, 3588, 3507, 3430, 3356, 3286, 3218, 3152, 3089, 3029, 2971, 2915, 2861, 2808, 2758, 2709, 2663, 2617, 2573, 2531, 2490, 2450, 2411, 2374, 2337, 2302, 2268, 2235, 2202, 2171, 2141, 2111, 2082, 2054, 2027, 2000, 1974, 1949, 1924, 1900, 1876, 1853, 1831, 1809, 1788, 1767, 1747, 1727, 1707, 1688, 1670, 1651, 1634, 1616, 1599, 1582, 1566, 1550, 1534, 1519, 1504, 1489, 1474, 1460, 1446, 1432, 1419, 1406, 1393, 1380, 1368, 1355, 1343, 1331, 1320, 1308, 1297, 1286, 1275, 1264, 1254, 1244, 1233, 1223, 1214, 1204, 1194, 1185, 1176, 
		15708, 15084, 14464, 13854, 13258, 12679, 12120, 11584, 11071, 10584, 10122, 9685, 9273, 8885, 8520, 8176, 7854, 7551, 7266, 6999, 6747, 6511, 6288, 6078, 5880, 5693, 5517, 5350, 5191, 5042, 4900, 4765, 4636, 4515, 4398, 4288, 4182, 4081, 3985, 3893, 3805, 3721, 3640, 3562, 3488, 3416, 3347, 3281, 3218, 3156, 3097, 3040, 2985, 2932, 2881, 2831, 2783, 2737, 2692, 2648, 2606, 2565, 2526, 2487, 2450, 2414, 2378, 2344, 2311, 2279, 2247, 2216, 2187, 2158, 2129, 2102, 2075, 2049, 2023, 1998, 1974, 1950, 1927, 1904, 1882, 1861, 1839, 1819, 1799, 1779, 1759, 1740, 1722, 1704, 1686, 1669, 1651, 1635, 1618, 1602, 1587, 1571, 1556, 1541, 1526, 1512, 1498, 1484, 1471, 1457, 1444, 1432, 1419, 1407, 1394, 1382, 1371, 1359, 1348, 1337, 1326, 1315, 1304, 1294, 1283, 1273, 1263, 1253, 
		15708, 15120, 14537, 13961, 13397, 12847, 12315, 11802, 11310, 10839, 10391, 9965, 9561, 9179, 8819, 8478, 8157, 7854, 7568, 7299, 7045, 6805, 6579, 6365, 6163, 5972, 5791, 5619, 5457, 5302, 5155, 5016, 4883, 4757, 4636, 4522, 4412, 4307, 4207, 4111, 4019, 3931, 3846, 3765, 3687, 3612, 3540, 3471, 3404, 3339, 3277, 3218, 3160, 3104, 3050, 2998, 2947, 2898, 2851, 2805, 2761, 2718, 2676, 2636, 2596, 2558, 2521, 2485, 2450, 2416, 2382, 2350, 2319, 2288, 2258, 2229, 2201, 2173, 2146, 2120, 2094, 2069, 2044, 2020, 1997, 1974, 1952, 1930, 1908, 1887, 1867, 1847, 1827, 1808, 1789, 1771, 1753, 1735, 1718, 1701, 1684, 1668, 1651, 1636, 1620, 1605, 1590, 1576, 1561, 1547, 1533, 1520, 1506, 1493, 1480, 1468, 1455, 1443, 1431, 1419, 1407, 1396, 1385, 1373, 1362, 1352, 1341, 1331, 
		15708, 15153, 14601, 14056, 13521, 12998, 12490, 11999, 11526, 11071, 10637, 10222, 9828, 9453, 9098, 8761, 8442, 8140, 7854, 7584, 7328, 7086, 6857, 6640, 6435, 6240, 6055, 5880, 5713, 5555, 5404, 5261, 5124, 4993, 4869, 4750, 4636, 4528, 4424, 4324, 4229, 4137, 4049, 3964, 3883, 3805, 3730, 3657, 3588, 3520, 3456, 3393, 3332, 3274, 3218, 3163, 3110, 3059, 3009, 2961, 2915, 2869, 2826, 2783, 2742, 2702, 2663, 2625, 2588, 2552, 2517, 2483, 2450, 2418, 2386, 2355, 2326, 2296, 2268, 2240, 2213, 2187, 2161, 2136, 2111, 2087, 2063, 2040, 2018, 1996, 1974, 1953, 1932, 1912, 1892, 1873, 1853, 1835, 1816, 1799, 1781, 1764, 1747, 1730, 1714, 1698, 1682, 1667, 1651, 1637, 1622, 1608, 1594, 1580, 1566, 1553, 1539, 1526, 1514, 1501, 1489, 1477, 1465, 1453, 1442, 1430, 1419, 1408, 
		15708, 15182, 14659, 14142, 13633, 13135, 12649, 12178, 11723, 11284, 10863, 10460, 10075, 9707, 9358, 9025, 8709, 8409, 8124, 7854, 7598, 7354, 7124, 6904, 6696, 6499, 6311, 6132, 5962, 5800, 5646, 5499, 5358, 5224, 5096, 4973, 4856, 4744, 4636, 4533, 4434, 4340, 4248, 4161, 4076, 3995, 3917, 3842, 3769, 3699, 3631, 3566, 3503, 3442, 3383, 3326, 3271, 3218, 3166, 3115, 3067, 3020, 2974, 2929, 2886, 2844, 2803, 2763, 2725, 2687, 2650, 2615, 2580, 2546, 2513, 2481, 2450, 2419, 2389, 2360, 2332, 2304, 2277, 2250, 2224, 2199, 2174, 2150, 2126, 2103, 2081, 2058, 2037, 2015, 1994, 1974, 1954, 1934, 1915, 1896, 1878, 1859, 1842, 1824, 1807, 1790, 1774, 1757, 1741, 1726, 1710, 1695, 1680, 1666, 1651, 1637, 1624, 1610, 1596, 1583, 1570, 1558, 1545, 1533, 1520, 1508, 1497, 1485, 
		15708, 15208, 14711, 14219, 13734, 13258, 12793, 12341, 11903, 11479, 11071, 10680, 10304, 9944, 9601, 9273, 8961, 8663, 8380, 8110, 7854, 7610, 7378, 7157, 6947, 6747, 6557, 6375, 6202, 6037, 5880, 5730, 5586, 5449, 5317, 5191, 5071, 4956, 4845, 4739, 4636, 4538, 4444, 4354, 4266, 4182, 4101, 4023, 3948, 3875, 3805, 3737, 3672, 3608, 3547, 3488, 3430, 3375, 3321, 3268, 3218, 3168, 3120, 3074, 3029, 2985, 2942, 2901, 2861, 2821, 2783, 2746, 2709, 2674, 2640, 2606, 2573, 2541, 2510, 2480, 2450, 2421, 2392, 2365, 2337, 2311, 2285, 2260, 2235, 2210, 2187, 2163, 2141, 2118, 2096, 2075, 2054, 2033, 2013, 1993, 1974, 1955, 1936, 1918, 1900, 1882, 1865, 1848, 1831, 1815, 1799, 1783, 1767, 1752, 1737, 1722, 1707, 1693, 1679, 1665, 1651, 1638, 1625, 1612, 1599, 1587, 1574, 1562, 
		15708, 15232, 14758, 14289, 13826, 13371, 12925, 12490, 12068, 11659, 11264, 10883, 10517, 10165, 9828, 9505, 9197, 8903, 8622, 8354, 8098, 7854, 7621, 7400, 7188, 6987, 6794, 6610, 6435, 6267, 6107, 5954, 5808, 5667, 5533, 5404, 5281, 5162, 5049, 4939, 4834, 4734, 4636, 4543, 4453, 4366, 4283, 4202, 4124, 4049, 3976, 3906, 3838, 3772, 3709, 3647, 3588, 3530, 3474, 3419, 3367, 3316, 3266, 3218, 3171, 3125, 3081, 3037, 2995, 2954, 2915, 2876, 2838, 2801, 2765, 2730, 2696, 2663, 2630, 2598, 2567, 2537, 2507, 2478, 2450, 2422, 2395, 2368, 2343, 2317, 2292, 2268, 2244, 2221, 2198, 2176, 2154, 2132, 2111, 2090, 2070, 2050, 2030, 2011, 1992, 1974, 1956, 1938, 1920, 1903, 1886, 1870, 1853, 1837, 1822, 1806, 1791, 1776, 1761, 1747, 1732, 1718, 1705, 1691, 1678, 1664, 1651, 1639, 
		15708, 15254, 14801, 14353, 13909, 13473, 13045, 12627, 12220, 11825, 11442, 11071, 10714, 10371, 10041, 9724, 9420, 9129, 8851, 8584, 8330, 8086, 7854, 7632, 7419, 7217, 7023, 6837, 6660, 6490, 6327, 6172, 6023, 5880, 5743, 5612, 5485, 5364, 5248, 5136, 5028, 4925, 4825, 4729, 4636, 4547, 4461, 4378, 4298, 4220, 4145, 4073, 4002, 3935, 3869, 3805, 3743, 3683, 3625, 3569, 3514, 3461, 3410, 3360, 3311, 3264, 3218, 3173, 3129, 3087, 3045, 3005, 2965, 2927, 2890, 2853, 2818, 2783, 2749, 2716, 2684, 2652, 2621, 2591, 2562, 2533, 2504, 2477, 2450, 2423, 2397, 2372, 2347, 2323, 2299, 2276, 2253, 2230, 2208, 2187, 2166, 2145, 2124, 2104, 2085, 2065, 2046, 2028, 2010, 1992, 1974, 1957, 1940, 1923, 1906, 1890, 1874, 1859, 1843, 1828, 1813, 1799, 1784, 1770, 1756, 1742, 1729, 1715, 
		15708, 15273, 14841, 14411, 13986, 13567, 13156, 12754, 12361, 11978, 11607, 11247, 10899, 10563, 10240, 9929, 9630, 9343, 9068, 8803, 8551, 8308, 8076, 7854, 7641, 7438, 7243, 7056, 6877, 6705, 6541, 6383, 6232, 6087, 5948, 5814, 5685, 5562, 5443, 5328, 5218, 5112, 5010, 4912, 4817, 4725, 4636, 4551, 4468, 4389, 4311, 4237, 4164, 4094, 4027, 3961, 3897, 3835, 3775, 3717, 3661, 3606, 3552, 3500, 3450, 3401, 3353, 3307, 3262, 3218, 3175, 3133, 3092, 3052, 3013, 2976, 2939, 2903, 2867, 2833, 2799, 2767, 2735, 2703, 2673, 2643, 2613, 2585, 2556, 2529, 2502, 2476, 2450, 2424, 2400, 2375, 2352, 2328, 2305, 2283, 2261, 2239, 2218, 2197, 2177, 2156, 2137, 2117, 2098, 2080, 2061, 2043, 2025, 2008, 1991, 1974, 1957, 1941, 1925, 1909, 1894, 1878, 1863, 1849, 1834, 1820, 1806, 1792, 
		15708, 15292, 14877, 14464, 14056, 13654, 13258, 12870, 12490, 12120, 11760, 11410, 11071, 10744, 10427, 10122, 9828, 9545, 9273, 9012, 8761, 8520, 8288, 8067, 7854, 7650, 7454, 7266, 7086, 6913, 6747, 6588, 6435, 6288, 6147, 6011, 5880, 5754, 5633, 5517, 5404, 5296, 5191, 5091, 4993, 4900, 4809, 4721, 4636, 4555, 4475, 4398, 4324, 4252, 4182, 4115, 4049, 3985, 3923, 3863, 3805, 3748, 3693, 3640, 3588, 3537, 3488, 3440, 3393, 3347, 3303, 3260, 3218, 3176, 3136, 3097, 3059, 3021, 2985, 2949, 2915, 2881, 2847, 2815, 2783, 2752, 2721, 2692, 2663, 2634, 2606, 2579, 2552, 2526, 2500, 2475, 2450, 2426, 2402, 2378, 2355, 2333, 2311, 2289, 2268, 2247, 2227, 2206, 2187, 2167, 2148, 2129, 2111, 2093, 2075, 2057, 2040, 2023, 2007, 1990, 1974, 1958, 1942, 1927, 1912, 1897, 1882, 1868, 
		15708, 15308, 14910, 14514, 14121, 13734, 13353, 12978, 12611, 12252, 11903, 11563, 11233, 10913, 10603, 10304, 10015, 9736, 9468, 9209, 8961, 8721, 8491, 8270, 8058, 7854, 7658, 7470, 7289, 7115, 6947, 6787, 6632, 6483, 6340, 6202, 6070, 5942, 5819, 5700, 5586, 5476, 5369, 5266, 5167, 5071, 4978, 4889, 4802, 4718, 4636, 4558, 4481, 4408, 4336, 4266, 4199, 4133, 4070, 4008, 3948, 3890, 3833, 3778, 3724, 3672, 3621, 3571, 3523, 3476, 3430, 3386, 3342, 3299, 3258, 3218, 3178, 3139, 3102, 3065, 3029, 2994, 2959, 2926, 2893, 2861, 2829, 2798, 2768, 2738, 2709, 2681, 2653, 2626, 2599, 2573, 2548, 2522, 2498, 2474, 2450, 2426, 2404, 2381, 2359, 2337, 2316, 2295, 2275, 2255, 2235, 2215, 2196, 2177, 2159, 2141, 2123, 2105, 2088, 2071, 2054, 2037, 2021, 2005, 1989, 1974, 1959, 1944, 
		15708, 15324, 14940, 14559, 14181, 13808, 13440, 13078, 12723, 12376, 12036, 11706, 11384, 11071, 10769, 10475, 10191, 9917, 9653, 9397, 9151, 8914, 8685, 8465, 8254, 8050, 7854, 7665, 7484, 7309, 7141, 6979, 6823, 6673, 6528, 6389, 6255, 6125, 6001, 5880, 5764, 5651, 5543, 5438, 5337, 5239, 5145, 5053, 4964, 4878, 4795, 4715, 4636, 4561, 4487, 4416, 4347, 4279, 4214, 4151, 4089, 4029, 3971, 3914, 3859, 3805, 3753, 3702, 3652, 3604, 3556, 3510, 3465, 3422, 3379, 3337, 3296, 3256, 3218, 3179, 3142, 3106, 3070, 3036, 3002, 2968, 2936, 2904, 2873, 2842, 2812, 2783, 2754, 2726, 2698, 2671, 2645, 2619, 2593, 2568, 2544, 2520, 2496, 2473, 2450, 2427, 2405, 2384, 2362, 2342, 2321, 2301, 2281, 2262, 2242, 2223, 2205, 2187, 2169, 2151, 2134, 2117, 2100, 2083, 2067, 2051, 2035, 2019, 
		15708, 15338, 14969, 14601, 14237, 13877, 13521, 13171, 12827, 12490, 12161, 11839, 11526, 11221, 10924, 10637, 10358, 10089, 9828, 9576, 9332, 9098, 8871, 8652, 8442, 8238, 8043, 7854, 7672, 7497, 7328, 7165, 7009, 6857, 6711, 6571, 6435, 6304, 6178, 6055, 5937, 5824, 5713, 5607, 5504, 5404, 5308, 5214, 5124, 5036, 4951, 4869, 4789, 4712, 4636, 4563, 4493, 4424, 4357, 4292, 4229, 4167, 4107, 4049, 3992, 3937, 3883, 3831, 3780, 3730, 3681, 3634, 3588, 3543, 3499, 3456, 3414, 3373, 3332, 3293, 3255, 3218, 3181, 3145, 3110, 3076, 3042, 3009, 2977, 2945, 2915, 2884, 2855, 2826, 2797, 2769, 2742, 2715, 2688, 2663, 2637, 2612, 2588, 2564, 2540, 2517, 2494, 2472, 2450, 2428, 2407, 2386, 2366, 2345, 2326, 2306, 2287, 2268, 2249, 2231, 2213, 2195, 2178, 2161, 2144, 2127, 2111, 2095, 
		15708, 15351, 14995, 14641, 14289, 13941, 13597, 13258, 12925, 12598, 12278, 11965, 11659, 11361, 11071, 10790, 10517, 10251, 9995, 9746, 9505, 9273, 9048, 8831, 8622, 8419, 8224, 8036, 7854, 7679, 7509, 7346, 7188, 7036, 6889, 6747, 6610, 6478, 6350, 6227, 6107, 5992, 5880, 5772, 5667, 5566, 5468, 5373, 5281, 5191, 5105, 5021, 4939, 4860, 4784, 4709, 4636, 4566, 4498, 4431, 4366, 4303, 4242, 4182, 4124, 4067, 4012, 3959, 3906, 3855, 3805, 3756, 3709, 3663, 3617, 3573, 3530, 3488, 3446, 3406, 3367, 3328, 3290, 3254, 3218, 3182, 3148, 3114, 3081, 3048, 3016, 2985, 2954, 2924, 2895, 2866, 2838, 2810, 2783, 2756, 2730, 2704, 2679, 2654, 2630, 2606, 2583, 2559, 2537, 2514, 2493, 2471, 2450, 2429, 2408, 2388, 2368, 2349, 2330, 2311, 2292, 2274, 2256, 2238, 2221, 2204, 2187, 2170, 
		15708, 15363, 15019, 14677, 14337, 14001, 13668, 13339, 13016, 12699, 12387, 12083, 11785, 11494, 11210, 10935, 10666, 10406, 10153, 9908, 9670, 9441, 9218, 9003, 8795, 8593, 8399, 8211, 8029, 7854, 7685, 7521, 7363, 7210, 7062, 6919, 6781, 6648, 6519, 6394, 6273, 6156, 6043, 5934, 5827, 5725, 5625, 5528, 5435, 5344, 5256, 5170, 5087, 5007, 4928, 4852, 4778, 4706, 4636, 4568, 4502, 4438, 4375, 4314, 4254, 4197, 4140, 4085, 4031, 3979, 3928, 3878, 3829, 3781, 3735, 3690, 3645, 3602, 3560, 3518, 3478, 3438, 3399, 3361, 3324, 3288, 3252, 3218, 3183, 3150, 3117, 3085, 3054, 3023, 2992, 2963, 2934, 2905, 2877, 2850, 2823, 2796, 2770, 2744, 2719, 2695, 2671, 2647, 2623, 2600, 2578, 2555, 2534, 2512, 2491, 2470, 2450, 2430, 2410, 2390, 2371, 2352, 2334, 2315, 2297, 2280, 2262, 2245, 
		15708, 15375, 15042, 14711, 14382, 14056, 13734, 13416, 13102, 12793, 12490, 12194, 11903, 11619, 11342, 11071, 10808, 10552, 10304, 10062, 9828, 9601, 9380, 9167, 8961, 8761, 8567, 8380, 8199, 8023, 7854, 7690, 7532, 7378, 7230, 7086, 6947, 6813, 6683, 6557, 6435, 6317, 6202, 6092, 5984, 5880, 5779, 5681, 5586, 5494, 5404, 5317, 5233, 5151, 5071, 4993, 4918, 4845, 4773, 4704, 4636, 4571, 4507, 4444, 4383, 4324, 4266, 4210, 4155, 4101, 4049, 3998, 3948, 3899, 3852, 3805, 3760, 3715, 3672, 3629, 3588, 3547, 3507, 3468, 3430, 3393, 3356, 3321, 3286, 3251, 3218, 3185, 3152, 3120, 3089, 3059, 3029, 2999, 2971, 2942, 2915, 2887, 2861, 2834, 2808, 2783, 2758, 2734, 2709, 2686, 2663, 2640, 2617, 2595, 2573, 2552, 2531, 2510, 2490, 2470, 2450, 2430, 2411, 2392, 2374, 2355, 2337, 2320, 
		15708, 15385, 15064, 14743, 14425, 14109, 13796, 13487, 13182, 12882, 12588, 12298, 12015, 11737, 11466, 11201, 10943, 10692, 10447, 10209, 9978, 9754, 9536, 9325, 9120, 8921, 8729, 8543, 8362, 8187, 8018, 7854, 7695, 7542, 7393, 7249, 7109, 6974, 6843, 6716, 6593, 6474, 6358, 6246, 6138, 6032, 5930, 5831, 5734, 5641, 5550, 5462, 5376, 5292, 5211, 5133, 5056, 4981, 4909, 4838, 4769, 4702, 4636, 4573, 4511, 4450, 4391, 4334, 4277, 4223, 4169, 4117, 4066, 4016, 3967, 3919, 3873, 3827, 3783, 3739, 3697, 3655, 3614, 3574, 3535, 3497, 3460, 3423, 3387, 3352, 3317, 3283, 3250, 3218, 3186, 3154, 3123, 3093, 3064, 3035, 3006, 2978, 2951, 2923, 2897, 2871, 2845, 2820, 2795, 2771, 2747, 2723, 2700, 2677, 2655, 2633, 2611, 2590, 2569, 2548, 2528, 2508, 2488, 2469, 2450, 2431, 2412, 2394, 
		15708, 15396, 15084, 14773, 14464, 14158, 13854, 13554, 13258, 12966, 12679, 12397, 12120, 11849, 11584, 11325, 11071, 10825, 10584, 10350, 10122, 9900, 9685, 9476, 9273, 9076, 8885, 8699, 8520, 8345, 8176, 8013, 7854, 7700, 7551, 7407, 7266, 7131, 6999, 6871, 6747, 6627, 6511, 6398, 6288, 6181, 6078, 5978, 5880, 5785, 5693, 5604, 5517, 5432, 5350, 5269, 5191, 5116, 5042, 4970, 4900, 4831, 4765, 4700, 4636, 4575, 4515, 4456, 4398, 4342, 4288, 4234, 4182, 4131, 4081, 4033, 3985, 3939, 3893, 3849, 3805, 3762, 3721, 3680, 3640, 3601, 3562, 3525, 3488, 3452, 3416, 3381, 3347, 3314, 3281, 3249, 3218, 3187, 3156, 3126, 3097, 3068, 3040, 3012, 2985, 2958, 2932, 2906, 2881, 2856, 2831, 2807, 2783, 2760, 2737, 2714, 2692, 2670, 2648, 2627, 2606, 2585, 2565, 2545, 2526, 2506, 2487, 2468, 
		15708, 15405, 15103, 14801, 14502, 14204, 13909, 13618, 13330, 13045, 12766, 12490, 12220, 11955, 11696, 11442, 11193, 10951, 10714, 10484, 10259, 10041, 9828, 9621, 9420, 9225, 9035, 8851, 8672, 8498, 8330, 8166, 8008, 7854, 7705, 7560, 7419, 7283, 7151, 7023, 6898, 6777, 6660, 6546, 6435, 6327, 6223, 6122, 6023, 5927, 5834, 5743, 5655, 5569, 5485, 5404, 5325, 5248, 5173, 5100, 5028, 4959, 4891, 4825, 4761, 4698, 4636, 4577, 4518, 4461, 4405, 4351, 4298, 4246, 4195, 4145, 4096, 4049, 4002, 3957, 3912, 3869, 3826, 3784, 3743, 3703, 3664, 3625, 3588, 3551, 3514, 3479, 3444, 3410, 3376, 3343, 3311, 3279, 3248, 3218, 3187, 3158, 3129, 3101, 3073, 3045, 3018, 2992, 2965, 2940, 2915, 2890, 2865, 2841, 2818, 2794, 2772, 2749, 2727, 2705, 2684, 2663, 2642, 2621, 2601, 2581, 2562, 2542, 
		15708, 15414, 15120, 14828, 14537, 14248, 13961, 13678, 13397, 13120, 12847, 12579, 12315, 12056, 11802, 11553, 11310, 11071, 10839, 10612, 10391, 10175, 9965, 9760, 9561, 9368, 9179, 8997, 8819, 8646, 8478, 8315, 8157, 8003, 7854, 7709, 7568, 7432, 7299, 7170, 7045, 6923, 6805, 6690, 6579, 6470, 6365, 6263, 6163, 6066, 5972, 5880, 5791, 5704, 5619, 5537, 5457, 5378, 5302, 5228, 5155, 5085, 5016, 4949, 4883, 4819, 4757, 4696, 4636, 4578, 4522, 4466, 4412, 4359, 4307, 4256, 4207, 4158, 4111, 4064, 4019, 3974, 3931, 3888, 3846, 3805, 3765, 3726, 3687, 3649, 3612, 3576, 3540, 3505, 3471, 3437, 3404, 3371, 3339, 3308, 3277, 3247, 3218, 3188, 3160, 3132, 3104, 3077, 3050, 3024, 2998, 2972, 2947, 2923, 2898, 2875, 2851, 2828, 2805, 2783, 2761, 2739, 2718, 2697, 2676, 2656, 2636, 2616, 
		15708, 15422, 15137, 14853, 14570, 14289, 14010, 13734, 13461, 13191, 12925, 12663, 12405, 12152, 11903, 11659, 11420, 11186, 10958, 10735, 10517, 10304, 10096, 9894, 9697, 9505, 9319, 9137, 8961, 8789, 8622, 8459, 8301, 8148, 7999, 7854, 7713, 7576, 7443, 7314, 7188, 7066, 6947, 6832, 6720, 6610, 6504, 6401, 6300, 6202, 6107, 6015, 5924, 5836, 5751, 5667, 5586, 5507, 5430, 5354, 5281, 5209, 5139, 5071, 5004, 4939, 4876, 4814, 4753, 4694, 4636, 4580, 4525, 4471, 4418, 4366, 4316, 4266, 4218, 4170, 4124, 4079, 4034, 3991, 3948, 3906, 3865, 3825, 3785, 3747, 3709, 3672, 3635, 3599, 3564, 3530, 3496, 3463, 3430, 3398, 3367, 3336, 3305, 3276, 3246, 3218, 3189, 3161, 3134, 3107, 3081, 3054, 3029, 3004, 2979, 2954, 2930, 2907, 2883, 2861, 2838, 2816, 2794, 2772, 2751, 2730, 2709, 2689, 
		15708, 15430, 15153, 14877, 14601, 14328, 14056, 13787, 13521, 13258, 12998, 12743, 12490, 12243, 11999, 11760, 11526, 11296, 11071, 10852, 10637, 10427, 10222, 10023, 9828, 9638, 9453, 9273, 9098, 8927, 8761, 8599, 8442, 8288, 8140, 7995, 7854, 7717, 7584, 7454, 7328, 7206, 7086, 6970, 6857, 6747, 6640, 6536, 6435, 6336, 6240, 6147, 6055, 5967, 5880, 5796, 5713, 5633, 5555, 5479, 5404, 5332, 5261, 5191, 5124, 5058, 4993, 4931, 4869, 4809, 4750, 4693, 4636, 4582, 4528, 4475, 4424, 4373, 4324, 4276, 4229, 4182, 4137, 4092, 4049, 4006, 3964, 3923, 3883, 3844, 3805, 3767, 3730, 3693, 3657, 3622, 3588, 3554, 3520, 3488, 3456, 3424, 3393, 3362, 3332, 3303, 3274, 3246, 3218, 3190, 3163, 3136, 3110, 3084, 3059, 3034, 3009, 2985, 2961, 2938, 2915, 2892, 2869, 2847, 2826, 2804, 2783, 2762, 
		15708, 15438, 15168, 14899, 14631, 14365, 14100, 13838, 13579, 13322, 13068, 12818, 12572, 12329, 12091, 11856, 11626, 11401, 11180, 10964, 10752, 10546, 10344, 10146, 9954, 9766, 9583, 9404, 9230, 9060, 8895, 8734, 8577, 8425, 8276, 8132, 7991, 7854, 7721, 7591, 7465, 7342, 7222, 7105, 6992, 6881, 6774, 6669, 6567, 6468, 6371, 6276, 6184, 6095, 6007, 5922, 5839, 5758, 5678, 5601, 5526, 5452, 5380, 5310, 5242, 5175, 5109, 5046, 4983, 4922, 4862, 4804, 4747, 4691, 4636, 4583, 4531, 4479, 4429, 4380, 4332, 4285, 4239, 4193, 4149, 4106, 4063, 4021, 3980, 3940, 3900, 3862, 3824, 3787, 3750, 3714, 3679, 3644, 3610, 3577, 3544, 3512, 3480, 3449, 3418, 3388, 3358, 3329, 3301, 3272, 3245, 3218, 3191, 3164, 3138, 3113, 3088, 3063, 3038, 3014, 2991, 2968, 2945, 2922, 2900, 2878, 2856, 2835, 
		15708, 15445, 15182, 14920, 14659, 14400, 14142, 13886, 13633, 13382, 13135, 12890, 12649, 12412, 12178, 11948, 11723, 11501, 11284, 11071, 10863, 10659, 10460, 10265, 10075, 9889, 9707, 9530, 9358, 9189, 9025, 8865, 8709, 8557, 8409, 8265, 8124, 7987, 7854, 7724, 7598, 7474, 7354, 7237, 7124, 7013, 6904, 6799, 6696, 6596, 6499, 6404, 6311, 6220, 6132, 6046, 5962, 5880, 5800, 5722, 5646, 5571, 5499, 5428, 5358, 5290, 5224, 5159, 5096, 5034, 4973, 4914, 4856, 4800, 4744, 4690, 4636, 4584, 4533, 4483, 4434, 4387, 4340, 4293, 4248, 4204, 4161, 4118, 4076, 4035, 3995, 3956, 3917, 3879, 3842, 3805, 3769, 3734, 3699, 3665, 3631, 3599, 3566, 3534, 3503, 3472, 3442, 3412, 3383, 3354, 3326, 3298, 3271, 3244, 3218, 3191, 3166, 3140, 3115, 3091, 3067, 3043, 3020, 2996, 2974, 2951, 2929, 2907, 
		15708, 15452, 15196, 14940, 14686, 14433, 14181, 13932, 13685, 13440, 13198, 12959, 12723, 12490, 12261, 12036, 11815, 11597, 11384, 11175, 10969, 10769, 10572, 10380, 10191, 10008, 9828, 9653, 9481, 9314, 9151, 8992, 8837, 8685, 8538, 8394, 8254, 8117, 7984, 7854, 7727, 7604, 7484, 7367, 7252, 7141, 7032, 6926, 6823, 6722, 6624, 6528, 6435, 6344, 6255, 6168, 6083, 6001, 5920, 5841, 5764, 5688, 5615, 5543, 5473, 5404, 5337, 5271, 5207, 5145, 5083, 5023, 4964, 4907, 4850, 4795, 4741, 4688, 4636, 4586, 4536, 4487, 4439, 4393, 4347, 4302, 4258, 4214, 4172, 4130, 4089, 4049, 4009, 3971, 3933, 3895, 3859, 3823, 3787, 3753, 3719, 3685, 3652, 3620, 3588, 3556, 3526, 3495, 3465, 3436, 3407, 3379, 3351, 3323, 3296, 3270, 3243, 3218, 3192, 3167, 3142, 3118, 3094, 3070, 3047, 3024, 3002, 2979, 
		15708, 15458, 15208, 14959, 14711, 14464, 14219, 13976, 13734, 13495, 13258, 13024, 12793, 12566, 12341, 12120, 11903, 11689, 11479, 11273, 11071, 10873, 10680, 10490, 10304, 10122, 9944, 9770, 9601, 9435, 9273, 9115, 8961, 8810, 8663, 8520, 8380, 8243, 8110, 7981, 7854, 7731, 7610, 7493, 7378, 7266, 7157, 7051, 6947, 6846, 6747, 6651, 6557, 6465, 6375, 6288, 6202, 6119, 6037, 5958, 5880, 5804, 5730, 5657, 5586, 5517, 5449, 5382, 5317, 5254, 5191, 5131, 5071, 5013, 4956, 4900, 4845, 4791, 4739, 4687, 4636, 4587, 4538, 4491, 4444, 4398, 4354, 4309, 4266, 4224, 4182, 4141, 4101, 4062, 4023, 3985, 3948, 3911, 3875, 3840, 3805, 3771, 3737, 3704, 3672, 3640, 3608, 3577, 3547, 3517, 3488, 3459, 3430, 3402, 3375, 3347, 3321, 3294, 3268, 3243, 3218, 3193, 3168, 3144, 3120, 3097, 3074, 3051, 
		15708, 15464, 15221, 14978, 14735, 14494, 14255, 14017, 13781, 13547, 13316, 13087, 12861, 12638, 12417, 12201, 11987, 11777, 11571, 11368, 11170, 10974, 10783, 10596, 10412, 10232, 10056, 9884, 9716, 9552, 9391, 9234, 9081, 8931, 8785, 8642, 8502, 8366, 8234, 8104, 7977, 7854, 7734, 7616, 7501, 7389, 7280, 7173, 7069, 6967, 6868, 6771, 6677, 6584, 6494, 6406, 6320, 6236, 6153, 6073, 5995, 5918, 5843, 5769, 5698, 5627, 5559, 5492, 5426, 5361, 5298, 5237, 5176, 5117, 5060, 5003, 4947, 4893, 4839, 4787, 4736, 4686, 4636, 4588, 4541, 4494, 4449, 4404, 4360, 4317, 4275, 4233, 4192, 4152, 4113, 4074, 4036, 3999, 3962, 3926, 3891, 3856, 3822, 3788, 3755, 3723, 3691, 3659, 3628, 3598, 3568, 3538, 3509, 3481, 3452, 3425, 3397, 3371, 3344, 3318, 3292, 3267, 3242, 3218, 3193, 3169, 3146, 3123, 
		15708, 15470, 15232, 14995, 14758, 14523, 14289, 14056, 13826, 13597, 13371, 13146, 12925, 12706, 12490, 12278, 12068, 11862, 11659, 11460, 11264, 11071, 10883, 10698, 10517, 10339, 10165, 9995, 9828, 9665, 9505, 9350, 9197, 9048, 8903, 8761, 8622, 8486, 8354, 8224, 8098, 7974, 7854, 7736, 7621, 7509, 7400, 7293, 7188, 7086, 6987, 6889, 6794, 6701, 6610, 6522, 6435, 6350, 6267, 6186, 6107, 6030, 5954, 5880, 5808, 5737, 5667, 5599, 5533, 5468, 5404, 5342, 5281, 5221, 5162, 5105, 5049, 4993, 4939, 4886, 4834, 4784, 4734, 4685, 4636, 4589, 4543, 4498, 4453, 4409, 4366, 4324, 4283, 4242, 4202, 4163, 4124, 4086, 4049, 4012, 3976, 3941, 3906, 3872, 3838, 3805, 3772, 3740, 3709, 3678, 3647, 3617, 3588, 3559, 3530, 3502, 3474, 3446, 3419, 3393, 3367, 3341, 3316, 3290, 3266, 3241, 3218, 3194, 
		15708, 15475, 15243, 15011, 14780, 14550, 14322, 14094, 13869, 13645, 13423, 13204, 12987, 12772, 12560, 12352, 12146, 11943, 11744, 11547, 11354, 11165, 10979, 10796, 10617, 10442, 10270, 10101, 9936, 9774, 9616, 9462, 9310, 9162, 9018, 8876, 8738, 8603, 8470, 8341, 8215, 8092, 7972, 7854, 7739, 7627, 7517, 7410, 7305, 7203, 7103, 7005, 6909, 6816, 6725, 6636, 6548, 6463, 6380, 6298, 6218, 6140, 6064, 5989, 5916, 5844, 5774, 5706, 5639, 5573, 5509, 5446, 5384, 5323, 5264, 5206, 5149, 5093, 5038, 4985, 4932, 4880, 4830, 4780, 4731, 4683, 4636, 4590, 4545, 4501, 4457, 4414, 4372, 4331, 4290, 4250, 4211, 4173, 4135, 4098, 4061, 4025, 3990, 3955, 3921, 3887, 3854, 3821, 3789, 3758, 3726, 3696, 3666, 3636, 3607, 3578, 3550, 3522, 3495, 3467, 3441, 3415, 3389, 3363, 3338, 3313, 3289, 3265, 
		15708, 15481, 15254, 15027, 14801, 14576, 14353, 14130, 13909, 13690, 13473, 13258, 13045, 12835, 12627, 12422, 12220, 12021, 11825, 11632, 11442, 11255, 11071, 10891, 10714, 10541, 10371, 10204, 10041, 9881, 9724, 9570, 9420, 9273, 9129, 8988, 8851, 8716, 8584, 8456, 8330, 8207, 8086, 7969, 7854, 7742, 7632, 7524, 7419, 7317, 7217, 7118, 7023, 6929, 6837, 6747, 6660, 6574, 6490, 6408, 6327, 6249, 6172, 6097, 6023, 5951, 5880, 5811, 5743, 5677, 5612, 5548, 5485, 5424, 5364, 5306, 5248, 5191, 5136, 5082, 5028, 4976, 4925, 4875, 4825, 4777, 4729, 4682, 4636, 4591, 4547, 4504, 4461, 4419, 4378, 4337, 4298, 4259, 4220, 4182, 4145, 4109, 4073, 4037, 4002, 3968, 3935, 3901, 3869, 3837, 3805, 3774, 3743, 3713, 3683, 3654, 3625, 3597, 3569, 3542, 3514, 3488, 3461, 3435, 3410, 3385, 3360, 3335, 
		15708, 15486, 15264, 15042, 14821, 14601, 14382, 14165, 13949, 13734, 13521, 13311, 13102, 12896, 12692, 12490, 12292, 12096, 11903, 11713, 11526, 11342, 11161, 10983, 10808, 10637, 10469, 10304, 10142, 9983, 9828, 9676, 9527, 9380, 9237, 9098, 8961, 8827, 8695, 8567, 8442, 8319, 8199, 8081, 7966, 7854, 7744, 7637, 7532, 7429, 7328, 7230, 7134, 7039, 6947, 6857, 6769, 6683, 6598, 6516, 6435, 6356, 6278, 6202, 6128, 6055, 5984, 5914, 5846, 5779, 5713, 5649, 5586, 5524, 5464, 5404, 5346, 5289, 5233, 5178, 5124, 5071, 5019, 4968, 4918, 4869, 4821, 4773, 4727, 4681, 4636, 4592, 4549, 4507, 4465, 4424, 4383, 4344, 4305, 4266, 4229, 4191, 4155, 4119, 4084, 4049, 4015, 3981, 3948, 3915, 3883, 3852, 3820, 3790, 3760, 3730, 3701, 3672, 3643, 3615, 3588, 3561, 3534, 3507, 3481, 3456, 3430, 3405, 
		15708, 15491, 15273, 15057, 14841, 14625, 14411, 14198, 13986, 13776, 13567, 13361, 13156, 12954, 12754, 12556, 12361, 12168, 11978, 11791, 11607, 11425, 11247, 11071, 10899, 10730, 10563, 10400, 10240, 10083, 9929, 9778, 9630, 9485, 9343, 9204, 9068, 8934, 8803, 8676, 8551, 8428, 8308, 8191, 8076, 7964, 7854, 7746, 7641, 7538, 7438, 7339, 7243, 7148, 7056, 6965, 6877, 6790, 6705, 6622, 6541, 6461, 6383, 6307, 6232, 6159, 6087, 6017, 5948, 5880, 5814, 5749, 5685, 5623, 5562, 5502, 5443, 5385, 5328, 5273, 5218, 5165, 5112, 5061, 5010, 4960, 4912, 4864, 4817, 4770, 4725, 4680, 4636, 4593, 4551, 4509, 4468, 4428, 4389, 4350, 4311, 4274, 4237, 4200, 4164, 4129, 4094, 4060, 4027, 3993, 3961, 3929, 3897, 3866, 3835, 3805, 3775, 3746, 3717, 3689, 3661, 3633, 3606, 3579, 3552, 3526, 3500, 3475, 
		15708, 15495, 15283, 15071, 14859, 14648, 14438, 14229, 14022, 13816, 13612, 13409, 13208, 13009, 12813, 12619, 12427, 12237, 12050, 11866, 11685, 11506, 11330, 11157, 10987, 10819, 10655, 10494, 10335, 10180, 10027, 9877, 9730, 9586, 9445, 9307, 9172, 9039, 8909, 8782, 8657, 8535, 8415, 8298, 8184, 8071, 7962, 7854, 7749, 7646, 7545, 7446, 7349, 7255, 7162, 7071, 6982, 6895, 6810, 6727, 6645, 6565, 6486, 6410, 6334, 6260, 6188, 6117, 6048, 5980, 5913, 5847, 5783, 5720, 5659, 5598, 5539, 5480, 5423, 5367, 5312, 5258, 5205, 5152, 5101, 5051, 5002, 4953, 4905, 4859, 4813, 4767, 4723, 4679, 4636, 4594, 4553, 4512, 4472, 4432, 4394, 4355, 4318, 4281, 4245, 4209, 4173, 4139, 4105, 4071, 4038, 4005, 3973, 3942, 3910, 3880, 3850, 3820, 3790, 3762, 3733, 3705, 3677, 3650, 3623, 3596, 3570, 3544, 
		15708, 15500, 15292, 15084, 14877, 14670, 14464, 14260, 14056, 13854, 13654, 13455, 13258, 13063, 12870, 12679, 12490, 12304, 12120, 11939, 11760, 11584, 11410, 11240, 11071, 10906, 10744, 10584, 10427, 10273, 10122, 9974, 9828, 9685, 9545, 9408, 9273, 9141, 9012, 8885, 8761, 8639, 8520, 8403, 8288, 8176, 8067, 7959, 7854, 7751, 7650, 7551, 7454, 7359, 7266, 7175, 7086, 6999, 6913, 6830, 6747, 6667, 6588, 6511, 6435, 6361, 6288, 6217, 6147, 6078, 6011, 5945, 5880, 5817, 5754, 5693, 5633, 5574, 5517, 5460, 5404, 5350, 5296, 5243, 5191, 5141, 5091, 5042, 4993, 4946, 4900, 4854, 4809, 4765, 4721, 4678, 4636, 4595, 4555, 4515, 4475, 4437, 4398, 4361, 4324, 4288, 4252, 4217, 4182, 4148, 4115, 4081, 4049, 4017, 3985, 3954, 3923, 3893, 3863, 3834, 3805, 3777, 3748, 3721, 3693, 3666, 3640, 3614, 
		15708, 15504, 15300, 15096, 14893, 14691, 14490, 14289, 14090, 13891, 13695, 13500, 13306, 13115, 12925, 12737, 12552, 12369, 12188, 12009, 11833, 11659, 11488, 11319, 11153, 10990, 10830, 10672, 10517, 10364, 10214, 10067, 9923, 9781, 9642, 9505, 9372, 9240, 9112, 8985, 8862, 8741, 8622, 8505, 8391, 8279, 8170, 8062, 7957, 7854, 7753, 7654, 7557, 7462, 7369, 7278, 7188, 7101, 7015, 6931, 6848, 6767, 6688, 6610, 6534, 6460, 6386, 6315, 6244, 6175, 6107, 6041, 5976, 5912, 5849, 5787, 5727, 5667, 5609, 5552, 5496, 5440, 5386, 5333, 5281, 5229, 5179, 5129, 5081, 5033, 4986, 4939, 4894, 4849, 4805, 4762, 4719, 4678, 4636, 4596, 4556, 4517, 4478, 4440, 4403, 4366, 4330, 4294, 4259, 4225, 4191, 4157, 4124, 4092, 4059, 4028, 3997, 3966, 3936, 3906, 3877, 3848, 3819, 3791, 3763, 3736, 3709, 3682, 
		15708, 15508, 15308, 15109, 14910, 14711, 14514, 14317, 14121, 13927, 13734, 13542, 13353, 13164, 12978, 12793, 12611, 12431, 12252, 12076, 11903, 11732, 11563, 11397, 11233, 11071, 10913, 10757, 10603, 10452, 10304, 10158, 10015, 9874, 9736, 9601, 9468, 9337, 9209, 9084, 8961, 8840, 8721, 8605, 8491, 8380, 8270, 8163, 8058, 7955, 7854, 7755, 7658, 7563, 7470, 7378, 7289, 7201, 7115, 7030, 6947, 6866, 6787, 6709, 6632, 6557, 6483, 6411, 6340, 6271, 6202, 6136, 6070, 6005, 5942, 5880, 5819, 5759, 5700, 5643, 5586, 5530, 5476, 5422, 5369, 5317, 5266, 5216, 5167, 5119, 5071, 5024, 4978, 4933, 4889, 4845, 4802, 4759, 4718, 4677, 4636, 4597, 4558, 4519, 4481, 4444, 4408, 4371, 4336, 4301, 4266, 4232, 4199, 4166, 4133, 4101, 4070, 4039, 4008, 3978, 3948, 3919, 3890, 3861, 3833, 3805, 3778, 3751, 
		15708, 15512, 15316, 15120, 14925, 14731, 14537, 14344, 14152, 13961, 13772, 13584, 13397, 13212, 13029, 12847, 12668, 12490, 12315, 12142, 11971, 11802, 11635, 11471, 11310, 11150, 10993, 10839, 10687, 10538, 10391, 10246, 10104, 9965, 9828, 9693, 9561, 9432, 9304, 9179, 9057, 8937, 8819, 8703, 8589, 8478, 8369, 8262, 8157, 8054, 7953, 7854, 7757, 7662, 7568, 7477, 7387, 7299, 7213, 7128, 7045, 6963, 6884, 6805, 6728, 6653, 6579, 6506, 6435, 6365, 6296, 6229, 6163, 6098, 6034, 5972, 5910, 5850, 5791, 5733, 5675, 5619, 5564, 5510, 5457, 5404, 5353, 5302, 5252, 5204, 5155, 5108, 5062, 5016, 4971, 4927, 4883, 4841, 4798, 4757, 4716, 4676, 4636, 4598, 4559, 4522, 4484, 4448, 4412, 4376, 4341, 4307, 4273, 4240, 4207, 4174, 4142, 4111, 4080, 4049, 4019, 3989, 3960, 3931, 3902, 3874, 3846, 3819, 
		15708, 15516, 15324, 15132, 14940, 14749, 14559, 14370, 14181, 13994, 13808, 13623, 13440, 13258, 13078, 12900, 12723, 12548, 12376, 12205, 12036, 11870, 11706, 11544, 11384, 11227, 11071, 10919, 10769, 10621, 10475, 10332, 10191, 10053, 9917, 9784, 9653, 9524, 9397, 9273, 9151, 9031, 8914, 8799, 8685, 8574, 8465, 8359, 8254, 8151, 8050, 7951, 7854, 7759, 7665, 7574, 7484, 7396, 7309, 7224, 7141, 7059, 6979, 6900, 6823, 6747, 6673, 6600, 6528, 6458, 6389, 6321, 6255, 6190, 6125, 6062, 6001, 5940, 5880, 5821, 5764, 5707, 5651, 5597, 5543, 5490, 5438, 5387, 5337, 5288, 5239, 5191, 5145, 5098, 5053, 5008, 4964, 4921, 4878, 4836, 4795, 4755, 4715, 4675, 4636, 4598, 4561, 4524, 4487, 4451, 4416, 4381, 4347, 4313, 4279, 4247, 4214, 4182, 4151, 4120, 4089, 4059, 4029, 4000, 3971, 3942, 3914, 3886, 
		15708, 15519, 15331, 15143, 14955, 14767, 14581, 14395, 14210, 14026, 13843, 13662, 13481, 13303, 13125, 12950, 12776, 12604, 12434, 12266, 12100, 11935, 11773, 11614, 11456, 11300, 11147, 10996, 10848, 10701, 10557, 10415, 10276, 10139, 10004, 9872, 9741, 9613, 9488, 9364, 9243, 9124, 9007, 8892, 8779, 8669, 8560, 8453, 8349, 8246, 8145, 8046, 7949, 7854, 7761, 7669, 7579, 7491, 7404, 7319, 7235, 7153, 7073, 6994, 6917, 6841, 6766, 6693, 6621, 6550, 6481, 6412, 6346, 6280, 6215, 6152, 6090, 6028, 5968, 5909, 5851, 5794, 5738, 5683, 5629, 5575, 5523, 5472, 5421, 5371, 5322, 5274, 5227, 5180, 5134, 5089, 5044, 5001, 4958, 4915, 4874, 4833, 4792, 4752, 4713, 4674, 4636, 4599, 4562, 4526, 4490, 4455, 4420, 4386, 4352, 4319, 4286, 4253, 4221, 4190, 4159, 4128, 4098, 4069, 4039, 4010, 3982, 3954, 
		15708, 15523, 15338, 15153, 14969, 14785, 14601, 14419, 14237, 14056, 13877, 13698, 13521, 13346, 13171, 12998, 12827, 12658, 12490, 12325, 12161, 11999, 11839, 11681, 11526, 11372, 11221, 11071, 10924, 10780, 10637, 10497, 10358, 10222, 10089, 9957, 9828, 9701, 9576, 9453, 9332, 9214, 9098, 8983, 8871, 8761, 8652, 8546, 8442, 8339, 8238, 8140, 8043, 7947, 7854, 7762, 7672, 7584, 7497, 7412, 7328, 7246, 7165, 7086, 7009, 6932, 6857, 6784, 6711, 6640, 6571, 6502, 6435, 6369, 6304, 6240, 6178, 6116, 6055, 5996, 5937, 5880, 5824, 5768, 5713, 5660, 5607, 5555, 5504, 5454, 5404, 5356, 5308, 5261, 5214, 5169, 5124, 5080, 5036, 4993, 4951, 4910, 4869, 4829, 4789, 4750, 4712, 4674, 4636, 4600, 4563, 4528, 4493, 4458, 4424, 4390, 4357, 4324, 4292, 4260, 4229, 4198, 4167, 4137, 4107, 4078, 4049, 4020, 
		15708, 15526, 15344, 15163, 14982, 14801, 14621, 14442, 14264, 14086, 13909, 13734, 13560, 13387, 13215, 13045, 12877, 12710, 12545, 12382, 12220, 12061, 11903, 11747, 11593, 11442, 11292, 11144, 10999, 10856, 10714, 10575, 10439, 10304, 10171, 10041, 9912, 9786, 9662, 9540, 9420, 9302, 9186, 9072, 8961, 8851, 8743, 8637, 8533, 8430, 8330, 8231, 8134, 8039, 7946, 7854, 7764, 7675, 7589, 7503, 7419, 7337, 7256, 7177, 7099, 7023, 6947, 6874, 6801, 6730, 6660, 6591, 6523, 6457, 6392, 6327, 6264, 6202, 6142, 6082, 6023, 5965, 5908, 5852, 5797, 5743, 5690, 5637, 5586, 5535, 5485, 5436, 5388, 5341, 5294, 5248, 5203, 5158, 5114, 5071, 5028, 4987, 4945, 4905, 4865, 4825, 4786, 4748, 4710, 4673, 4636, 4600, 4565, 4530, 4495, 4461, 4427, 4394, 4362, 4329, 4298, 4266, 4235, 4205, 4175, 4145, 4116, 4087, 
		15708, 15529, 15351, 15173, 14995, 14817, 14641, 14464, 14289, 14114, 13941, 13768, 13597, 13427, 13258, 13091, 12925, 12761, 12598, 12437, 12278, 12120, 11965, 11811, 11659, 11509, 11361, 11215, 11071, 10930, 10790, 10652, 10517, 10383, 10251, 10122, 9995, 9869, 9746, 9625, 9505, 9388, 9273, 9160, 9048, 8939, 8831, 8726, 8622, 8520, 8419, 8321, 8224, 8129, 8036, 7944, 7854, 7765, 7679, 7593, 7509, 7427, 7346, 7266, 7188, 7112, 7036, 6962, 6889, 6818, 6747, 6678, 6610, 6544, 6478, 6414, 6350, 6288, 6227, 6166, 6107, 6049, 5992, 5935, 5880, 5826, 5772, 5719, 5667, 5616, 5566, 5517, 5468, 5420, 5373, 5326, 5281, 5236, 5191, 5148, 5105, 5063, 5021, 4980, 4939, 4900, 4860, 4822, 4784, 4746, 4709, 4672, 4636, 4601, 4566, 4532, 4498, 4464, 4431, 4398, 4366, 4335, 4303, 4272, 4242, 4212, 4182, 4153, 
		15708, 15533, 15357, 15182, 15007, 14833, 14659, 14486, 14314, 14142, 13971, 13802, 13633, 13466, 13299, 13135, 12971, 12809, 12649, 12490, 12333, 12178, 12024, 11873, 11723, 11575, 11428, 11284, 11142, 11002, 10863, 10727, 10592, 10460, 10330, 10201, 10075, 9950, 9828, 9707, 9589, 9472, 9358, 9245, 9134, 9025, 8918, 8813, 8709, 8607, 8507, 8409, 8312, 8217, 8124, 8033, 7942, 7854, 7767, 7682, 7598, 7515, 7434, 7354, 7276, 7199, 7124, 7049, 6976, 6904, 6834, 6765, 6696, 6629, 6563, 6499, 6435, 6372, 6311, 6250, 6191, 6132, 6074, 6018, 5962, 5907, 5853, 5800, 5748, 5696, 5646, 5596, 5547, 5499, 5451, 5404, 5358, 5313, 5268, 5224, 5181, 5138, 5096, 5054, 5014, 4973, 4934, 4895, 4856, 4818, 4781, 4744, 4708, 4672, 4636, 4602, 4567, 4533, 4500, 4467, 4434, 4402, 4371, 4340, 4309, 4278, 4248, 4219, 
		15708, 15536, 15363, 15191, 15019, 14848, 14677, 14507, 14337, 14169, 14001, 13834, 13668, 13503, 13339, 13177, 13016, 12857, 12699, 12542, 12387, 12234, 12083, 11933, 11785, 11638, 11494, 11351, 11210, 11071, 10935, 10799, 10666, 10535, 10406, 10278, 10153, 10030, 9908, 9788, 9670, 9555, 9441, 9328, 9218, 9109, 9003, 8898, 8795, 8693, 8593, 8495, 8399, 8304, 8211, 8119, 8029, 7941, 7854, 7769, 7685, 7602, 7521, 7441, 7363, 7285, 7210, 7135, 7062, 6990, 6919, 6850, 6781, 6714, 6648, 6583, 6519, 6456, 6394, 6333, 6273, 6214, 6156, 6099, 6043, 5988, 5934, 5880, 5827, 5776, 5725, 5674, 5625, 5576, 5528, 5481, 5435, 5389, 5344, 5300, 5256, 5213, 5170, 5129, 5087, 5047, 5007, 4967, 4928, 4890, 4852, 4815, 4778, 4742, 4706, 4671, 4636, 4602, 4568, 4535, 4502, 4470, 4438, 4406, 4375, 4344, 4314, 4284, 
		15708, 15538, 15369, 15200, 15031, 14863, 14694, 14527, 14360, 14194, 14029, 13865, 13701, 13539, 13378, 13218, 13060, 12903, 12747, 12592, 12440, 12288, 12139, 11991, 11845, 11700, 11557, 11416, 11277, 11140, 11004, 10870, 10738, 10608, 10480, 10354, 10229, 10107, 9986, 9867, 9750, 9635, 9522, 9410, 9300, 9192, 9086, 8981, 8878, 8777, 8678, 8580, 8484, 8389, 8296, 8205, 8115, 8026, 7939, 7854, 7770, 7687, 7606, 7526, 7448, 7370, 7295, 7220, 7147, 7074, 7003, 6934, 6865, 6797, 6731, 6666, 6601, 6538, 6476, 6415, 6355, 6295, 6237, 6180, 6123, 6068, 6013, 5959, 5906, 5854, 5803, 5752, 5702, 5653, 5605, 5558, 5511, 5465, 5419, 5374, 5330, 5287, 5244, 5202, 5160, 5119, 5079, 5039, 5000, 4961, 4923, 4886, 4848, 4812, 4776, 4740, 4705, 4671, 4636, 4603, 4570, 4537, 4504, 4473, 4441, 4410, 4379, 4349, 
		15708, 15541, 15375, 15208, 15042, 14877, 14711, 14547, 14382, 14219, 14056, 13895, 13734, 13574, 13416, 13258, 13102, 12947, 12793, 12641, 12490, 12341, 12194, 12047, 11903, 11760, 11619, 11479, 11342, 11206, 11071, 10939, 10808, 10680, 10552, 10427, 10304, 10182, 10062, 9944, 9828, 9713, 9601, 9490, 9380, 9273, 9167, 9063, 8961, 8860, 8761, 8663, 8567, 8473, 8380, 8288, 8199, 8110, 8023, 7938, 7854, 7771, 7690, 7610, 7532, 7454, 7378, 7303, 7230, 7157, 7086, 7016, 6947, 6880, 6813, 6747, 6683, 6619, 6557, 6495, 6435, 6375, 6317, 6259, 6202, 6147, 6092, 6037, 5984, 5932, 5880, 5829, 5779, 5730, 5681, 5633, 5586, 5540, 5494, 5449, 5404, 5360, 5317, 5275, 5233, 5191, 5151, 5111, 5071, 5032, 4993, 4956, 4918, 4881, 4845, 4809, 4773, 4739, 4704, 4670, 4636, 4603, 4571, 4538, 4507, 4475, 4444, 4414, 
		15708, 15544, 15380, 15217, 15053, 14890, 14728, 14565, 14404, 14243, 14083, 13924, 13766, 13608, 13452, 13297, 13143, 12990, 12839, 12688, 12540, 12392, 12247, 12102, 11960, 11818, 11679, 11541, 11405, 11270, 11137, 11006, 10877, 10749, 10623, 10499, 10376, 10256, 10137, 10019, 9904, 9790, 9678, 9568, 9459, 9352, 9247, 9143, 9041, 8941, 8842, 8744, 8649, 8555, 8462, 8371, 8281, 8193, 8106, 8021, 7937, 7854, 7773, 7693, 7614, 7537, 7460, 7386, 7312, 7239, 7168, 7098, 7029, 6961, 6894, 6828, 6763, 6700, 6637, 6575, 6515, 6455, 6396, 6338, 6281, 6225, 6169, 6115, 6061, 6009, 5957, 5905, 5855, 5805, 5756, 5708, 5661, 5614, 5568, 5522, 5477, 5433, 5390, 5347, 5305, 5263, 5222, 5181, 5141, 5102, 5063, 5025, 4987, 4950, 4913, 4877, 4841, 4806, 4771, 4737, 4703, 4669, 4636, 4604, 4572, 4540, 4509, 4478, 
		15708, 15547, 15385, 15224, 15064, 14903, 14743, 14584, 14425, 14266, 14109, 13952, 13796, 13641, 13487, 13334, 13182, 13032, 12882, 12734, 12588, 12442, 12298, 12156, 12015, 11875, 11737, 11601, 11466, 11333, 11201, 11071, 10943, 10817, 10692, 10569, 10447, 10328, 10209, 10093, 9978, 9865, 9754, 9644, 9536, 9430, 9325, 9222, 9120, 9020, 8921, 8824, 8729, 8635, 8543, 8452, 8362, 8274, 8187, 8102, 8018, 7935, 7854, 7774, 7695, 7618, 7542, 7467, 7393, 7320, 7249, 7178, 7109, 7041, 6974, 6908, 6843, 6779, 6716, 6654, 6593, 6533, 6474, 6416, 6358, 6302, 6246, 6192, 6138, 6085, 6032, 5981, 5930, 5880, 5831, 5782, 5734, 5687, 5641, 5595, 5550, 5505, 5462, 5418, 5376, 5334, 5292, 5252, 5211, 5172, 5133, 5094, 5056, 5018, 4981, 4945, 4909, 4873, 4838, 4803, 4769, 4735, 4702, 4669, 4636, 4604, 4573, 4542, 
		15708, 15549, 15391, 15232, 15074, 14916, 14758, 14601, 14445, 14289, 14134, 13979, 13826, 13673, 13521, 13371, 13221, 13072, 12925, 12779, 12634, 12490, 12348, 12208, 12068, 11930, 11794, 11659, 11526, 11394, 11264, 11135, 11008, 10883, 10759, 10637, 10517, 10398, 10280, 10165, 10051, 9939, 9828, 9719, 9611, 9505, 9401, 9298, 9197, 9098, 8999, 8903, 8808, 8714, 8622, 8531, 8442, 8354, 8267, 8182, 8098, 8015, 7934, 7854, 7775, 7698, 7621, 7546, 7472, 7400, 7328, 7258, 7188, 7120, 7053, 6987, 6921, 6857, 6794, 6732, 6671, 6610, 6551, 6493, 6435, 6378, 6322, 6267, 6213, 6160, 6107, 6055, 6004, 5954, 5905, 5856, 5808, 5760, 5713, 5667, 5622, 5577, 5533, 5489, 5447, 5404, 5362, 5321, 5281, 5241, 5201, 5162, 5124, 5086, 5049, 5012, 4975, 4939, 4904, 4869, 4834, 4800, 4767, 4734, 4701, 4668, 4636, 4605, 
		15708, 15552, 15396, 15240, 15084, 14928, 14773, 14619, 14464, 14311, 14158, 14006, 13854, 13704, 13554, 13406, 13258, 13112, 12966, 12822, 12679, 12537, 12397, 12258, 12120, 11984, 11849, 11716, 11584, 11453, 11325, 11197, 11071, 10947, 10825, 10704, 10584, 10466, 10350, 10235, 10122, 10010, 9900, 9792, 9685, 9580, 9476, 9374, 9273, 9174, 9076, 8980, 8885, 8791, 8699, 8609, 8520, 8432, 8345, 8260, 8176, 8094, 8013, 7933, 7854, 7776, 7700, 7625, 7551, 7478, 7407, 7336, 7266, 7198, 7131, 7064, 6999, 6935, 6871, 6809, 6747, 6687, 6627, 6569, 6511, 6454, 6398, 6342, 6288, 6234, 6181, 6129, 6078, 6027, 5978, 5928, 5880, 5832, 5785, 5739, 5693, 5648, 5604, 5560, 5517, 5474, 5432, 5390, 5350, 5309, 5269, 5230, 5191, 5153, 5116, 5078, 5042, 5005, 4970, 4934, 4900, 4865, 4831, 4798, 4765, 4732, 4700, 4668, 
		15708, 15554, 15400, 15247, 15093, 14940, 14787, 14635, 14483, 14332, 14181, 14032, 13882, 13734, 13587, 13440, 13294, 13150, 13006, 12864, 12723, 12583, 12444, 12307, 12171, 12036, 11903, 11771, 11640, 11511, 11384, 11258, 11133, 11010, 10889, 10769, 10650, 10533, 10418, 10304, 10191, 10081, 9971, 9864, 9757, 9653, 9549, 9447, 9347, 9248, 9151, 9055, 8961, 8867, 8776, 8685, 8596, 8509, 8422, 8337, 8254, 8171, 8090, 8010, 7931, 7854, 7778, 7702, 7628, 7556, 7484, 7413, 7343, 7275, 7207, 7141, 7075, 7011, 6947, 6885, 6823, 6762, 6703, 6644, 6586, 6528, 6472, 6417, 6362, 6308, 6255, 6202, 6151, 6100, 6050, 6001, 5952, 5904, 5856, 5810, 5764, 5718, 5674, 5630, 5586, 5543, 5501, 5459, 5418, 5377, 5337, 5298, 5259, 5220, 5182, 5145, 5108, 5071, 5035, 4999, 4964, 4930, 4895, 4862, 4828, 4795, 4763, 4731, 
		15708, 15556, 15405, 15254, 15103, 14952, 14801, 14651, 14502, 14353, 14204, 14056, 13909, 13763, 13618, 13473, 13330, 13187, 13045, 12905, 12766, 12627, 12490, 12355, 12220, 12087, 11955, 11825, 11696, 11568, 11442, 11317, 11193, 11071, 10951, 10832, 10714, 10598, 10484, 10371, 10259, 10149, 10041, 9934, 9828, 9724, 9621, 9520, 9420, 9322, 9225, 9129, 9035, 8942, 8851, 8761, 8672, 8584, 8498, 8413, 8330, 8247, 8166, 8086, 8008, 7930, 7854, 7779, 7705, 7632, 7560, 7489, 7419, 7351, 7283, 7217, 7151, 7086, 7023, 6960, 6898, 6837, 6777, 6718, 6660, 6602, 6546, 6490, 6435, 6381, 6327, 6275, 6223, 6172, 6122, 6072, 6023, 5975, 5927, 5880, 5834, 5788, 5743, 5699, 5655, 5612, 5569, 5527, 5485, 5445, 5404, 5364, 5325, 5286, 5248, 5210, 5173, 5136, 5100, 5064, 5028, 4993, 4959, 4925, 4891, 4858, 4825, 4793, 
		15708, 15559, 15410, 15261, 15112, 14963, 14815, 14667, 14520, 14373, 14226, 14081, 13936, 13791, 13648, 13505, 13364, 13223, 13083, 12945, 12807, 12671, 12535, 12401, 12268, 12137, 12006, 11877, 11749, 11623, 11498, 11374, 11252, 11131, 11012, 10894, 10777, 10662, 10549, 10437, 10326, 10216, 10109, 10002, 9897, 9794, 9691, 9591, 9491, 9393, 9297, 9202, 9108, 9015, 8924, 8834, 8746, 8659, 8573, 8488, 8405, 8322, 8241, 8162, 8083, 8005, 7929, 7854, 7780, 7707, 7635, 7564, 7494, 7426, 7358, 7291, 7225, 7161, 7097, 7034, 6972, 6911, 6851, 6791, 6733, 6675, 6618, 6563, 6507, 6453, 6399, 6347, 6294, 6243, 6192, 6142, 6093, 6045, 5997, 5950, 5903, 5857, 5812, 5767, 5723, 5680, 5637, 5594, 5553, 5511, 5471, 5431, 5391, 5352, 5313, 5275, 5238, 5201, 5164, 5128, 5092, 5057, 5022, 4988, 4954, 4920, 4887, 4855, 
		15708, 15561, 15414, 15267, 15120, 14974, 14828, 14682, 14537, 14392, 14248, 14104, 13961, 13819, 13678, 13537, 13397, 13258, 13120, 12983, 12847, 12713, 12579, 12446, 12315, 12185, 12056, 11928, 11802, 11677, 11553, 11431, 11310, 11190, 11071, 10955, 10839, 10725, 10612, 10501, 10391, 10282, 10175, 10069, 9965, 9862, 9760, 9660, 9561, 9464, 9368, 9273, 9179, 9087, 8997, 8907, 8819, 8732, 8646, 8561, 8478, 8396, 8315, 8235, 8157, 8080, 8003, 7928, 7854, 7781, 7709, 7638, 7568, 7500, 7432, 7365, 7299, 7234, 7170, 7107, 7045, 6984, 6923, 6864, 6805, 6747, 6690, 6634, 6579, 6524, 6470, 6417, 6365, 6313, 6263, 6212, 6163, 6114, 6066, 6019, 5972, 5926, 5880, 5835, 5791, 5747, 5704, 5661, 5619, 5578, 5537, 5496, 5457, 5417, 5378, 5340, 5302, 5265, 5228, 5191, 5155, 5120, 5085, 5050, 5016, 4982, 4949, 4916, 
		15708, 15563, 15418, 15273, 15129, 14985, 14841, 14697, 14554, 14411, 14269, 14127, 13986, 13846, 13706, 13567, 13429, 13292, 13156, 13021, 12887, 12754, 12621, 12490, 12361, 12232, 12104, 11978, 11853, 11729, 11607, 11485, 11366, 11247, 11130, 11014, 10899, 10786, 10674, 10563, 10454, 10347, 10240, 10135, 10031, 9929, 9828, 9728, 9630, 9533, 9437, 9343, 9250, 9158, 9068, 8978, 8890, 8803, 8718, 8634, 8551, 8469, 8388, 8308, 8230, 8152, 8076, 8001, 7927, 7854, 7782, 7711, 7641, 7572, 7504, 7438, 7372, 7307, 7243, 7179, 7117, 7056, 6995, 6936, 6877, 6819, 6762, 6705, 6650, 6595, 6541, 6488, 6435, 6383, 6332, 6282, 6232, 6183, 6135, 6087, 6040, 5993, 5948, 5902, 5858, 5814, 5770, 5728, 5685, 5644, 5602, 5562, 5522, 5482, 5443, 5404, 5366, 5328, 5291, 5255, 5218, 5183, 5147, 5112, 5078, 5044, 5010, 4977, 
		15708, 15565, 15422, 15280, 15137, 14995, 14853, 14711, 14570, 14429, 14289, 14149, 14010, 13872, 13734, 13597, 13461, 13326, 13191, 13058, 12925, 12793, 12663, 12533, 12405, 12278, 12152, 12027, 11903, 11780, 11659, 11539, 11420, 11303, 11186, 11071, 10958, 10846, 10735, 10625, 10517, 10409, 10304, 10199, 10096, 9995, 9894, 9795, 9697, 9601, 9505, 9412, 9319, 9227, 9137, 9048, 8961, 8874, 8789, 8705, 8622, 8540, 8459, 8380, 8301, 8224, 8148, 8073, 7999, 7926, 7854, 7783, 7713, 7644, 7576, 7509, 7443, 7378, 7314, 7251, 7188, 7127, 7066, 7006, 6947, 6889, 6832, 6775, 6720, 6665, 6610, 6557, 6504, 6452, 6401, 6350, 6300, 6251, 6202, 6155, 6107, 6061, 6015, 5969, 5924, 5880, 5836, 5793, 5751, 5709, 5667, 5626, 5586, 5546, 5507, 5468, 5430, 5392, 5354, 5317, 5281, 5245, 5209, 5174, 5139, 5105, 5071, 5037, 
		15708, 15567, 15426, 15286, 15145, 15005, 14865, 14725, 14586, 14447, 14309, 14171, 14034, 13897, 13761, 13626, 13491, 13358, 13225, 13093, 12962, 12832, 12703, 12575, 12448, 12322, 12198, 12074, 11952, 11830, 11710, 11591, 11474, 11357, 11242, 11128, 11015, 10904, 10794, 10685, 10577, 10471, 10366, 10262, 10160, 10059, 9959, 9861, 9763, 9667, 9572, 9479, 9387, 9296, 9206, 9117, 9030, 8943, 8858, 8774, 8692, 8610, 8530, 8450, 8372, 8295, 8219, 8144, 8070, 7997, 7925, 7854, 7784, 7715, 7647, 7580, 7514, 7449, 7385, 7321, 7259, 7197, 7136, 7076, 7017, 6959, 6901, 6845, 6789, 6734, 6679, 6626, 6573, 6520, 6469, 6418, 6368, 6319, 6270, 6222, 6174, 6127, 6081, 6035, 5990, 5946, 5902, 5858, 5816, 5773, 5732, 5691, 5650, 5610, 5570, 5531, 5492, 5454, 5417, 5379, 5343, 5306, 5271, 5235, 5200, 5166, 5131, 5098, 
		15708, 15569, 15430, 15292, 15153, 15015, 14877, 14739, 14601, 14464, 14328, 14192, 14056, 13922, 13787, 13654, 13521, 13389, 13258, 13128, 12998, 12870, 12743, 12616, 12490, 12366, 12243, 12120, 11999, 11879, 11760, 11642, 11526, 11410, 11296, 11183, 11071, 10961, 10852, 10744, 10637, 10531, 10427, 10324, 10222, 10122, 10023, 9925, 9828, 9732, 9638, 9545, 9453, 9362, 9273, 9185, 9098, 9012, 8927, 8843, 8761, 8679, 8599, 8520, 8442, 8364, 8288, 8214, 8140, 8067, 7995, 7924, 7854, 7785, 7717, 7650, 7584, 7519, 7454, 7391, 7328, 7266, 7206, 7145, 7086, 7028, 6970, 6913, 6857, 6802, 6747, 6694, 6640, 6588, 6536, 6485, 6435, 6385, 6336, 6288, 6240, 6193, 6147, 6101, 6055, 6011, 5967, 5923, 5880, 5838, 5796, 5754, 5713, 5673, 5633, 5594, 5555, 5517, 5479, 5441, 5404, 5368, 5332, 5296, 5261, 5226, 5191, 5157, 
		15708, 15571, 15434, 15297, 15161, 15024, 14888, 14752, 14616, 14481, 14347, 14212, 14079, 13946, 13813, 13681, 13550, 13420, 13290, 13162, 13034, 12907, 12781, 12656, 12532, 12408, 12286, 12165, 12045, 11927, 11809, 11692, 11577, 11462, 11349, 11237, 11126, 11017, 10908, 10801, 10695, 10591, 10487, 10385, 10284, 10184, 10085, 9988, 9891, 9796, 9703, 9610, 9518, 9428, 9339, 9251, 9164, 9079, 8994, 8911, 8828, 8747, 8667, 8588, 8510, 8433, 8357, 8282, 8208, 8136, 8064, 7993, 7923, 7854, 7786, 7719, 7653, 7587, 7523, 7459, 7397, 7335, 7274, 7214, 7154, 7096, 7038, 6981, 6925, 6870, 6815, 6761, 6708, 6655, 6603, 6552, 6501, 6451, 6402, 6354, 6306, 6258, 6212, 6166, 6120, 6075, 6031, 5987, 5944, 5901, 5859, 5817, 5776, 5736, 5696, 5656, 5617, 5578, 5540, 5502, 5465, 5428, 5392, 5356, 5321, 5286, 5251, 5217, 
		15708, 15573, 15438, 15303, 15168, 15033, 14899, 14765, 14631, 14498, 14365, 14232, 14100, 13969, 13838, 13708, 13579, 13450, 13322, 13195, 13068, 12943, 12818, 12695, 12572, 12450, 12329, 12209, 12091, 11973, 11856, 11741, 11626, 11513, 11401, 11290, 11180, 11071, 10964, 10858, 10752, 10648, 10546, 10444, 10344, 10244, 10146, 10049, 9954, 9859, 9766, 9674, 9583, 9493, 9404, 9316, 9230, 9144, 9060, 8977, 8895, 8814, 8734, 8655, 8577, 8501, 8425, 8350, 8276, 8203, 8132, 8061, 7991, 7922, 7854, 7787, 7721, 7655, 7591, 7527, 7465, 7403, 7342, 7281, 7222, 7163, 7105, 7048, 6992, 6936, 6881, 6827, 6774, 6721, 6669, 6618, 6567, 6517, 6468, 6419, 6371, 6323, 6276, 6230, 6184, 6139, 6095, 6051, 6007, 5964, 5922, 5880, 5839, 5798, 5758, 5718, 5678, 5640, 5601, 5563, 5526, 5489, 5452, 5416, 5380, 5345, 5310, 5276, 
		15708, 15575, 15441, 15308, 15175, 15042, 14910, 14777, 14645, 14514, 14382, 14252, 14121, 13992, 13863, 13734, 13606, 13479, 13353, 13227, 13102, 12978, 12855, 12732, 12611, 12490, 12371, 12252, 12135, 12018, 11903, 11788, 11675, 11563, 11452, 11342, 11233, 11125, 11018, 10913, 10808, 10705, 10603, 10502, 10402, 10304, 10206, 10110, 10015, 9921, 9828, 9736, 9646, 9556, 9468, 9380, 9294, 9209, 9125, 9042, 8961, 8880, 8800, 8721, 8644, 8567, 8491, 8417, 8343, 8270, 8199, 8128, 8058, 7989, 7921, 7854, 7788, 7722, 7658, 7594, 7532, 7470, 7408, 7348, 7289, 7230, 7172, 7115, 7058, 7002, 6947, 6893, 6840, 6787, 6734, 6683, 6632, 6582, 6532, 6483, 6435, 6387, 6340, 6294, 6248, 6202, 6158, 6114, 6070, 6027, 5984, 5942, 5901, 5860, 5819, 5779, 5739, 5700, 5662, 5624, 5586, 5549, 5512, 5476, 5440, 5404, 5369, 5334, 
		15708, 15576, 15445, 15313, 15182, 15051, 14920, 14790, 14659, 14529, 14400, 14271, 14142, 14014, 13886, 13759, 13633, 13507, 13382, 13258, 13135, 13012, 12890, 12769, 12649, 12530, 12412, 12294, 12178, 12063, 11948, 11835, 11723, 11612, 11501, 11392, 11284, 11177, 11071, 10967, 10863, 10761, 10659, 10559, 10460, 10362, 10265, 10169, 10075, 9981, 9889, 9798, 9707, 9618, 9530, 9444, 9358, 9273, 9189, 9107, 9025, 8945, 8865, 8787, 8709, 8633, 8557, 8483, 8409, 8336, 8265, 8194, 8124, 8055, 7987, 7920, 7854, 7789, 7724, 7660, 7598, 7536, 7474, 7414, 7354, 7296, 7237, 7180, 7124, 7068, 7013, 6958, 6904, 6851, 6799, 6747, 6696, 6646, 6596, 6547, 6499, 6451, 6404, 6357, 6311, 6265, 6220, 6176, 6132, 6089, 6046, 6004, 5962, 5921, 5880, 5840, 5800, 5761, 5722, 5684, 5646, 5608, 5571, 5535, 5499, 5463, 5428, 5393, 
		15708, 15578, 15448, 15319, 15189, 15060, 14930, 14801, 14673, 14544, 14416, 14289, 14162, 14035, 13909, 13784, 13659, 13535, 13412, 13289, 13167, 13045, 12925, 12805, 12687, 12569, 12452, 12335, 12220, 12106, 11993, 11881, 11769, 11659, 11550, 11442, 11335, 11229, 11124, 11020, 10917, 10815, 10714, 10615, 10517, 10419, 10323, 10228, 10134, 10041, 9949, 9858, 9768, 9680, 9592, 9505, 9420, 9336, 9252, 9170, 9089, 9008, 8929, 8851, 8773, 8697, 8622, 8547, 8474, 8401, 8330, 8259, 8189, 8121, 8053, 7986, 7919, 7854, 7789, 7726, 7663, 7601, 7540, 7479, 7419, 7361, 7302, 7245, 7188, 7132, 7077, 7023, 6969, 6916, 6863, 6811, 6760, 6710, 6660, 6610, 6562, 6514, 6466, 6419, 6373, 6327, 6282, 6238, 6194, 6150, 6107, 6065, 6023, 5981, 5941, 5900, 5860, 5821, 5782, 5743, 5705, 5667, 5630, 5593, 5557, 5521, 5485, 5450, 
		15708, 15580, 15452, 15324, 15196, 15068, 14940, 14813, 14686, 14559, 14433, 14307, 14181, 14056, 13932, 13808, 13685, 13562, 13440, 13319, 13198, 13078, 12959, 12841, 12723, 12606, 12490, 12376, 12261, 12148, 12036, 11925, 11815, 11706, 11597, 11490, 11384, 11279, 11175, 11071, 10969, 10868, 10769, 10670, 10572, 10475, 10380, 10285, 10191, 10099, 10008, 9917, 9828, 9740, 9653, 9566, 9481, 9397, 9314, 9232, 9151, 9071, 8992, 8914, 8837, 8761, 8685, 8611, 8538, 8465, 8394, 8323, 8254, 8185, 8117, 8050, 7984, 7918, 7854, 7790, 7727, 7665, 7604, 7544, 7484, 7425, 7367, 7309, 7252, 7196, 7141, 7086, 7032, 6979, 6926, 6874, 6823, 6773, 6722, 6673, 6624, 6576, 6528, 6481, 6435, 6389, 6344, 6299, 6255, 6211, 6168, 6125, 6083, 6042, 6001, 5960, 5920, 5880, 5841, 5802, 5764, 5726, 5688, 5651, 5615, 5579, 5543, 5508, 
		15708, 15581, 15455, 15328, 15202, 15076, 14950, 14824, 14699, 14574, 14449, 14324, 14200, 14077, 13954, 13832, 13710, 13588, 13468, 13348, 13228, 13110, 12992, 12875, 12759, 12643, 12528, 12415, 12302, 12190, 12079, 11969, 11859, 11751, 11644, 11537, 11432, 11328, 11225, 11122, 11021, 10921, 10822, 10723, 10626, 10530, 10435, 10341, 10248, 10156, 10065, 9975, 9887, 9799, 9712, 9626, 9541, 9458, 9375, 9293, 9212, 9133, 9054, 8976, 8899, 8823, 8748, 8674, 8601, 8529, 8457, 8387, 8317, 8249, 8181, 8114, 8048, 7982, 7918, 7854, 7791, 7729, 7668, 7607, 7547, 7488, 7430, 7372, 7316, 7259, 7204, 7149, 7095, 7042, 6989, 6937, 6886, 6835, 6785, 6735, 6686, 6638, 6590, 6543, 6496, 6450, 6405, 6360, 6315, 6272, 6228, 6185, 6143, 6101, 6060, 6019, 5979, 5939, 5900, 5861, 5822, 5784, 5746, 5709, 5673, 5636, 5600, 5565, 
		15708, 15583, 15458, 15333, 15208, 15084, 14959, 14835, 14711, 14588, 14464, 14342, 14219, 14097, 13976, 13854, 13734, 13614, 13495, 13376, 13258, 13141, 13024, 12908, 12793, 12679, 12566, 12453, 12341, 12230, 12120, 12011, 11903, 11796, 11689, 11584, 11479, 11376, 11273, 11172, 11071, 10972, 10873, 10776, 10680, 10584, 10490, 10396, 10304, 10212, 10122, 10033, 9944, 9857, 9770, 9685, 9601, 9517, 9435, 9353, 9273, 9193, 9115, 9037, 8961, 8885, 8810, 8736, 8663, 8591, 8520, 8449, 8380, 8311, 8243, 8176, 8110, 8045, 7981, 7917, 7854, 7792, 7731, 7670, 7610, 7551, 7493, 7435, 7378, 7322, 7266, 7212, 7157, 7104, 7051, 6999, 6947, 6896, 6846, 6796, 6747, 6699, 6651, 6604, 6557, 6511, 6465, 6420, 6375, 6331, 6288, 6245, 6202, 6161, 6119, 6078, 6037, 5997, 5958, 5919, 5880, 5842, 5804, 5767, 5730, 5693, 5657, 5621, 
		15708, 15585, 15461, 15338, 15215, 15091, 14969, 14846, 14724, 14601, 14480, 14358, 14237, 14117, 13996, 13877, 13758, 13639, 13521, 13404, 13287, 13171, 13056, 12941, 12827, 12714, 12602, 12490, 12380, 12270, 12161, 12053, 11946, 11839, 11734, 11629, 11526, 11423, 11321, 11221, 11121, 11022, 10924, 10828, 10732, 10637, 10543, 10450, 10358, 10268, 10178, 10089, 10001, 9914, 9828, 9743, 9659, 9576, 9494, 9413, 9332, 9253, 9175, 9098, 9021, 8946, 8871, 8797, 8724, 8652, 8581, 8511, 8442, 8373, 8305, 8238, 8172, 8107, 8043, 7979, 7916, 7854, 7793, 7732, 7672, 7613, 7555, 7497, 7440, 7384, 7328, 7273, 7219, 7165, 7112, 7060, 7009, 6958, 6907, 6857, 6808, 6759, 6711, 6664, 6617, 6571, 6525, 6480, 6435, 6391, 6347, 6304, 6261, 6219, 6178, 6136, 6096, 6055, 6016, 5976, 5937, 5899, 5861, 5824, 5786, 5750, 5713, 5677, 
		15708, 15586, 15464, 15342, 15221, 15099, 14978, 14856, 14735, 14615, 14494, 14374, 14255, 14136, 14017, 13899, 13781, 13664, 13547, 13431, 13316, 13201, 13087, 12973, 12861, 12749, 12638, 12527, 12417, 12309, 12201, 12094, 11987, 11882, 11777, 11674, 11571, 11469, 11368, 11269, 11170, 11071, 10974, 10878, 10783, 10689, 10596, 10503, 10412, 10322, 10232, 10144, 10056, 9970, 9884, 9800, 9716, 9634, 9552, 9471, 9391, 9312, 9234, 9157, 9081, 9005, 8931, 8857, 8785, 8713, 8642, 8572, 8502, 8434, 8366, 8300, 8234, 8168, 8104, 8040, 7977, 7915, 7854, 7793, 7734, 7674, 7616, 7558, 7501, 7445, 7389, 7334, 7280, 7226, 7173, 7121, 7069, 7018, 6967, 6918, 6868, 6819, 6771, 6724, 6677, 6630, 6584, 6539, 6494, 6450, 6406, 6363, 6320, 6277, 6236, 6194, 6153, 6113, 6073, 6034, 5995, 5956, 5918, 5880, 5843, 5806, 5769, 5733, 
		15708, 15587, 15467, 15347, 15226, 15106, 14986, 14867, 14747, 14628, 14509, 14390, 14272, 14154, 14037, 13920, 13804, 13688, 13572, 13458, 13343, 13230, 13117, 13005, 12893, 12782, 12672, 12563, 12454, 12347, 12240, 12133, 12028, 11924, 11820, 11717, 11616, 11515, 11414, 11315, 11217, 11120, 11023, 10928, 10833, 10740, 10647, 10556, 10465, 10375, 10286, 10198, 10111, 10025, 9940, 9856, 9773, 9690, 9609, 9528, 9449, 9370, 9292, 9215, 9139, 9064, 8990, 8917, 8844, 8772, 8702, 8632, 8562, 8494, 8427, 8360, 8294, 8229, 8164, 8101, 8038, 7976, 7915, 7854, 7794, 7735, 7676, 7619, 7562, 7505, 7450, 7395, 7340, 7286, 7233, 7181, 7129, 7078, 7027, 6977, 6928, 6879, 6831, 6783, 6736, 6689, 6643, 6597, 6552, 6508, 6464, 6421, 6378, 6335, 6293, 6252, 6211, 6170, 6130, 6090, 6051, 6012, 5974, 5936, 5899, 5862, 5825, 5789, 
		15708, 15589, 15470, 15351, 15232, 15113, 14995, 14877, 14758, 14641, 14523, 14406, 14289, 14173, 14056, 13941, 13826, 13711, 13597, 13483, 13371, 13258, 13146, 13035, 12925, 12815, 12706, 12598, 12490, 12384, 12278, 12173, 12068, 11965, 11862, 11760, 11659, 11559, 11460, 11361, 11264, 11167, 11071, 10977, 10883, 10790, 10698, 10607, 10517, 10427, 10339, 10251, 10165, 10079, 9995, 9911, 9828, 9746, 9665, 9585, 9505, 9427, 9350, 9273, 9197, 9122, 9048, 8975, 8903, 8831, 8761, 8691, 8622, 8553, 8486, 8419, 8354, 8288, 8224, 8161, 8098, 8036, 7974, 7914, 7854, 7795, 7736, 7679, 7621, 7565, 7509, 7454, 7400, 7346, 7293, 7240, 7188, 7137, 7086, 7036, 6987, 6938, 6889, 6841, 6794, 6747, 6701, 6656, 6610, 6566, 6522, 6478, 6435, 6392, 6350, 6309, 6267, 6227, 6186, 6147, 6107, 6068, 6030, 5992, 5954, 5917, 5880, 5844, 
		15708, 15590, 15473, 15355, 15238, 15120, 15003, 14886, 14770, 14653, 14537, 14421, 14305, 14190, 14076, 13961, 13847, 13734, 13621, 13509, 13397, 13286, 13175, 13065, 12956, 12847, 12740, 12632, 12526, 12420, 12315, 12211, 12107, 12005, 11903, 11802, 11702, 11602, 11504, 11406, 11310, 11214, 11119, 11025, 10931, 10839, 10748, 10657, 10567, 10479, 10391, 10304, 10218, 10133, 10048, 9965, 9882, 9801, 9720, 9640, 9561, 9483, 9406, 9330, 9254, 9179, 9106, 9033, 8961, 8889, 8819, 8749, 8680, 8612, 8545, 8478, 8412, 8347, 8283, 8220, 8157, 8095, 8034, 7973, 7913, 7854, 7796, 7738, 7681, 7624, 7568, 7513, 7459, 7405, 7352, 7299, 7247, 7196, 7145, 7095, 7045, 6996, 6947, 6899, 6852, 6805, 6759, 6713, 6668, 6623, 6579, 6535, 6492, 6449, 6407, 6365, 6324, 6283, 6242, 6202, 6163, 6124, 6085, 6047, 6009, 5972, 5935, 5898, 
		15708, 15592, 15475, 15359, 15243, 15127, 15011, 14896, 14780, 14665, 14550, 14436, 14322, 14208, 14094, 13981, 13869, 13756, 13645, 13534, 13423, 13313, 13204, 13095, 12987, 12879, 12772, 12666, 12560, 12456, 12352, 12248, 12146, 12044, 11943, 11843, 11744, 11645, 11547, 11450, 11354, 11259, 11165, 11071, 10979, 10887, 10796, 10706, 10617, 10529, 10442, 10355, 10270, 10185, 10101, 10018, 9936, 9855, 9774, 9695, 9616, 9539, 9462, 9386, 9310, 9236, 9162, 9089, 9018, 8946, 8876, 8807, 8738, 8670, 8603, 8536, 8470, 8406, 8341, 8278, 8215, 8153, 8092, 8031, 7972, 7912, 7854, 7796, 7739, 7683, 7627, 7572, 7517, 7463, 7410, 7357, 7305, 7254, 7203, 7152, 7103, 7054, 7005, 6957, 6909, 6862, 6816, 6770, 6725, 6680, 6636, 6592, 6548, 6505, 6463, 6421, 6380, 6339, 6298, 6258, 6218, 6179, 6140, 6102, 6064, 6026, 5989, 5952, 
		15708, 15593, 15478, 15363, 15249, 15134, 15019, 14905, 14791, 14677, 14564, 14450, 14337, 14225, 14112, 14001, 13889, 13778, 13668, 13558, 13448, 13339, 13231, 13123, 13016, 12910, 12804, 12699, 12594, 12490, 12387, 12285, 12183, 12083, 11982, 11883, 11785, 11687, 11590, 11494, 11398, 11304, 11210, 11118, 11026, 10935, 10844, 10755, 10666, 10579, 10492, 10406, 10321, 10236, 10153, 10070, 9989, 9908, 9828, 9749, 9670, 9593, 9516, 9441, 9366, 9291, 9218, 9145, 9074, 9003, 8933, 8863, 8795, 8727, 8660, 8593, 8528, 8463, 8399, 8336, 8273, 8211, 8150, 8089, 8029, 7970, 7912, 7854, 7797, 7740, 7685, 7629, 7575, 7521, 7467, 7415, 7363, 7311, 7260, 7210, 7160, 7111, 7062, 7014, 6966, 6919, 6873, 6827, 6781, 6736, 6692, 6648, 6604, 6561, 6519, 6477, 6435, 6394, 6353, 6313, 6273, 6234, 6195, 6156, 6118, 6080, 6043, 6006, 
		15708, 15594, 15481, 15367, 15254, 15140, 15027, 14914, 14801, 14689, 14576, 14464, 14353, 14241, 14130, 14020, 13909, 13800, 13690, 13582, 13473, 13365, 13258, 13152, 13045, 12940, 12835, 12731, 12627, 12525, 12422, 12321, 12220, 12120, 12021, 11923, 11825, 11728, 11632, 11536, 11442, 11348, 11255, 11163, 11071, 10981, 10891, 10802, 10714, 10627, 10541, 10456, 10371, 10287, 10204, 10122, 10041, 9960, 9881, 9802, 9724, 9647, 9570, 9495, 9420, 9346, 9273, 9201, 9129, 9058, 8988, 8919, 8851, 8783, 8716, 8650, 8584, 8520, 8456, 8392, 8330, 8268, 8207, 8146, 8086, 8027, 7969, 7911, 7854, 7797, 7742, 7686, 7632, 7578, 7524, 7472, 7419, 7368, 7317, 7266, 7217, 7167, 7118, 7070, 7023, 6975, 6929, 6883, 6837, 6792, 6747, 6703, 6660, 6617, 6574, 6532, 6490, 6449, 6408, 6367, 6327, 6288, 6249, 6210, 6172, 6134, 6097, 6060, 
		15708, 15596, 15483, 15371, 15259, 15147, 15035, 14923, 14811, 14700, 14589, 14478, 14368, 14258, 14148, 14038, 13929, 13821, 13712, 13605, 13497, 13391, 13285, 13179, 13074, 12970, 12866, 12762, 12660, 12558, 12457, 12356, 12256, 12157, 12059, 11961, 11864, 11768, 11673, 11578, 11484, 11391, 11299, 11207, 11117, 11027, 10938, 10849, 10762, 10675, 10589, 10504, 10420, 10337, 10254, 10173, 10092, 10012, 9932, 9854, 9776, 9699, 9623, 9548, 9474, 9400, 9327, 9255, 9184, 9113, 9043, 8974, 8906, 8838, 8772, 8706, 8640, 8576, 8512, 8449, 8386, 8324, 8263, 8203, 8143, 8084, 8025, 7968, 7910, 7854, 7798, 7743, 7688, 7634, 7581, 7528, 7476, 7424, 7373, 7323, 7273, 7223, 7174, 7126, 7078, 7031, 6984, 6938, 6892, 6847, 6803, 6758, 6715, 6671, 6629, 6586, 6544, 6503, 6462, 6422, 6381, 6342, 6303, 6264, 6225, 6187, 6150, 6113, 
		15708, 15597, 15486, 15375, 15264, 15153, 15042, 14932, 14821, 14711, 14601, 14492, 14382, 14273, 14165, 14056, 13949, 13841, 13734, 13627, 13521, 13416, 13311, 13206, 13102, 12998, 12896, 12793, 12692, 12591, 12490, 12391, 12292, 12194, 12096, 11999, 11903, 11807, 11713, 11619, 11526, 11433, 11342, 11251, 11161, 11071, 10983, 10895, 10808, 10722, 10637, 10552, 10469, 10386, 10304, 10222, 10142, 10062, 9983, 9905, 9828, 9751, 9676, 9601, 9527, 9453, 9380, 9309, 9237, 9167, 9098, 9029, 8961, 8893, 8827, 8761, 8695, 8631, 8567, 8504, 8442, 8380, 8319, 8258, 8199, 8140, 8081, 8023, 7966, 7910, 7854, 7799, 7744, 7690, 7637, 7584, 7532, 7480, 7429, 7378, 7328, 7279, 7230, 7181, 7134, 7086, 7039, 6993, 6947, 6902, 6857, 6813, 6769, 6726, 6683, 6640, 6598, 6557, 6516, 6475, 6435, 6395, 6356, 6317, 6278, 6240, 6202, 6165, 
		15708, 15598, 15488, 15378, 15269, 15159, 15050, 14940, 14831, 14722, 14613, 14505, 14397, 14289, 14181, 14074, 13968, 13861, 13755, 13650, 13545, 13440, 13336, 13232, 13129, 13027, 12925, 12824, 12723, 12623, 12523, 12425, 12327, 12229, 12132, 12036, 11941, 11846, 11752, 11659, 11567, 11475, 11384, 11294, 11204, 11116, 11028, 10940, 10854, 10769, 10684, 10600, 10517, 10434, 10352, 10272, 10191, 10112, 10034, 9956, 9879, 9803, 9727, 9653, 9579, 9505, 9433, 9361, 9291, 9220, 9151, 9082, 9014, 8947, 8881, 8815, 8750, 8685, 8622, 8559, 8496, 8435, 8374, 8313, 8254, 8195, 8136, 8079, 8022, 7965, 7909, 7854, 7799, 7745, 7692, 7639, 7587, 7535, 7484, 7433, 7383, 7334, 7285, 7236, 7188, 7141, 7094, 7048, 7002, 6956, 6912, 6867, 6823, 6780, 6737, 6694, 6652, 6610, 6569, 6528, 6488, 6448, 6409, 6370, 6331, 6293, 6255, 6217, 
		15708, 15599, 15491, 15382, 15273, 15165, 15057, 14949, 14841, 14733, 14625, 14518, 14411, 14304, 14198, 14092, 13986, 13881, 13776, 13671, 13567, 13464, 13361, 13258, 13156, 13055, 12954, 12853, 12754, 12654, 12556, 12458, 12361, 12264, 12168, 12073, 11978, 11884, 11791, 11698, 11607, 11516, 11425, 11336, 11247, 11159, 11071, 10985, 10899, 10814, 10730, 10646, 10563, 10481, 10400, 10320, 10240, 10161, 10083, 10006, 9929, 9853, 9778, 9704, 9630, 9557, 9485, 9414, 9343, 9273, 9204, 9135, 9068, 9000, 8934, 8868, 8803, 8739, 8676, 8613, 8551, 8489, 8428, 8368, 8308, 8249, 8191, 8133, 8076, 8020, 7964, 7909, 7854, 7800, 7746, 7694, 7641, 7589, 7538, 7488, 7438, 7388, 7339, 7290, 7243, 7195, 7148, 7102, 7056, 7010, 6965, 6921, 6877, 6833, 6790, 6747, 6705, 6663, 6622, 6581, 6541, 6501, 6461, 6422, 6383, 6345, 6307, 6269, 
		15708, 15600, 15493, 15385, 15278, 15171, 15064, 14957, 14850, 14743, 14637, 14531, 14425, 14319, 14214, 14109, 14004, 13900, 13796, 13693, 13590, 13487, 13385, 13283, 13182, 13082, 12982, 12882, 12784, 12685, 12588, 12490, 12394, 12298, 12203, 12108, 12015, 11921, 11829, 11737, 11646, 11556, 11466, 11377, 11289, 11201, 11115, 11029, 10943, 10859, 10775, 10692, 10610, 10528, 10447, 10367, 10288, 10209, 10132, 10055, 9978, 9903, 9828, 9754, 9681, 9608, 9536, 9465, 9394, 9325, 9256, 9187, 9120, 9053, 8987, 8921, 8857, 8792, 8729, 8666, 8604, 8543, 8482, 8422, 8362, 8303, 8245, 8187, 8130, 8074, 8018, 7963, 7908, 7854, 7801, 7748, 7695, 7643, 7592, 7542, 7491, 7442, 7393, 7344, 7296, 7249, 7202, 7155, 7109, 7064, 7018, 6974, 6930, 6886, 6843, 6800, 6758, 6716, 6675, 6634, 6593, 6553, 6513, 6474, 6435, 6397, 6358, 6321, 
		15708, 15602, 15495, 15389, 15283, 15177, 15071, 14965, 14859, 14753, 14648, 14543, 14438, 14334, 14229, 14126, 14022, 13919, 13816, 13714, 13612, 13510, 13409, 13308, 13208, 13109, 13009, 12911, 12813, 12716, 12619, 12522, 12427, 12332, 12237, 12144, 12050, 11958, 11866, 11775, 11685, 11595, 11506, 11418, 11330, 11243, 11157, 11071, 10987, 10903, 10819, 10737, 10655, 10574, 10494, 10414, 10335, 10257, 10180, 10103, 10027, 9952, 9877, 9803, 9730, 9658, 9586, 9516, 9445, 9376, 9307, 9239, 9172, 9105, 9039, 8974, 8909, 8845, 8782, 8719, 8657, 8595, 8535, 8475, 8415, 8356, 8298, 8241, 8184, 8127, 8071, 8016, 7962, 7907, 7854, 7801, 7749, 7697, 7646, 7595, 7545, 7495, 7446, 7397, 7349, 7302, 7255, 7208, 7162, 7116, 7071, 7027, 6982, 6939, 6895, 6853, 6810, 6768, 6727, 6686, 6645, 6605, 6565, 6525, 6486, 6448, 6410, 6372, 
		15708, 15603, 15497, 15392, 15287, 15182, 15077, 14972, 14868, 14763, 14659, 14555, 14451, 14348, 14245, 14142, 14039, 13937, 13835, 13734, 13633, 13532, 13432, 13333, 13233, 13135, 13037, 12939, 12842, 12745, 12649, 12554, 12459, 12365, 12271, 12178, 12086, 11994, 11903, 11812, 11723, 11634, 11545, 11458, 11371, 11284, 11199, 11114, 11029, 10946, 10863, 10781, 10700, 10619, 10539, 10460, 10382, 10304, 10227, 10150, 10075, 10000, 9926, 9852, 9780, 9707, 9636, 9565, 9496, 9426, 9358, 9290, 9223, 9156, 9090, 9025, 8961, 8897, 8833, 8771, 8709, 8648, 8587, 8527, 8468, 8409, 8351, 8293, 8236, 8180, 8124, 8069, 8014, 7960, 7907, 7854, 7802, 7750, 7699, 7648, 7598, 7548, 7499, 7450, 7402, 7354, 7307, 7261, 7214, 7169, 7124, 7079, 7035, 6991, 6947, 6904, 6862, 6820, 6778, 6737, 6696, 6656, 6616, 6577, 6537, 6499, 6460, 6422, 
		15708, 15604, 15500, 15396, 15292, 15188, 15084, 14980, 14877, 14773, 14670, 14567, 14464, 14362, 14260, 14158, 14056, 13955, 13854, 13754, 13654, 13554, 13455, 13356, 13258, 13160, 13063, 12966, 12870, 12774, 12679, 12584, 12490, 12397, 12304, 12212, 12120, 12029, 11939, 11849, 11760, 11672, 11584, 11497, 11410, 11325, 11240, 11155, 11071, 10989, 10906, 10825, 10744, 10664, 10584, 10505, 10427, 10350, 10273, 10197, 10122, 10047, 9974, 9900, 9828, 9756, 9685, 9615, 9545, 9476, 9408, 9340, 9273, 9207, 9141, 9076, 9012, 8948, 8885, 8822, 8761, 8699, 8639, 8579, 8520, 8461, 8403, 8345, 8288, 8232, 8176, 8121, 8067, 8013, 7959, 7906, 7854, 7802, 7751, 7700, 7650, 7600, 7551, 7502, 7454, 7407, 7359, 7313, 7266, 7221, 7175, 7131, 7086, 7042, 6999, 6956, 6913, 6871, 6830, 6788, 6747, 6707, 6667, 6627, 6588, 6549, 6511, 6473, 
		15708, 15605, 15502, 15399, 15296, 15193, 15090, 14988, 14885, 14783, 14681, 14579, 14477, 14376, 14275, 14174, 14073, 13973, 13873, 13774, 13675, 13576, 13478, 13380, 13282, 13186, 13089, 12993, 12898, 12803, 12708, 12615, 12521, 12429, 12337, 12245, 12154, 12064, 11974, 11885, 11797, 11709, 11622, 11535, 11449, 11364, 11280, 11196, 11113, 11030, 10949, 10867, 10787, 10707, 10628, 10550, 10472, 10395, 10319, 10243, 10168, 10094, 10021, 9948, 9876, 9804, 9733, 9663, 9594, 9525, 9457, 9389, 9323, 9256, 9191, 9126, 9062, 8998, 8935, 8873, 8811, 8750, 8690, 8630, 8571, 8512, 8454, 8397, 8340, 8284, 8228, 8173, 8118, 8064, 8011, 7958, 7906, 7854, 7803, 7752, 7702, 7652, 7603, 7554, 7506, 7458, 7411, 7364, 7318, 7272, 7227, 7182, 7138, 7094, 7050, 7007, 6964, 6922, 6880, 6839, 6798, 6757, 6717, 6678, 6638, 6599, 6561, 6523, 
		15708, 15606, 15504, 15402, 15300, 15198, 15096, 14995, 14893, 14792, 14691, 14590, 14490, 14389, 14289, 14189, 14090, 13990, 13891, 13793, 13695, 13597, 13500, 13403, 13306, 13210, 13115, 13020, 12925, 12831, 12737, 12644, 12552, 12460, 12369, 12278, 12188, 12098, 12009, 11921, 11833, 11746, 11659, 11573, 11488, 11403, 11319, 11236, 11153, 11071, 10990, 10910, 10830, 10750, 10672, 10594, 10517, 10440, 10364, 10289, 10214, 10140, 10067, 9995, 9923, 9852, 9781, 9711, 9642, 9573, 9505, 9438, 9372, 9306, 9240, 9176, 9112, 9048, 8985, 8923, 8862, 8801, 8741, 8681, 8622, 8563, 8505, 8448, 8391, 8335, 8279, 8224, 8170, 8116, 8062, 8009, 7957, 7905, 7854, 7803, 7753, 7703, 7654, 7605, 7557, 7509, 7462, 7415, 7369, 7323, 7278, 7233, 7188, 7144, 7101, 7058, 7015, 6973, 6931, 6889, 6848, 6808, 6767, 6728, 6688, 6649, 6610, 6572, 
		15708, 15607, 15506, 15405, 15304, 15203, 15103, 15002, 14902, 14801, 14701, 14601, 14502, 14402, 14303, 14204, 14106, 14007, 13909, 13812, 13715, 13618, 13521, 13425, 13330, 13234, 13140, 13045, 12952, 12858, 12766, 12673, 12582, 12490, 12400, 12310, 12220, 12131, 12043, 11955, 11868, 11782, 11696, 11610, 11526, 11442, 11358, 11276, 11193, 11112, 11031, 10951, 10872, 10793, 10714, 10637, 10560, 10484, 10408, 10334, 10259, 10186, 10113, 10041, 9969, 9898, 9828, 9758, 9689, 9621, 9553, 9486, 9420, 9354, 9289, 9225, 9161, 9098, 9035, 8973, 8911, 8851, 8790, 8731, 8672, 8613, 8556, 8498, 8442, 8385, 8330, 8275, 8220, 8166, 8113, 8060, 8008, 7956, 7905, 7854, 7804, 7754, 7705, 7656, 7608, 7560, 7513, 7466, 7419, 7374, 7328, 7283, 7239, 7195, 7151, 7108, 7065, 7023, 6981, 6939, 6898, 6857, 6817, 6777, 6738, 6698, 6660, 6621, 
		15708, 15608, 15508, 15408, 15308, 15208, 15109, 15009, 14910, 14810, 14711, 14612, 14514, 14415, 14317, 14219, 14121, 14024, 13927, 13830, 13734, 13638, 13542, 13447, 13353, 13258, 13164, 13071, 12978, 12885, 12793, 12702, 12611, 12520, 12431, 12341, 12252, 12164, 12076, 11989, 11903, 11817, 11732, 11647, 11563, 11479, 11397, 11314, 11233, 11152, 11071, 10992, 10913, 10834, 10757, 10680, 10603, 10527, 10452, 10378, 10304, 10231, 10158, 10086, 10015, 9944, 9874, 9805, 9736, 9668, 9601, 9534, 9468, 9402, 9337, 9273, 9209, 9146, 9084, 9022, 8961, 8900, 8840, 8780, 8721, 8663, 8605, 8548, 8491, 8435, 8380, 8325, 8270, 8217, 8163, 8110, 8058, 8006, 7955, 7904, 7854, 7804, 7755, 7706, 7658, 7610, 7563, 7516, 7470, 7424, 7378, 7333, 7289, 7244, 7201, 7157, 7115, 7072, 7030, 6989, 6947, 6907, 6866, 6826, 6787, 6747, 6709, 6670, 
		15708, 15609, 15510, 15411, 15312, 15213, 15115, 15016, 14918, 14819, 14721, 14623, 14525, 14428, 14331, 14234, 14137, 14040, 13944, 13849, 13753, 13658, 13563, 13469, 13375, 13281, 13188, 13096, 13004, 12912, 12821, 12730, 12640, 12550, 12461, 12372, 12284, 12196, 12109, 12023, 11937, 11852, 11767, 11683, 11599, 11517, 11434, 11353, 11271, 11191, 11111, 11032, 10953, 10875, 10798, 10721, 10645, 10570, 10495, 10421, 10348, 10275, 10202, 10131, 10060, 9990, 9920, 9851, 9782, 9715, 9647, 9581, 9515, 9449, 9385, 9321, 9257, 9194, 9132, 9070, 9009, 8948, 8889, 8829, 8770, 8712, 8654, 8597, 8541, 8485, 8429, 8374, 8320, 8266, 8213, 8160, 8108, 8056, 8005, 7954, 7904, 7854, 7805, 7756, 7708, 7660, 7612, 7566, 7519, 7473, 7428, 7383, 7338, 7294, 7250, 7207, 7164, 7121, 7079, 7038, 6996, 6956, 6915, 6875, 6835, 6796, 6757, 6719, 
		15708, 15610, 15512, 15414, 15316, 15218, 15120, 15023, 14925, 14828, 14731, 14634, 14537, 14440, 14344, 14248, 14152, 14056, 13961, 13866, 13772, 13678, 13584, 13490, 13397, 13304, 13212, 13120, 13029, 12938, 12847, 12757, 12668, 12579, 12490, 12402, 12315, 12228, 12142, 12056, 11971, 11886, 11802, 11718, 11635, 11553, 11471, 11390, 11310, 11230, 11150, 11071, 10993, 10916, 10839, 10763, 10687, 10612, 10538, 10464, 10391, 10318, 10246, 10175, 10104, 10034, 9965, 9896, 9828, 9760, 9693, 9627, 9561, 9496, 9432, 9368, 9304, 9242, 9179, 9118, 9057, 8997, 8937, 8877, 8819, 8761, 8703, 8646, 8589, 8534, 8478, 8423, 8369, 8315, 8262, 8209, 8157, 8105, 8054, 8003, 7953, 7903, 7854, 7805, 7757, 7709, 7662, 7615, 7568, 7522, 7477, 7432, 7387, 7343, 7299, 7256, 7213, 7170, 7128, 7086, 7045, 7004, 6963, 6923, 6884, 6844, 6805, 6767, 
		15708, 15611, 15514, 15417, 15320, 15223, 15126, 15029, 14933, 14836, 14740, 14644, 14548, 14452, 14357, 14262, 14167, 14072, 13978, 13884, 13790, 13697, 13604, 13511, 13419, 13327, 13235, 13144, 13054, 12963, 12874, 12784, 12696, 12607, 12520, 12432, 12346, 12259, 12174, 12088, 12004, 11920, 11836, 11753, 11671, 11589, 11508, 11427, 11347, 11268, 11189, 11110, 11033, 10956, 10879, 10803, 10728, 10653, 10579, 10506, 10433, 10361, 10290, 10219, 10148, 10078, 10009, 9941, 9873, 9806, 9739, 9673, 9607, 9542, 9478, 9414, 9351, 9289, 9227, 9165, 9104, 9044, 8984, 8925, 8867, 8809, 8751, 8694, 8638, 8582, 8527, 8472, 8417, 8364, 8311, 8258, 8206, 8154, 8103, 8052, 8002, 7952, 7903, 7854, 7806, 7758, 7710, 7664, 7617, 7571, 7525, 7480, 7436, 7391, 7348, 7304, 7261, 7218, 7176, 7135, 7093, 7052, 7012, 6971, 6931, 6892, 6853, 6814, 
		15708, 15612, 15516, 15420, 15324, 15228, 15132, 15036, 14940, 14845, 14749, 14654, 14559, 14464, 14370, 14276, 14181, 14088, 13994, 13901, 13808, 13716, 13623, 13531, 13440, 13349, 13258, 13168, 13078, 12989, 12900, 12811, 12723, 12635, 12548, 12462, 12376, 12290, 12205, 12120, 12036, 11953, 11870, 11787, 11706, 11624, 11544, 11463, 11384, 11305, 11227, 11149, 11071, 10995, 10919, 10843, 10769, 10694, 10621, 10548, 10475, 10403, 10332, 10261, 10191, 10122, 10053, 9985, 9917, 9850, 9784, 9718, 9653, 9588, 9524, 9460, 9397, 9335, 9273, 9212, 9151, 9091, 9031, 8972, 8914, 8856, 8799, 8742, 8685, 8630, 8574, 8520, 8465, 8412, 8359, 8306, 8254, 8202, 8151, 8100, 8050, 8000, 7951, 7902, 7854, 7806, 7759, 7712, 7665, 7619, 7574, 7529, 7484, 7439, 7396, 7352, 7309, 7266, 7224, 7182, 7141, 7100, 7059, 7019, 6979, 6940, 6900, 6862, 
		15708, 15613, 15518, 15422, 15327, 15232, 15137, 15042, 14948, 14853, 14758, 14664, 14570, 14476, 14382, 14289, 14196, 14103, 14010, 13918, 13826, 13734, 13643, 13552, 13461, 13371, 13281, 13191, 13102, 13013, 12925, 12837, 12750, 12663, 12576, 12490, 12405, 12320, 12236, 12152, 12068, 11985, 11903, 11821, 11740, 11659, 11579, 11499, 11420, 11342, 11264, 11186, 11110, 11033, 10958, 10883, 10808, 10735, 10661, 10589, 10517, 10445, 10374, 10304, 10234, 10165, 10096, 10028, 9961, 9894, 9828, 9762, 9697, 9633, 9569, 9505, 9443, 9380, 9319, 9258, 9197, 9137, 9078, 9019, 8961, 8903, 8845, 8789, 8733, 8677, 8622, 8567, 8513, 8459, 8406, 8354, 8301, 8250, 8199, 8148, 8098, 8048, 7999, 7950, 7902, 7854, 7807, 7760, 7713, 7667, 7621, 7576, 7532, 7487, 7443, 7400, 7357, 7314, 7272, 7230, 7188, 7147, 7106, 7066, 7026, 6987, 6947, 6909, 
		15708, 15614, 15519, 15425, 15331, 15237, 15143, 15049, 14955, 14861, 14767, 14674, 14581, 14488, 14395, 14302, 14210, 14118, 14026, 13934, 13843, 13752, 13662, 13571, 13481, 13392, 13303, 13214, 13125, 13037, 12950, 12863, 12776, 12690, 12604, 12519, 12434, 12350, 12266, 12182, 12100, 12017, 11935, 11854, 11773, 11693, 11614, 11534, 11456, 11378, 11300, 11224, 11147, 11071, 10996, 10922, 10848, 10774, 10701, 10629, 10557, 10486, 10415, 10345, 10276, 10207, 10139, 10071, 10004, 9938, 9872, 9806, 9741, 9677, 9613, 9550, 9488, 9426, 9364, 9303, 9243, 9183, 9124, 9065, 9007, 8949, 8892, 8835, 8779, 8724, 8669, 8614, 8560, 8506, 8453, 8401, 8349, 8297, 8246, 8195, 8145, 8095, 8046, 7998, 7949, 7901, 7854, 7807, 7761, 7714, 7669, 7624, 7579, 7534, 7491, 7447, 7404, 7361, 7319, 7277, 7235, 7194, 7153, 7113, 7073, 7033, 6994, 6955, 
		15708, 15615, 15521, 15428, 15334, 15241, 15148, 15055, 14962, 14869, 14776, 14684, 14591, 14499, 14407, 14315, 14224, 14132, 14041, 13951, 13860, 13770, 13680, 13591, 13501, 13413, 13324, 13236, 13149, 13061, 12974, 12888, 12802, 12716, 12631, 12547, 12462, 12379, 12295, 12213, 12131, 12049, 11968, 11887, 11807, 11727, 11648, 11569, 11491, 11414, 11337, 11260, 11184, 11109, 11034, 10960, 10886, 10813, 10741, 10669, 10597, 10527, 10456, 10387, 10318, 10249, 10181, 10114, 10047, 9980, 9915, 9850, 9785, 9721, 9657, 9594, 9532, 9470, 9409, 9348, 9288, 9228, 9169, 9110, 9052, 8995, 8938, 8881, 8825, 8770, 8715, 8660, 8606, 8553, 8500, 8447, 8395, 8344, 8293, 8242, 8192, 8142, 8093, 8044, 7996, 7948, 7901, 7854, 7807, 7761, 7716, 7671, 7626, 7581, 7537, 7494, 7451, 7408, 7365, 7324, 7282, 7241, 7200, 7159, 7119, 7080, 7040, 7001, 
		15708, 15615, 15523, 15430, 15338, 15245, 15153, 15061, 14969, 14877, 14785, 14693, 14601, 14510, 14419, 14328, 14237, 14147, 14056, 13967, 13877, 13787, 13698, 13610, 13521, 13433, 13346, 13258, 13171, 13085, 12998, 12913, 12827, 12743, 12658, 12574, 12490, 12407, 12325, 12243, 12161, 12080, 11999, 11919, 11839, 11760, 11681, 11603, 11526, 11449, 11372, 11296, 11221, 11146, 11071, 10998, 10924, 10852, 10780, 10708, 10637, 10567, 10497, 10427, 10358, 10290, 10222, 10155, 10089, 10023, 9957, 9892, 9828, 9764, 9701, 9638, 9576, 9514, 9453, 9393, 9332, 9273, 9214, 9155, 9098, 9040, 8983, 8927, 8871, 8815, 8761, 8706, 8652, 8599, 8546, 8494, 8442, 8390, 8339, 8288, 8238, 8189, 8140, 8091, 8043, 7995, 7947, 7900, 7854, 7808, 7762, 7717, 7672, 7628, 7584, 7540, 7497, 7454, 7412, 7370, 7328, 7287, 7246, 7206, 7165, 7126, 7086, 7047, 
		15708, 15616, 15524, 15433, 15341, 15250, 15158, 15067, 14975, 14884, 14793, 14702, 14611, 14521, 14431, 14340, 14250, 14161, 14071, 13982, 13893, 13805, 13716, 13628, 13541, 13453, 13366, 13280, 13194, 13108, 13022, 12937, 12852, 12768, 12684, 12601, 12518, 12436, 12353, 12272, 12191, 12110, 12030, 11950, 11871, 11793, 11715, 11637, 11560, 11483, 11407, 11332, 11257, 11182, 11108, 11035, 10962, 10890, 10818, 10747, 10676, 10606, 10536, 10467, 10399, 10331, 10263, 10197, 10130, 10064, 9999, 9935, 9870, 9807, 9744, 9681, 9619, 9558, 9497, 9436, 9376, 9317, 9258, 9200, 9142, 9085, 9028, 8972, 8916, 8861, 8806, 8752, 8698, 8644, 8592, 8539, 8487, 8436, 8385, 8334, 8284, 8235, 8186, 8137, 8089, 8041, 7994, 7947, 7900, 7854, 7808, 7763, 7718, 7674, 7630, 7586, 7543, 7500, 7458, 7416, 7374, 7333, 7292, 7251, 7211, 7171, 7132, 7093, 
		15708, 15617, 15526, 15435, 15344, 15254, 15163, 15072, 14982, 14892, 14801, 14711, 14621, 14532, 14442, 14353, 14264, 14175, 14086, 13998, 13909, 13822, 13734, 13647, 13560, 13473, 13387, 13301, 13215, 13130, 13045, 12961, 12877, 12793, 12710, 12627, 12545, 12463, 12382, 12301, 12220, 12140, 12061, 11982, 11903, 11825, 11747, 11670, 11593, 11517, 11442, 11367, 11292, 11218, 11144, 11071, 10999, 10927, 10856, 10785, 10714, 10645, 10575, 10507, 10439, 10371, 10304, 10237, 10171, 10106, 10041, 9976, 9912, 9849, 9786, 9724, 9662, 9601, 9540, 9480, 9420, 9361, 9302, 9244, 9186, 9129, 9072, 9016, 8961, 8905, 8851, 8796, 8743, 8689, 8637, 8584, 8533, 8481, 8430, 8380, 8330, 8280, 8231, 8183, 8134, 8086, 8039, 7992, 7946, 7900, 7854, 7809, 7764, 7719, 7675, 7632, 7589, 7546, 7503, 7461, 7419, 7378, 7337, 7297, 7256, 7217, 7177, 7138, 
		15708, 15618, 15528, 15438, 15348, 15258, 15168, 15078, 14988, 14899, 14809, 14720, 14631, 14542, 14453, 14365, 14276, 14188, 14100, 14013, 13925, 13838, 13751, 13665, 13579, 13493, 13407, 13322, 13237, 13152, 13068, 12985, 12901, 12818, 12736, 12653, 12572, 12490, 12410, 12329, 12249, 12170, 12091, 12012, 11934, 11856, 11779, 11703, 11626, 11551, 11476, 11401, 11327, 11253, 11180, 11108, 11036, 10964, 10893, 10822, 10752, 10683, 10614, 10546, 10478, 10410, 10344, 10277, 10212, 10146, 10082, 10017, 9954, 9891, 9828, 9766, 9704, 9643, 9583, 9523, 9463, 9404, 9345, 9287, 9230, 9173, 9116, 9060, 9005, 8950, 8895, 8841, 8787, 8734, 8681, 8629, 8577, 8526, 8475, 8425, 8375, 8325, 8276, 8228, 8179, 8132, 8084, 8037, 7991, 7945, 7899, 7854, 7809, 7765, 7721, 7677, 7634, 7591, 7548, 7506, 7465, 7423, 7382, 7342, 7301, 7261, 7222, 7183, 
		15708, 15619, 15529, 15440, 15351, 15262, 15173, 15084, 14995, 14906, 14817, 14729, 14641, 14552, 14464, 14377, 14289, 14202, 14114, 14028, 13941, 13854, 13768, 13683, 13597, 13512, 13427, 13342, 13258, 13174, 13091, 13008, 12925, 12843, 12761, 12679, 12598, 12517, 12437, 12357, 12278, 12199, 12120, 12042, 11965, 11888, 11811, 11735, 11659, 11584, 11509, 11435, 11361, 11288, 11215, 11143, 11071, 11000, 10930, 10860, 10790, 10721, 10652, 10584, 10517, 10449, 10383, 10317, 10251, 10186, 10122, 10058, 9995, 9932, 9869, 9807, 9746, 9685, 9625, 9565, 9505, 9447, 9388, 9330, 9273, 9216, 9160, 9104, 9048, 8993, 8939, 8885, 8831, 8778, 8726, 8673, 8622, 8570, 8520, 8469, 8419, 8370, 8321, 8272, 8224, 8176, 8129, 8082, 8036, 7990, 7944, 7899, 7854, 7810, 7765, 7722, 7679, 7636, 7593, 7551, 7509, 7468, 7427, 7386, 7346, 7306, 7266, 7227, 
		15708, 15619, 15531, 15443, 15354, 15266, 15177, 15089, 15001, 14913, 14825, 14738, 14650, 14563, 14475, 14388, 14301, 14215, 14128, 14042, 13956, 13871, 13785, 13700, 13615, 13531, 13446, 13363, 13279, 13196, 13113, 13030, 12948, 12867, 12785, 12704, 12624, 12544, 12464, 12385, 12306, 12227, 12149, 12072, 11995, 11918, 11842, 11766, 11691, 11616, 11542, 11468, 11395, 11322, 11250, 11178, 11107, 11036, 10966, 10896, 10827, 10758, 10690, 10622, 10555, 10488, 10422, 10356, 10291, 10226, 10162, 10098, 10035, 9972, 9910, 9848, 9787, 9727, 9666, 9607, 9547, 9489, 9431, 9373, 9316, 9259, 9203, 9147, 9091, 9037, 8982, 8928, 8875, 8822, 8769, 8717, 8666, 8614, 8564, 8513, 8464, 8414, 8365, 8317, 8268, 8221, 8174, 8127, 8080, 8034, 7989, 7943, 7898, 7854, 7810, 7766, 7723, 7680, 7638, 7595, 7554, 7512, 7471, 7431, 7390, 7350, 7311, 7271, 
		15708, 15620, 15533, 15445, 15357, 15270, 15182, 15095, 15007, 14920, 14833, 14746, 14659, 14573, 14486, 14400, 14314, 14228, 14142, 14056, 13971, 13886, 13802, 13717, 13633, 13549, 13466, 13382, 13299, 13217, 13135, 13053, 12971, 12890, 12809, 12729, 12649, 12570, 12490, 12412, 12333, 12256, 12178, 12101, 12024, 11948, 11873, 11797, 11723, 11648, 11575, 11501, 11428, 11356, 11284, 11213, 11142, 11071, 11002, 10932, 10863, 10795, 10727, 10659, 10592, 10526, 10460, 10395, 10330, 10265, 10201, 10138, 10075, 10012, 9950, 9889, 9828, 9767, 9707, 9648, 9589, 9530, 9472, 9415, 9358, 9301, 9245, 9189, 9134, 9079, 9025, 8971, 8918, 8865, 8813, 8761, 8709, 8658, 8607, 8557, 8507, 8458, 8409, 8360, 8312, 8265, 8217, 8171, 8124, 8078, 8033, 7987, 7942, 7898, 7854, 7810, 7767, 7724, 7682, 7639, 7598, 7556, 7515, 7474, 7434, 7394, 7354, 7315, 
		15708, 15621, 15534, 15447, 15360, 15273, 15187, 15100, 15013, 14927, 14841, 14754, 14668, 14582, 14497, 14411, 14326, 14240, 14155, 14071, 13986, 13902, 13818, 13734, 13651, 13567, 13484, 13402, 13320, 13238, 13156, 13075, 12994, 12913, 12833, 12754, 12674, 12595, 12517, 12438, 12361, 12283, 12206, 12130, 12054, 11978, 11903, 11828, 11754, 11680, 11607, 11534, 11461, 11389, 11318, 11247, 11176, 11106, 11037, 10968, 10899, 10831, 10763, 10696, 10630, 10563, 10498, 10433, 10368, 10304, 10240, 10177, 10114, 10052, 9990, 9929, 9868, 9808, 9748, 9689, 9630, 9572, 9514, 9456, 9399, 9343, 9287, 9231, 9176, 9122, 9068, 9014, 8961, 8908, 8855, 8803, 8752, 8701, 8650, 8600, 8551, 8501, 8452, 8404, 8356, 8308, 8261, 8214, 8168, 8122, 8076, 8031, 7986, 7942, 7898, 7854, 7811, 7768, 7725, 7683, 7641, 7600, 7559, 7518, 7478, 7438, 7398, 7359, 
		15708, 15622, 15536, 15449, 15363, 15277, 15191, 15105, 15019, 14934, 14848, 14763, 14677, 14592, 14507, 14422, 14337, 14253, 14169, 14084, 14001, 13917, 13834, 13751, 13668, 13585, 13503, 13421, 13339, 13258, 13177, 13097, 13016, 12936, 12857, 12778, 12699, 12620, 12542, 12465, 12387, 12311, 12234, 12158, 12083, 12007, 11933, 11858, 11785, 11711, 11638, 11566, 11494, 11422, 11351, 11281, 11210, 11141, 11071, 11003, 10935, 10867, 10799, 10733, 10666, 10600, 10535, 10470, 10406, 10342, 10278, 10215, 10153, 10091, 10030, 9968, 9908, 9848, 9788, 9729, 9670, 9612, 9555, 9497, 9441, 9384, 9328, 9273, 9218, 9164, 9109, 9056, 9003, 8950, 8898, 8846, 8795, 8744, 8693, 8643, 8593, 8544, 8495, 8447, 8399, 8351, 8304, 8257, 8211, 8165, 8119, 8074, 8029, 7985, 7941, 7897, 7854, 7811, 7769, 7726, 7685, 7643, 7602, 7561, 7521, 7481, 7441, 7402, 
		15708, 15622, 15537, 15452, 15366, 15281, 15196, 15110, 15025, 14940, 14855, 14771, 14686, 14601, 14517, 14433, 14349, 14265, 14181, 14098, 14015, 13932, 13849, 13767, 13685, 13603, 13521, 13440, 13359, 13278, 13198, 13118, 13038, 12959, 12880, 12801, 12723, 12645, 12568, 12490, 12414, 12337, 12261, 12186, 12111, 12036, 11962, 11888, 11815, 11742, 11669, 11597, 11526, 11455, 11384, 11314, 11244, 11175, 11106, 11037, 10969, 10902, 10835, 10769, 10703, 10637, 10572, 10507, 10443, 10380, 10316, 10254, 10191, 10130, 10068, 10008, 9947, 9887, 9828, 9769, 9711, 9653, 9595, 9538, 9481, 9425, 9369, 9314, 9259, 9205, 9151, 9098, 9044, 8992, 8940, 8888, 8837, 8786, 8735, 8685, 8636, 8587, 8538, 8489, 8442, 8394, 8347, 8300, 8254, 8208, 8162, 8117, 8072, 8028, 7984, 7940, 7897, 7854, 7811, 7769, 7727, 7686, 7645, 7604, 7564, 7524, 7484, 7444, 
		15708, 15623, 15538, 15454, 15369, 15284, 15200, 15115, 15031, 14947, 14863, 14778, 14694, 14611, 14527, 14444, 14360, 14277, 14194, 14111, 14029, 13947, 13865, 13783, 13701, 13620, 13539, 13459, 13378, 13298, 13218, 13139, 13060, 12981, 12903, 12825, 12747, 12669, 12592, 12516, 12440, 12364, 12288, 12213, 12139, 12065, 11991, 11918, 11845, 11772, 11700, 11628, 11557, 11486, 11416, 11346, 11277, 11208, 11140, 11071, 11004, 10937, 10870, 10804, 10738, 10673, 10608, 10544, 10480, 10417, 10354, 10291, 10229, 10168, 10107, 10046, 9986, 9926, 9867, 9808, 9750, 9692, 9635, 9578, 9522, 9466, 9410, 9355, 9300, 9246, 9192, 9139, 9086, 9033, 8981, 8930, 8878, 8828, 8777, 8727, 8678, 8629, 8580, 8532, 8484, 8436, 8389, 8342, 8296, 8250, 8205, 8160, 8115, 8070, 8026, 7983, 7939, 7897, 7854, 7812, 7770, 7728, 7687, 7647, 7606, 7566, 7526, 7487, 
		15708, 15624, 15540, 15456, 15372, 15288, 15204, 15120, 15037, 14953, 14870, 14786, 14703, 14620, 14537, 14454, 14371, 14289, 14207, 14125, 14043, 13961, 13880, 13799, 13718, 13637, 13557, 13477, 13397, 13318, 13238, 13160, 13081, 13003, 12925, 12847, 12770, 12693, 12617, 12541, 12465, 12390, 12315, 12241, 12166, 12093, 12019, 11946, 11874, 11802, 11730, 11659, 11588, 11518, 11448, 11379, 11310, 11241, 11173, 11105, 11038, 10971, 10905, 10839, 10774, 10709, 10644, 10580, 10517, 10453, 10391, 10329, 10267, 10205, 10145, 10084, 10024, 9965, 9906, 9847, 9789, 9732, 9674, 9618, 9561, 9505, 9450, 9395, 9341, 9286, 9233, 9179, 9127, 9074, 9022, 8971, 8920, 8869, 8819, 8769, 8719, 8670, 8622, 8573, 8526, 8478, 8431, 8384, 8338, 8292, 8247, 8202, 8157, 8113, 8069, 8025, 7982, 7939, 7896, 7854, 7812, 7771, 7730, 7689, 7648, 7608, 7568, 7529, 
		15708, 15625, 15541, 15458, 15375, 15292, 15208, 15125, 15042, 14959, 14877, 14794, 14711, 14629, 14547, 14464, 14382, 14301, 14219, 14138, 14056, 13976, 13895, 13814, 13734, 13654, 13574, 13495, 13416, 13337, 13258, 13180, 13102, 13024, 12947, 12870, 12793, 12717, 12641, 12566, 12490, 12416, 12341, 12267, 12194, 12120, 12047, 11975, 11903, 11831, 11760, 11689, 11619, 11549, 11479, 11410, 11342, 11273, 11206, 11138, 11071, 11005, 10939, 10873, 10808, 10744, 10680, 10616, 10552, 10490, 10427, 10365, 10304, 10243, 10182, 10122, 10062, 10003, 9944, 9886, 9828, 9770, 9713, 9657, 9601, 9545, 9490, 9435, 9380, 9326, 9273, 9220, 9167, 9115, 9063, 9012, 8961, 8910, 8860, 8810, 8761, 8712, 8663, 8615, 8567, 8520, 8473, 8426, 8380, 8334, 8288, 8243, 8199, 8154, 8110, 8067, 8023, 7981, 7938, 7896, 7854, 7812, 7771, 7731, 7690, 7650, 7610, 7571, 
		15708, 15625, 15543, 15460, 15378, 15295, 15213, 15130, 15048, 14966, 14883, 14801, 14719, 14638, 14556, 14475, 14393, 14312, 14231, 14150, 14070, 13990, 13909, 13830, 13750, 13671, 13591, 13513, 13434, 13356, 13278, 13200, 13123, 13045, 12969, 12892, 12816, 12740, 12665, 12590, 12515, 12441, 12367, 12293, 12220, 12147, 12075, 12003, 11931, 11860, 11789, 11719, 11649, 11580, 11510, 11442, 11373, 11306, 11238, 11171, 11105, 11038, 10973, 10908, 10843, 10778, 10714, 10651, 10588, 10525, 10463, 10402, 10340, 10279, 10219, 10159, 10100, 10041, 9982, 9924, 9866, 9809, 9752, 9696, 9640, 9584, 9529, 9474, 9420, 9366, 9313, 9260, 9207, 9155, 9103, 9052, 9001, 8950, 8900, 8851, 8801, 8752, 8704, 8656, 8608, 8561, 8514, 8467, 8421, 8375, 8330, 8285, 8240, 8196, 8152, 8108, 8065, 8022, 7979, 7937, 7895, 7854, 7813, 7772, 7732, 7691, 7652, 7612, 
		15708, 15626, 15544, 15462, 15380, 15298, 15217, 15135, 15053, 14972, 14890, 14809, 14728, 14646, 14565, 14485, 14404, 14323, 14243, 14163, 14083, 14003, 13924, 13845, 13766, 13687, 13608, 13530, 13452, 13374, 13297, 13220, 13143, 13066, 12990, 12914, 12839, 12763, 12688, 12614, 12540, 12466, 12392, 12319, 12247, 12174, 12102, 12031, 11960, 11889, 11818, 11748, 11679, 11610, 11541, 11473, 11405, 11337, 11270, 11203, 11137, 11071, 11006, 10941, 10877, 10813, 10749, 10686, 10623, 10561, 10499, 10437, 10376, 10316, 10256, 10196, 10137, 10078, 10019, 9962, 9904, 9847, 9790, 9734, 9678, 9623, 9568, 9513, 9459, 9405, 9352, 9299, 9247, 9195, 9143, 9092, 9041, 8991, 8941, 8891, 8842, 8793, 8744, 8696, 8649, 8601, 8555, 8508, 8462, 8416, 8371, 8326, 8281, 8237, 8193, 8149, 8106, 8063, 8021, 7978, 7937, 7895, 7854, 7813, 7773, 7733, 7693, 7653, 
		15708, 15627, 15545, 15464, 15383, 15302, 15221, 15139, 15058, 14978, 14897, 14816, 14735, 14655, 14575, 14494, 14414, 14335, 14255, 14175, 14096, 14017, 13938, 13859, 13781, 13703, 13625, 13547, 13470, 13393, 13316, 13239, 13163, 13087, 13011, 12936, 12861, 12786, 12712, 12638, 12564, 12490, 12417, 12345, 12273, 12201, 12129, 12058, 11987, 11917, 11847, 11777, 11708, 11639, 11571, 11503, 11436, 11368, 11302, 11235, 11170, 11104, 11039, 10974, 10910, 10846, 10783, 10720, 10658, 10596, 10534, 10473, 10412, 10352, 10292, 10232, 10173, 10115, 10056, 9999, 9941, 9884, 9828, 9772, 9716, 9661, 9606, 9552, 9498, 9444, 9391, 9338, 9286, 9234, 9183, 9131, 9081, 9030, 8980, 8931, 8882, 8833, 8785, 8737, 8689, 8642, 8595, 8549, 8502, 8457, 8411, 8366, 8322, 8277, 8234, 8190, 8147, 8104, 8061, 8019, 7977, 7936, 7895, 7854, 7813, 7773, 7734, 7694, 
		15708, 15627, 15547, 15466, 15385, 15305, 15224, 15144, 15064, 14983, 14903, 14823, 14743, 14663, 14584, 14504, 14425, 14345, 14266, 14188, 14109, 14030, 13952, 13874, 13796, 13719, 13641, 13564, 13487, 13411, 13334, 13258, 13182, 13107, 13032, 12957, 12882, 12808, 12734, 12661, 12588, 12515, 12442, 12370, 12298, 12227, 12156, 12085, 12015, 11945, 11875, 11806, 11737, 11669, 11601, 11533, 11466, 11399, 11333, 11267, 11201, 11136, 11071, 11007, 10943, 10880, 10817, 10754, 10692, 10630, 10569, 10508, 10447, 10387, 10328, 10268, 10209, 10151, 10093, 10035, 9978, 9922, 9865, 9809, 9754, 9699, 9644, 9590, 9536, 9483, 9430, 9377, 9325, 9273, 9222, 9171, 9120, 9070, 9020, 8970, 8921, 8873, 8824, 8776, 8729, 8682, 8635, 8589, 8543, 8497, 8452, 8407, 8362, 8318, 8274, 8230, 8187, 8144, 8102, 8060, 8018, 7976, 7935, 7894, 7854, 7814, 7774, 7734, 
		15708, 15628, 15548, 15468, 15388, 15308, 15228, 15149, 15069, 14989, 14910, 14830, 14751, 14672, 14593, 14514, 14435, 14356, 14278, 14200, 14121, 14044, 13966, 13888, 13811, 13734, 13657, 13581, 13504, 13428, 13353, 13277, 13202, 13127, 13052, 12978, 12904, 12830, 12757, 12684, 12611, 12539, 12466, 12395, 12323, 12252, 12182, 12111, 12042, 11972, 11903, 11834, 11766, 11698, 11630, 11563, 11496, 11430, 11364, 11298, 11233, 11168, 11104, 11040, 10976, 10913, 10850, 10788, 10726, 10664, 10603, 10542, 10482, 10422, 10363, 10304, 10245, 10187, 10129, 10072, 10015, 9958, 9902, 9846, 9791, 9736, 9682, 9628, 9574, 9521, 9468, 9415, 9363, 9311, 9260, 9209, 9159, 9109, 9059, 9010, 8961, 8912, 8864, 8816, 8768, 8721, 8675, 8628, 8582, 8537, 8491, 8447, 8402, 8358, 8314, 8270, 8227, 8184, 8142, 8100, 8058, 8017, 7975, 7935, 7894, 7854, 7814, 7775, 
		15708, 15629, 15549, 15470, 15391, 15311, 15232, 15153, 15074, 14995, 14916, 14837, 14758, 14680, 14601, 14523, 14445, 14367, 14289, 14211, 14134, 14056, 13979, 13902, 13826, 13749, 13673, 13597, 13521, 13446, 13371, 13296, 13221, 13146, 13072, 12998, 12925, 12852, 12779, 12706, 12634, 12562, 12490, 12419, 12348, 12278, 12208, 12138, 12068, 11999, 11930, 11862, 11794, 11726, 11659, 11592, 11526, 11460, 11394, 11329, 11264, 11199, 11135, 11071, 11008, 10945, 10883, 10821, 10759, 10698, 10637, 10577, 10517, 10457, 10398, 10339, 10280, 10222, 10165, 10108, 10051, 9995, 9939, 9883, 9828, 9773, 9719, 9665, 9611, 9558, 9505, 9453, 9401, 9350, 9298, 9248, 9197, 9147, 9098, 9048, 8999, 8951, 8903, 8855, 8808, 8761, 8714, 8668, 8622, 8576, 8531, 8486, 8442, 8397, 8354, 8310, 8267, 8224, 8182, 8140, 8098, 8056, 8015, 7974, 7934, 7894, 7854, 7814, 
		15708, 15629, 15550, 15472, 15393, 15314, 15236, 15157, 15079, 15000, 14922, 14844, 14766, 14688, 14610, 14532, 14455, 14377, 14300, 14223, 14146, 14069, 13993, 13916, 13840, 13764, 13689, 13613, 13538, 13463, 13388, 13314, 13240, 13166, 13092, 13019, 12946, 12873, 12801, 12729, 12657, 12585, 12514, 12443, 12373, 12303, 12233, 12163, 12094, 12026, 11957, 11889, 11822, 11754, 11688, 11621, 11555, 11489, 11424, 11359, 11294, 11230, 11166, 11103, 11040, 10977, 10915, 10853, 10792, 10731, 10670, 10610, 10550, 10491, 10432, 10374, 10315, 10258, 10200, 10143, 10087, 10030, 9975, 9919, 9864, 9810, 9756, 9702, 9648, 9595, 9543, 9491, 9439, 9387, 9336, 9286, 9235, 9185, 9136, 9087, 9038, 8989, 8941, 8894, 8846, 8799, 8753, 8707, 8661, 8615, 8570, 8525, 8481, 8437, 8393, 8349, 8306, 8264, 8221, 8179, 8137, 8096, 8055, 8014, 7973, 7933, 7894, 7854, 
	};

        #endregion


#if UNITY_EDITOR
    public static string GenerateString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\tspublic static readonly  int[] table = new int[]");
        sb.AppendLine("\t{");
        {
            double dim = (double)DIM;
            
            for (int j = 0; j < DIM; ++j)
            {
                StringBuilder data = new StringBuilder();
                data.Append("\t\t");

                for (int i = 0; i < DIM; ++i)    
                {
                    double x0 = (double)i / dim;
                    double y0 = (double)j / dim;

                    double f = Math.Atan2(y0, x0);

                    int element = (int)Math.Round(f * 10000.0);

                    data.Append(element);
                    data.Append(", ");
                }

                sb.AppendLine(data.ToString());
            }
        }
        sb.AppendLine("\t};");
        sb.AppendLine();

        return sb.ToString();
    }
#endif

        static Atan2LookupTable()
        {
          //  DebugHelper.Assert(table.Length == COUNT);
        }
    }

    public static class AcosLookupTable
    {
        public static readonly int COUNT = 1024;
        public static readonly int HALF_COUNT = COUNT >> 1;

        #region Acos lookup table

        public static readonly int[] table = new int[]
	{
		31416, 30791, 30532, 30333, 30165, 30017, 29883, 29760, 29646, 29538, 29436, 29339, 29247, 29158, 29072, 28989, 28909, 28832, 28756, 28683, 28612, 28542, 28474, 28407, 28342, 28278, 28215, 28154, 28093, 28034, 27976, 27918, 
		27862, 27806, 27751, 27697, 27644, 27591, 27539, 27488, 27437, 27387, 27337, 27288, 27240, 27192, 27145, 27098, 27051, 27005, 26960, 26915, 26870, 26826, 26782, 26738, 26695, 26652, 26610, 26568, 26526, 26485, 26444, 26403, 
		26362, 26322, 26282, 26243, 26203, 26164, 26125, 26087, 26048, 26010, 25973, 25935, 25898, 25860, 25823, 25787, 25750, 25714, 25678, 25642, 25606, 25571, 25536, 25500, 25466, 25431, 25396, 25362, 25328, 25293, 25260, 25226, 
		25192, 25159, 25126, 25092, 25059, 25027, 24994, 24961, 24929, 24897, 24865, 24833, 24801, 24769, 24737, 24706, 24675, 24643, 24612, 24581, 24550, 24520, 24489, 24459, 24428, 24398, 24368, 24337, 24308, 24278, 24248, 24218, 
		24189, 24159, 24130, 24100, 24071, 24042, 24013, 23984, 23955, 23927, 23898, 23869, 23841, 23813, 23784, 23756, 23728, 23700, 23672, 23644, 23616, 23589, 23561, 23533, 23506, 23478, 23451, 23424, 23397, 23369, 23342, 23315, 
		23288, 23262, 23235, 23208, 23181, 23155, 23128, 23102, 23075, 23049, 23023, 22997, 22970, 22944, 22918, 22892, 22866, 22840, 22815, 22789, 22763, 22738, 22712, 22687, 22661, 22636, 22610, 22585, 22560, 22535, 22509, 22484, 
		22459, 22434, 22409, 22384, 22360, 22335, 22310, 22285, 22261, 22236, 22212, 22187, 22163, 22138, 22114, 22089, 22065, 22041, 22017, 21992, 21968, 21944, 21920, 21896, 21872, 21848, 21824, 21801, 21777, 21753, 21729, 21706, 
		21682, 21658, 21635, 21611, 21588, 21564, 21541, 21518, 21494, 21471, 21448, 21424, 21401, 21378, 21355, 21332, 21309, 21286, 21263, 21240, 21217, 21194, 21171, 21148, 21125, 21103, 21080, 21057, 21034, 21012, 20989, 20967, 
		20944, 20921, 20899, 20876, 20854, 20832, 20809, 20787, 20764, 20742, 20720, 20698, 20675, 20653, 20631, 20609, 20587, 20565, 20543, 20520, 20498, 20476, 20455, 20433, 20411, 20389, 20367, 20345, 20323, 20301, 20280, 20258, 
		20236, 20214, 20193, 20171, 20149, 20128, 20106, 20085, 20063, 20042, 20020, 19999, 19977, 19956, 19934, 19913, 19891, 19870, 19849, 19827, 19806, 19785, 19764, 19742, 19721, 19700, 19679, 19658, 19636, 19615, 19594, 19573, 
		19552, 19531, 19510, 19489, 19468, 19447, 19426, 19405, 19384, 19363, 19342, 19321, 19300, 19280, 19259, 19238, 19217, 19196, 19175, 19155, 19134, 19113, 19093, 19072, 19051, 19030, 19010, 18989, 18969, 18948, 18927, 18907, 
		18886, 18866, 18845, 18825, 18804, 18784, 18763, 18743, 18722, 18702, 18681, 18661, 18640, 18620, 18600, 18579, 18559, 18539, 18518, 18498, 18478, 18457, 18437, 18417, 18396, 18376, 18356, 18336, 18316, 18295, 18275, 18255, 
		18235, 18215, 18194, 18174, 18154, 18134, 18114, 18094, 18074, 18054, 18034, 18013, 17993, 17973, 17953, 17933, 17913, 17893, 17873, 17853, 17833, 17813, 17793, 17773, 17753, 17734, 17714, 17694, 17674, 17654, 17634, 17614, 
		17594, 17574, 17554, 17535, 17515, 17495, 17475, 17455, 17435, 17415, 17396, 17376, 17356, 17336, 17316, 17297, 17277, 17257, 17237, 17218, 17198, 17178, 17158, 17139, 17119, 17099, 17079, 17060, 17040, 17020, 17001, 16981, 
		16961, 16942, 16922, 16902, 16883, 16863, 16843, 16824, 16804, 16784, 16765, 16745, 16725, 16706, 16686, 16666, 16647, 16627, 16608, 16588, 16568, 16549, 16529, 16510, 16490, 16470, 16451, 16431, 16412, 16392, 16373, 16353, 
		16333, 16314, 16294, 16275, 16255, 16236, 16216, 16196, 16177, 16157, 16138, 16118, 16099, 16079, 16060, 16040, 16021, 16001, 15981, 15962, 15942, 15923, 15903, 15884, 15864, 15845, 15825, 15806, 15786, 15767, 15747, 15727, 
		15708, 15688, 15669, 15649, 15630, 15610, 15591, 15571, 15552, 15532, 15513, 15493, 15474, 15454, 15434, 15415, 15395, 15376, 15356, 15337, 15317, 15298, 15278, 15259, 15239, 15219, 15200, 15180, 15161, 15141, 15122, 15102, 
		15083, 15063, 15043, 15024, 15004, 14985, 14965, 14946, 14926, 14906, 14887, 14867, 14848, 14828, 14808, 14789, 14769, 14749, 14730, 14710, 14691, 14671, 14651, 14632, 14612, 14592, 14573, 14553, 14533, 14514, 14494, 14474, 
		14455, 14435, 14415, 14396, 14376, 14356, 14336, 14317, 14297, 14277, 14258, 14238, 14218, 14198, 14179, 14159, 14139, 14119, 14099, 14080, 14060, 14040, 14020, 14000, 13981, 13961, 13941, 13921, 13901, 13881, 13862, 13842, 
		13822, 13802, 13782, 13762, 13742, 13722, 13702, 13682, 13662, 13643, 13623, 13603, 13583, 13563, 13543, 13523, 13503, 13483, 13463, 13443, 13422, 13402, 13382, 13362, 13342, 13322, 13302, 13282, 13262, 13242, 13221, 13201, 
		13181, 13161, 13141, 13121, 13100, 13080, 13060, 13040, 13019, 12999, 12979, 12959, 12938, 12918, 12898, 12877, 12857, 12837, 12816, 12796, 12775, 12755, 12735, 12714, 12694, 12673, 12653, 12632, 12612, 12591, 12571, 12550, 
		12530, 12509, 12489, 12468, 12447, 12427, 12406, 12385, 12365, 12344, 12323, 12303, 12282, 12261, 12240, 12220, 12199, 12178, 12157, 12136, 12116, 12095, 12074, 12053, 12032, 12011, 11990, 11969, 11948, 11927, 11906, 11885, 
		11864, 11843, 11822, 11801, 11780, 11758, 11737, 11716, 11695, 11674, 11652, 11631, 11610, 11589, 11567, 11546, 11524, 11503, 11482, 11460, 11439, 11417, 11396, 11374, 11353, 11331, 11310, 11288, 11266, 11245, 11223, 11202, 
		11180, 11158, 11136, 11115, 11093, 11071, 11049, 11027, 11005, 10983, 10961, 10939, 10917, 10895, 10873, 10851, 10829, 10807, 10785, 10763, 10741, 10718, 10696, 10674, 10651, 10629, 10607, 10584, 10562, 10540, 10517, 10495, 
		10472, 10449, 10427, 10404, 10382, 10359, 10336, 10313, 10291, 10268, 10245, 10222, 10199, 10176, 10153, 10130, 10107, 10084, 10061, 10038, 10015, 9992, 9968, 9945, 9922, 9898, 9875, 9852, 9828, 9805, 9781, 9758, 
		9734, 9710, 9687, 9663, 9639, 9615, 9591, 9568, 9544, 9520, 9496, 9472, 9448, 9423, 9399, 9375, 9351, 9327, 9302, 9278, 9253, 9229, 9204, 9180, 9155, 9131, 9106, 9081, 9056, 9031, 9007, 8982, 
		8957, 8932, 8907, 8881, 8856, 8831, 8806, 8780, 8755, 8729, 8704, 8678, 8653, 8627, 8601, 8575, 8550, 8524, 8498, 8472, 8446, 8419, 8393, 8367, 8341, 8314, 8288, 8261, 8235, 8208, 8181, 8154, 
		8128, 8101, 8074, 8047, 8019, 7992, 7965, 7938, 7910, 7883, 7855, 7827, 7800, 7772, 7744, 7716, 7688, 7660, 7632, 7603, 7575, 7546, 7518, 7489, 7461, 7432, 7403, 7374, 7345, 7315, 7286, 7257, 
		7227, 7198, 7168, 7138, 7108, 7078, 7048, 7018, 6988, 6957, 6927, 6896, 6865, 6835, 6804, 6773, 6741, 6710, 6678, 6647, 6615, 6583, 6551, 6519, 6487, 6455, 6422, 6389, 6356, 6324, 6290, 6257, 
		6224, 6190, 6156, 6122, 6088, 6054, 6020, 5985, 5950, 5915, 5880, 5845, 5810, 5774, 5738, 5702, 5666, 5629, 5592, 5556, 5518, 5481, 5443, 5406, 5368, 5329, 5291, 5252, 5213, 5173, 5134, 5094, 
		5054, 5013, 4972, 4931, 4890, 4848, 4806, 4764, 4721, 4678, 4634, 4590, 4546, 4501, 4456, 4411, 4365, 4318, 4271, 4224, 4176, 4128, 4079, 4029, 3979, 3928, 3877, 3825, 3772, 3719, 3665, 3610, 
		3554, 3498, 3440, 3382, 3322, 3262, 3201, 3138, 3074, 3009, 2942, 2874, 2804, 2733, 2659, 2584, 2507, 2427, 2344, 2258, 2169, 2077, 1980, 1878, 1770, 1655, 1532, 1399, 1251, 1083, 884, 625, 
		0, 
	};



        #endregion

        static AcosLookupTable()
        {
          //  DebugHelper.Assert(table.Length == COUNT + 1);
        }

#if UNITY_EDITOR
    public static string GenerateString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("\tpublic static readonly int[] table = new int[]");
        sb.AppendLine("\t{");
        {
            int numElementPerLine = 32;

            sb.Append("\t\t");

            int num = 0;
            for (int i = 0; i <= COUNT; ++i)
            {
                double d = (i / (double)COUNT) * 2.0 - 1.0;
                double a = Math.Acos(d);
                int element = (int)Math.Round(a * 10000);

                sb.Append(element);
                sb.Append(", ");

                if (++num >= numElementPerLine && i < COUNT)
                {
                    num = 0;
                    sb.AppendLine();
                    if (i != COUNT)
                    {
                        sb.Append("\t\t");
                    }
                }
            }
        }
        sb.AppendLine();
        sb.AppendLine("\t};");
        sb.AppendLine();

        return sb.ToString();
    }
#endif
    }

    public static class SinCosLookupTable
    {
        public static readonly int BITS = 12;
        public static readonly int MASK = ~(-1 << BITS);
        public static readonly int COUNT = MASK + 1;

        public static readonly int FACTOR = 10000;
        public static readonly int NOM_MUL = FACTOR * COUNT;

        #region Sin lookup table
        public static readonly int[] sin_table = new int[]
	{
		0, 15, 31, 46, 61, 77, 92, 107, 123, 138, 153, 169, 184, 199, 215, 230, 245, 261, 276, 291, 307, 322, 337, 353, 368, 383, 399, 414, 429, 445, 460, 475, 491, 506, 521, 537, 552, 567, 583, 598, 613, 629, 644, 659, 674, 690, 705, 720, 736, 751, 766, 782, 797, 812, 827, 843, 858, 873, 889, 904, 919, 934, 950, 965, 
		980, 995, 1011, 1026, 1041, 1056, 1072, 1087, 1102, 1117, 1133, 1148, 1163, 1178, 1194, 1209, 1224, 1239, 1255, 1270, 1285, 1300, 1315, 1331, 1346, 1361, 1376, 1391, 1407, 1422, 1437, 1452, 1467, 1482, 1498, 1513, 1528, 1543, 1558, 1573, 1589, 1604, 1619, 1634, 1649, 1664, 1679, 1695, 1710, 1725, 1740, 1755, 1770, 1785, 1800, 1815, 1830, 1845, 1861, 1876, 1891, 1906, 1921, 1936, 
		1951, 1966, 1981, 1996, 2011, 2026, 2041, 2056, 2071, 2086, 2101, 2116, 2131, 2146, 2161, 2176, 2191, 2206, 2221, 2236, 2251, 2266, 2281, 2296, 2311, 2326, 2340, 2355, 2370, 2385, 2400, 2415, 2430, 2445, 2460, 2474, 2489, 2504, 2519, 2534, 2549, 2563, 2578, 2593, 2608, 2623, 2638, 2652, 2667, 2682, 2697, 2711, 2726, 2741, 2756, 2770, 2785, 2800, 2815, 2829, 2844, 2859, 2873, 2888, 
		2903, 2918, 2932, 2947, 2962, 2976, 2991, 3005, 3020, 3035, 3049, 3064, 3078, 3093, 3108, 3122, 3137, 3151, 3166, 3180, 3195, 3210, 3224, 3239, 3253, 3268, 3282, 3297, 3311, 3326, 3340, 3354, 3369, 3383, 3398, 3412, 3427, 3441, 3455, 3470, 3484, 3499, 3513, 3527, 3542, 3556, 3570, 3585, 3599, 3613, 3628, 3642, 3656, 3670, 3685, 3699, 3713, 3727, 3742, 3756, 3770, 3784, 3798, 3813, 
		3827, 3841, 3855, 3869, 3883, 3898, 3912, 3926, 3940, 3954, 3968, 3982, 3996, 4010, 4024, 4038, 4052, 4066, 4080, 4094, 4108, 4122, 4136, 4150, 4164, 4178, 4192, 4206, 4220, 4234, 4248, 4262, 4276, 4289, 4303, 4317, 4331, 4345, 4359, 4372, 4386, 4400, 4414, 4427, 4441, 4455, 4469, 4482, 4496, 4510, 4523, 4537, 4551, 4564, 4578, 4592, 4605, 4619, 4633, 4646, 4660, 4673, 4687, 4700, 
		4714, 4727, 4741, 4755, 4768, 4781, 4795, 4808, 4822, 4835, 4849, 4862, 4876, 4889, 4902, 4916, 4929, 4942, 4956, 4969, 4982, 4996, 5009, 5022, 5035, 5049, 5062, 5075, 5088, 5102, 5115, 5128, 5141, 5154, 5167, 5180, 5194, 5207, 5220, 5233, 5246, 5259, 5272, 5285, 5298, 5311, 5324, 5337, 5350, 5363, 5376, 5389, 5402, 5415, 5428, 5440, 5453, 5466, 5479, 5492, 5505, 5517, 5530, 5543, 
		5556, 5568, 5581, 5594, 5607, 5619, 5632, 5645, 5657, 5670, 5683, 5695, 5708, 5720, 5733, 5746, 5758, 5771, 5783, 5796, 5808, 5821, 5833, 5846, 5858, 5870, 5883, 5895, 5908, 5920, 5932, 5945, 5957, 5969, 5982, 5994, 6006, 6018, 6031, 6043, 6055, 6067, 6079, 6092, 6104, 6116, 6128, 6140, 6152, 6164, 6176, 6189, 6201, 6213, 6225, 6237, 6249, 6261, 6273, 6284, 6296, 6308, 6320, 6332, 
		6344, 6356, 6368, 6379, 6391, 6403, 6415, 6427, 6438, 6450, 6462, 6473, 6485, 6497, 6508, 6520, 6532, 6543, 6555, 6567, 6578, 6590, 6601, 6613, 6624, 6636, 6647, 6659, 6670, 6681, 6693, 6704, 6716, 6727, 6738, 6750, 6761, 6772, 6784, 6795, 6806, 6817, 6828, 6840, 6851, 6862, 6873, 6884, 6895, 6907, 6918, 6929, 6940, 6951, 6962, 6973, 6984, 6995, 7006, 7017, 7028, 7038, 7049, 7060, 
		7071, 7082, 7093, 7104, 7114, 7125, 7136, 7147, 7157, 7168, 7179, 7189, 7200, 7211, 7221, 7232, 7242, 7253, 7264, 7274, 7285, 7295, 7306, 7316, 7327, 7337, 7347, 7358, 7368, 7379, 7389, 7399, 7410, 7420, 7430, 7440, 7451, 7461, 7471, 7481, 7491, 7502, 7512, 7522, 7532, 7542, 7552, 7562, 7572, 7582, 7592, 7602, 7612, 7622, 7632, 7642, 7652, 7662, 7671, 7681, 7691, 7701, 7711, 7720, 
		7730, 7740, 7750, 7759, 7769, 7779, 7788, 7798, 7807, 7817, 7827, 7836, 7846, 7855, 7865, 7874, 7883, 7893, 7902, 7912, 7921, 7930, 7940, 7949, 7958, 7968, 7977, 7986, 7995, 8005, 8014, 8023, 8032, 8041, 8050, 8059, 8068, 8078, 8087, 8096, 8105, 8114, 8123, 8131, 8140, 8149, 8158, 8167, 8176, 8185, 8193, 8202, 8211, 8220, 8228, 8237, 8246, 8255, 8263, 8272, 8280, 8289, 8298, 8306, 
		8315, 8323, 8332, 8340, 8349, 8357, 8365, 8374, 8382, 8391, 8399, 8407, 8416, 8424, 8432, 8440, 8449, 8457, 8465, 8473, 8481, 8489, 8497, 8505, 8514, 8522, 8530, 8538, 8546, 8554, 8561, 8569, 8577, 8585, 8593, 8601, 8609, 8616, 8624, 8632, 8640, 8647, 8655, 8663, 8670, 8678, 8686, 8693, 8701, 8708, 8716, 8723, 8731, 8738, 8746, 8753, 8761, 8768, 8775, 8783, 8790, 8797, 8805, 8812, 
		8819, 8826, 8834, 8841, 8848, 8855, 8862, 8869, 8876, 8883, 8890, 8897, 8904, 8911, 8918, 8925, 8932, 8939, 8946, 8953, 8960, 8966, 8973, 8980, 8987, 8993, 9000, 9007, 9013, 9020, 9027, 9033, 9040, 9046, 9053, 9059, 9066, 9072, 9079, 9085, 9092, 9098, 9104, 9111, 9117, 9123, 9130, 9136, 9142, 9148, 9154, 9161, 9167, 9173, 9179, 9185, 9191, 9197, 9203, 9209, 9215, 9221, 9227, 9233, 
		9239, 9245, 9250, 9256, 9262, 9268, 9274, 9279, 9285, 9291, 9296, 9302, 9308, 9313, 9319, 9324, 9330, 9335, 9341, 9346, 9352, 9357, 9363, 9368, 9373, 9379, 9384, 9389, 9395, 9400, 9405, 9410, 9415, 9421, 9426, 9431, 9436, 9441, 9446, 9451, 9456, 9461, 9466, 9471, 9476, 9481, 9486, 9490, 9495, 9500, 9505, 9510, 9514, 9519, 9524, 9528, 9533, 9538, 9542, 9547, 9551, 9556, 9560, 9565, 
		9569, 9574, 9578, 9583, 9587, 9591, 9596, 9600, 9604, 9609, 9613, 9617, 9621, 9625, 9630, 9634, 9638, 9642, 9646, 9650, 9654, 9658, 9662, 9666, 9670, 9674, 9678, 9681, 9685, 9689, 9693, 9697, 9700, 9704, 9708, 9711, 9715, 9719, 9722, 9726, 9729, 9733, 9736, 9740, 9743, 9747, 9750, 9754, 9757, 9760, 9764, 9767, 9770, 9774, 9777, 9780, 9783, 9786, 9789, 9793, 9796, 9799, 9802, 9805, 
		9808, 9811, 9814, 9817, 9820, 9823, 9825, 9828, 9831, 9834, 9837, 9839, 9842, 9845, 9847, 9850, 9853, 9855, 9858, 9861, 9863, 9866, 9868, 9871, 9873, 9875, 9878, 9880, 9883, 9885, 9887, 9890, 9892, 9894, 9896, 9898, 9901, 9903, 9905, 9907, 9909, 9911, 9913, 9915, 9917, 9919, 9921, 9923, 9925, 9927, 9929, 9930, 9932, 9934, 9936, 9937, 9939, 9941, 9942, 9944, 9946, 9947, 9949, 9950, 
		9952, 9953, 9955, 9956, 9958, 9959, 9960, 9962, 9963, 9964, 9966, 9967, 9968, 9969, 9971, 9972, 9973, 9974, 9975, 9976, 9977, 9978, 9979, 9980, 9981, 9982, 9983, 9984, 9985, 9986, 9986, 9987, 9988, 9989, 9989, 9990, 9991, 9991, 9992, 9993, 9993, 9994, 9994, 9995, 9995, 9996, 9996, 9997, 9997, 9997, 9998, 9998, 9998, 9999, 9999, 9999, 9999, 9999, 10000, 10000, 10000, 10000, 10000, 10000, 
		10000, 10000, 10000, 10000, 10000, 10000, 10000, 9999, 9999, 9999, 9999, 9999, 9998, 9998, 9998, 9997, 9997, 9997, 9996, 9996, 9995, 9995, 9994, 9994, 9993, 9993, 9992, 9991, 9991, 9990, 9989, 9989, 9988, 9987, 9986, 9986, 9985, 9984, 9983, 9982, 9981, 9980, 9979, 9978, 9977, 9976, 9975, 9974, 9973, 9972, 9971, 9969, 9968, 9967, 9966, 9964, 9963, 9962, 9960, 9959, 9958, 9956, 9955, 9953, 
		9952, 9950, 9949, 9947, 9946, 9944, 9942, 9941, 9939, 9937, 9936, 9934, 9932, 9930, 9929, 9927, 9925, 9923, 9921, 9919, 9917, 9915, 9913, 9911, 9909, 9907, 9905, 9903, 9901, 9898, 9896, 9894, 9892, 9890, 9887, 9885, 9883, 9880, 9878, 9875, 9873, 9871, 9868, 9866, 9863, 9861, 9858, 9855, 9853, 9850, 9847, 9845, 9842, 9839, 9837, 9834, 9831, 9828, 9825, 9823, 9820, 9817, 9814, 9811, 
		9808, 9805, 9802, 9799, 9796, 9793, 9789, 9786, 9783, 9780, 9777, 9774, 9770, 9767, 9764, 9760, 9757, 9754, 9750, 9747, 9743, 9740, 9736, 9733, 9729, 9726, 9722, 9719, 9715, 9711, 9708, 9704, 9700, 9697, 9693, 9689, 9685, 9681, 9678, 9674, 9670, 9666, 9662, 9658, 9654, 9650, 9646, 9642, 9638, 9634, 9630, 9625, 9621, 9617, 9613, 9609, 9604, 9600, 9596, 9591, 9587, 9583, 9578, 9574, 
		9569, 9565, 9560, 9556, 9551, 9547, 9542, 9538, 9533, 9528, 9524, 9519, 9514, 9510, 9505, 9500, 9495, 9490, 9486, 9481, 9476, 9471, 9466, 9461, 9456, 9451, 9446, 9441, 9436, 9431, 9426, 9421, 9415, 9410, 9405, 9400, 9395, 9389, 9384, 9379, 9373, 9368, 9363, 9357, 9352, 9346, 9341, 9335, 9330, 9324, 9319, 9313, 9308, 9302, 9296, 9291, 9285, 9279, 9274, 9268, 9262, 9256, 9250, 9245, 
		9239, 9233, 9227, 9221, 9215, 9209, 9203, 9197, 9191, 9185, 9179, 9173, 9167, 9161, 9154, 9148, 9142, 9136, 9130, 9123, 9117, 9111, 9104, 9098, 9092, 9085, 9079, 9072, 9066, 9059, 9053, 9046, 9040, 9033, 9027, 9020, 9013, 9007, 9000, 8993, 8987, 8980, 8973, 8966, 8960, 8953, 8946, 8939, 8932, 8925, 8918, 8911, 8904, 8897, 8890, 8883, 8876, 8869, 8862, 8855, 8848, 8841, 8834, 8826, 
		8819, 8812, 8805, 8797, 8790, 8783, 8775, 8768, 8761, 8753, 8746, 8738, 8731, 8723, 8716, 8708, 8701, 8693, 8686, 8678, 8670, 8663, 8655, 8647, 8640, 8632, 8624, 8616, 8609, 8601, 8593, 8585, 8577, 8569, 8561, 8554, 8546, 8538, 8530, 8522, 8514, 8505, 8497, 8489, 8481, 8473, 8465, 8457, 8449, 8440, 8432, 8424, 8416, 8407, 8399, 8391, 8382, 8374, 8365, 8357, 8349, 8340, 8332, 8323, 
		8315, 8306, 8298, 8289, 8280, 8272, 8263, 8255, 8246, 8237, 8228, 8220, 8211, 8202, 8193, 8185, 8176, 8167, 8158, 8149, 8140, 8131, 8123, 8114, 8105, 8096, 8087, 8078, 8068, 8059, 8050, 8041, 8032, 8023, 8014, 8005, 7995, 7986, 7977, 7968, 7958, 7949, 7940, 7930, 7921, 7912, 7902, 7893, 7883, 7874, 7865, 7855, 7846, 7836, 7827, 7817, 7807, 7798, 7788, 7779, 7769, 7759, 7750, 7740, 
		7730, 7720, 7711, 7701, 7691, 7681, 7671, 7662, 7652, 7642, 7632, 7622, 7612, 7602, 7592, 7582, 7572, 7562, 7552, 7542, 7532, 7522, 7512, 7502, 7491, 7481, 7471, 7461, 7451, 7440, 7430, 7420, 7410, 7399, 7389, 7379, 7368, 7358, 7347, 7337, 7327, 7316, 7306, 7295, 7285, 7274, 7264, 7253, 7242, 7232, 7221, 7211, 7200, 7189, 7179, 7168, 7157, 7147, 7136, 7125, 7114, 7104, 7093, 7082, 
		7071, 7060, 7049, 7038, 7028, 7017, 7006, 6995, 6984, 6973, 6962, 6951, 6940, 6929, 6918, 6907, 6895, 6884, 6873, 6862, 6851, 6840, 6828, 6817, 6806, 6795, 6784, 6772, 6761, 6750, 6738, 6727, 6716, 6704, 6693, 6681, 6670, 6659, 6647, 6636, 6624, 6613, 6601, 6590, 6578, 6567, 6555, 6543, 6532, 6520, 6508, 6497, 6485, 6473, 6462, 6450, 6438, 6427, 6415, 6403, 6391, 6379, 6368, 6356, 
		6344, 6332, 6320, 6308, 6296, 6284, 6273, 6261, 6249, 6237, 6225, 6213, 6201, 6189, 6176, 6164, 6152, 6140, 6128, 6116, 6104, 6092, 6079, 6067, 6055, 6043, 6031, 6018, 6006, 5994, 5982, 5969, 5957, 5945, 5932, 5920, 5908, 5895, 5883, 5870, 5858, 5846, 5833, 5821, 5808, 5796, 5783, 5771, 5758, 5746, 5733, 5720, 5708, 5695, 5683, 5670, 5657, 5645, 5632, 5619, 5607, 5594, 5581, 5568, 
		5556, 5543, 5530, 5517, 5505, 5492, 5479, 5466, 5453, 5440, 5428, 5415, 5402, 5389, 5376, 5363, 5350, 5337, 5324, 5311, 5298, 5285, 5272, 5259, 5246, 5233, 5220, 5207, 5194, 5180, 5167, 5154, 5141, 5128, 5115, 5102, 5088, 5075, 5062, 5049, 5035, 5022, 5009, 4996, 4982, 4969, 4956, 4942, 4929, 4916, 4902, 4889, 4876, 4862, 4849, 4835, 4822, 4808, 4795, 4781, 4768, 4755, 4741, 4727, 
		4714, 4700, 4687, 4673, 4660, 4646, 4633, 4619, 4605, 4592, 4578, 4564, 4551, 4537, 4523, 4510, 4496, 4482, 4469, 4455, 4441, 4427, 4414, 4400, 4386, 4372, 4359, 4345, 4331, 4317, 4303, 4289, 4276, 4262, 4248, 4234, 4220, 4206, 4192, 4178, 4164, 4150, 4136, 4122, 4108, 4094, 4080, 4066, 4052, 4038, 4024, 4010, 3996, 3982, 3968, 3954, 3940, 3926, 3912, 3898, 3883, 3869, 3855, 3841, 
		3827, 3813, 3798, 3784, 3770, 3756, 3742, 3727, 3713, 3699, 3685, 3670, 3656, 3642, 3628, 3613, 3599, 3585, 3570, 3556, 3542, 3527, 3513, 3499, 3484, 3470, 3455, 3441, 3427, 3412, 3398, 3383, 3369, 3354, 3340, 3326, 3311, 3297, 3282, 3268, 3253, 3239, 3224, 3210, 3195, 3180, 3166, 3151, 3137, 3122, 3108, 3093, 3078, 3064, 3049, 3035, 3020, 3005, 2991, 2976, 2962, 2947, 2932, 2918, 
		2903, 2888, 2873, 2859, 2844, 2829, 2815, 2800, 2785, 2770, 2756, 2741, 2726, 2711, 2697, 2682, 2667, 2652, 2638, 2623, 2608, 2593, 2578, 2563, 2549, 2534, 2519, 2504, 2489, 2474, 2460, 2445, 2430, 2415, 2400, 2385, 2370, 2355, 2340, 2326, 2311, 2296, 2281, 2266, 2251, 2236, 2221, 2206, 2191, 2176, 2161, 2146, 2131, 2116, 2101, 2086, 2071, 2056, 2041, 2026, 2011, 1996, 1981, 1966, 
		1951, 1936, 1921, 1906, 1891, 1876, 1861, 1845, 1830, 1815, 1800, 1785, 1770, 1755, 1740, 1725, 1710, 1695, 1679, 1664, 1649, 1634, 1619, 1604, 1589, 1573, 1558, 1543, 1528, 1513, 1498, 1482, 1467, 1452, 1437, 1422, 1407, 1391, 1376, 1361, 1346, 1331, 1315, 1300, 1285, 1270, 1255, 1239, 1224, 1209, 1194, 1178, 1163, 1148, 1133, 1117, 1102, 1087, 1072, 1056, 1041, 1026, 1011, 995, 
		980, 965, 950, 934, 919, 904, 889, 873, 858, 843, 827, 812, 797, 782, 766, 751, 736, 720, 705, 690, 674, 659, 644, 629, 613, 598, 583, 567, 552, 537, 521, 506, 491, 475, 460, 445, 429, 414, 399, 383, 368, 353, 337, 322, 307, 291, 276, 261, 245, 230, 215, 199, 184, 169, 153, 138, 123, 107, 92, 77, 61, 46, 31, 15, 
		0, -15, -31, -46, -61, -77, -92, -107, -123, -138, -153, -169, -184, -199, -215, -230, -245, -261, -276, -291, -307, -322, -337, -353, -368, -383, -399, -414, -429, -445, -460, -475, -491, -506, -521, -537, -552, -567, -583, -598, -613, -629, -644, -659, -674, -690, -705, -720, -736, -751, -766, -782, -797, -812, -827, -843, -858, -873, -889, -904, -919, -934, -950, -965, 
		-980, -995, -1011, -1026, -1041, -1056, -1072, -1087, -1102, -1117, -1133, -1148, -1163, -1178, -1194, -1209, -1224, -1239, -1255, -1270, -1285, -1300, -1315, -1331, -1346, -1361, -1376, -1391, -1407, -1422, -1437, -1452, -1467, -1482, -1498, -1513, -1528, -1543, -1558, -1573, -1589, -1604, -1619, -1634, -1649, -1664, -1679, -1695, -1710, -1725, -1740, -1755, -1770, -1785, -1800, -1815, -1830, -1845, -1861, -1876, -1891, -1906, -1921, -1936, 
		-1951, -1966, -1981, -1996, -2011, -2026, -2041, -2056, -2071, -2086, -2101, -2116, -2131, -2146, -2161, -2176, -2191, -2206, -2221, -2236, -2251, -2266, -2281, -2296, -2311, -2326, -2340, -2355, -2370, -2385, -2400, -2415, -2430, -2445, -2460, -2474, -2489, -2504, -2519, -2534, -2549, -2563, -2578, -2593, -2608, -2623, -2638, -2652, -2667, -2682, -2697, -2711, -2726, -2741, -2756, -2770, -2785, -2800, -2815, -2829, -2844, -2859, -2873, -2888, 
		-2903, -2918, -2932, -2947, -2962, -2976, -2991, -3005, -3020, -3035, -3049, -3064, -3078, -3093, -3108, -3122, -3137, -3151, -3166, -3180, -3195, -3210, -3224, -3239, -3253, -3268, -3282, -3297, -3311, -3326, -3340, -3354, -3369, -3383, -3398, -3412, -3427, -3441, -3455, -3470, -3484, -3499, -3513, -3527, -3542, -3556, -3570, -3585, -3599, -3613, -3628, -3642, -3656, -3670, -3685, -3699, -3713, -3727, -3742, -3756, -3770, -3784, -3798, -3813, 
		-3827, -3841, -3855, -3869, -3883, -3898, -3912, -3926, -3940, -3954, -3968, -3982, -3996, -4010, -4024, -4038, -4052, -4066, -4080, -4094, -4108, -4122, -4136, -4150, -4164, -4178, -4192, -4206, -4220, -4234, -4248, -4262, -4276, -4289, -4303, -4317, -4331, -4345, -4359, -4372, -4386, -4400, -4414, -4427, -4441, -4455, -4469, -4482, -4496, -4510, -4523, -4537, -4551, -4564, -4578, -4592, -4605, -4619, -4633, -4646, -4660, -4673, -4687, -4700, 
		-4714, -4727, -4741, -4755, -4768, -4781, -4795, -4808, -4822, -4835, -4849, -4862, -4876, -4889, -4902, -4916, -4929, -4942, -4956, -4969, -4982, -4996, -5009, -5022, -5035, -5049, -5062, -5075, -5088, -5102, -5115, -5128, -5141, -5154, -5167, -5180, -5194, -5207, -5220, -5233, -5246, -5259, -5272, -5285, -5298, -5311, -5324, -5337, -5350, -5363, -5376, -5389, -5402, -5415, -5428, -5440, -5453, -5466, -5479, -5492, -5505, -5517, -5530, -5543, 
		-5556, -5568, -5581, -5594, -5607, -5619, -5632, -5645, -5657, -5670, -5683, -5695, -5708, -5720, -5733, -5746, -5758, -5771, -5783, -5796, -5808, -5821, -5833, -5846, -5858, -5870, -5883, -5895, -5908, -5920, -5932, -5945, -5957, -5969, -5982, -5994, -6006, -6018, -6031, -6043, -6055, -6067, -6079, -6092, -6104, -6116, -6128, -6140, -6152, -6164, -6176, -6189, -6201, -6213, -6225, -6237, -6249, -6261, -6273, -6284, -6296, -6308, -6320, -6332, 
		-6344, -6356, -6368, -6379, -6391, -6403, -6415, -6427, -6438, -6450, -6462, -6473, -6485, -6497, -6508, -6520, -6532, -6543, -6555, -6567, -6578, -6590, -6601, -6613, -6624, -6636, -6647, -6659, -6670, -6681, -6693, -6704, -6716, -6727, -6738, -6750, -6761, -6772, -6784, -6795, -6806, -6817, -6828, -6840, -6851, -6862, -6873, -6884, -6895, -6907, -6918, -6929, -6940, -6951, -6962, -6973, -6984, -6995, -7006, -7017, -7028, -7038, -7049, -7060, 
		-7071, -7082, -7093, -7104, -7114, -7125, -7136, -7147, -7157, -7168, -7179, -7189, -7200, -7211, -7221, -7232, -7242, -7253, -7264, -7274, -7285, -7295, -7306, -7316, -7327, -7337, -7347, -7358, -7368, -7379, -7389, -7399, -7410, -7420, -7430, -7440, -7451, -7461, -7471, -7481, -7491, -7502, -7512, -7522, -7532, -7542, -7552, -7562, -7572, -7582, -7592, -7602, -7612, -7622, -7632, -7642, -7652, -7662, -7671, -7681, -7691, -7701, -7711, -7720, 
		-7730, -7740, -7750, -7759, -7769, -7779, -7788, -7798, -7807, -7817, -7827, -7836, -7846, -7855, -7865, -7874, -7883, -7893, -7902, -7912, -7921, -7930, -7940, -7949, -7958, -7968, -7977, -7986, -7995, -8005, -8014, -8023, -8032, -8041, -8050, -8059, -8068, -8078, -8087, -8096, -8105, -8114, -8123, -8131, -8140, -8149, -8158, -8167, -8176, -8185, -8193, -8202, -8211, -8220, -8228, -8237, -8246, -8255, -8263, -8272, -8280, -8289, -8298, -8306, 
		-8315, -8323, -8332, -8340, -8349, -8357, -8365, -8374, -8382, -8391, -8399, -8407, -8416, -8424, -8432, -8440, -8449, -8457, -8465, -8473, -8481, -8489, -8497, -8505, -8514, -8522, -8530, -8538, -8546, -8554, -8561, -8569, -8577, -8585, -8593, -8601, -8609, -8616, -8624, -8632, -8640, -8647, -8655, -8663, -8670, -8678, -8686, -8693, -8701, -8708, -8716, -8723, -8731, -8738, -8746, -8753, -8761, -8768, -8775, -8783, -8790, -8797, -8805, -8812, 
		-8819, -8826, -8834, -8841, -8848, -8855, -8862, -8869, -8876, -8883, -8890, -8897, -8904, -8911, -8918, -8925, -8932, -8939, -8946, -8953, -8960, -8966, -8973, -8980, -8987, -8993, -9000, -9007, -9013, -9020, -9027, -9033, -9040, -9046, -9053, -9059, -9066, -9072, -9079, -9085, -9092, -9098, -9104, -9111, -9117, -9123, -9130, -9136, -9142, -9148, -9154, -9161, -9167, -9173, -9179, -9185, -9191, -9197, -9203, -9209, -9215, -9221, -9227, -9233, 
		-9239, -9245, -9250, -9256, -9262, -9268, -9274, -9279, -9285, -9291, -9296, -9302, -9308, -9313, -9319, -9324, -9330, -9335, -9341, -9346, -9352, -9357, -9363, -9368, -9373, -9379, -9384, -9389, -9395, -9400, -9405, -9410, -9415, -9421, -9426, -9431, -9436, -9441, -9446, -9451, -9456, -9461, -9466, -9471, -9476, -9481, -9486, -9490, -9495, -9500, -9505, -9510, -9514, -9519, -9524, -9528, -9533, -9538, -9542, -9547, -9551, -9556, -9560, -9565, 
		-9569, -9574, -9578, -9583, -9587, -9591, -9596, -9600, -9604, -9609, -9613, -9617, -9621, -9625, -9630, -9634, -9638, -9642, -9646, -9650, -9654, -9658, -9662, -9666, -9670, -9674, -9678, -9681, -9685, -9689, -9693, -9697, -9700, -9704, -9708, -9711, -9715, -9719, -9722, -9726, -9729, -9733, -9736, -9740, -9743, -9747, -9750, -9754, -9757, -9760, -9764, -9767, -9770, -9774, -9777, -9780, -9783, -9786, -9789, -9793, -9796, -9799, -9802, -9805, 
		-9808, -9811, -9814, -9817, -9820, -9823, -9825, -9828, -9831, -9834, -9837, -9839, -9842, -9845, -9847, -9850, -9853, -9855, -9858, -9861, -9863, -9866, -9868, -9871, -9873, -9875, -9878, -9880, -9883, -9885, -9887, -9890, -9892, -9894, -9896, -9898, -9901, -9903, -9905, -9907, -9909, -9911, -9913, -9915, -9917, -9919, -9921, -9923, -9925, -9927, -9929, -9930, -9932, -9934, -9936, -9937, -9939, -9941, -9942, -9944, -9946, -9947, -9949, -9950, 
		-9952, -9953, -9955, -9956, -9958, -9959, -9960, -9962, -9963, -9964, -9966, -9967, -9968, -9969, -9971, -9972, -9973, -9974, -9975, -9976, -9977, -9978, -9979, -9980, -9981, -9982, -9983, -9984, -9985, -9986, -9986, -9987, -9988, -9989, -9989, -9990, -9991, -9991, -9992, -9993, -9993, -9994, -9994, -9995, -9995, -9996, -9996, -9997, -9997, -9997, -9998, -9998, -9998, -9999, -9999, -9999, -9999, -9999, -10000, -10000, -10000, -10000, -10000, -10000, 
		-10000, -10000, -10000, -10000, -10000, -10000, -10000, -9999, -9999, -9999, -9999, -9999, -9998, -9998, -9998, -9997, -9997, -9997, -9996, -9996, -9995, -9995, -9994, -9994, -9993, -9993, -9992, -9991, -9991, -9990, -9989, -9989, -9988, -9987, -9986, -9986, -9985, -9984, -9983, -9982, -9981, -9980, -9979, -9978, -9977, -9976, -9975, -9974, -9973, -9972, -9971, -9969, -9968, -9967, -9966, -9964, -9963, -9962, -9960, -9959, -9958, -9956, -9955, -9953, 
		-9952, -9950, -9949, -9947, -9946, -9944, -9942, -9941, -9939, -9937, -9936, -9934, -9932, -9930, -9929, -9927, -9925, -9923, -9921, -9919, -9917, -9915, -9913, -9911, -9909, -9907, -9905, -9903, -9901, -9898, -9896, -9894, -9892, -9890, -9887, -9885, -9883, -9880, -9878, -9875, -9873, -9871, -9868, -9866, -9863, -9861, -9858, -9855, -9853, -9850, -9847, -9845, -9842, -9839, -9837, -9834, -9831, -9828, -9825, -9823, -9820, -9817, -9814, -9811, 
		-9808, -9805, -9802, -9799, -9796, -9793, -9789, -9786, -9783, -9780, -9777, -9774, -9770, -9767, -9764, -9760, -9757, -9754, -9750, -9747, -9743, -9740, -9736, -9733, -9729, -9726, -9722, -9719, -9715, -9711, -9708, -9704, -9700, -9697, -9693, -9689, -9685, -9681, -9678, -9674, -9670, -9666, -9662, -9658, -9654, -9650, -9646, -9642, -9638, -9634, -9630, -9625, -9621, -9617, -9613, -9609, -9604, -9600, -9596, -9591, -9587, -9583, -9578, -9574, 
		-9569, -9565, -9560, -9556, -9551, -9547, -9542, -9538, -9533, -9528, -9524, -9519, -9514, -9510, -9505, -9500, -9495, -9490, -9486, -9481, -9476, -9471, -9466, -9461, -9456, -9451, -9446, -9441, -9436, -9431, -9426, -9421, -9415, -9410, -9405, -9400, -9395, -9389, -9384, -9379, -9373, -9368, -9363, -9357, -9352, -9346, -9341, -9335, -9330, -9324, -9319, -9313, -9308, -9302, -9296, -9291, -9285, -9279, -9274, -9268, -9262, -9256, -9250, -9245, 
		-9239, -9233, -9227, -9221, -9215, -9209, -9203, -9197, -9191, -9185, -9179, -9173, -9167, -9161, -9154, -9148, -9142, -9136, -9130, -9123, -9117, -9111, -9104, -9098, -9092, -9085, -9079, -9072, -9066, -9059, -9053, -9046, -9040, -9033, -9027, -9020, -9013, -9007, -9000, -8993, -8987, -8980, -8973, -8966, -8960, -8953, -8946, -8939, -8932, -8925, -8918, -8911, -8904, -8897, -8890, -8883, -8876, -8869, -8862, -8855, -8848, -8841, -8834, -8826, 
		-8819, -8812, -8805, -8797, -8790, -8783, -8775, -8768, -8761, -8753, -8746, -8738, -8731, -8723, -8716, -8708, -8701, -8693, -8686, -8678, -8670, -8663, -8655, -8647, -8640, -8632, -8624, -8616, -8609, -8601, -8593, -8585, -8577, -8569, -8561, -8554, -8546, -8538, -8530, -8522, -8514, -8505, -8497, -8489, -8481, -8473, -8465, -8457, -8449, -8440, -8432, -8424, -8416, -8407, -8399, -8391, -8382, -8374, -8365, -8357, -8349, -8340, -8332, -8323, 
		-8315, -8306, -8298, -8289, -8280, -8272, -8263, -8255, -8246, -8237, -8228, -8220, -8211, -8202, -8193, -8185, -8176, -8167, -8158, -8149, -8140, -8131, -8123, -8114, -8105, -8096, -8087, -8078, -8068, -8059, -8050, -8041, -8032, -8023, -8014, -8005, -7995, -7986, -7977, -7968, -7958, -7949, -7940, -7930, -7921, -7912, -7902, -7893, -7883, -7874, -7865, -7855, -7846, -7836, -7827, -7817, -7807, -7798, -7788, -7779, -7769, -7759, -7750, -7740, 
		-7730, -7720, -7711, -7701, -7691, -7681, -7671, -7662, -7652, -7642, -7632, -7622, -7612, -7602, -7592, -7582, -7572, -7562, -7552, -7542, -7532, -7522, -7512, -7502, -7491, -7481, -7471, -7461, -7451, -7440, -7430, -7420, -7410, -7399, -7389, -7379, -7368, -7358, -7347, -7337, -7327, -7316, -7306, -7295, -7285, -7274, -7264, -7253, -7242, -7232, -7221, -7211, -7200, -7189, -7179, -7168, -7157, -7147, -7136, -7125, -7114, -7104, -7093, -7082, 
		-7071, -7060, -7049, -7038, -7028, -7017, -7006, -6995, -6984, -6973, -6962, -6951, -6940, -6929, -6918, -6907, -6895, -6884, -6873, -6862, -6851, -6840, -6828, -6817, -6806, -6795, -6784, -6772, -6761, -6750, -6738, -6727, -6716, -6704, -6693, -6681, -6670, -6659, -6647, -6636, -6624, -6613, -6601, -6590, -6578, -6567, -6555, -6543, -6532, -6520, -6508, -6497, -6485, -6473, -6462, -6450, -6438, -6427, -6415, -6403, -6391, -6379, -6368, -6356, 
		-6344, -6332, -6320, -6308, -6296, -6284, -6273, -6261, -6249, -6237, -6225, -6213, -6201, -6189, -6176, -6164, -6152, -6140, -6128, -6116, -6104, -6092, -6079, -6067, -6055, -6043, -6031, -6018, -6006, -5994, -5982, -5969, -5957, -5945, -5932, -5920, -5908, -5895, -5883, -5870, -5858, -5846, -5833, -5821, -5808, -5796, -5783, -5771, -5758, -5746, -5733, -5720, -5708, -5695, -5683, -5670, -5657, -5645, -5632, -5619, -5607, -5594, -5581, -5568, 
		-5556, -5543, -5530, -5517, -5505, -5492, -5479, -5466, -5453, -5440, -5428, -5415, -5402, -5389, -5376, -5363, -5350, -5337, -5324, -5311, -5298, -5285, -5272, -5259, -5246, -5233, -5220, -5207, -5194, -5180, -5167, -5154, -5141, -5128, -5115, -5102, -5088, -5075, -5062, -5049, -5035, -5022, -5009, -4996, -4982, -4969, -4956, -4942, -4929, -4916, -4902, -4889, -4876, -4862, -4849, -4835, -4822, -4808, -4795, -4781, -4768, -4755, -4741, -4727, 
		-4714, -4700, -4687, -4673, -4660, -4646, -4633, -4619, -4605, -4592, -4578, -4564, -4551, -4537, -4523, -4510, -4496, -4482, -4469, -4455, -4441, -4427, -4414, -4400, -4386, -4372, -4359, -4345, -4331, -4317, -4303, -4289, -4276, -4262, -4248, -4234, -4220, -4206, -4192, -4178, -4164, -4150, -4136, -4122, -4108, -4094, -4080, -4066, -4052, -4038, -4024, -4010, -3996, -3982, -3968, -3954, -3940, -3926, -3912, -3898, -3883, -3869, -3855, -3841, 
		-3827, -3813, -3798, -3784, -3770, -3756, -3742, -3727, -3713, -3699, -3685, -3670, -3656, -3642, -3628, -3613, -3599, -3585, -3570, -3556, -3542, -3527, -3513, -3499, -3484, -3470, -3455, -3441, -3427, -3412, -3398, -3383, -3369, -3354, -3340, -3326, -3311, -3297, -3282, -3268, -3253, -3239, -3224, -3210, -3195, -3180, -3166, -3151, -3137, -3122, -3108, -3093, -3078, -3064, -3049, -3035, -3020, -3005, -2991, -2976, -2962, -2947, -2932, -2918, 
		-2903, -2888, -2873, -2859, -2844, -2829, -2815, -2800, -2785, -2770, -2756, -2741, -2726, -2711, -2697, -2682, -2667, -2652, -2638, -2623, -2608, -2593, -2578, -2563, -2549, -2534, -2519, -2504, -2489, -2474, -2460, -2445, -2430, -2415, -2400, -2385, -2370, -2355, -2340, -2326, -2311, -2296, -2281, -2266, -2251, -2236, -2221, -2206, -2191, -2176, -2161, -2146, -2131, -2116, -2101, -2086, -2071, -2056, -2041, -2026, -2011, -1996, -1981, -1966, 
		-1951, -1936, -1921, -1906, -1891, -1876, -1861, -1845, -1830, -1815, -1800, -1785, -1770, -1755, -1740, -1725, -1710, -1695, -1679, -1664, -1649, -1634, -1619, -1604, -1589, -1573, -1558, -1543, -1528, -1513, -1498, -1482, -1467, -1452, -1437, -1422, -1407, -1391, -1376, -1361, -1346, -1331, -1315, -1300, -1285, -1270, -1255, -1239, -1224, -1209, -1194, -1178, -1163, -1148, -1133, -1117, -1102, -1087, -1072, -1056, -1041, -1026, -1011, -995, 
		-980, -965, -950, -934, -919, -904, -889, -873, -858, -843, -827, -812, -797, -782, -766, -751, -736, -720, -705, -690, -674, -659, -644, -629, -613, -598, -583, -567, -552, -537, -521, -506, -491, -475, -460, -445, -429, -414, -399, -383, -368, -353, -337, -322, -307, -291, -276, -261, -245, -230, -215, -199, -184, -169, -153, -138, -123, -107, -92, -77, -61, -46, -31, -15, 
	};
        #endregion

        #region Cos lookup table
        public static readonly int[] cos_table = new int[]
	{
		10000, 10000, 10000, 10000, 10000, 10000, 10000, 9999, 9999, 9999, 9999, 9999, 9998, 9998, 9998, 9997, 9997, 9997, 9996, 9996, 9995, 9995, 9994, 9994, 9993, 9993, 9992, 9991, 9991, 9990, 9989, 9989, 9988, 9987, 9986, 9986, 9985, 9984, 9983, 9982, 9981, 9980, 9979, 9978, 9977, 9976, 9975, 9974, 9973, 9972, 9971, 9969, 9968, 9967, 9966, 9964, 9963, 9962, 9960, 9959, 9958, 9956, 9955, 9953, 
		9952, 9950, 9949, 9947, 9946, 9944, 9942, 9941, 9939, 9937, 9936, 9934, 9932, 9930, 9929, 9927, 9925, 9923, 9921, 9919, 9917, 9915, 9913, 9911, 9909, 9907, 9905, 9903, 9901, 9898, 9896, 9894, 9892, 9890, 9887, 9885, 9883, 9880, 9878, 9875, 9873, 9871, 9868, 9866, 9863, 9861, 9858, 9855, 9853, 9850, 9847, 9845, 9842, 9839, 9837, 9834, 9831, 9828, 9825, 9823, 9820, 9817, 9814, 9811, 
		9808, 9805, 9802, 9799, 9796, 9793, 9789, 9786, 9783, 9780, 9777, 9774, 9770, 9767, 9764, 9760, 9757, 9754, 9750, 9747, 9743, 9740, 9736, 9733, 9729, 9726, 9722, 9719, 9715, 9711, 9708, 9704, 9700, 9697, 9693, 9689, 9685, 9681, 9678, 9674, 9670, 9666, 9662, 9658, 9654, 9650, 9646, 9642, 9638, 9634, 9630, 9625, 9621, 9617, 9613, 9609, 9604, 9600, 9596, 9591, 9587, 9583, 9578, 9574, 
		9569, 9565, 9560, 9556, 9551, 9547, 9542, 9538, 9533, 9528, 9524, 9519, 9514, 9510, 9505, 9500, 9495, 9490, 9486, 9481, 9476, 9471, 9466, 9461, 9456, 9451, 9446, 9441, 9436, 9431, 9426, 9421, 9415, 9410, 9405, 9400, 9395, 9389, 9384, 9379, 9373, 9368, 9363, 9357, 9352, 9346, 9341, 9335, 9330, 9324, 9319, 9313, 9308, 9302, 9296, 9291, 9285, 9279, 9274, 9268, 9262, 9256, 9250, 9245, 
		9239, 9233, 9227, 9221, 9215, 9209, 9203, 9197, 9191, 9185, 9179, 9173, 9167, 9161, 9154, 9148, 9142, 9136, 9130, 9123, 9117, 9111, 9104, 9098, 9092, 9085, 9079, 9072, 9066, 9059, 9053, 9046, 9040, 9033, 9027, 9020, 9013, 9007, 9000, 8993, 8987, 8980, 8973, 8966, 8960, 8953, 8946, 8939, 8932, 8925, 8918, 8911, 8904, 8897, 8890, 8883, 8876, 8869, 8862, 8855, 8848, 8841, 8834, 8826, 
		8819, 8812, 8805, 8797, 8790, 8783, 8775, 8768, 8761, 8753, 8746, 8738, 8731, 8723, 8716, 8708, 8701, 8693, 8686, 8678, 8670, 8663, 8655, 8647, 8640, 8632, 8624, 8616, 8609, 8601, 8593, 8585, 8577, 8569, 8561, 8554, 8546, 8538, 8530, 8522, 8514, 8505, 8497, 8489, 8481, 8473, 8465, 8457, 8449, 8440, 8432, 8424, 8416, 8407, 8399, 8391, 8382, 8374, 8365, 8357, 8349, 8340, 8332, 8323, 
		8315, 8306, 8298, 8289, 8280, 8272, 8263, 8255, 8246, 8237, 8228, 8220, 8211, 8202, 8193, 8185, 8176, 8167, 8158, 8149, 8140, 8131, 8123, 8114, 8105, 8096, 8087, 8078, 8068, 8059, 8050, 8041, 8032, 8023, 8014, 8005, 7995, 7986, 7977, 7968, 7958, 7949, 7940, 7930, 7921, 7912, 7902, 7893, 7883, 7874, 7865, 7855, 7846, 7836, 7827, 7817, 7807, 7798, 7788, 7779, 7769, 7759, 7750, 7740, 
		7730, 7720, 7711, 7701, 7691, 7681, 7671, 7662, 7652, 7642, 7632, 7622, 7612, 7602, 7592, 7582, 7572, 7562, 7552, 7542, 7532, 7522, 7512, 7502, 7491, 7481, 7471, 7461, 7451, 7440, 7430, 7420, 7410, 7399, 7389, 7379, 7368, 7358, 7347, 7337, 7327, 7316, 7306, 7295, 7285, 7274, 7264, 7253, 7242, 7232, 7221, 7211, 7200, 7189, 7179, 7168, 7157, 7147, 7136, 7125, 7114, 7104, 7093, 7082, 
		7071, 7060, 7049, 7038, 7028, 7017, 7006, 6995, 6984, 6973, 6962, 6951, 6940, 6929, 6918, 6907, 6895, 6884, 6873, 6862, 6851, 6840, 6828, 6817, 6806, 6795, 6784, 6772, 6761, 6750, 6738, 6727, 6716, 6704, 6693, 6681, 6670, 6659, 6647, 6636, 6624, 6613, 6601, 6590, 6578, 6567, 6555, 6543, 6532, 6520, 6508, 6497, 6485, 6473, 6462, 6450, 6438, 6427, 6415, 6403, 6391, 6379, 6368, 6356, 
		6344, 6332, 6320, 6308, 6296, 6284, 6273, 6261, 6249, 6237, 6225, 6213, 6201, 6189, 6176, 6164, 6152, 6140, 6128, 6116, 6104, 6092, 6079, 6067, 6055, 6043, 6031, 6018, 6006, 5994, 5982, 5969, 5957, 5945, 5932, 5920, 5908, 5895, 5883, 5870, 5858, 5846, 5833, 5821, 5808, 5796, 5783, 5771, 5758, 5746, 5733, 5720, 5708, 5695, 5683, 5670, 5657, 5645, 5632, 5619, 5607, 5594, 5581, 5568, 
		5556, 5543, 5530, 5517, 5505, 5492, 5479, 5466, 5453, 5440, 5428, 5415, 5402, 5389, 5376, 5363, 5350, 5337, 5324, 5311, 5298, 5285, 5272, 5259, 5246, 5233, 5220, 5207, 5194, 5180, 5167, 5154, 5141, 5128, 5115, 5102, 5088, 5075, 5062, 5049, 5035, 5022, 5009, 4996, 4982, 4969, 4956, 4942, 4929, 4916, 4902, 4889, 4876, 4862, 4849, 4835, 4822, 4808, 4795, 4781, 4768, 4755, 4741, 4727, 
		4714, 4700, 4687, 4673, 4660, 4646, 4633, 4619, 4605, 4592, 4578, 4564, 4551, 4537, 4523, 4510, 4496, 4482, 4469, 4455, 4441, 4427, 4414, 4400, 4386, 4372, 4359, 4345, 4331, 4317, 4303, 4289, 4276, 4262, 4248, 4234, 4220, 4206, 4192, 4178, 4164, 4150, 4136, 4122, 4108, 4094, 4080, 4066, 4052, 4038, 4024, 4010, 3996, 3982, 3968, 3954, 3940, 3926, 3912, 3898, 3883, 3869, 3855, 3841, 
		3827, 3813, 3798, 3784, 3770, 3756, 3742, 3727, 3713, 3699, 3685, 3670, 3656, 3642, 3628, 3613, 3599, 3585, 3570, 3556, 3542, 3527, 3513, 3499, 3484, 3470, 3455, 3441, 3427, 3412, 3398, 3383, 3369, 3354, 3340, 3326, 3311, 3297, 3282, 3268, 3253, 3239, 3224, 3210, 3195, 3180, 3166, 3151, 3137, 3122, 3108, 3093, 3078, 3064, 3049, 3035, 3020, 3005, 2991, 2976, 2962, 2947, 2932, 2918, 
		2903, 2888, 2873, 2859, 2844, 2829, 2815, 2800, 2785, 2770, 2756, 2741, 2726, 2711, 2697, 2682, 2667, 2652, 2638, 2623, 2608, 2593, 2578, 2563, 2549, 2534, 2519, 2504, 2489, 2474, 2460, 2445, 2430, 2415, 2400, 2385, 2370, 2355, 2340, 2326, 2311, 2296, 2281, 2266, 2251, 2236, 2221, 2206, 2191, 2176, 2161, 2146, 2131, 2116, 2101, 2086, 2071, 2056, 2041, 2026, 2011, 1996, 1981, 1966, 
		1951, 1936, 1921, 1906, 1891, 1876, 1861, 1845, 1830, 1815, 1800, 1785, 1770, 1755, 1740, 1725, 1710, 1695, 1679, 1664, 1649, 1634, 1619, 1604, 1589, 1573, 1558, 1543, 1528, 1513, 1498, 1482, 1467, 1452, 1437, 1422, 1407, 1391, 1376, 1361, 1346, 1331, 1315, 1300, 1285, 1270, 1255, 1239, 1224, 1209, 1194, 1178, 1163, 1148, 1133, 1117, 1102, 1087, 1072, 1056, 1041, 1026, 1011, 995, 
		980, 965, 950, 934, 919, 904, 889, 873, 858, 843, 827, 812, 797, 782, 766, 751, 736, 720, 705, 690, 674, 659, 644, 629, 613, 598, 583, 567, 552, 537, 521, 506, 491, 475, 460, 445, 429, 414, 399, 383, 368, 353, 337, 322, 307, 291, 276, 261, 245, 230, 215, 199, 184, 169, 153, 138, 123, 107, 92, 77, 61, 46, 31, 15, 
		0, -15, -31, -46, -61, -77, -92, -107, -123, -138, -153, -169, -184, -199, -215, -230, -245, -261, -276, -291, -307, -322, -337, -353, -368, -383, -399, -414, -429, -445, -460, -475, -491, -506, -521, -537, -552, -567, -583, -598, -613, -629, -644, -659, -674, -690, -705, -720, -736, -751, -766, -782, -797, -812, -827, -843, -858, -873, -889, -904, -919, -934, -950, -965, 
		-980, -995, -1011, -1026, -1041, -1056, -1072, -1087, -1102, -1117, -1133, -1148, -1163, -1178, -1194, -1209, -1224, -1239, -1255, -1270, -1285, -1300, -1315, -1331, -1346, -1361, -1376, -1391, -1407, -1422, -1437, -1452, -1467, -1482, -1498, -1513, -1528, -1543, -1558, -1573, -1589, -1604, -1619, -1634, -1649, -1664, -1679, -1695, -1710, -1725, -1740, -1755, -1770, -1785, -1800, -1815, -1830, -1845, -1861, -1876, -1891, -1906, -1921, -1936, 
		-1951, -1966, -1981, -1996, -2011, -2026, -2041, -2056, -2071, -2086, -2101, -2116, -2131, -2146, -2161, -2176, -2191, -2206, -2221, -2236, -2251, -2266, -2281, -2296, -2311, -2326, -2340, -2355, -2370, -2385, -2400, -2415, -2430, -2445, -2460, -2474, -2489, -2504, -2519, -2534, -2549, -2563, -2578, -2593, -2608, -2623, -2638, -2652, -2667, -2682, -2697, -2711, -2726, -2741, -2756, -2770, -2785, -2800, -2815, -2829, -2844, -2859, -2873, -2888, 
		-2903, -2918, -2932, -2947, -2962, -2976, -2991, -3005, -3020, -3035, -3049, -3064, -3078, -3093, -3108, -3122, -3137, -3151, -3166, -3180, -3195, -3210, -3224, -3239, -3253, -3268, -3282, -3297, -3311, -3326, -3340, -3354, -3369, -3383, -3398, -3412, -3427, -3441, -3455, -3470, -3484, -3499, -3513, -3527, -3542, -3556, -3570, -3585, -3599, -3613, -3628, -3642, -3656, -3670, -3685, -3699, -3713, -3727, -3742, -3756, -3770, -3784, -3798, -3813, 
		-3827, -3841, -3855, -3869, -3883, -3898, -3912, -3926, -3940, -3954, -3968, -3982, -3996, -4010, -4024, -4038, -4052, -4066, -4080, -4094, -4108, -4122, -4136, -4150, -4164, -4178, -4192, -4206, -4220, -4234, -4248, -4262, -4276, -4289, -4303, -4317, -4331, -4345, -4359, -4372, -4386, -4400, -4414, -4427, -4441, -4455, -4469, -4482, -4496, -4510, -4523, -4537, -4551, -4564, -4578, -4592, -4605, -4619, -4633, -4646, -4660, -4673, -4687, -4700, 
		-4714, -4727, -4741, -4755, -4768, -4781, -4795, -4808, -4822, -4835, -4849, -4862, -4876, -4889, -4902, -4916, -4929, -4942, -4956, -4969, -4982, -4996, -5009, -5022, -5035, -5049, -5062, -5075, -5088, -5102, -5115, -5128, -5141, -5154, -5167, -5180, -5194, -5207, -5220, -5233, -5246, -5259, -5272, -5285, -5298, -5311, -5324, -5337, -5350, -5363, -5376, -5389, -5402, -5415, -5428, -5440, -5453, -5466, -5479, -5492, -5505, -5517, -5530, -5543, 
		-5556, -5568, -5581, -5594, -5607, -5619, -5632, -5645, -5657, -5670, -5683, -5695, -5708, -5720, -5733, -5746, -5758, -5771, -5783, -5796, -5808, -5821, -5833, -5846, -5858, -5870, -5883, -5895, -5908, -5920, -5932, -5945, -5957, -5969, -5982, -5994, -6006, -6018, -6031, -6043, -6055, -6067, -6079, -6092, -6104, -6116, -6128, -6140, -6152, -6164, -6176, -6189, -6201, -6213, -6225, -6237, -6249, -6261, -6273, -6284, -6296, -6308, -6320, -6332, 
		-6344, -6356, -6368, -6379, -6391, -6403, -6415, -6427, -6438, -6450, -6462, -6473, -6485, -6497, -6508, -6520, -6532, -6543, -6555, -6567, -6578, -6590, -6601, -6613, -6624, -6636, -6647, -6659, -6670, -6681, -6693, -6704, -6716, -6727, -6738, -6750, -6761, -6772, -6784, -6795, -6806, -6817, -6828, -6840, -6851, -6862, -6873, -6884, -6895, -6907, -6918, -6929, -6940, -6951, -6962, -6973, -6984, -6995, -7006, -7017, -7028, -7038, -7049, -7060, 
		-7071, -7082, -7093, -7104, -7114, -7125, -7136, -7147, -7157, -7168, -7179, -7189, -7200, -7211, -7221, -7232, -7242, -7253, -7264, -7274, -7285, -7295, -7306, -7316, -7327, -7337, -7347, -7358, -7368, -7379, -7389, -7399, -7410, -7420, -7430, -7440, -7451, -7461, -7471, -7481, -7491, -7502, -7512, -7522, -7532, -7542, -7552, -7562, -7572, -7582, -7592, -7602, -7612, -7622, -7632, -7642, -7652, -7662, -7671, -7681, -7691, -7701, -7711, -7720, 
		-7730, -7740, -7750, -7759, -7769, -7779, -7788, -7798, -7807, -7817, -7827, -7836, -7846, -7855, -7865, -7874, -7883, -7893, -7902, -7912, -7921, -7930, -7940, -7949, -7958, -7968, -7977, -7986, -7995, -8005, -8014, -8023, -8032, -8041, -8050, -8059, -8068, -8078, -8087, -8096, -8105, -8114, -8123, -8131, -8140, -8149, -8158, -8167, -8176, -8185, -8193, -8202, -8211, -8220, -8228, -8237, -8246, -8255, -8263, -8272, -8280, -8289, -8298, -8306, 
		-8315, -8323, -8332, -8340, -8349, -8357, -8365, -8374, -8382, -8391, -8399, -8407, -8416, -8424, -8432, -8440, -8449, -8457, -8465, -8473, -8481, -8489, -8497, -8505, -8514, -8522, -8530, -8538, -8546, -8554, -8561, -8569, -8577, -8585, -8593, -8601, -8609, -8616, -8624, -8632, -8640, -8647, -8655, -8663, -8670, -8678, -8686, -8693, -8701, -8708, -8716, -8723, -8731, -8738, -8746, -8753, -8761, -8768, -8775, -8783, -8790, -8797, -8805, -8812, 
		-8819, -8826, -8834, -8841, -8848, -8855, -8862, -8869, -8876, -8883, -8890, -8897, -8904, -8911, -8918, -8925, -8932, -8939, -8946, -8953, -8960, -8966, -8973, -8980, -8987, -8993, -9000, -9007, -9013, -9020, -9027, -9033, -9040, -9046, -9053, -9059, -9066, -9072, -9079, -9085, -9092, -9098, -9104, -9111, -9117, -9123, -9130, -9136, -9142, -9148, -9154, -9161, -9167, -9173, -9179, -9185, -9191, -9197, -9203, -9209, -9215, -9221, -9227, -9233, 
		-9239, -9245, -9250, -9256, -9262, -9268, -9274, -9279, -9285, -9291, -9296, -9302, -9308, -9313, -9319, -9324, -9330, -9335, -9341, -9346, -9352, -9357, -9363, -9368, -9373, -9379, -9384, -9389, -9395, -9400, -9405, -9410, -9415, -9421, -9426, -9431, -9436, -9441, -9446, -9451, -9456, -9461, -9466, -9471, -9476, -9481, -9486, -9490, -9495, -9500, -9505, -9510, -9514, -9519, -9524, -9528, -9533, -9538, -9542, -9547, -9551, -9556, -9560, -9565, 
		-9569, -9574, -9578, -9583, -9587, -9591, -9596, -9600, -9604, -9609, -9613, -9617, -9621, -9625, -9630, -9634, -9638, -9642, -9646, -9650, -9654, -9658, -9662, -9666, -9670, -9674, -9678, -9681, -9685, -9689, -9693, -9697, -9700, -9704, -9708, -9711, -9715, -9719, -9722, -9726, -9729, -9733, -9736, -9740, -9743, -9747, -9750, -9754, -9757, -9760, -9764, -9767, -9770, -9774, -9777, -9780, -9783, -9786, -9789, -9793, -9796, -9799, -9802, -9805, 
		-9808, -9811, -9814, -9817, -9820, -9823, -9825, -9828, -9831, -9834, -9837, -9839, -9842, -9845, -9847, -9850, -9853, -9855, -9858, -9861, -9863, -9866, -9868, -9871, -9873, -9875, -9878, -9880, -9883, -9885, -9887, -9890, -9892, -9894, -9896, -9898, -9901, -9903, -9905, -9907, -9909, -9911, -9913, -9915, -9917, -9919, -9921, -9923, -9925, -9927, -9929, -9930, -9932, -9934, -9936, -9937, -9939, -9941, -9942, -9944, -9946, -9947, -9949, -9950, 
		-9952, -9953, -9955, -9956, -9958, -9959, -9960, -9962, -9963, -9964, -9966, -9967, -9968, -9969, -9971, -9972, -9973, -9974, -9975, -9976, -9977, -9978, -9979, -9980, -9981, -9982, -9983, -9984, -9985, -9986, -9986, -9987, -9988, -9989, -9989, -9990, -9991, -9991, -9992, -9993, -9993, -9994, -9994, -9995, -9995, -9996, -9996, -9997, -9997, -9997, -9998, -9998, -9998, -9999, -9999, -9999, -9999, -9999, -10000, -10000, -10000, -10000, -10000, -10000, 
		-10000, -10000, -10000, -10000, -10000, -10000, -10000, -9999, -9999, -9999, -9999, -9999, -9998, -9998, -9998, -9997, -9997, -9997, -9996, -9996, -9995, -9995, -9994, -9994, -9993, -9993, -9992, -9991, -9991, -9990, -9989, -9989, -9988, -9987, -9986, -9986, -9985, -9984, -9983, -9982, -9981, -9980, -9979, -9978, -9977, -9976, -9975, -9974, -9973, -9972, -9971, -9969, -9968, -9967, -9966, -9964, -9963, -9962, -9960, -9959, -9958, -9956, -9955, -9953, 
		-9952, -9950, -9949, -9947, -9946, -9944, -9942, -9941, -9939, -9937, -9936, -9934, -9932, -9930, -9929, -9927, -9925, -9923, -9921, -9919, -9917, -9915, -9913, -9911, -9909, -9907, -9905, -9903, -9901, -9898, -9896, -9894, -9892, -9890, -9887, -9885, -9883, -9880, -9878, -9875, -9873, -9871, -9868, -9866, -9863, -9861, -9858, -9855, -9853, -9850, -9847, -9845, -9842, -9839, -9837, -9834, -9831, -9828, -9825, -9823, -9820, -9817, -9814, -9811, 
		-9808, -9805, -9802, -9799, -9796, -9793, -9789, -9786, -9783, -9780, -9777, -9774, -9770, -9767, -9764, -9760, -9757, -9754, -9750, -9747, -9743, -9740, -9736, -9733, -9729, -9726, -9722, -9719, -9715, -9711, -9708, -9704, -9700, -9697, -9693, -9689, -9685, -9681, -9678, -9674, -9670, -9666, -9662, -9658, -9654, -9650, -9646, -9642, -9638, -9634, -9630, -9625, -9621, -9617, -9613, -9609, -9604, -9600, -9596, -9591, -9587, -9583, -9578, -9574, 
		-9569, -9565, -9560, -9556, -9551, -9547, -9542, -9538, -9533, -9528, -9524, -9519, -9514, -9510, -9505, -9500, -9495, -9490, -9486, -9481, -9476, -9471, -9466, -9461, -9456, -9451, -9446, -9441, -9436, -9431, -9426, -9421, -9415, -9410, -9405, -9400, -9395, -9389, -9384, -9379, -9373, -9368, -9363, -9357, -9352, -9346, -9341, -9335, -9330, -9324, -9319, -9313, -9308, -9302, -9296, -9291, -9285, -9279, -9274, -9268, -9262, -9256, -9250, -9245, 
		-9239, -9233, -9227, -9221, -9215, -9209, -9203, -9197, -9191, -9185, -9179, -9173, -9167, -9161, -9154, -9148, -9142, -9136, -9130, -9123, -9117, -9111, -9104, -9098, -9092, -9085, -9079, -9072, -9066, -9059, -9053, -9046, -9040, -9033, -9027, -9020, -9013, -9007, -9000, -8993, -8987, -8980, -8973, -8966, -8960, -8953, -8946, -8939, -8932, -8925, -8918, -8911, -8904, -8897, -8890, -8883, -8876, -8869, -8862, -8855, -8848, -8841, -8834, -8826, 
		-8819, -8812, -8805, -8797, -8790, -8783, -8775, -8768, -8761, -8753, -8746, -8738, -8731, -8723, -8716, -8708, -8701, -8693, -8686, -8678, -8670, -8663, -8655, -8647, -8640, -8632, -8624, -8616, -8609, -8601, -8593, -8585, -8577, -8569, -8561, -8554, -8546, -8538, -8530, -8522, -8514, -8505, -8497, -8489, -8481, -8473, -8465, -8457, -8449, -8440, -8432, -8424, -8416, -8407, -8399, -8391, -8382, -8374, -8365, -8357, -8349, -8340, -8332, -8323, 
		-8315, -8306, -8298, -8289, -8280, -8272, -8263, -8255, -8246, -8237, -8228, -8220, -8211, -8202, -8193, -8185, -8176, -8167, -8158, -8149, -8140, -8131, -8123, -8114, -8105, -8096, -8087, -8078, -8068, -8059, -8050, -8041, -8032, -8023, -8014, -8005, -7995, -7986, -7977, -7968, -7958, -7949, -7940, -7930, -7921, -7912, -7902, -7893, -7883, -7874, -7865, -7855, -7846, -7836, -7827, -7817, -7807, -7798, -7788, -7779, -7769, -7759, -7750, -7740, 
		-7730, -7720, -7711, -7701, -7691, -7681, -7671, -7662, -7652, -7642, -7632, -7622, -7612, -7602, -7592, -7582, -7572, -7562, -7552, -7542, -7532, -7522, -7512, -7502, -7491, -7481, -7471, -7461, -7451, -7440, -7430, -7420, -7410, -7399, -7389, -7379, -7368, -7358, -7347, -7337, -7327, -7316, -7306, -7295, -7285, -7274, -7264, -7253, -7242, -7232, -7221, -7211, -7200, -7189, -7179, -7168, -7157, -7147, -7136, -7125, -7114, -7104, -7093, -7082, 
		-7071, -7060, -7049, -7038, -7028, -7017, -7006, -6995, -6984, -6973, -6962, -6951, -6940, -6929, -6918, -6907, -6895, -6884, -6873, -6862, -6851, -6840, -6828, -6817, -6806, -6795, -6784, -6772, -6761, -6750, -6738, -6727, -6716, -6704, -6693, -6681, -6670, -6659, -6647, -6636, -6624, -6613, -6601, -6590, -6578, -6567, -6555, -6543, -6532, -6520, -6508, -6497, -6485, -6473, -6462, -6450, -6438, -6427, -6415, -6403, -6391, -6379, -6368, -6356, 
		-6344, -6332, -6320, -6308, -6296, -6284, -6273, -6261, -6249, -6237, -6225, -6213, -6201, -6189, -6176, -6164, -6152, -6140, -6128, -6116, -6104, -6092, -6079, -6067, -6055, -6043, -6031, -6018, -6006, -5994, -5982, -5969, -5957, -5945, -5932, -5920, -5908, -5895, -5883, -5870, -5858, -5846, -5833, -5821, -5808, -5796, -5783, -5771, -5758, -5746, -5733, -5720, -5708, -5695, -5683, -5670, -5657, -5645, -5632, -5619, -5607, -5594, -5581, -5568, 
		-5556, -5543, -5530, -5517, -5505, -5492, -5479, -5466, -5453, -5440, -5428, -5415, -5402, -5389, -5376, -5363, -5350, -5337, -5324, -5311, -5298, -5285, -5272, -5259, -5246, -5233, -5220, -5207, -5194, -5180, -5167, -5154, -5141, -5128, -5115, -5102, -5088, -5075, -5062, -5049, -5035, -5022, -5009, -4996, -4982, -4969, -4956, -4942, -4929, -4916, -4902, -4889, -4876, -4862, -4849, -4835, -4822, -4808, -4795, -4781, -4768, -4755, -4741, -4727, 
		-4714, -4700, -4687, -4673, -4660, -4646, -4633, -4619, -4605, -4592, -4578, -4564, -4551, -4537, -4523, -4510, -4496, -4482, -4469, -4455, -4441, -4427, -4414, -4400, -4386, -4372, -4359, -4345, -4331, -4317, -4303, -4289, -4276, -4262, -4248, -4234, -4220, -4206, -4192, -4178, -4164, -4150, -4136, -4122, -4108, -4094, -4080, -4066, -4052, -4038, -4024, -4010, -3996, -3982, -3968, -3954, -3940, -3926, -3912, -3898, -3883, -3869, -3855, -3841, 
		-3827, -3813, -3798, -3784, -3770, -3756, -3742, -3727, -3713, -3699, -3685, -3670, -3656, -3642, -3628, -3613, -3599, -3585, -3570, -3556, -3542, -3527, -3513, -3499, -3484, -3470, -3455, -3441, -3427, -3412, -3398, -3383, -3369, -3354, -3340, -3326, -3311, -3297, -3282, -3268, -3253, -3239, -3224, -3210, -3195, -3180, -3166, -3151, -3137, -3122, -3108, -3093, -3078, -3064, -3049, -3035, -3020, -3005, -2991, -2976, -2962, -2947, -2932, -2918, 
		-2903, -2888, -2873, -2859, -2844, -2829, -2815, -2800, -2785, -2770, -2756, -2741, -2726, -2711, -2697, -2682, -2667, -2652, -2638, -2623, -2608, -2593, -2578, -2563, -2549, -2534, -2519, -2504, -2489, -2474, -2460, -2445, -2430, -2415, -2400, -2385, -2370, -2355, -2340, -2326, -2311, -2296, -2281, -2266, -2251, -2236, -2221, -2206, -2191, -2176, -2161, -2146, -2131, -2116, -2101, -2086, -2071, -2056, -2041, -2026, -2011, -1996, -1981, -1966, 
		-1951, -1936, -1921, -1906, -1891, -1876, -1861, -1845, -1830, -1815, -1800, -1785, -1770, -1755, -1740, -1725, -1710, -1695, -1679, -1664, -1649, -1634, -1619, -1604, -1589, -1573, -1558, -1543, -1528, -1513, -1498, -1482, -1467, -1452, -1437, -1422, -1407, -1391, -1376, -1361, -1346, -1331, -1315, -1300, -1285, -1270, -1255, -1239, -1224, -1209, -1194, -1178, -1163, -1148, -1133, -1117, -1102, -1087, -1072, -1056, -1041, -1026, -1011, -995, 
		-980, -965, -950, -934, -919, -904, -889, -873, -858, -843, -827, -812, -797, -782, -766, -751, -736, -720, -705, -690, -674, -659, -644, -629, -613, -598, -583, -567, -552, -537, -521, -506, -491, -475, -460, -445, -429, -414, -399, -383, -368, -353, -337, -322, -307, -291, -276, -261, -245, -230, -215, -199, -184, -169, -153, -138, -123, -107, -92, -77, -61, -46, -31, -15, 
		0, 15, 31, 46, 61, 77, 92, 107, 123, 138, 153, 169, 184, 199, 215, 230, 245, 261, 276, 291, 307, 322, 337, 353, 368, 383, 399, 414, 429, 445, 460, 475, 491, 506, 521, 537, 552, 567, 583, 598, 613, 629, 644, 659, 674, 690, 705, 720, 736, 751, 766, 782, 797, 812, 827, 843, 858, 873, 889, 904, 919, 934, 950, 965, 
		980, 995, 1011, 1026, 1041, 1056, 1072, 1087, 1102, 1117, 1133, 1148, 1163, 1178, 1194, 1209, 1224, 1239, 1255, 1270, 1285, 1300, 1315, 1331, 1346, 1361, 1376, 1391, 1407, 1422, 1437, 1452, 1467, 1482, 1498, 1513, 1528, 1543, 1558, 1573, 1589, 1604, 1619, 1634, 1649, 1664, 1679, 1695, 1710, 1725, 1740, 1755, 1770, 1785, 1800, 1815, 1830, 1845, 1861, 1876, 1891, 1906, 1921, 1936, 
		1951, 1966, 1981, 1996, 2011, 2026, 2041, 2056, 2071, 2086, 2101, 2116, 2131, 2146, 2161, 2176, 2191, 2206, 2221, 2236, 2251, 2266, 2281, 2296, 2311, 2326, 2340, 2355, 2370, 2385, 2400, 2415, 2430, 2445, 2460, 2474, 2489, 2504, 2519, 2534, 2549, 2563, 2578, 2593, 2608, 2623, 2638, 2652, 2667, 2682, 2697, 2711, 2726, 2741, 2756, 2770, 2785, 2800, 2815, 2829, 2844, 2859, 2873, 2888, 
		2903, 2918, 2932, 2947, 2962, 2976, 2991, 3005, 3020, 3035, 3049, 3064, 3078, 3093, 3108, 3122, 3137, 3151, 3166, 3180, 3195, 3210, 3224, 3239, 3253, 3268, 3282, 3297, 3311, 3326, 3340, 3354, 3369, 3383, 3398, 3412, 3427, 3441, 3455, 3470, 3484, 3499, 3513, 3527, 3542, 3556, 3570, 3585, 3599, 3613, 3628, 3642, 3656, 3670, 3685, 3699, 3713, 3727, 3742, 3756, 3770, 3784, 3798, 3813, 
		3827, 3841, 3855, 3869, 3883, 3898, 3912, 3926, 3940, 3954, 3968, 3982, 3996, 4010, 4024, 4038, 4052, 4066, 4080, 4094, 4108, 4122, 4136, 4150, 4164, 4178, 4192, 4206, 4220, 4234, 4248, 4262, 4276, 4289, 4303, 4317, 4331, 4345, 4359, 4372, 4386, 4400, 4414, 4427, 4441, 4455, 4469, 4482, 4496, 4510, 4523, 4537, 4551, 4564, 4578, 4592, 4605, 4619, 4633, 4646, 4660, 4673, 4687, 4700, 
		4714, 4727, 4741, 4755, 4768, 4781, 4795, 4808, 4822, 4835, 4849, 4862, 4876, 4889, 4902, 4916, 4929, 4942, 4956, 4969, 4982, 4996, 5009, 5022, 5035, 5049, 5062, 5075, 5088, 5102, 5115, 5128, 5141, 5154, 5167, 5180, 5194, 5207, 5220, 5233, 5246, 5259, 5272, 5285, 5298, 5311, 5324, 5337, 5350, 5363, 5376, 5389, 5402, 5415, 5428, 5440, 5453, 5466, 5479, 5492, 5505, 5517, 5530, 5543, 
		5556, 5568, 5581, 5594, 5607, 5619, 5632, 5645, 5657, 5670, 5683, 5695, 5708, 5720, 5733, 5746, 5758, 5771, 5783, 5796, 5808, 5821, 5833, 5846, 5858, 5870, 5883, 5895, 5908, 5920, 5932, 5945, 5957, 5969, 5982, 5994, 6006, 6018, 6031, 6043, 6055, 6067, 6079, 6092, 6104, 6116, 6128, 6140, 6152, 6164, 6176, 6189, 6201, 6213, 6225, 6237, 6249, 6261, 6273, 6284, 6296, 6308, 6320, 6332, 
		6344, 6356, 6368, 6379, 6391, 6403, 6415, 6427, 6438, 6450, 6462, 6473, 6485, 6497, 6508, 6520, 6532, 6543, 6555, 6567, 6578, 6590, 6601, 6613, 6624, 6636, 6647, 6659, 6670, 6681, 6693, 6704, 6716, 6727, 6738, 6750, 6761, 6772, 6784, 6795, 6806, 6817, 6828, 6840, 6851, 6862, 6873, 6884, 6895, 6907, 6918, 6929, 6940, 6951, 6962, 6973, 6984, 6995, 7006, 7017, 7028, 7038, 7049, 7060, 
		7071, 7082, 7093, 7104, 7114, 7125, 7136, 7147, 7157, 7168, 7179, 7189, 7200, 7211, 7221, 7232, 7242, 7253, 7264, 7274, 7285, 7295, 7306, 7316, 7327, 7337, 7347, 7358, 7368, 7379, 7389, 7399, 7410, 7420, 7430, 7440, 7451, 7461, 7471, 7481, 7491, 7502, 7512, 7522, 7532, 7542, 7552, 7562, 7572, 7582, 7592, 7602, 7612, 7622, 7632, 7642, 7652, 7662, 7671, 7681, 7691, 7701, 7711, 7720, 
		7730, 7740, 7750, 7759, 7769, 7779, 7788, 7798, 7807, 7817, 7827, 7836, 7846, 7855, 7865, 7874, 7883, 7893, 7902, 7912, 7921, 7930, 7940, 7949, 7958, 7968, 7977, 7986, 7995, 8005, 8014, 8023, 8032, 8041, 8050, 8059, 8068, 8078, 8087, 8096, 8105, 8114, 8123, 8131, 8140, 8149, 8158, 8167, 8176, 8185, 8193, 8202, 8211, 8220, 8228, 8237, 8246, 8255, 8263, 8272, 8280, 8289, 8298, 8306, 
		8315, 8323, 8332, 8340, 8349, 8357, 8365, 8374, 8382, 8391, 8399, 8407, 8416, 8424, 8432, 8440, 8449, 8457, 8465, 8473, 8481, 8489, 8497, 8505, 8514, 8522, 8530, 8538, 8546, 8554, 8561, 8569, 8577, 8585, 8593, 8601, 8609, 8616, 8624, 8632, 8640, 8647, 8655, 8663, 8670, 8678, 8686, 8693, 8701, 8708, 8716, 8723, 8731, 8738, 8746, 8753, 8761, 8768, 8775, 8783, 8790, 8797, 8805, 8812, 
		8819, 8826, 8834, 8841, 8848, 8855, 8862, 8869, 8876, 8883, 8890, 8897, 8904, 8911, 8918, 8925, 8932, 8939, 8946, 8953, 8960, 8966, 8973, 8980, 8987, 8993, 9000, 9007, 9013, 9020, 9027, 9033, 9040, 9046, 9053, 9059, 9066, 9072, 9079, 9085, 9092, 9098, 9104, 9111, 9117, 9123, 9130, 9136, 9142, 9148, 9154, 9161, 9167, 9173, 9179, 9185, 9191, 9197, 9203, 9209, 9215, 9221, 9227, 9233, 
		9239, 9245, 9250, 9256, 9262, 9268, 9274, 9279, 9285, 9291, 9296, 9302, 9308, 9313, 9319, 9324, 9330, 9335, 9341, 9346, 9352, 9357, 9363, 9368, 9373, 9379, 9384, 9389, 9395, 9400, 9405, 9410, 9415, 9421, 9426, 9431, 9436, 9441, 9446, 9451, 9456, 9461, 9466, 9471, 9476, 9481, 9486, 9490, 9495, 9500, 9505, 9510, 9514, 9519, 9524, 9528, 9533, 9538, 9542, 9547, 9551, 9556, 9560, 9565, 
		9569, 9574, 9578, 9583, 9587, 9591, 9596, 9600, 9604, 9609, 9613, 9617, 9621, 9625, 9630, 9634, 9638, 9642, 9646, 9650, 9654, 9658, 9662, 9666, 9670, 9674, 9678, 9681, 9685, 9689, 9693, 9697, 9700, 9704, 9708, 9711, 9715, 9719, 9722, 9726, 9729, 9733, 9736, 9740, 9743, 9747, 9750, 9754, 9757, 9760, 9764, 9767, 9770, 9774, 9777, 9780, 9783, 9786, 9789, 9793, 9796, 9799, 9802, 9805, 
		9808, 9811, 9814, 9817, 9820, 9823, 9825, 9828, 9831, 9834, 9837, 9839, 9842, 9845, 9847, 9850, 9853, 9855, 9858, 9861, 9863, 9866, 9868, 9871, 9873, 9875, 9878, 9880, 9883, 9885, 9887, 9890, 9892, 9894, 9896, 9898, 9901, 9903, 9905, 9907, 9909, 9911, 9913, 9915, 9917, 9919, 9921, 9923, 9925, 9927, 9929, 9930, 9932, 9934, 9936, 9937, 9939, 9941, 9942, 9944, 9946, 9947, 9949, 9950, 
		9952, 9953, 9955, 9956, 9958, 9959, 9960, 9962, 9963, 9964, 9966, 9967, 9968, 9969, 9971, 9972, 9973, 9974, 9975, 9976, 9977, 9978, 9979, 9980, 9981, 9982, 9983, 9984, 9985, 9986, 9986, 9987, 9988, 9989, 9989, 9990, 9991, 9991, 9992, 9993, 9993, 9994, 9994, 9995, 9995, 9996, 9996, 9997, 9997, 9997, 9998, 9998, 9998, 9999, 9999, 9999, 9999, 9999, 10000, 10000, 10000, 10000, 10000, 10000, 
	};
        #endregion

#if UNITY_EDITOR
    public static string GenerateString(bool sin)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("\tpublic static readonly int[] {0}_table = new int[]", sin ? "sin" : "cos");
        sb.AppendLine();
        sb.AppendLine("\t{");
        {
            int numElementPerLine = 64;
            double rad = Math.PI * 2.0;

            sb.Append("\t\t");

            int num = 0;
            for (int i = 0; i < COUNT; ++i)
            {
                double angle = (double)i / (double)COUNT * rad;

                double value = sin ? Math.Sin(angle) : Math.Cos(angle);

                int element = (int)Math.Round(value * FACTOR);

                sb.Append(element);
                sb.Append(", ");

                if (++num >= numElementPerLine && i < COUNT)
                {
                    num = 0;
                    sb.AppendLine();
                    if (i != COUNT)
                    {
                        sb.Append("\t\t");
                    }
                }
            }
        }
        sb.AppendLine();
        sb.AppendLine("\t};");
        sb.AppendLine();

        return sb.ToString();
    }
#endif

        public static int getIndex(long nom, long den)
        {
            nom *= NOM_MUL;
            den *= 62832;

            int index = (int)(nom / den);
            index &= MASK;

            return index;
        }

        static SinCosLookupTable()
        {
         //   DebugHelper.Assert(sin_table.Length == COUNT);
         //   DebugHelper.Assert(cos_table.Length == COUNT);
        }
    }
}