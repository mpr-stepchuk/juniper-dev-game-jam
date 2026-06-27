using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class WaveManager : Node
{
	
	[Signal] public delegate void WaveStartEventHandler();
	[Signal] public delegate void WaveEndEventHandler();
	
	[Export] public PackedScene WalkerScene { get; set; }
	[Export] public PackedScene FlyerScene { get; set; }
	[Export] public CanvasLayer WheelLayer { get; set; }
	
	public List<Spawner> _spawners;
	public int EnemiesLeft; 
	public int WaveCount;
	private Random _rand;
	private bool _waveActive;
	private Node _wheelLayer;
	
	// Called when the node enters the scene tree for the first time.
	public void OnPlayerSpawn()
	{
		WaveCount = 0;
		_spawners = GetChildren().OfType<Spawner>().ToList();
		_rand = new Random();
		_waveActive = false;
		StartWave();
		_wheelLayer = GetTree().GetFirstNodeInGroup("WheelLayer");
		_wheelLayer.Connect("end_wheel", new Callable(this, MethodName.OnEndWheel));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		EnemiesLeft = GetTree().GetNodesInGroup("Enemy").Count;
		
		if (EnemiesLeft == 0 && _waveActive) {
			_waveActive = false;
			EmitSignal(SignalName.WaveEnd);
			GD.Print("EMITTING WAVE END");
		}

		if (Input.IsActionJustPressed("debug_kill_all_enemies"))
			KillAllEnemies();


	}
	
	public void RandomizeSpawns() {
		foreach (Spawner s in _spawners) {
			int num = _rand.Next(2);
			switch(num) {
				case 0:
					s.ActorScene = WalkerScene;
					break;
				case 1:
					s.ActorScene = FlyerScene;
					break;
			}
		}
	}
	
	public void OnEndWheel() {
		GD.Print("RECEIVED END WHEEL");
		StartWave();
	}
	
	public void StartWave() {
		WaveCount++;
		GD.Print("Starting wave "+WaveCount);
		RandomizeSpawns();
		EmitSignal(SignalName.WaveStart);
		_waveActive = true;
	}
	
	
	public void KillAllEnemies() {
		var enemies = GetTree().GetNodesInGroup("Enemy").Cast<Enemy>();
		foreach(Enemy e in enemies) {
			e.OnDeath();
		}
	}
	
}
