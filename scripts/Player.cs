using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public const float Speed = 450.0f;
	public const float JumpVelocity = -800.0f;
	public const float DashDistance = 200.0f;
	public const float DashDuration = 0.15f;
	public const float DashSpeed = DashDistance / DashDuration;
	public const int MaxGuts = 100;
	public const float MaxSpins = 100.0f;
	
	private bool _isDashing = false;
	private bool _isAttacking = false;
	private bool _facingRight = true;
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
	
	private AnimatedSprite2D _animatedSprite2D;
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _testSFX;
	private Area2D _areaAtk;
	private CollisionShape2D _hitboxAtk;
	
	
	
	public override void _Ready()
	{
		// fetch nodes
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_testSFX = GetNode<AudioStreamPlayer2D>("testSFX");
		_areaAtk = GetNode<Area2D>("AnimatedSprite2D/AttackArea");
		_hitboxAtk = GetNode<CollisionShape2D>("AnimatedSprite2D/AttackArea/AttackHitbox"); 
		// connect signals
		_areaAtk.AreaEntered += OnAttackAreaEntered;
		//instantiate objects
		_hurtboxesHit = new();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("left", "right", "up", "down"); 
		CheckFlipped(ref direction);
		HandleGravity(ref velocity, delta);
		HandleAttack(delta);
		
		HandleMovement(ref direction, ref velocity, delta);
		
		Velocity = velocity;
		MoveAndSlide();
		
		HandleAnimation(direction, velocity);
		
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
