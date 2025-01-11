using Godot;
using System;

public partial class PlayerCamera : Camera2D
{	
	[Export]
	public float decay = 0.8f;

	[Export]
	public Vector2 maxOffset = new Vector2(10, 5);

	[Export]
	public float maxRoll = 0.1f;

	private float _trauma = 0.0f;
	private int _traumaPower = 2;

	private FastNoiseLite _noise = new FastNoiseLite();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Randomize();
		_noise.Seed = (int)GD.Randi();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (_trauma > 0) {
			_trauma = Mathf.Max(_trauma - decay * (float)delta, 0.0f);
			Shake();
		}
	}

	public void AddTrauma(float amount) {
		_trauma = Mathf.Min(_trauma + amount, 1.0f);
	}

	public void Shake() {
		double amount = Math.Pow(_trauma, _traumaPower);
		Rotation = (float)(maxRoll * amount * GD.RandRange(-1, 1));
		var x = new Vector2();
		x.X = (float)(maxOffset.X * amount * GD.RandRange(-1, 1));
		x.Y = (float)(maxOffset.Y * amount * GD.RandRange(-1, 1));

		Offset = x;
	}

	public void OnPlayerShipCollided(float amount) {
		// Must meet threshold
		if (amount < 0.25 || _trauma > 0.5) return;
		amount *= 0.75f;
		GD.Print(amount);
		AddTrauma(Mathf.Min(0.75f - _trauma,amount));
	}

}
