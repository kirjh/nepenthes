using Godot;
using System;

public partial class PlayerCamera : Camera2D
{	
	[Export]
	public float Decay = 0.8f;

	[Export]
	public Vector2 MaxOffset = new Vector2(10, 5);

	[Export]
	public float MaxRoll = 0.1f;

	private float _Trauma = 0.0f;
	private int _TraumaPower = 2;

	private FastNoiseLite _Noise = new FastNoiseLite();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Randomize();
		_Noise.Seed = (int)GD.Randi();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (_Trauma > 0) {
			_Trauma = Mathf.Max(_Trauma - Decay * (float)delta, 0.0f);
			Shake();
		}
	}

	public void AddTrauma(float amount) {
		_Trauma = Mathf.Min(_Trauma + amount, 1.0f);
	}

	public void Shake() {
		double amount = Math.Pow(_Trauma, _TraumaPower);
		Rotation = (float)(MaxRoll * amount * GD.RandRange(-1, 1));
		var x = new Vector2();
		x.X = (float)(MaxOffset.X * amount * GD.RandRange(-1, 1));
		x.Y = (float)(MaxOffset.Y * amount * GD.RandRange(-1, 1));

		Offset = x;
	}

	public void OnPlayerShipCollided(float amount) {
		GD.Print(amount);
		AddTrauma(amount);
	}

}
