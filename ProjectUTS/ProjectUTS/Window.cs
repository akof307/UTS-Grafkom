using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using static ThreeDee.Asset3D;

namespace ProjectUTS
{
    static class Constants
    {
        public const string path = "../../../ShaderAldo/";
        public const string pathJason = "../../../Shader/";
        public const string pathJustin = "../../../Shaders/";
    }
    internal class Window : GameWindow
    {
        ThreeDee.Asset3D[] _object3d = new ThreeDee.Asset3D[1];
        ThreeDee.Asset3D child;
        ThreeDee.Asset3D childchild;
        ProjectUTS.Asset3d r2d2;
        Grafkom1.Asset3D d_0;

        double _time;
        Camera _camera;
        bool _firstMove = true;
        Vector2 _lastPos;
        Vector3 _objectPos = new Vector3(0.0f, 0.0f, 0.0f);
        float _rotationSpeed = 1f;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.25f, 0.2f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // MainBody
            _object3d[0] = new ThreeDee.Asset3D();

            child = new ThreeDee.Asset3D(new Vector3(192 / 255f, 192 / 255f, 192 / 255f));
            child.createEllipsoid(0.7f, 0.7f, 0.7f, 0, 0, 0, 36, 36);
            _object3d[0].addChild(child);

            // Neck
            child = new ThreeDee.Asset3D(new Vector3(100 / 255f, 100 / 255f, 100 / 255f));
            child.createPolyCylinder(0.15f, 3.0f, 18);
            _object3d[0].addChild(child);

            child = new ThreeDee.Asset3D(new Vector3(192 / 255f, 192 / 255f, 192 / 255f));
            child.createNeckBox(1.5f, 0.15f, 1.2f, 0.4f);
            child.rotate(child._centerPosition, child._euler[0], 90);
            child.rotate(child._centerPosition, child._euler[1], 90);
            child.translate(new Vector3(0, 0, -0.5f));
            _object3d[0].addChild(child);

            child = new ThreeDee.Asset3D(new Vector3(192 / 255f, 192 / 255f, 192 / 255f));
            child.createNeckBox(1.5f, 0.15f, 0.4f, 1.2f);
            child.rotate(child._centerPosition, child._euler[0], 90);
            child.rotate(child._centerPosition, child._euler[1], 90);
            child.translate(new Vector3(0, 0, 0.5f));
            _object3d[0].addChild(child);

            // Wings
            child = new ThreeDee.Asset3D(new Vector3(100 / 255f, 100 / 255f, 100 / 255f));
            child.createPolyCylinder(2.2f, 0.2f, 6);
            childchild = new ThreeDee.Asset3D(new Vector3(192 / 255f, 192 / 255f, 192 / 255f));
            childchild.createPolyCylinder(0.4f, 0.4f, 18);
            child.addChild(childchild);
            child.translate(new Vector3(0, 0, 1.6f));
            _object3d[0].addChild(child);

            child = new ThreeDee.Asset3D(new Vector3(100 / 255f, 100 / 255f, 100 / 255f));
            child.createPolyCylinder(2.2f, 0.2f, 6);
            childchild = new ThreeDee.Asset3D(new Vector3(192 / 255f, 192 / 255f, 192 / 255f));
            childchild.createPolyCylinder(0.4f, 0.4f, 18);
            child.addChild(childchild);
            child.translate(new Vector3(0, 0, -1.6f));
            _object3d[0].addChild(child);

            // Weapon
            child = new ThreeDee.Asset3D(new Vector3(100 / 255f, 100 / 255f, 100 / 255f));
            child.createNeckBox(0.1f, 0.5f, 0.1f, 0.1f);
            child.rotate(child._centerPosition, child._euler[2], 90);
            child.translate(new Vector3(0, -0.3f, 0.5f));
            _object3d[0].addChild(child);

            child = new ThreeDee.Asset3D(new Vector3(100 / 255f, 100 / 255f, 100 / 255f));
            child.createNeckBox(0.1f, 0.5f, 0.1f, 0.1f);
            child.rotate(child._centerPosition, child._euler[2], 90);
            child.translate(new Vector3(0, -0.3f, -0.5f));
            _object3d[0].addChild(child);

            r2d2 = ProjectUTS.R2D2.build(new Vector3(0, -2.2f, 0));
            r2d2.doScale(3f);
            //CursorGrabbed = true;

            // Load All
            _object3d[0].rotate(_object3d[0]._centerPosition, _object3d[0]._euler[1], 90);
            _object3d[0].load(Constants.pathJason + "shader.vert", Constants.pathJason + "black.frag", Size.X, Size.Y);

            r2d2.load(Constants.pathJustin + "shader.vert", Constants.pathJustin + "shader.frag", Size.X, Size.Y);

            d_0 = Grafkom1.D_0.build(new Vector3(1, -2, 0));
            d_0.doScale(3f);
            d_0.load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[0].Child[0].load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[1].Child[0].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);
            d_0.Child[1].Child[1].load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[1].Child[2].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);
            d_0.Child[1].Child[3].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);
            d_0.Child[1].Child[4].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);
            d_0.Child[1].Child[5].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);
            d_0.Child[1].Child[6].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);
            d_0.Child[0].Child[1].load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[0].Child[2].load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[0].Child[3].load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[2].load(Constants.path + "Silver.vert", Constants.path + "Silver.frag", Size.X, Size.Y);
            d_0.Child[1].Child[7].load(Constants.path + "Black.vert", Constants.path + "Black.frag", Size.X, Size.Y);

           


            _camera = new Camera(new Vector3(0, -2, 7), Size.X / Size.Y);
            _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
        }

        double time = 0;
        float oldT = 0f;
        float oldA = 0f;
        public float TFunction(double x)
        {
            return (float)((0.3523936107587275 * Math.Pow(x, 1)) + (19.316552166466042 * Math.Pow(x, 2)) + (-51.96114756626698 * Math.Pow(x, 3)) + (52.502089273661404 * Math.Pow(x, 4)) + (-26.575484758668964 * Math.Pow(x, 5)) + (7.229578760990677 * Math.Pow(x, 6)) + (-1.0119916396169555 * Math.Pow(x, 7)) + (0.05730961514432533 * Math.Pow(x, 8)) + 0.004278150075903082);
        }
        public float AFunction(double x)
        {
            return (float)((58.93113663183121 * Math.Pow(x, 1)) + (-506.9188149447852 * Math.Pow(x, 2)) + (629.8099322422216 * Math.Pow(x, 3)) + (-221.3678124324893 * Math.Pow(x, 4)) + (-54.854548607162016 * Math.Pow(x, 5)) + (55.51823053182386 * Math.Pow(x, 6)) + (-12.931370293798222 * Math.Pow(x, 7)) + (0.994936898469982 * Math.Pow(x, 8)) + 28.60383405560329);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _time += 1 * args.Time;
            Matrix4 temp = Matrix4.Identity;
            //temp = temp * Matrix4.CreateTranslation((float)Math.Sin(i * Math.PI / 180), shift, 0.0f);
            //degr += MathHelper.DegreesToRadians(20f);
            //temp = temp * Matrix4.CreateRotationX(degr);

            time += 0.005;
            time = time % 4.25;
            _object3d[0].translate(new Vector3((TFunction(time) - oldT) * 6, 0, 0));
            oldT = TFunction(time);
            _object3d[0].rotate(_object3d[0]._centerPosition, _object3d[0]._euler[2], oldA - AFunction(time));
            oldA = AFunction(time);
            _object3d[0].miniLoad();
            _object3d[0].render(0, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            //_object3d[0].resetEuler();
            //r2d2.render(temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            R2D2.animation(r2d2, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            
            d_0.render(0, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            d_0.moveForwardBackward(-1, 1);

            SwapBuffers();
        }



        // Camera (Gak Penting)
        public Matrix4 generateArbRotationMatrix(Vector3 axis, Vector3 center, float degree)
        {
            var rads = MathHelper.DegreesToRadians(degree);

            var secretFormula = new float[4, 4] {
                { (float)Math.Cos(rads) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(rads)), axis.X* axis.Y * (1 - (float)Math.Cos(rads)) - axis.Z * (float)Math.Sin(rads),    axis.X * axis.Z * (1 - (float)Math.Cos(rads)) + axis.Y * (float)Math.Sin(rads),   0 },
                { axis.Y * axis.X * (1 - (float)Math.Cos(rads)) + axis.Z * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(rads)), axis.Y * axis.Z * (1 - (float)Math.Cos(rads)) - axis.X * (float)Math.Sin(rads),   0 },
                { axis.Z * axis.X * (1 - (float)Math.Cos(rads)) - axis.Y * (float)Math.Sin(rads),   axis.Z * axis.Y * (1 - (float)Math.Cos(rads)) + axis.X * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(rads)), 0 },
                { 0, 0, 0, 1}
            };
            var secretFormulaMatix = new Matrix4
            (
                new Vector4(secretFormula[0, 0], secretFormula[0, 1], secretFormula[0, 2], secretFormula[0, 3]),
                new Vector4(secretFormula[1, 0], secretFormula[1, 1], secretFormula[1, 2], secretFormula[1, 3]),
                new Vector4(secretFormula[2, 0], secretFormula[2, 1], secretFormula[2, 2], secretFormula[2, 3]),
                new Vector4(secretFormula[3, 0], secretFormula[3, 1], secretFormula[3, 2], secretFormula[3, 3])
            );

            return secretFormulaMatix;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _camera.Fov = _camera.Fov - e.OffsetY;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            float cameraSpeed = 0.5f;
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)args.Time;
            }

            if (KeyboardState.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)args.Time;
            }
            var mouse = MouseState;
            var sensitivity = 0.2f;
            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
            if (KeyboardState.IsKeyDown(Keys.N))
            {
                var axis = new Vector3(0, 1, 0);
                _camera.Position -= _objectPos;
                _camera.Position = Vector3.Transform(
                    _camera.Position,
                    generateArbRotationMatrix(axis, _objectPos, _rotationSpeed)
                    .ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position
                    - _objectPos);
            }
            if (KeyboardState.IsKeyDown(Keys.Comma))
            {
                var axis = new Vector3(0, 1, 0);
                _camera.Position -= _objectPos;
                _camera.Yaw -= _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objectPos, -_rotationSpeed)
                    .ExtractRotation());
                _camera.Position += _objectPos;

                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
            if (KeyboardState.IsKeyDown(Keys.K))
            {
                var axis = new Vector3(1, 0, 0);
                _camera.Position -= _objectPos;
                _camera.Pitch -= _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objectPos, _rotationSpeed).ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
            if (KeyboardState.IsKeyDown(Keys.M))
            {
                var axis = new Vector3(1, 0, 0);
                _camera.Position -= _objectPos;
                _camera.Pitch += _rotationSpeed;
                _camera.Position = Vector3.Transform(_camera.Position,
                    generateArbRotationMatrix(axis, _objectPos, -_rotationSpeed).ExtractRotation());
                _camera.Position += _objectPos;
                _camera._front = -Vector3.Normalize(_camera.Position - _objectPos);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButton.Left)
            {
                float _x = (MousePosition.X - Size.X / 2) / Size.X * 2;
                float _y = -(MousePosition.Y - Size.Y / 2) / Size.Y * 2;

                Console.WriteLine("x = " + _x + "; y = " + _y);
            }
        }

    }
}
    
