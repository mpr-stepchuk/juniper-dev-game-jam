using Godot;
using System;

public partial class Actor : CharacterBody2D
{
	
	protected bool _facingRight = true;
	protected bool _inKnockback = false;
	protected bool _knockbackable = false;
	protected float _kbForce; 
	protected float _kbTimer = 0.0f;
	protected float _kbAngle;
	protected Vector2 _kbVelocity;
	
	protected AnimatedSprite2D _animatedSprite2D;
	protected AnimationPlayer _animationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_kbVelocity = new Vector2(0.0f, 0.0f);
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ProcessKnockback(double delta, ref Vector2 velocity) {
		if (_inKnockback && _knockbackable) {
			_kbTimer -= (float)delta;
			
			velocity += _kbVelocity;
			
			// reset shit when done kb
			if (_kbTimer <= 0) {
				_inKnockback = false;
				_kbVelocity.X = 0;
				_kbVelocity.Y = 0;
				_kbForce = 0.0f;
				_kbTimer = 0.0f;
				_kbAngle = 0.0f;
			}
		}
	}
	
	// here to be called elsewhere on hit
	public void ApplyKnockback(float force, float angle, float duration) {
		// don't do anything with damage yet 
		_kbForce = force;
		_kbAngle = angle;
		_kbTimer = duration;
		if (_kbForce > 0) {
			_inKnockback = true;
			BuildKBVelocity();
		}
		
	}
	
	public void BuildKBVelocity() {
			float rad = Mathf.DegToRad(_kbAngle);
			_kbVelocity.X = Mathf.Cos(rad);
			_kbVelocity.Y = -Mathf.Sin(rad); // have to invert bc godot is backwards
			GD.Print("normalized vector:   " + _kbVelocity);
			_kbVelocity *= _kbForce;
			
			GD.Print("after applying force: " + _kbVelocity);
	}
	
	public void HandleGravity (double delta, ref Vector2 velocity) {
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
	}
	
	public virtual void ApplyHit(int damage, float force, float angle, float duration) {
	}
}
