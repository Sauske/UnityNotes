using System;
using System.Text;
using UnityEngine;

namespace UMI.FrameCommand
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
}