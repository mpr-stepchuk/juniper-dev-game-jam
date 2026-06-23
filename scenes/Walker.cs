using Godot;
using System;

public partial class Walker : Enemy
{
	
	[Export] public AttackData data;
	
	public const float speed = 250.0f;
	
	private bool _playerDetected = false;
	private bool _inAttackRange = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public override void InitStats() {
		return;
	}
	
	public override void OnDeath() {
		return;
	}
}
