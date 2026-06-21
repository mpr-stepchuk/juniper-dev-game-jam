using Godot;
using System;

public partial class HurtBox : Area2D
{
	[Export] public bool Friendly;
	
	private Sandbag _owner;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_owner = GetParent<Sandbag>();
	}

	public void ApplyHit(int damage, float force, float angle, float duration) {
		_owner.ApplyHit(damage, force, angle, duration);
	}
}
