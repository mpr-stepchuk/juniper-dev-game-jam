using Godot;
using System;

public partial class Card : Area2D
{
	[Export] public Vector2 Direction;
	[Export] public float _speed;

	[Export] public AttackData AtkData;
	
	private AnimatedSprite2D _animatedSprite2D;
	private Vector2 _velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AreaEntered += OnAreaEntered;
	}

	public void Init(Vector2 origin, Vector2 dir)
	{
		Direction = dir;
		GlobalPosition = origin;
		if (dir.X < 0)
		{
			_animatedSprite2D.SetFlipH(true);
		}
		GD.Print("Created card @ "+GlobalPosition+", moving "+Direction);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += _speed * Direction * (float) delta;
	}
	
	private void OnAreaEntered (Area2D area) {
		
		if (area is not HurtBox hurtbox)
			return;
		
		if (hurtbox.Friendly){
			GD.Print ("CARD HIT PLAYER");
			hurtbox.ApplyHit(AtkData.Damage, AtkData.KnockbackForce, AtkData.KnockbackAngle, AtkData.KnockbackDuration);
			GD.Print("destroying card");
			QueueFree();
		}

		
	}
}
