using Godot;
using System;

public partial class Spawner : Marker2D
{
	[Export] public PackedScene ActorScene;
	private WaveManager _wm;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_wm = GetParent<WaveManager>();
		_wm.WaveStart += OnWaveStart;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnWaveStart() {
		GD.Print("Wave Started!");
		if (ActorScene != null) {
			Actor actor = ActorScene.Instantiate<Actor>();
			actor.GlobalTransform = GlobalTransform;
			GetParent().AddChild(actor);
		}
	}
}
