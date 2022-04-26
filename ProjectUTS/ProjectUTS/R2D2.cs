using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUTS
{
    internal class R2D2
    {
        public static Asset3d build(Vector3 centerP)
        {
            Asset3d body = new Asset3d(new float[]
            {
                0.5f, 0.0f, 0.5f, 1.0f
            });
            body.createCylinder(0.25f, 0.25f, 0.6f, 0.0f + centerP.X, 0.0f + centerP.Y, 0.0f + centerP.Z);

            Asset3d head = new Asset3d(new float[]
            {
                0.5f, 1.0f, 0.5f, 1.0f
            });
            head.createHalfSphere(0.25f, 0.0f + centerP.X, 0.3f + centerP.Y, 0.0f + centerP.Z);

            Asset3d footR = new Asset3d(new float[]
            {
                0.5f, 0.0f, 0.0f, 1.0f
            });
            footR.createTrapezoidVertices(0.1f, 0.2f, 0.25f, 0.4f, 0.33f + centerP.X, -0.45f + centerP.Y, 0.0f + centerP.Z);
            Asset3d arm = new Asset3d(new float[]
            {
                0.3f, 0.0f, 0.0f, 1.0f
            });
            arm.createTrapezoidVertices(0.09f, 0.32f, 0.18f, 0.18f, 0.33f + centerP.X, -0.19f + centerP.Y, 0.0f + centerP.Z);
            footR.addChild(arm);

            Asset3d footL = new Asset3d(new float[]
            {
                0.5f, 0.0f, 0.0f, 1.0f
            });
            footL.createTrapezoidVertices(0.1f, 0.2f, 0.25f, 0.4f, -0.33f + centerP.X, -0.45f + centerP.Y, 0.0f + centerP.Z);
            arm = new Asset3d(new float[]
            {
                0.3f, 0.0f, 0.0f, 1.0f
            });
            arm.createTrapezoidVertices(0.09f, 0.32f, 0.18f, 0.18f, -0.33f + centerP.X, -0.19f + centerP.Y, 0.0f + centerP.Z);
            footL.addChild(arm);

            Asset3d bottom = new Asset3d(new float[]
            {
                0.4f, 0.0f, 0.6f, 1.0f
            });
            bottom.createCylinder(0.2f, 0.17f, 0.05f, 0.0f + centerP.X, -0.33f + centerP.Y, 0.0f + centerP.Z);

            Asset3d eye = new Asset3d(new float[]
            {
                0.1f, 0.1f, 0.1f, 1.0f
            });
            eye.createElipsoid(0.05f, 0.05f, 0.05f, 0.0f + centerP.X, 0.43f + centerP.Y, 0.2f + centerP.Z);
            head.addChild(eye);
            Asset3d outerEye = new Asset3d(new float[]
            {
                0.1f, 0.1f, 1.0f, 1.0f
            });
            outerEye.createTrapezoidVertices(0.12f, 0.12f, 0.08f, 0.2f, 0.0f + centerP.X, 0.43f + centerP.Y, 0.15f + centerP.Z);
            head.addChild(outerEye);

            Asset3d connected = new Asset3d(new float[]
            {
                0.2f, 0.0f, 0.5f, 1.0f
            });
            connected.createCylinder(0.12f, 0.12f, 0.75f, 0.0f + centerP.X, 0.05f + centerP.Y, 0.0f + centerP.Z);
            connected.rotatede(connected._centerPosition, connected._euler[2], 90f);
            bottom.addChild(connected);

            Asset3d shield = new Asset3d(new float[]
            {
                1.0f, 1.0f, 1.0f, 1.0f
            });
            shield.makeCurveBezier(new Vector3(centerP.X, 0.6f + centerP.Y, centerP.Z));
            shield.rotatede(shield._centerPosition, shield._euler[2], 0f);
            shield.rotatede(shield._centerPosition, shield._euler[0], 90f);

            Asset3d ground = new Asset3d(new float[]
            {
                0.8f, 0.6f, 0.4f, 1.0f
            });
            ground.createTrapezoidVertices(4f, 0.2f, 3f, 3f, centerP.X,centerP.Y - 0.9f, centerP.Z);

            body.addChild(shield);
            body.addChild(head);
            body.addChild(footR);
            body.addChild(footL);
            body.addChild(bottom);
            body.addChild(ground);

            return body;
        }

        public static void animation(ProjectUTS.Asset3d asset, Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection)
        {
            asset.render(temp, camera_view, camera_projection, true);
            asset.Child[0].render(temp, camera_view, camera_projection, true);
            asset.Child[0].rotatede(asset.Child[0]._centerPosition, asset.Child[0]._euler[2], 1f);

            asset.Child[1].animScale(asset.Child[1]._centerPosition, 0.002f);
            asset.Child[1].rotatede(asset.Child[1]._centerPosition, asset.Child[1]._euler[1], 1f);
        }
    }
}
