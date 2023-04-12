using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransformByHand
{
    public class Mesh
    {
        public Vector3[] vertex;
        public int[] tris;
        public Vector3[] normal;
        public Color color;
    }

    public class Quad : Mesh
    {
        public Vector3 scale;
        public Vector3 position;
        public List<Quad> children = new List<Quad>();

        public Quad(Vector3 position, Vector3 scale)
        {
            this.position = position;
            this.scale = scale;
            vertex = new Vector3[4]{
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(0,1,0),
        };
            tris = new int[6]{
            0,3,1,
            3,2,1
        };

            Transform.Scale(ref vertex, scale);
            Transform.Translate(ref vertex, position);
        }

        public void Translate(Vector3 pos)
        {
            position = pos;
            Transform.Translate(ref vertex, pos);
            foreach (Quad q in children)
            {
                q.Translate(pos);
            }
        }
    }

    public class Transform
    {
        const float DEG2RAD = (3.1415265f / 180.0f);
        public static void Translate(ref Vector3[] p, Vector3 d)
        {
            for (int i = 0; i < p.Length; i++)
            {
                p[i] += d;
            }
        }

        public static void Scale(ref Vector3[] p, Vector3 d)
        {
            for (var i = 0; i < p.Length; i++)
            {
                float x = Vector3.Dot(new Vector3(d.x, 0, 0), p[i]);
                float y = Vector3.Dot(new Vector3(0, d.y, 0), p[i]);
                float z = Vector3.Dot(new Vector3(d.x, 0, d.z), p[i]);
                p[i] = new Vector3(x, y, z);
            }
        }

        public static void RotateX(ref Vector3[] p, float ang)
        {
            float a = ang * DEG2RAD;
            for (int i = 0; i < p.Length; i++)
            {
                float x = Vector3.Dot(new Vector3(1, 0, 0), p[i]);
                float y = Vector3.Dot(new Vector3(0, Mathf.Cos(a), -Mathf.Sin(a)), p[i]);
                float z = Vector3.Dot(new Vector3(0, Mathf.Cos(a), Mathf.Sin(a)), p[i]);
                p[i] = new Vector3(x, y, z);
            }
        }
    }

    public class Buffer
    {
        Texture2D tex;

        public Buffer(ref Texture2D tex)
        {
            this.tex = tex;
        }

        public void Clear(Color color)
        {
            for (int h = 0; h < tex.height; h++)
            {
                for (int w = 0; w < tex.width; w++)
                {
                    tex.SetPixel(w, h, color);
                }
            }
        }

        private int Clamp(int v, int min, int max)
        {
            return (v < min) ? min : (v > max) ? max : v;
        }

        private void LineDDA(Vector3 p1, Vector3 p2, Color color)
        {
            Vector3 delta = p2 - p1;
            Vector3 ponto = p1;
            float passo = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
            if (passo != 0) delta = delta / passo;
            for (int i = 0; i <= passo; i++)
            {
                tex.SetPixel((int)ponto.x, (int)ponto.y, color);
                ponto = ponto + delta;
            }
            tex.Apply();
        }

        public void DrawMesh(Mesh m)
        {
            for (int i = 0, n = 0; i < (m.tris.Length - 1); i += 3, n++)
            {
                LineDDA(m.vertex[m.tris[i]], m.vertex[m.tris[i + 1]], m.color);
                LineDDA(m.vertex[m.tris[i + 1]], m.vertex[m.tris[i + 2]], m.color);
                LineDDA(m.vertex[m.tris[i + 2]], m.vertex[m.tris[i]], m.color);
            }
        }
    }

    public class TransformHierarquia : MonoBehaviour
    {
        Texture2D tex;
        Buffer buffer;
        private void Start()
        {
            Renderer rend = GetComponent<Renderer>();
            tex = new Texture2D(200, 200);
            tex.filterMode = FilterMode.Point;
            rend.material.mainTexture = tex;
            buffer = new Buffer(ref tex);
            StartCoroutine("Loop");
        }

        IEnumerator Loop()
        {
            Quad q1 = new Quad(new Vector3(50, 50, 50), new Vector3(100, 100, 100));
            q1.color = Color.red;
            Quad q2 = new Quad(new Vector3(20, 20, 20), new Vector3(50, 50, 50));
            q2.color = Color.blue;
            q1.children.Add(q2);
            while (true)
            {
                buffer.Clear(Color.white);
                buffer.DrawMesh(q1);
                q1.Translate(Vector3.right);
                buffer.DrawMesh(q2);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}