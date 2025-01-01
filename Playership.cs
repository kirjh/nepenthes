using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security;

public partial class Playership : CharacterBody2D
{
	[Export]
	public float MaxSpeed {get; set;} = 300f;
	[Export]
	public float Acceleration {get; set;} = 10f;
	[Export]
	public float RotationSpeed {get; set;} = 5f;
	private float _rotationDirection;

	private float Accelerate(string throttleKey, string dethrottleKey) {
		float _speed = Mathf.Clamp(Input.GetAxis(dethrottleKey, throttleKey) * Acceleration, -1 * MaxSpeed, MaxSpeed);
		return _speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		var newAcceleration = new Vector2(1,0) * Acceleration;
		_rotationDirection = Input.GetAxis("left", "right");
		Rotation += _rotationDirection * RotationSpeed * (float)delta;

		Velocity = (Velocity + newAcceleration.Rotated(Rotation) * Input.GetAxis("down", "up")).LimitLength(MaxSpeed);

		if (Input.IsActionPressed("stop")) {
			if (!Velocity.IsZeroApprox()) {
				Velocity += -1 * Velocity.LimitLength(Acceleration);;
			} else {
				Velocity = Vector2.Zero;
			}
		}
		//GD.Print(Rotation + " | " + Velocity.X + ", " + Velocity.Y);
		MoveAndSlide();
	}
}
