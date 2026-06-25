using Godot;
using System;

using Enemies;

public partial class Flyer : Enemy
{
	
	[Export] public AttackData AttackData;
	
	private RayCast2D _rayDown;
	private RayCast2D _rayUp;
	private RayCast2D _rayRight;
	private RayCast2D _rayLeft;
	private Area2D _hitbox;
	private CollisionShape2D _hitboxDim;
	
	private bool _chasing;
	private float _speed;
	private float _chaseSpeed;
	private float _detectionRad;
	private Vector2 _direction;
	private Random _rand;
	
	public override void _Ready() {
		_knockbackable = true;
		base._Ready();
		_rand = new Random();
		_detectionRad = 200.0f;
		
		_rayDown = GetNode<RayCast2D>("RayDown");           
		_rayUp = GetNode<RayCast2D>("RayUp");
		_rayLeft = GetNode<RayCast2D>("RayLeft");
		_rayRight = GetNode<RayCast2D>("RayRight");
		_hitbox = GetNode<Area2D>("HitBox");
		_hitboxDim = GetNode<CollisionShape2D>("HitBox/CollisionShape2D");
		_hitbox.AreaEntered += OnAttackAreaEntered;
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
			
		PlayerDetected();
		HandleBehaviour(delta, ref velocity);
		
		ProcessKnockback(delta, ref velocity);
		
		HandleAnimation(delta, ref velocity);

		Velocity = velocity;
		MoveAndSlide();
	}
	
	private bool PlayerDetected() {
		Vector2 distance = DistanceToPlayer();
		if (distance.Length() <= _detectionRad) {
			// GD.Print("PLAYER DETECTED");
			return true;
		} else {
			return false;
		}
	}
	
	private void HandleBehaviour(double delta, ref Vector2 velocity) {
		if (PlayerDetected()) { 
			_chasing = true;
		} else {
			_chasing = false;
		}
			
		if (_chasing) {
			HandleChase(delta, ref velocity);
		} else {
			HandleWander(delta, ref velocity);
		}
		
	}
	
	private void HandleWander(double delta, ref Vector2 velocity) {
		
		Vector2 dir = Vector2.Zero;
		
		while (dir == Vector2.Zero) {
			int rand = _rand.Next(4);
			switch(rand){
				case 0:
					if(!_rayUp.IsColliding()){
						dir = Vector2.Up;
					}
					break;
				case 1:
					if(!_rayDown.IsColliding()){
						dir = Vector2.Down;
					}
					break;
				case 2:
					if(!_rayLeft.IsColliding()){
						dir = Vector2.Left;
					}
					break;
				case 3:
					if(!_rayRight.IsColliding()){
						dir = Vector2.Right;
					}
					break;
			}
		}
		
		_direction = dir;
		velocity = _direction * _speed;
		
	}
	
	private void HandleChase(double delta, ref Vector2 velocity) {
		_direction = DistanceToPlayer().Normalized();
		velocity = _direction * _chaseSpeed;
	}
	
	private void HandleAnimation(double delta, ref Vector2 velocity) {
		if (DistanceToPlayer().X < 0) {
			_facingRight = false;
		} else {
			_facingRight = true;
		}
		
		if (_facingRight && _animatedSprite2D.FlipH == false) {
			_animatedSprite2D.SetFlipH(true);
		} else if (!_facingRight && _animatedSprite2D.FlipH == true) {
			_animatedSprite2D.SetFlipH(false);
		}
		
		
		_animatedSprite2D.Play("idle");
	}
	
	private void OnAttackAreaEntered(Area2D area) {
		GD.Print("ENTITY DETECTED IN FLYER HITBOX");
		
		if (area is not HurtBox hurtbox)
			return;
			
		if (hurtbox.Friendly) {
			hurtbox.ApplyHit(AttackData.Damage, AttackData.KnockbackForce, AttackData.KnockbackAngle, AttackData.KnockbackDuration);
		}
	}
	
	protected override void InitStats() {
		_stats = new EnemyStats{
			Guts = 50,
			CashValue = 2000,
			MinGuts = 1,
			MaxGuts = 50
		};
		_speed = 15.0f;
		_chaseSpeed = 60;
	}
	
	public override void OnDeath() {
		GD.Print("Flyer died");
		_player.AddCash(_stats.CashValue);
		QueueFree();
	}
	
}
