using Godot;
using System;

using Enemies;

public partial class Sandbag : Enemy
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_knockbackable = true;
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		HandleGravity(delta, ref velocity);
		ProcessKnockback(delta, ref velocity);
		
		// debug 
		Vector2 test = new Vector2(0.0f, 0.0f);
		if (_kbVelocity != test) {
			GD.Print(_kbTimer);
		}
		// FacePlayer();
		
		// move when all the calcs are done
		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void InitStats() {
		stats = new EnemyStats{
			
			CashValue = 1000, 
			MaxGuts = 100,
			MinGuts = 1
			
		};
		
		stats.Guts = stats.MaxGuts;
	}
	
	public override void OnDeath() {
		GD.Print("DIED! - resetting guts to "+stats.MaxGuts);
		stats.Guts = stats.MaxGuts;
	}
	
	
}
