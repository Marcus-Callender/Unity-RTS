using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexUtils
{
    [Serializable]
    public struct Cube
    {
        public float x;
        public float y;
        public float z;

        public Cube(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }

    [Serializable]
    public struct Hex
    {
        public float q;
        public float r;

        public Hex(float _q, float _r)
        {
            q = _q;
            r = _r;
        }
    }

    public static class HexUtils
    {
        const float size = 0.5f;

        public static Hex cube_to_axial(Cube cube)
        {
            float q = cube.x;
            float r = cube.z;
            return new Hex(q, r);
        }

        public static Cube axial_to_cube(Hex hex)
        {
            var x = hex.q;
            var z = hex.r;
            var y = -x - z;
            return new Cube(x, y, z);
        }

        public static Hex cube_to_oddr(Cube cube)
        {
            var col = cube.x + (cube.z - Mathf.RoundToInt(Mathf.Abs(cube.z) % 2.0f)) / 2;
            var row = cube.z;
            return new Hex(col, row);
        }

        public static Cube oddr_to_cube(Hex hex)
        {
            var x = hex.q - (hex.r - Mathf.RoundToInt(Mathf.Abs(hex.r) % 2.0f)) / 2;
            var z = hex.r;
            var y = -x - z;
            return new Cube(x, y, z);
        }

        public static Hex pofloaty_hex_to_pixel(Hex hex)
        {
            float x = size * (Mathf.Sqrt(3.0f) * hex.q + Mathf.Sqrt(3.0f) / 2.0f * hex.r);
            float y = size * (3.0f/ 2.0f * hex.r);
            return new Hex(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }

        public static Hex pixel_to_pointy_hex(Vector2 vec)
        {
            var q = Mathf.RoundToInt((Mathf.Sqrt(3.0f) / 3.0f * vec.x - 1.0f/ 3.0f * vec.y) / size);
            var r = Mathf.RoundToInt((2.0f/ 3.0f * vec.y) / size);
            return hex_round(new Hex(q, r));
        }

        public static Cube cube_round(Cube cube)
        {
            float rx = Mathf.RoundToInt(cube.x);
            float ry = Mathf.RoundToInt(cube.y);
            float rz = Mathf.RoundToInt(cube.z);

            float x_diff = Mathf.Abs(rx - cube.x);
            float y_diff = Mathf.Abs(ry - cube.y);
            float z_diff = Mathf.Abs(rz - cube.z);

            if (x_diff > y_diff && x_diff > z_diff)
                rx = -ry - rz;
            else if (y_diff > z_diff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Cube(rx, ry, rz);
        }

        public static Hex hex_round(Hex hex)
        {
            return cube_to_axial(cube_round(axial_to_cube(hex)));
        }
    }
}
