using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security;

public partial class Playership : CharacterBody2D
{
	[Signal]
	public delegate void CollidedEventHandler(float amount);
	[Export]
	public float MaxSpeed {get; set;} = 300f;
	[Export]
	public float Acceleration {get; set;} = 10f;
	[Export]
	public float RotationSpeed {get; set;} = 5f;

	public override void _Ready() {
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Set Acceleration and Rotation respectively.
		var newAcceleration = new Vector2(1,0) * Input.GetAxis("down", "up") * Acceleration;
		var sideAcceleration = new Vector2(0,1) * Input.GetAxis("left", "right") * Acceleration/4;
		var targetAngle = Position.AngleToPoint(GetGlobalMousePosition()) - Rotation;

		if (Mathf.Abs(targetAngle) < RotationSpeed * (float)delta) {
			LookAt(GetGlobalMousePosition());
		} else {
			var _multiplier = 1;
			if ((targetAngle > 0 && targetAngle > Math.PI) || (targetAngle < 0 && targetAngle > -1 * Math.PI)) {
				_multiplier = -1;
			}
			Rotation += _multiplier * RotationSpeed * (float)delta;
		}	

		newAcceleration = newAcceleration.Rotated(Rotation);
		sideAcceleration = sideAcceleration.Rotated(Rotation);

		// Constrain Velocity to MaxSpeed.
		Velocity = (Velocity + newAcceleration + sideAcceleration).LimitLength(MaxSpeed);

		// Disable Particles if not moving.
		var Particles = GetNode<CpuParticles2D>("CPUParticles2D");
		//Particles.InitialVelocityMin = Velocity.Length() / 7;
		//Particles.InitialVelocityMax = Velocity.Length() / 5;
		Particles.Emitting = (Velocity.Length() < Acceleration) ? false : true;

		// Process Decceleration.
		if (Input.IsActionPressed("stop")) {
			if (!Velocity.IsZeroApprox()) {
				Velocity += -1 * Velocity.LimitLength(Acceleration);;
			} else {
				Velocity = Vector2.Zero;
			}
		}
		//GD.Print(Rotation + " | " + Velocity.X + ", " + Velocity.Y);
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null) {
			EmitSignal(SignalName.Collided, 1f);
			Velocity = Velocity.Bounce(collision.GetNormal()) / 2;
		}
	}
}
