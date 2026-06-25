using Godot;
using System;

using Enemies;

public abstract partial class Enemy : Actor
{
	
	protected EnemyStats _stats;
	protected Player _player; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		AddToGroup("Enemy");
		GD.Print($"{Name} added to Enemy group");
		GD.Print(IsInGroup("Enemy"));
		_facingRight = false;
		InitStats();
		// TODO: rewrite this so it doesn't rely on a certain node tree structure
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		// debug
		if (_player != null) {
			GD.Print("successfully passed player reference to enemy");
		} else {
			GD.PrintErr("ERR: could not find player reference in node tree to pass to enemy");
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void ApplyHit(int damage, float force, float angle, float duration) {
		GD.Print("HIT!");
		GD.Print("HP was: "+_stats.Guts);
		_stats.Guts -= damage;
		GD.Print("HP now: "+_stats.Guts);
		if (_stats.Guts < _stats.MinGuts) {
			OnDeath();
		}
		ApplyKnockback(force, angle, duration);
		
	}
	
	public Vector2 DistanceToPlayer() {
		Vector2 distance = new Vector2(0.0f, 0.0f);
		if (_player == null) {
			GD.PrintErr("ERR(DistanceToPlayer): enemy entity has no reference to player");
			return distance;
		}
		
		distance.X = _player.GlobalPosition.X - GlobalPosition.X;
		distance.Y = _player.GlobalPosition.Y - GlobalPosition.Y;
		return distance;
	}
	
	public bool FacingPlayer () {
		if (DistanceToPlayer().X < 0) { // player to the left
			if (_player.FacingRight() && !_facingRight) {
				return true;
			} else {
				return false;
			}
		} else { // player to the right 
			if (!_player.FacingRight() && _facingRight) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	protected void FacePlayer() {
		if (_player != null) {
			if (_player.GlobalPosition.X > GlobalPosition.X) {
				_animatedSprite2D.SetFlipH(true);
				_facingRight = true;
			} else {
				_animatedSprite2D.SetFlipH(false);
				_facingRight = false;
			}
		} else {
			GD.PrintErr("No player found");
		}
	}
	
	protected virtual void Flip () {
		if (_facingRight) {
			_animatedSprite2D.SetFlipH(false);
			_facingRight = false;
		} else {
			_animatedSprite2D.SetFlipH(true);
			_facingRight = true;
		}
	}
	
	public abstract void OnDeath();
	
	protected abstract void InitStats();
}
