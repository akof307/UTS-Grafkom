using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafkom1
{
    internal class D_0
    {
        public static Grafkom1.Asset3D build(Vector3 centerPos)
        {
            Grafkom1.Asset3D asset = new Grafkom1.Asset3D();

            Grafkom1.Asset3D a0 = new Grafkom1.Asset3D(); 
            a0.createCylinder2(0.09f, 0.17f, 0.6f, 0.0f + centerPos.X, 0.0f + centerPos.Y, 0.0f + centerPos.Z); //Kepala
            a0.rotatede(a0._centerPosition, a0._euler[0], 90f);

            Grafkom1.Asset3D a1 = new Grafkom1.Asset3D();
            a1.createCylinder(0.05f, 0.15f, 0.0f + centerPos.X, -0.15f + centerPos.Y, 0.0f + centerPos.Z); //Leher

            Grafkom1.Asset3D a2 = new Grafkom1.Asset3D();
            a2.createBoxVertices(0.0f + centerPos.X, -0.25f + centerPos.Y, 0.0f + centerPos.Z, 0.2f); //Badan

            Grafkom1.Asset3D a3 = new Grafkom1.Asset3D();
            a3.createCylinder(0.025f, 0.05f, 0.11f + centerPos.X, -0.235f + centerPos.Y, 0.0f + centerPos.Z); //Baut Besar
            a3.rotatede(a3._centerPosition, a3._euler[2], 90f);

            Grafkom1.Asset3D a4 = new Grafkom1.Asset3D();
            a4.createCylinder(0.015f, 0.05f, 0.11f + centerPos.X, -0.235f + centerPos.Y, -0.05f + centerPos.Z); //Baut kecil
            a4.rotatede(a4._centerPosition, a4._euler[2], 90f);

            Grafkom1.Asset3D a5 = new Grafkom1.Asset3D();
            a5.createCylinder(0.01f, 0.3f, 0.115f + centerPos.X, -0.37f + centerPos.Y, -0.075f + centerPos.Z); //Pipa Besar
            a5.rotatede(a5._centerPosition, a5._euler[0], 30f);

            Grafkom1.Asset3D a6 = new Grafkom1.Asset3D();
            a6.createCylinder(0.005f, 0.35f, 0.115f + centerPos.X, -0.375f + centerPos.Y, -0.128f + centerPos.Z); //Pipa Kecil
            a6.rotatede(a6._centerPosition, a6._euler[0], 30f);

            Grafkom1.Asset3D a7 = new Grafkom1.Asset3D();
            a7.createCylinder(0.035f, 0.05f, 0.11f + centerPos.X, -0.525f + centerPos.Y, -0.16f + centerPos.Z); //Baut Bawah
            a7.rotatede(a7._centerPosition, a7._euler[2], 90f);

            Grafkom1.Asset3D a8 = new Grafkom1.Asset3D();
            a8.createCylinder(0.225f, 0.2f, 0.0f + centerPos.X, -0.525f + centerPos.Y, -0.16f + centerPos.Z); //Roda
            a8.rotatede(a8._centerPosition, a8._euler[2], 90f);

            Grafkom1.Asset3D a9 = new Asset3D(new List<Vector3> { (0.0f, -0.05f, -0.20f), (0.0f, -0.05f, -0.25f), (0.0f, -0.05f, -0.30f), (0.0f, -0.075f, -0.45f) }, new List<uint> { });
            Grafkom1.Asset3D a10 = new Asset3D(new List<Vector3> { (0.05f, 0.05f, -0.20f), (0.05f, 0.05f, -0.25f), (0.05f, -0.05f, -0.30f), (0.05f, -0.015f, -0.45f) }, new List<uint> { });
            Grafkom1.Asset3D a11 = new Asset3D(new List<Vector3> { (-0.05f, 0.05f, -0.20f), (-0.05f, 0.05f, -0.25f), (-0.05f, -0.05f, -0.30f), (-0.05f, -0.015f, -0.45f) }, new List<uint> { });

            a9.createCurveBezier(centerPos);
            a10.createCurveBezier(centerPos);
            a11.createCurveBezier(centerPos);

            Grafkom1.Asset3D a12 = new Grafkom1.Asset3D();
            a12.createBoxVertices(0.11f + centerPos.X, -0.525f + centerPos.Y, -0.2f + centerPos.Z, 0.035f); //BoxRoda

            Grafkom1.Asset3D b1 = new Grafkom1.Asset3D();
            b1.AddChild(a0);
            b1.AddChild(a9);
            b1.AddChild(a10);
            b1.AddChild(a11);

            Grafkom1.Asset3D b2 = new Grafkom1.Asset3D();
            b2.AddChild(a1);
            b2.AddChild(a2);
            b2.AddChild(a3);
            b2.AddChild(a4);
            b2.AddChild(a5);
            b2.AddChild(a6);
            b2.AddChild(a7);
            b2.AddChild(a12);

            Grafkom1.Asset3D c1 = new Grafkom1.Asset3D();
            c1.AddChild(b1);
            c1.AddChild(b2);
            c1.AddChild(a8);

            //a0.load(path + "Silver.vert", path + "Silver.frag", _x, _y);
            //a1.load(path + "Black.vert", path + "Black.frag", _x, _y);
            //a2.load(path + "Silver.vert", path + "Silver.frag", _x, _y);
            //a3.load(path + "Black.vert", path + "Black.frag", _x, _y);
            //a4.load(path + "Black.vert", path + "Black.frag", _x, _y);
            //a5.load(path + "Black.vert", path + "Black.frag", _x, _y);
            //a6.load(path + "Black.vert", path + "Black.frag", _x, _y);
            //a7.load(path + "Black.vert", path + "Black.frag", _x, _y);
            //a8.load(path + "Silver.vert", path + "Silver.frag", _x, _y);
            //a9.load(path + "Silver.vert", path + "Silver.frag", _x, _y);
            //a10.load(path + "Silver.vert", path + "Silver.frag", _x, _y);
            //a11.load(path + "Silver.vert", path + "Silver.frag", _x, _y);
            //a12.load(path + "Black.vert", path + "Black.frag", _x, _y);
            return c1;
        }
        

    }
}
