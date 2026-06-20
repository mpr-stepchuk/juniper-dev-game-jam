using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 450.0f;
	public const float JumpVelocity = -800.0f;
	public const float DashDistance = 200.0f;
	public const float DashDuration = 0.15f;
	public const float DashSpeed = DashDistance / DashDuration;
	
	private bool _isDashing = false;
	private bool _canAirDash;
	private float _dashTimer = 0f;
	private Vector2 _dashDirection;
	
	private AnimatedSprite2D _animatedSprite2D;
	private AudioStreamPlayer2D _jumpSFX;
	
	public override void _Ready()
	{
		_canAirDash = true;
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_jumpSFX = GetNode<AudioStreamPlayer2D>("JumpSFX");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		HandleGravity(ref velocity, delta);
		
		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("left", "right", "up", "down"); 
		
		HandleMovement(ref direction, ref velocity, delta);
		
		Velocity = velocity;
		MoveAndSlide();
		
		HandleAnimation(direction, velocity);
		
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
			_jumpSFX.Play();
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
		
		if (direction.X == 1.0f) {
			_animatedSprite2D.SetFlipH(false);
		} else if (direction.X == -1.0f) {
			_animatedSprite2D.SetFlipH(true);
		}
	}
}
