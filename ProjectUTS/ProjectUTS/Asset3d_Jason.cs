using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThreeDee
{
    internal class Asset3D
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;
        Matrix4 _view;
        Matrix4 _projection;
        Matrix4 _model;
        public Vector3 color;
        public Vector3 _centerPosition = new Vector3(0, 0, 0);
        public List<Vector3> _euler;
        public List<Asset3D> Child;

        public Asset3D(List<Vector3> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
            setdefault();
        }
        public Asset3D()
        {
            color = new Vector3(1, 1, 1);
            _vertices = new List<Vector3>();
            setdefault();
        }
        public Asset3D(Vector3 c)
        {
            color = c;
            _vertices = new List<Vector3>();
            setdefault();
        }
        public void setdefault()
        {
            _euler = new List<Vector3>();
            //sumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //sumbu y
            _euler.Add(new Vector3(0, 1, 0));
            //sumbu z
            _euler.Add(new Vector3(0, 0, 1));
            _model = Matrix4.Identity;
            _centerPosition = new Vector3(0, 0, 0);
            Child = new List<Asset3D>();

        }

        public void loadShader(string shadervert, string shaderfrag)
        {
            _shader = new Shader(shadervert, shaderfrag);
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
            //kalau mau bikin object settingannya beda dikasih if
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
            if (_shader is null)
            {
                _shader = new Shader(shadervert, shaderfrag);
            }
            _shader.Use();
            /*
            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -15.0f);

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size_x / (float)Size_y, 0.1f, 100.0f);*/
            foreach (var item in Child)
            {
                item.load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }

        public void miniLoad()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);
            foreach (var item in Child)
            {
                item.miniLoad();
            }
        }
        public void render(int _lines, double time, Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();
            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, color[0], color[1], color[2], 1f);
            //_shader.SetVector3("objectColor", color);
            GL.BindVertexArray(_vertexArrayObject);
            //_model = _model * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time));
            _model = temp;

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
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
                else if (_lines == 2)
                {

                }
                else if (_lines == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
            }

            foreach (Asset3D item in Child)
            {
                item.render(_lines, time, temp, camera_view, camera_projection);
            }
        }

        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            // Vertices
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

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            // Indices
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
        public void createPolyCylinder(float radius, float height, float polycount)
        {
            // Vertices
            _vertices.Add(new Vector3(0, 0, height / 2));
            _vertices.Add(new Vector3(0, 0, height / -2));
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u < pi; u += pi / (polycount / 2))
            {
                temp_vector.X = (float)Math.Cos(u) * radius;
                temp_vector.Y = (float)Math.Sin(u) * radius;
                temp_vector.Z = (float)height / 2;
                _vertices.Add(temp_vector);

                temp_vector.Z = (float)height / -2;
                _vertices.Add(temp_vector);
            }

            // Indices
            for (uint i = 1; i <= _vertices.Count / 2 - 2; i++)
            {
                _indices.Add(0);
                _indices.Add(i * 2);
                _indices.Add(i * 2 + 2);

                _indices.Add(1);
                _indices.Add(i * 2 + 1);
                _indices.Add(i * 2 + 3);
            }
            _indices.Add(0);
            _indices.Add((uint)_vertices.Count - 2);
            _indices.Add(2);

            _indices.Add(1);
            _indices.Add((uint)_vertices.Count - 1);
            _indices.Add(3);

            for (uint i = 2; i <= _vertices.Count - 3; i++)
            {
                _indices.Add(i);
                _indices.Add(i + 1);
                _indices.Add(i + 2);
            }
            _indices.Add((uint)_vertices.Count() - 2);
            _indices.Add((uint)_vertices.Count() - 1);
            _indices.Add(2);

            _indices.Add((uint)_vertices.Count() - 1);
            _indices.Add(2);
            _indices.Add(3);
        }
        public void createNeckBox(float length, float width, float height1, float height2)
        {
            Vector3 temp_vector;

            temp_vector.X = -length / 2.0f;
            temp_vector.Y = width / 2.0f;
            temp_vector.Z = -height1 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = length / 2.0f;
            temp_vector.Y = width / 2.0f;
            temp_vector.Z = -height2 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = -length / 2.0f;
            temp_vector.Y = -width / 2.0f;
            temp_vector.Z = -height1 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = length / 2.0f;
            temp_vector.Y = -width / 2.0f;
            temp_vector.Z = -height2 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = -length / 2.0f;
            temp_vector.Y = width / 2.0f;
            temp_vector.Z = height1 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = length / 2.0f;
            temp_vector.Y = width / 2.0f;
            temp_vector.Z = height2 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = -length / 2.0f;
            temp_vector.Y = -width / 2.0f;
            temp_vector.Z = height1 / 2.0f;
            _vertices.Add(temp_vector);

            temp_vector.X = length / 2.0f;
            temp_vector.Y = -width / 2.0f;
            temp_vector.Z = height2 / 2.0f;
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


        public void translate(Vector3 vector)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = vector + _vertices[i];
            }
            _centerPosition += vector;

            foreach (Asset3D child in Child)
            {
                child.translate(vector);
            }
        }
        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = getRotationResult(pivot, vector, MathHelper.DegreesToRadians(angle), _vertices[i]);
            }
            _centerPosition = getRotationResult(pivot, vector, MathHelper.DegreesToRadians(angle), _centerPosition);

            foreach (Asset3D child in Child)
            {
                child.rotate(pivot, vector, angle);
            }
            /*            for (int i = 0; i < 3; i++)
                        {
                            _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

                            //NORMALIZE
                            //LANGKAH - LANGKAH
                            //length = akar(x^2+y^2+z^2)
                            float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
                            Vector3 temporary = new Vector3(0, 0, 0);
                            temporary.X = _euler[i].X / length;
                            temporary.Y = _euler[i].Y / length;
                            temporary.Z = _euler[i].Z / length;
                            _euler[i] = temporary;
                        }
                        _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);*/
        }

        //public void rotatede(Vector3 pivot, Vector3 vector, float angle)
        //{
        //    var radAngle = MathHelper.DegreesToRadians(angle);

        //    var arbRotationMatrix = new Matrix4
        //        (
        //        new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
        //        new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
        //        new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
        //        Vector4.UnitW
        //        );

        //    _model *= Matrix4.CreateTranslation(-pivot);
        //    _model *= arbRotationMatrix;
        //    _model *= Matrix4.CreateTranslation(pivot);

        //    for (int i = 0; i < 3; i++)
        //    {
        //        _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
        //    }

        //    _centerPosition = getRotationResult(pivot, vector, radAngle, _centerPosition);

        //    //foreach (var i in child)
        //    //{
        //    //    i.rotate(pivot, vector, angle);
        //    //}
        //}

        public void render_rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            //pivot -> mau rotate di titik mana
            //vector -> mau rotate di sumbu apa? (x,y,z)
            //angle -> rotatenya berapa derajat?
            var real_angle = angle;
            angle = MathHelper.DegreesToRadians(angle);

            //mulai ngerotasi
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = getRotationResult(pivot, vector, angle, _vertices[i]);
            }
            //rotate the euler direction
            for (int i = 0; i < 3; i++)
            {
                _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

                //NORMALIZE
                //LANGKAH - LANGKAH
                //length = akar(x^2+y^2+z^2)
                float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
                Vector3 temporary = new Vector3(0, 0, 0);
                temporary.X = _euler[i].X / length;
                temporary.Y = _euler[i].Y / length;
                temporary.Z = _euler[i].Z / length;
                _euler[i] = temporary;
            }
            _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);
            foreach (var item in Child)
            {
                item.render_rotate(pivot, vector, real_angle);
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

        public void resetEuler()
        {
            _euler[0] = new Vector3(1, 0, 0);
            _euler[1] = new Vector3(0, 1, 0);
            _euler[2] = new Vector3(0, 0, 1);
        }
        public void addChild(Asset3D newChild)
        {
            Child.Add(newChild);
        }
    }
}