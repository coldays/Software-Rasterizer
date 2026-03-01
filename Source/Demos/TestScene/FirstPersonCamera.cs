using SoftwareRasterizer.Types;
using SoftwareRasterizer.Core;
using static SoftwareRasterizer.Maths;
using System.Numerics;

namespace SoftwareRasterizer.Demo;

public class FirstPersonCamera(Transform transform)
{
	float camYawTarget;
	float camPitchTarget;

	public void Update(float deltaTime, Vector2 screenSize)
	{
		// Rotate camera with mouse
		const float mouseSensitivity = 2f;
		if (Input.IsHoldingMouse(MouseButton.Left))
		{
			Vector2 mouseDelta = Input.GetMouseDelta() / screenSize.X * mouseSensitivity;
			camPitchTarget = Clamp(camPitchTarget - mouseDelta.Y, ToRadians(-85), ToRadians(85));
			camYawTarget -= mouseDelta.X;
			Input.LockCursor();
		}
		else if (Input.IsKeyDownThisFrame(Key.Q))
		{
			Input.UnlockCursor();
			
		}

		transform.Pitch = Lerp(transform.Pitch, camPitchTarget, deltaTime * 15);
		transform.Yaw = Lerp(transform.Yaw, camYawTarget, deltaTime * 15);

		// Move camera with WASD
		const float camSpeed = 2.5f;
		Vector3 moveDelta = Vector3.Zero;

		if (Input.IsKeyHeld(Key.W)) moveDelta += transform.Forward;
		if (Input.IsKeyHeld(Key.S)) moveDelta -= transform.Forward;
		if (Input.IsKeyHeld(Key.A)) moveDelta -= transform.Right;
		if (Input.IsKeyHeld(Key.D)) moveDelta += transform.Right;

		transform.Position += Vector3.Normalize(moveDelta) * camSpeed * deltaTime;
		transform.Position.Y = 1;
	}
	
}