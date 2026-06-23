using Godot;
using System;

using Enemies;

public abstract partial class Enemy : Actor
{
	
	protected EnemyStats stats;
	private Player Player; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitStats();
		// TODO: rewrite this so it doesn't rely on a certain node tree structure
		Player = GetTree().GetFirstNodeInGroup("Player") as Player;
		// debug
		GD.Print(Player.Speed);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void ApplyHit(int damage, float force, float angle, float duration) {
		GD.Print("HIT!");
		GD.Print("HP was: "+stats.Guts);
		stats.Guts -= damage;
		GD.Print("HP now: "+stats.Guts);
		if (stats.Guts <= stats.MinGuts) {
			OnDeath();
		}
		ApplyKnockback(force, angle, duration);
		
	}
	
	protected void FacePlayer() {
		if (Player != null) {
			if (Player.GlobalPosition.X > GlobalPosition.X) {
				_animatedSprite2D.SetFlipH(true);
			} else {
				_animatedSprite2D.SetFlipH(false);
			}
		} else {
			GD.PrintErr("No player found");
		}
	}
	
	public abstract void OnDeath();
	
	public abstract void InitStats();
}
