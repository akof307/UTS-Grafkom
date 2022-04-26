using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUTS
{
    internal class Asset3d
    {
        public List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;
        
        Matrix4 _model; // ngerubbah transformasi
        public Vector3 _centerPosition;
        public List<Vector3> _euler;
        public List<Asset3d> Child;
        float[] _assetColor;

        int indexs;

        bool forward = true;

        public Asset3d(List<Vector3> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
            setDefault();
        }
        public Asset3d(float[] _assetColor)
        {
            this._assetColor = _assetColor;
            _vertices = new List<Vector3>();
            setDefault();
        }
        public void setDefault()
        {
            _euler = new List<Vector3>();
            //sumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //sumbu y
            _euler.Add(new Vector3(0, 1, 0));
            //sumbu z
            _euler.Add(new Vector3(0, 0, 1));
            _centerPosition = new Vector3(0, 0, 0);
            _model = Matrix4.Identity;
            Child = new List<Asset3d>();

            indexs = 0;
        }
        public void load(string shaderVert, string shaderFrag, float Size_X, float Size_Y)
        {
            //Buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

            //VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            //kalau mau bikin object settingannya beda dikasih if
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //0 == referensi dari atas
            GL.EnableVertexAttribArray(0);
            //ada data yanh disimpan di _indices
            if (_indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);

            }

            _shader = new Shader(shaderVert, shaderFrag);
            _shader.Use();

            foreach (var item in Child)
            {
                item.load(shaderVert, shaderFrag, Size_X, Size_Y);
            }
        }

        public void render(Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection, bool curve = false)
        {
            _shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, _assetColor[0], _assetColor[1], _assetColor[2], _assetColor[3]);
            
            GL.BindVertexArray(_vertexArrayObject);
            //_model = temp;
            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);
            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else if(!curve)
            {
                GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Count));
            }
            else
            {
                GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Count));
            }


            foreach (var item in Child)
            {
                item.render(temp, camera_view, camera_projection, curve);
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

        float temp = 1f;
        float curTemp = 0f;
        bool plus = true;
        public void animScale(Vector3 pivot, float num)
        {
            if (plus)
            {
                _model *= Matrix4.CreateScale(1 + num);
                _model *= Matrix4.CreateTranslation(new Vector3(0,0.0105f,0));
            }
            else
            {
                _model *= Matrix4.CreateScale(1 - num);
                _model *= Matrix4.CreateTranslation(new Vector3(0, -0.0105f, 0));
            }

            foreach (var i in Child)
            {
                i.animScale(pivot, num);
            }
            curTemp += 0.01f;
            if (curTemp > temp)
            {
                if (plus) plus = false;
                else plus = true;
                curTemp = 0;
            }
        }
        public void moveForwardBackward(float start, float fin)
        {
            if (plus)
            {
                _model *= Matrix4.CreateTranslation(new Vector3(-0.01f, 0, 0f));
                _centerPosition.X -= 0.01f;
            }
            else
            {
                _model *= Matrix4.CreateTranslation(new Vector3(0.01f, 0, 0f));
                _centerPosition.X += 0.01f;
            }

            foreach (var i in Child)
            {
                i.moveForwardBackward(start, fin);
            }
        }
        public void createHalfSphere(float radius, float _x, float _y, float _z)
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
                        temp_vector.Y = _centerPosition.Y;
                    temp_vector.Z = radius * (float)Math.Sin(i) + _centerPosition.Z;
                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createCylinder(float top_radius, float bot_radius, float height, float _x, float _y, float _z)
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
        public void createTorus(float r1, float r2, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float i = 0; i < 2 * pi; i+= pi / 360)
            {
                for (float j = 0; j < 2 * pi; j += pi / 360)
                {
                    temp_vector.X = (r1 + r2 * (float)Math.Cos(i)) * (float)Math.Cos(j) + _centerPosition.X;
                    temp_vector.Y = (r1 + r2 * (float)Math.Cos(i)) * (float)Math.Sin(j) + _centerPosition.Y;
                    temp_vector.Z = r2 * (float)Math.Sin(i);
                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createElipsoid(float rX, float rY, float rZ, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float i = -pi / 2; i <= pi / 2; i += pi / 720)
            {
                for (float j = -pi; j <= pi; j += pi / 720)
                {
                    temp_vector.X = rX * (float)Math.Cos(i) * (float)Math.Cos(j) + _centerPosition.X;
                    temp_vector.Y = rY * (float)Math.Cos(i) * (float)Math.Sin(j) + _centerPosition.Y;
                    temp_vector.Z = rZ* (float)Math.Sin(i) + _centerPosition.Z;
                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createTrapezoidVertices(float _X, float _Y, float _Z1, float _Z2, float x, float y, float z)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - _X / 2.0f;
            temp_vector.Y = y + _Y / 2.0f;
            temp_vector.Z = z - _Z1 / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + _X / 2.0f;
            temp_vector.Y = y + _Y / 2.0f;
            temp_vector.Z = z - _Z1/ 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - _X / 2.0f;
            temp_vector.Y = y - _Y / 2.0f;
            temp_vector.Z = z - _Z2 / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + _X / 2.0f;
            temp_vector.Y = y - _Y/ 2.0f;
            temp_vector.Z = z - _Z2/ 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - _X/ 2.0f;
            temp_vector.Y = y + _Y/ 2.0f;
            temp_vector.Z = z + _Z1/ 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + _X/ 2.0f;
            temp_vector.Y = y + _Y/ 2.0f;
            temp_vector.Z = z + _Z1/ 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - _X/ 2.0f;
            temp_vector.Y = y - _Y/ 2.0f;
            temp_vector.Z = z + _Z2/ 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + _X/ 2.0f;
            temp_vector.Y = y - _Y/ 2.0f;
            temp_vector.Z = z + _Z2/ 2.0f;
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
        public void makeCurveBezier(Vector3 centerP)
        {
            _centerPosition = centerP;
            //ini nyoba di tiga titik
            Console.WriteLine(centerP);
            _vertices.Add(new Vector3(0, 0, 0) + centerP);
            _vertices.Add(new Vector3(1f, 1f, 0) + centerP);
            _vertices.Add(new Vector3(-1f, 1f, 0) + centerP);
            _vertices.Add(new Vector3(0,0,0 )+ centerP);
            _vertices.Add(new Vector3(1, -1f, 0) + centerP);
            _vertices.Add(new Vector3(-1f, -1f, 0) + centerP);
            _vertices.Add(new Vector3(0,0,0) + centerP);

            List<Vector3> _verticesBezier = new List<Vector3>();
            List<int> pascal = new List<int>();
            if (_vertices.Count > 1)
            {
                pascal = getRow(_vertices.Count-1);
                for (float t = 0; t <= 1.0f; t += 0.005f)
                {
                    Vector3 p = getP(pascal, t);
                    _verticesBezier.Add(p);
                }
            }
            _vertices = _verticesBezier;
            //GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(0);

        }
        public Vector3 getP(List<int> pascal, float t)
        {
            Vector3 p = new Vector3(0, 0, 0);
            int n = _vertices.Count;
            for (int i = 0; i < n; i++)
            {
                float tmp = (float)Math.Pow((1 - t), n - 1 - i) * (float)Math.Pow(t, i) * pascal[i];
                p.X += tmp * _vertices[i].X;
                p.Y += tmp * _vertices[i].Y;
                p.Z += tmp * _vertices[i].Z;
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
        public void addChild(Asset3d child)
        {
            Asset3d newChild;
            newChild = child;
            Child.Add(newChild);
        }
        public void setVertices(Asset3d from)
        {
            _vertices = from._vertices;
        }
        public void addVertices(Vector3 vert)
        {
            _vertices.Add(vert);
        }
    }
}
