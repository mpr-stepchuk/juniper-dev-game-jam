using Godot;
using System;
using System.Collections.Generic;

using Dizzy;

public partial class Player : Actor
{
	public const float Speed = 450.0f;
	public const float JumpVelocity = -800.0f;
	public const float DashDistance = 200.0f;
	public const float DashDuration = 0.15f;
	public const float DashSpeed = DashDistance / DashDuration;
	
	private DizzyStats stats;
	private bool _isDashing = false;
	private bool _isAttacking = false;
	private bool _canAirDash = true;
	private float _attackTimer = 0.0f;
	private float _dashTimer = 0.0f;
	
	private Vector2 _dashDirection;
	// tracks all hurtboxes hit by a hitbox while it's active to prevent multi-hits
	private HashSet<HurtBox> _hurtboxesHit;
	
	private AttackData _currentAttack;
	
	// TODO: preload this instead
	[Export] public AttackData AtkLight;
	[Export] public AttackData AtkHeavy;
	
	private AudioStreamPlayer2D _testSFX;
	private Area2D _areaAtk;
	private CollisionShape2D _hitboxAtk;
	
	
	
	public override void _Ready()
	{
		// fetch nodes
		base._Ready();
		_testSFX = GetNode<AudioStreamPlayer2D>("testSFX");
		_areaAtk = GetNode<Area2D>("AnimatedSprite2D/AttackArea");
		_hitboxAtk = GetNode<CollisionShape2D>("AnimatedSprite2D/AttackArea/AttackHitbox"); 
		// connect signals
		_areaAtk.AreaEntered += OnAttackAreaEntered;
		//instantiate objects
		_hurtboxesHit = new();
		InitStats();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("left", "right", "up", "down"); 
		CheckFlipped(ref direction);
		HandleGravity(delta, ref velocity);
		HandleAttack(delta);
		
		HandleMovement(ref direction, ref velocity, delta);
		
		ProcessKnockback(delta, ref velocity);
		
		Velocity = velocity;
		if (Input.IsActionPressed("down")) {
			SetCollisionMaskValue(5, false);
		} else {
			SetCollisionMaskValue(5, true);
		}
		MoveAndSlide();
		
		HandleAnimation(direction, velocity);
		
	}
	
	private void InitStats() {
		stats = new DizzyStats {
	
		// modifiers, % values represented as scalar factors
		Hustle = 1.0f,
		CritChance = 0.05f,
		CritMod = 1.2f,
		Steadiness = 1.0f,
		Coordination = 1.0f,
		
		// max values
		MaxGuts = 100,
		MaxSpins = 100,
		MaxSuper = 100,
		MaxHustle = 1.5f,
		MaxCritChance = 2.0f,
		MaxCritMod = 2.0f,
		MaxSteadiness = 2.0f,
		MaxCoordination = 2.0f,
		
		// min values
		MinGuts = 1,
		MinSpins = 1,
		MinSuper = 1,
		MinHustle = 1.0f,
		MinCritChance = 0.0f,
		MinCritMod = 1.0f,
		MinSteadiness = 0.85f,
		MinCoordination = 0.85f,
		
		};
		
		// metered stats
		stats.Guts = stats.MaxGuts;
		stats.Spins = stats.MinSpins;
		stats.Super = stats.MinSuper;
		
	}
	
	private void CheckFlipped(ref Vector2 direction) {
		if (direction.X < 0 && _facingRight) {
			_facingRight = false;
		} else if (direction.X > 0 && !_facingRight) {
			_facingRight = true;
		} 
	}
	
	private void HandleAttack(double delta) {		
		if (_isAttacking)
			return;
		if (Input.IsActionJustPressed("atk_light")) {
			_isAttacking = true;
			_currentAttack = AtkLight;
		} else if (Input.IsActionJustPressed("atk_heavy")) {
			_isAttacking = true;
			_currentAttack = AtkHeavy;
		}
		if (_isAttacking) {
			BuildHitbox();
			_animatedSprite2D.Play(_currentAttack.AnimationName); 
			_animationPlayer.Play(_currentAttack.AnimationName);
		} 
	}
	// CHANGE NAME OF ENABLE DISABLE BOX ONCE THIS WORKS SO ANIMATION PLAYER DOESN'T BRICK
	private void BuildHitbox() {
		Vector2 offset = _currentAttack.HitboxOffset;
		if (!_facingRight) {
			offset.X *= -1;
		}
		_areaAtk.Position = offset;
		_hitboxAtk.Scale = _currentAttack.HitboxScale;
	}
	
	private void OnAttackAreaEntered(Area2D area) {
		GD.Print("ENTITY DETECTED IN HITBOX");
		
		if (area is not HurtBox hurtbox)
			return;
			
		if (_hurtboxesHit.Contains(hurtbox))
			return;
			
		if (!hurtbox.Friendly) {
			_hurtboxesHit.Add(hurtbox);
			hurtbox.ApplyHit(_currentAttack.Damage, _currentAttack.KnockbackForce, _currentAttack.KnockbackAngle, _currentAttack.KnockbackDuration);
		}
	}
	
	public void EnableHitbox() {	
		_hurtboxesHit.Clear();
		_hitboxAtk.Disabled = false;
		_testSFX.Play();
	}
	
	public void DisableHitbox() {
		_hitboxAtk.Disabled = true;
	}
	
	public void AttackFinished() {
		_attackTimer = 0.0f;
		_isAttacking = false;
	}
	
	
	private void HandleGravity (ref Vector2 velocity, double delta) {
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
	}
	
	private void HandleMovement (ref Vector2 direction, ref Vector2 velocity, double delta) {
		
		// Handle Dash
		if (Input.IsActionJustPressed("dash") && !_isDashing && !IsOnFloor() && _canAirDash) {
			if (direction == Vector2.Zero) {
				direction = _animatedSprite2D.FlipH ? Vector2.Left : Vector2.Right;
			}
			_dashDirection = direction;
			_dashTimer = DashDuration;
			
			_canAirDash = false; 
			_isDashing = true;
			GD.Print("dashing!");
			
		}
		if (_isDashing) {
			_dashTimer -= (float)delta;
			velocity = _dashDirection * DashSpeed;
			GD.Print(velocity);
			if (_dashTimer <=0) {
				_isDashing = false;
				GD.Print("not dashing.");
			}
			return;
		}
		
		// check if grounded to reset air actions, just airdash for now
		if (IsOnFloor()) {
			_canAirDash = true;
		}
		
		// Handle Jump.
		if (Input.IsActionJustPressed("up") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if (direction.X != 0.0f)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		
	}
	
	private void HandleAnimation(Vector2 direction, Vector2 velocity) {
		if (_isAttacking) {
			return;
		}
		if (velocity.X > 1 || velocity.X < -1) {
			_animatedSprite2D.Play("run");
		} else {
			_animatedSprite2D.Play("idle");
		}
		
		if (!IsOnFloor()) {
			if (velocity.Y > 0) {
				_animatedSprite2D.Play("fall");
			} else {
				_animatedSprite2D.Play("jump");
			}
		}
		
		if (_facingRight) {
			_animatedSprite2D.SetFlipH(false);
		} else if (!_facingRight) {
			_animatedSprite2D.SetFlipH(true);
		}
	}
}
