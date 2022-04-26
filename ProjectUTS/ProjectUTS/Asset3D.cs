using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafkom1
{
    internal class Asset3D
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();


        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;
        Matrix4 _model; //Ngerubah Transformasi
        public Vector3 _centerPosition;
        public List<Vector3> _euler;
        public List<Asset3D> Child;
        int[] _pascal = { };
        int index;
        bool forward = true;
        int timeJalan = 1;



        public Asset3D(List<Vector3> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
            setDefault();
        }
        
        public Asset3D()
        {
            _vertices = new List<Vector3>();
            setDefault();
        }

    public void setDefault()
        {
            _model = Matrix4.Identity;
            _euler = new List<Vector3>();

            //Sumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //Sumbu Y
            _euler.Add(new Vector3(0, 1, 0));
            //Sumbu Z
            _euler.Add(new Vector3(0, 0, 1));
            _centerPosition = new Vector3(0, 0, 0);
            Child = new List<Asset3D>();
            
        }
        public void load(string shadervert, string shaderfrag, float Size_x, float Size_y)
        {
            //Buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count
                * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            //VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            //Kalo mau bikin obejct settingnya beda dikasih if
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
                false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            //ada data yang disimpan di _indices
            if (_indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count
                    * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            
            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();

           

            foreach (var item in Child)
            {
                item.load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }
        public void doScale(float num)
        {
            _model *= Matrix4.CreateScale(num);

            foreach (var i in Child)
            {
                i.doScale(num);
            }
        }
        public void render(int _lines, Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);
            //_model = temp;
            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);
            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (_lines == 0)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.LineStripAdjacencyArb, 0, _vertices.Count);
                }
                else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Count);
                }
                else if (_lines == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
            }
            foreach (var item in Child)
            {
                item.render(_lines, temp, camera_view, camera_projection);
            }
        }

        public void createBoxVertices(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            Vector3 temp_vector;

            //TITIK 1
            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 2
            //Atas

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 4

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 5
            //Atas
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 8

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1 
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createBoxVertices2(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            Vector3 temp_vector;

            //TITIK 1
            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 2
            //Atas

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 4

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 5
            //Atas
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 8

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 0.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,                              
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1 
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createBoxVertices3(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            Vector3 temp_vector;

            //TITIK 1
            //TITIK KIRI-ATAS = SISI DEPAN
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 0.0f;
            _vertices.Add(temp_vector);

            //TITIK 2
            //Atas

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z - length / 0.0f;
            _vertices.Add(temp_vector);

            //TITIK 3

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 0.0f;
            _vertices.Add(temp_vector);

            //TITIK 4

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z - length / 0.0f;
            _vertices.Add(temp_vector);

            //TITIK 5
            //Atas
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 6

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 7

            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            //TITIK 8

            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 10.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,                              
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1 
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float v = -pi; v <=pi ; v += pi / 360)
            {
                for (float u = -pi / 2; u <= pi / 2; u += pi / 360)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }

        public void createHalfEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float v = -pi / 2; v <= pi / 2; v += pi / 360)
            {
                for (float u = -pi; u <= 0; u += pi / 360)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }
        public void createHyperboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + 1.0f/(float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + 1.0f/(float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Tan(v) * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }

        public void createHyperboloid2Sisi(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;


            for (float u = -pi / 2; u <= pi / 2; u += pi / 360)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 360)
                {
                    temp_vector.X = radiusX * (float)Math.Tan(v) * (float)Math.Cos(u);
                    temp_vector.Y = radiusY * (float)Math.Tan(v) * (float)Math.Sin(u);
                    temp_vector.Z = radiusZ * 1 / (float)Math.Cos(v);
                    _vertices.Add(temp_vector);
                }

            }

            for (float u = pi / 2; u <= 3 * pi / 2; u += pi / 360)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 360)
                {
                    temp_vector.X = radiusX * (float)Math.Tan(v) * (float)Math.Cos(u);
                    temp_vector.Y = radiusY * (float)Math.Tan(v) * (float)Math.Sin(u);
                    temp_vector.Z = radiusZ * 1 / (float)Math.Cos(v);
                    _vertices.Add(temp_vector);
                }

            }


        }

        public void createEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle) + _x;
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle) + _y;
                    temp_vector.Z = z + _z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
     
        
        }

        public void createHalfEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = sectorCount/2; j >= 0; --j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle) + _x;
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle) + _y;
                    temp_vector.Z = z + _z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }


        }

        public void createEllipticCone(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = 0 ; v <= 64; v += pi / 300)
                {
                    temp_vector.X = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + v * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }

        public void createEllipticParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float v = -pi; v <= pi; v += pi / 300)
            {
                for (float u = 0; u <= 15; u += pi / 300)
                {
                    temp_vector.X = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (v*v) * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }

        public void createHyperboloidParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 90)
            {
                for (float v = 0; v <= 10; v += pi / 90)
                {
                    temp_vector.X = _x + v * (float)Math.Tan(u) * radiusX;
                    temp_vector.Y = _y + v * (1/(float)Math.Cos(u)) * radiusY;
                    temp_vector.Z = _z + (v * v) * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }


        public Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;

            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));

            newPosition.Y =
                temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));

            newPosition.Z =
                temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void rotatede(Vector3 pivot, Vector3 vector, float angle)
        {
            var radAngle = MathHelper.DegreesToRadians(angle);

            var arbRotationMatrix = new Matrix4
                (
                new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
                Vector4.UnitW
                );

            _model *= Matrix4.CreateTranslation(-pivot);
            _model *= arbRotationMatrix;
            _model *= Matrix4.CreateTranslation(pivot);

            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
            }

            _centerPosition = getRotationResult(pivot, vector, radAngle, _centerPosition);



            foreach (var i in Child)
            {
                i.rotatede(pivot, vector, angle);
            }
        }


        public void addChild(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            Asset3D newChild = new Asset3D();
            newChild.createHalfEllipsoid(radiusX, radiusY, radiusZ, _x, _y, _z);
            Child.Add(newChild);
        }

        public void addChildEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            Asset3D newChild = new Asset3D();
            newChild.createEllipsoid(radiusX, radiusY, radiusZ, _x, _y, _z);
            Child.Add(newChild);
        }

        public void createCylinder(float radius, float height, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float i = -pi / 2; i <= pi / 2; i += pi / 360)
            {
                for (float j = -pi; j <= pi; j += pi / 360)
                {
                    temp_vector.X = radius * (float)Math.Cos(i) * (float)Math.Cos(j) + _centerPosition.X;
                    temp_vector.Y = radius * (float)Math.Cos(i) * (float)Math.Sin(j) + _centerPosition.Y;
                    if (temp_vector.Y < _centerPosition.Y)
                        temp_vector.Y = _centerPosition.Y - height * 0.5f;
                    else
                        temp_vector.Y = _centerPosition.Y + height * 0.5f;
                    temp_vector.Z = radius * (float)Math.Sin(i) + _centerPosition.Z;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createCylinder2(float top_radius, float bot_radius, float height, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float i = -pi / 2; i <= pi / 2; i += pi / 360)
            {
                for (float j = -pi; j <= pi; j += pi / 360)
                {

                    temp_vector.Y = top_radius * (float)Math.Cos(i) * (float)Math.Sin(j) + _centerPosition.Y;
                    if (temp_vector.Y < _centerPosition.Y)
                    {
                        temp_vector.Y = _centerPosition.Y - height * 0.5f;
                        temp_vector.X = bot_radius * (float)Math.Cos(i) * (float)Math.Cos(j) + _centerPosition.X;
                        temp_vector.Z = bot_radius * (float)Math.Sin(i) + _centerPosition.Z;
                    }
                    else
                    {
                        temp_vector.X = top_radius * (float)Math.Cos(i) * (float)Math.Cos(j) + _centerPosition.X;
                        temp_vector.Y = _centerPosition.Y + height * 0.5f;
                        temp_vector.Z = top_radius * (float)Math.Sin(i) + _centerPosition.Z;
                    }
                    _vertices.Add(temp_vector);
                }
            }
        }


        public void createCurveBezier(Vector3 centerP)
        {
            _centerPosition = centerP;
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] += centerP;
            }

            List<Vector3> _verticesBezier = new List<Vector3>();
            List<int> pascal = new List<int>();
            if (_vertices.Count > 1)
            {
                pascal = getRow(_vertices.Count - 1);
                for (float t = 0; t <= 1.0f; t += 0.005f)
                {
                    Vector3 p = getP(pascal, t);
                    _verticesBezier.Add(p);
                }
            }
            _vertices = _verticesBezier;
        }
        public Vector3 getP(List<int> pascal, float t)
        {
            Vector3 p = new Vector3(0, 0, 0);
            for (int i = 0; i < _vertices.Count; i++)
            {
                float temp = (float)Math.Pow((1 - t), _vertices.Count - 1 - i) * (float)Math.Pow(t, i) * pascal[i];
                p += temp * _vertices[i];
            }
            return p;
        }
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();
            currow.Add(1);
            if (rowIndex == 0)
            {
                return currow;
            }

            List<int> prev = getRow(rowIndex - 1);
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            currow.Add(1);
            return currow;
        }

        public void AddChild(Asset3D temp)
        {
            Child.Add(temp);
        }

        public void Run(float x, float y, float z)
        {
            _model = _model * Matrix4.CreateTranslation(x, y, z);
            _centerPosition.X += x;
            _centerPosition.Y += y;
            _centerPosition.Z += z;

            foreach (var item in Child)
            {
                item.Run(x, y, z);
            }
        }

        float temp = 1.5f;
        float curTemp = 0f;
        bool move = true;
        public void moveForwardBackward(float start, float fin)
        {
            if (move)
            {
                _model *= Matrix4.CreateTranslation(new Vector3(0, 0, 0.01f));
                _centerPosition.Z += 0.01f;
            }
            else
            {
                _model *= Matrix4.CreateTranslation(new Vector3(0, 0, -0.01f));
                _centerPosition.Z -= 0.01f;
            }

            foreach (var i in Child)
            {
                i.moveForwardBackward(start, fin);
            }
            curTemp += 0.01f;
            if (curTemp > temp)
            {
                if (move) move = false;
                else move = true;
                curTemp = 0;
            }

        }




    }
}
