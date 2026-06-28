using Godot;
using System;
using System.Collections.Generic;

using Enemies;

public partial class Boss : Enemy
{
	
	public Timer ThrowTimer;
	public Timer StunTimer;
	public Timer TPTimer;

	public Timer BarrageTimer;
	
	public Random _rand;

	public BossSM SM; 

	public bool _barrage;
	public bool _barrageDone;
	public bool LastBarrageRight = false;
	public int _barrageBreakpoint;
	public float _detectionRad;

	public Vector2 TPDest;
	public Vector2 PosBarrageL;
	public Vector2 PosBarrageR;

	public Marker2D EvadeL;
	public Marker2D EvadeR;

	public Vector2 PosEvadeL;
	public Vector2 PosEvadeR;

	public Area2D Hitbox;
	public CollisionShape2D HitboxDim;

	public PackedScene TPCardScene;

	public PackedScene CardScene;


	// stores directions of cards to be thrown
	Stack<Vector2> cardStack;

	public int BoundX;
	[Export] public AttackData AtkData;

	[Signal] public delegate void FinishedThrowEventHandler();
	[Signal] public delegate void FinishedTeleportEventHandler();
	[Signal] public delegate void FinishedBarrageShotEventHandler();

	[Signal] public delegate void FinishedStrikeEventHandler();

	[Signal] public delegate void FinishedBarrageEventHandler();


	public override void _Ready() {
		base._Ready();
		
		ThrowTimer = GetNode<Timer>("ThrowTimer"); 
		StunTimer = GetNode<Timer>("StunTimer"); 
		BarrageTimer = GetNode<Timer>("BarrageTimer");
		SM = GetNode<BossSM>("BossSM");
		SM.Init();

		InitStats();
		_rand = new Random();
		_barrage = false;
		_facingRight = true;
		_detectionRad = 75.0f;
		EvadeL = GetParent().GetNode<Marker2D>("MarkerL");
		EvadeR = GetParent().GetNode<Marker2D>("MarkerR");
		PosEvadeL = EvadeL.GlobalTransform.Origin; 
		PosEvadeR = EvadeR.GlobalTransform.Origin; 
		cardStack = new Stack<Vector2>();
		BoundX = 200;

		TPCardScene = GD.Load<PackedScene>("res://scenes/TPCard.tscn");
		CardScene = GD.Load<PackedScene>("res://scenes/Card.tscn");
		Hitbox = GetNode<Area2D>("HitBox");
		HitboxDim = GetNode<CollisionShape2D>("HitBox/CollisionShape2D");

	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		ProcessKnockback(delta, ref velocity);
		
		if (SM.CurrentState.Name == "Idle" || SM.CurrentState.Name == "Stunned")
			{
			HandleGravity(delta, ref velocity);
		}

		if (Input.IsActionJustPressed("debug_trigger_barrage"))
		{
			_stats.Guts = 50;
		}

		Velocity = velocity;
		MoveAndSlide();

	}
	

	public void TeleportEvade()
	{
		GD.Print("TeleportEvade()");
		float distL = PosEvadeL.DistanceTo(GlobalTransform.Origin);
		float distR = PosEvadeR.DistanceTo(GlobalTransform.Origin);

		if (distL < distR)
		{
			Teleport(PosEvadeR);
		}
		else
		{
			Teleport(PosEvadeL);
		}

	}

	public void TeleportBarrage()
	{
		GD.Print("TeleportBarrage() -> LastBarrageRight = "+LastBarrageRight);
		if (!LastBarrageRight)
		{
			
			if (PosBarrageR.X < BoundX)
			{
				GD.Print("Right TP OK");
				Teleport(PosBarrageR);
				LastBarrageRight = true;
			}
			else
			{
				GD.Print("Right TP OOB, going left");
				Teleport(PosBarrageL);
				LastBarrageRight = false;
			}
		}
		else
		{
			
			if (PosBarrageL.X > -BoundX)
			{
				GD.Print("Left TP OK");
				Teleport(PosBarrageL);
				LastBarrageRight = false;
			}
			else
			{
				GD.Print("Left TP OOB, going left");
				Teleport(PosBarrageR);
				LastBarrageRight = true;
			}
		}
	}

	public void Teleport(Vector2 dest)
	{
		GD.Print($"Teleport(): {GlobalPosition} -> {dest}");
		TPDest = dest;
		
		TPCard srcCard = TPCardScene.Instantiate<TPCard>();
		srcCard.Init(true, GlobalPosition);

		TPCard destCard = TPCardScene.Instantiate<TPCard>();
		destCard.Init(false, dest);

		_animatedSprite2D.Play("teleport");
		_animationPlayer.Play("teleport");

		GetParent().AddChild(srcCard);
		GetParent().AddChild(destCard);
	}

	public void OnTeleportFinished()
	{
		GlobalPosition = TPDest;
		EmitSignal(SignalName.FinishedTeleport);
	}

	public void ThrowCone()
	{
		GD.Print("ThrowCone()");
		Vector2 dir = DistanceToPlayer().Normalized();
		Vector2 dirUp = dir.Rotated(Mathf.DegToRad(10));
		Vector2 dirDown = dir.Rotated(Mathf.DegToRad(-10));


		QueueCard(dir);
		QueueCard(dirUp);
		QueueCard(dirDown);

		_animatedSprite2D.Play("throw");
		_animationPlayer.Play("throw");

	}

	public void ThrowStraight()
	{
		GD.Print("ThrowStraight()");
		Vector2 dir;
		if (DistanceToPlayer().X < 0)
		{
			dir = Vector2.Left;
		}
		else
		{
			dir = Vector2.Right;
		}
		QueueCard(dir);
		
		_animatedSprite2D.Play("throw");
		_animationPlayer.Play("throw");
	}

	public void ThrowBarrage()
	{
		GD.Print("ThrowBarrage()");
		QueueCard(DistanceToPlayer().Normalized());
		_animatedSprite2D.Play("throw");
		_animationPlayer.Play("barrage");	
	}

	public void QueueCard (Vector2 dir)
	{
		GD.Print($"QueueCard{dir}");
		cardStack.Push(dir);
	}

	public void ThrowCard()
	{
		GD.Print("ThrowCard");
		GD.Print("GlobalPosition = "+GlobalPosition);
		foreach(Vector2 v in cardStack)
		{
			Card card = CardScene.Instantiate<Card>();
			GetParent().AddChild(card);
			card.Init(GlobalPosition, v);
			
		}
		cardStack.Clear();
	}

	public void OnFinishedThrow()
	{
		EmitSignal(SignalName.FinishedThrow);
	}

	public void OnFinishedBarrageShot()
	{
		EmitSignal(SignalName.FinishedBarrageShot);
	}

	public void HandleStrike()
	{
		GD.Print("HandleStrike");
		_animatedSprite2D.Play("strike");
		_animationPlayer.Play("strike");
		_knockbackable = true;
		BuildHitbox();

	}

	public bool InKnockback()
	{
		return _inKnockback;
	}

	private void BuildHitbox() {
		Vector2 offset = AtkData.HitboxOffset;
		if (!_facingRight) {
			offset.X *= -1;
			AtkData.KnockbackAngle = 180 - AtkData.KnockbackAngle;
			
		} 
		
		if (_facingRight && AtkData.KnockbackAngle > 90) 
			AtkData.KnockbackAngle = 180 - AtkData.KnockbackAngle;
			
		Hitbox.Position = offset;
		HitboxDim.Scale = AtkData.HitboxScale;
	}
	private void OnAttackAreaEntered(Area2D area) {
		GD.Print("ENTITY DETECTED IN HITBOX");
		
		if (area is not HurtBox hurtbox)
			return;
			
		if (hurtbox.Friendly) {
			hurtbox.ApplyHit(AtkData.Damage, AtkData.KnockbackForce, AtkData.KnockbackAngle, AtkData.KnockbackDuration);
		}
	}
	public void EnableHitbox()
	{
		_knockbackable = false;
		HitboxDim.Disabled = false;
	}

	public void DisableHitbox()
	{
		HitboxDim.Disabled = true;
	}

	public void OnStrikeFinished()
	{
		if (SM.CurrentState.Name == State.Striking)
		{
			GD.Print("Strike finished. Emitting Boss.FinishedStrike");
			EmitSignal(SignalName.FinishedStrike);
		}
	}

	public virtual void Flip () {
		GD.Print("Boss.Flip()");
		if (_facingRight) {
			_animatedSprite2D.SetFlipH(true);
			_facingRight = false;
		} else {
			_animatedSprite2D.SetFlipH(false);
			_facingRight = true;
		}
	}

	protected override void InitStats() {
		_stats = new EnemyStats{
			
			CashValue = 20000, 
			MaxGuts = 200,
			MinGuts = 1
			
		};
		
		_stats.Guts = _stats.MaxGuts;
		_barrageBreakpoint = 50;
	}
	
	public override void OnDeath() {
		GD.Print("Boss died");
		_player.AddCash(_stats.CashValue);
		QueueFree();
	}
}
