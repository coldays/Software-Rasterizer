namespace SoftwareRasterizer.Types;

using System.Numerics;
using static MathF;

public class Transform
{
	public Vector3 Position;
	public Vector3 Scale = Vector3.One;
	public Transform Parent;

	public float Pitch
	{
		get => _pitch;
		set => SetRotation(value, _yaw, _roll);
	}

	public float Yaw
	{
		get => _yaw;
		set => SetRotation(_pitch, value, _roll);
	}

	public float Roll
	{
		get => _roll;
		set => SetRotation(_pitch, _yaw, value);
	}

	public Vector3 Right => ihat;
	public Vector3 Up => jhat;
	public Vector3 Forward => khat;

	float _pitch;
	float _yaw;
	float _roll;

	Vector3 ihat;
	Vector3 jhat;
	Vector3 khat;
	Vector3 ihat_inv;
	Vector3 jhat_inv;
	Vector3 khat_inv;

	public Transform()
	{
		UpdateBasisVectors();
	}

	public void SetPosRotScale(Vector3 pos, Vector3 angles, Vector3 scale)
	{
		Position = pos;
		Scale = scale;
		SetRotation(angles.X, angles.Y, angles.Z);
	}

	public void SetRotation(float pitch, float yaw, float roll)
	{
		_pitch = pitch;
		_yaw = yaw;
		_roll = roll;
		UpdateBasisVectors();
	}


	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public Vector3 ToWorldPoint(Vector3 localPoint)
	{
		Vector3 p = localPoint;
		p = TransformVector(ihat * Scale.X, jhat * Scale.Y, khat * Scale.Z, p) + Position;
		if (Parent != null) p = Parent.ToWorldPoint(p);
		return p;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public Vector3 ToLocalPoint(Vector3 worldPoint)
	{
		Vector3 p = worldPoint;
		if (Parent != null) p = Parent.ToLocalPoint(p);
		p = TransformVector(ihat_inv, jhat_inv, khat_inv, p - Position);
		p.X /= Scale.X;
		p.Y /= Scale.Y;
		p.Z /= Scale.Z;
		return p;
	}
	
	void UpdateBasisVectors()
	{
		(ihat, jhat, khat) = GetBasisVectors();
		(ihat_inv, jhat_inv, khat_inv) = GetInverseBasisVectors();
	}
	
	// Calculate right/up/forward vectors (î, ĵ, k̂)
	(Vector3 ihat, Vector3 jhat, Vector3 khat) GetBasisVectors()
	{
		// ---- Yaw ----
		Vector3 ihat_yaw = new(Cos(Yaw), 0, Sin(Yaw));
		Vector3 jhat_yaw = new(0, 1, 0);
		Vector3 khat_yaw = new(-Sin(Yaw), 0, Cos(Yaw));
		// ---- Pitch ----
		Vector3 ihat_pitch = new(1, 0, 0);
		Vector3 jhat_pitch = new(0, Cos(Pitch), -Sin(Pitch));
		Vector3 khat_pitch = new(0, Sin(Pitch), Cos(Pitch));
		// ---- Roll ----
		Vector3 ihat_roll = new(Cos(Roll), Sin(Roll), 0);
		Vector3 jhat_roll = new(-Sin(Roll), Cos(Roll), 0);
		Vector3 khat_roll = new(0, 0, 1);
		// ---- Yaw and Pitch combined ----
		Vector3 ihat_pitchYaw = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, ihat_pitch);
		Vector3 jhat_pitchYaw = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, jhat_pitch);
		Vector3 khat_pitchYaw = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, khat_pitch);
		// Combine roll
		Vector3 ihat = TransformVector(ihat_pitchYaw, jhat_pitchYaw, khat_pitchYaw, ihat_roll);
		Vector3 jhat = TransformVector(ihat_pitchYaw, jhat_pitchYaw, khat_pitchYaw, jhat_roll);
		Vector3 khat = TransformVector(ihat_pitchYaw, jhat_pitchYaw, khat_pitchYaw, khat_roll);
		return (ihat, jhat, khat);
	}

	(Vector3 ihat, Vector3 jhat, Vector3 khat) GetInverseBasisVectors()
	{
		(Vector3 ihat, Vector3 jhat, Vector3 khat) = GetBasisVectors();
		Vector3 ihat_inverse = new(ihat.X, jhat.X, khat.X);
		Vector3 jhat_inverse = new(ihat.Y, jhat.Y, khat.Y);
		Vector3 khat_inverse = new(ihat.Z, jhat.Z, khat.Z);
		return (ihat_inverse, jhat_inverse, khat_inverse);
	}


	// Move each coordinate of given vector along the corresponding basis vector
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	static Vector3 TransformVector(Vector3 ihat, Vector3 jhat, Vector3 khat, Vector3 v)
	{
		return v.X * ihat + v.Y * jhat + v.Z * khat;
	}
}