using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security;

public partial class Playership : CharacterBody2D
{
	[Signal]
	public delegate void CollidedEventHandler(float speed);
	[Export]
	public float maxSpeed {get; set;} = 300f;
	[Export]
	public float acceleration {get; set;} = 10f;
	[Export]
	public float rotationSpeed {get; set;} = 5f;

	private bool _allowCameraShake = true;

	public override void _Ready() {
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Set Acceleration and Rotation respectively.
		var newAcceleration = new Vector2(1,0) * Input.GetAxis("down", "up") * acceleration;
		var sideAcceleration = new Vector2(0,1) * Input.GetAxis("left", "right") * acceleration/4;
		var targetAngle = Position.AngleToPoint(GetGlobalMousePosition()) - Rotation;

		if (Mathf.Abs(targetAngle) < rotationSpeed * (float)delta) {
			LookAt(GetGlobalMousePosition());
		} else {
			var _multiplier = 1;
			if ((targetAngle > 0 && targetAngle > Math.PI) || (targetAngle < 0 && targetAngle > -1 * Math.PI)) {
				_multiplier = -1;
			}
			Rotation += _multiplier * rotationSpeed * (float)delta;
		}	

		newAcceleration = newAcceleration.Rotated(Rotation);
		sideAcceleration = sideAcceleration.Rotated(Rotation);

		// Constrain Velocity to maxSpeed.
		Velocity = (Velocity + newAcceleration + sideAcceleration).LimitLength(maxSpeed);

		// Disable Particles if not moving.
		var Particles = GetNode<CpuParticles2D>("CPUParticles2D");
		//Particles.InitialVelocityMin = Velocity.Length() / 7;
		//Particles.InitialVelocityMax = Velocity.Length() / 5;
		Particles.Emitting = (Velocity.Length() < acceleration) ? false : true;

		// Process Decceleration.
		if (Input.IsActionPressed("stop")) {
			if (!Velocity.IsZeroApprox()) {
				Velocity += -1 * Velocity.LimitLength(acceleration);;
			} else {
				Velocity = Vector2.Zero;
			}
		}
		//GD.Print(Rotation + " | " + Velocity.X + ", " + Velocity.Y);
		var collision = MoveAndCollide(Velocity * (float)delta);
		if (collision != null) {
			// Emit Collision signal
			EmitSignal(SignalName.Collided, Velocity.Length() / maxSpeed);
			_allowCameraShake = false;
			var timer = GetNode<Timer>("CollisionTimer");
			timer.Start();

			// Bounce off of collided object
			Velocity = Velocity.Bounce(collision.GetNormal()) / 2;
		}
	}
}
