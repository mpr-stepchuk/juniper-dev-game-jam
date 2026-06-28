using Godot;
using System;

using Enemies;

public partial class Walker : Enemy
{
	
	[Export] public AttackData AttackData;
	
	public const float Speed = 7.5f;
	public const float ChaseSpeed = 50.0f;
	public const float DetectionRad = 140.0f;
	public const float AttackCD = 0.8f;
	
	private bool _playerDetected = false;
	private bool _inAttackRange = false;
	private bool _chasing = false;
	private bool _attacking = false;
	private float _atkTimer = 0.0f;
	private float _atkCD = 2.5f;
	private float _flipTimer = 0.0f;
	private float _flipCD = 0.75f;
	private int _direction = -1;
	private float _rayTimer = 0.0f;
	
	private AttackData _atkData;
	private RayCast2D _floorRay;
	private RayCast2D _atkRangeRay;
	private Area2D _hitbox; 
	private CollisionShape2D _hitboxDim;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_knockbackable = true;
		base._Ready();
		
		
		_floorRay = GetNode<RayCast2D>("FloorRay");
		_atkRangeRay = GetNode<RayCast2D>("DetectionRay");
		_hitbox = GetNode<Area2D>("HitBox");
		_hitboxDim = GetNode<CollisionShape2D>("HitBox/CollisionShape2D"); 
		_hitbox.AreaEntered += OnAttackAreaEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		HandleGravity(delta, ref velocity);
		ProcessKnockback(delta, ref velocity);
		
		PlayerDetected();
		HandleBehaviour(delta, ref velocity);
		
		if (_rayTimer > 0) {
			_rayTimer -= (float) delta;
		}
		if (_rayTimer < 0) {
			_rayTimer = 0.0f;
		}
		
		if (_flipTimer > 0) {
			_flipTimer -= (float) delta;
		}
		if (_flipTimer < 0) {
			_flipTimer = 0.0f;
		}
		
		HandleAnimation(delta, ref velocity);

		Velocity = velocity;
		MoveAndSlide();
	}
	
	private void HandleBehaviour(double delta, ref Vector2 velocity) {
		if (!_attacking) {
			
			if (PlayerDetected()) { 
				_chasing = true;
			} else {
				_chasing = false;
			}
			
			if (_chasing) {
				HandleChase(delta, ref velocity);
			} else {
				HandlePatrol(delta, ref velocity);
			}
		}
		
		HandleAttack(delta, ref velocity); 
		if (_atkTimer > 0) {
			_atkTimer -= (float) delta;
			if (_atkTimer < 0) 
				_atkTimer = 0;
		}
	}
	
	private void HandlePatrol(double delta, ref Vector2 velocity) {
		if (IsOnFloor() && _rayTimer == 0.0f && !_floorRay.IsColliding()) {
			Flip();	
			_rayTimer = 2.5f;	
		} else if (IsOnFloor() && IsOnWall()) {
			Flip();
		}
		velocity.X = Speed * _direction; 
	}
	
	private void HandleChase(double delta, ref Vector2 velocity) {
		if ( (_facingRight && DistanceToPlayer().X < 0) || (!_facingRight && DistanceToPlayer().X > 0) ) {
			Flip();
		} else if (IsOnFloor() && IsOnWall()) {
			Flip();
		}
		velocity.X = ChaseSpeed * _direction;
	}
	
	private void HandleAnimation(double delta, ref Vector2 velocity) {
		if (!_attacking) {
			_animatedSprite2D.Play("walk");
		} 
	}
	
	private void HandleAttack(double delta, ref Vector2 velocity) {
		if (_attacking)
			return;
		
		if (_atkRangeRay.IsColliding() && _atkTimer == 0) {
			GD.Print("WALKER ATTACK");
			_attacking = true;
			_hitboxDim.Scale = AttackData.HitboxScale;
			_hitbox.Position = AttackData.HitboxOffset;
			_animationPlayer.Play(AttackData.AnimationName);
			_animatedSprite2D.Play(AttackData.AnimationName);
		}
		
	}
	
	private void OnAttackAreaEntered(Area2D area) {
		GD.Print("ENTITY DETECTED IN WALKER HITBOX");
		
		if (area is not HurtBox hurtbox)
			return;
			
		if (hurtbox.Friendly) {
			hurtbox.ApplyHit(AttackData.Damage, AttackData.KnockbackForce, AttackData.KnockbackAngle, AttackData.KnockbackDuration);
		}
	}
	
	public void EnableHitbox() {
		_hitboxDim.Disabled = false;
	}
	
	public void DisableHitbox() {
		_hitboxDim.Disabled = true;
	}
	
	public void AttackFinished() {
		_attacking = false;
		_atkTimer = _atkCD;
		GD.Print("AttackFinished");
	}
	
	private bool PlayerDetected() {
		Vector2 distance = DistanceToPlayer();
		if (distance.Length() <= DetectionRad) {
			// GD.Print("PLAYER DETECTED");
			return true;
		} else {
			return false;
		}
	}
	
	public override void Flip() {
		if (_flipTimer > 0)
			return;
		
		base.Flip();
		_direction *= -1;
		_hitbox.Position *= -1;
		_floorRay.SetTargetPosition( new Vector2((_floorRay.GetTargetPosition().X * -1), _floorRay.GetTargetPosition().Y));
		_atkRangeRay.SetTargetPosition( new Vector2((_atkRangeRay.GetTargetPosition().X * -1), _atkRangeRay.GetTargetPosition().Y));
		_flipTimer = _flipCD;
	}
	
	protected override void InitStats() {
		_stats = new EnemyStats{
			
			CashValue = 1000, 
			MaxGuts = 100,
			MinGuts = 1
			
		};
		
		_stats.Guts = _stats.MaxGuts;
	}
	
	public override void OnDeath() {
		GD.Print("Walker died");
		_player.AddCash(_stats.CashValue);
		QueueFree();
	}
	
}
