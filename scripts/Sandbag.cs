using Godot;
using System;

public partial class Sandbag : CharacterBody2D
{
	
	private bool _facingRight = true;
	private bool _inKnockback = false;
	private float _kbForce; 
	private float _kbTimer = 0.0f;
	private float _kbAngle;
	private Vector2 _kbVelocity;
	
	[Export] public CharacterBody2D Player;
	
	private AnimatedSprite2D _animatedSprite2D;
	private AnimationPlayer _animationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_kbVelocity = new Vector2(0.0f, 0.0f);
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		HandleGravity(ref velocity, delta);
		
		if (_inKnockback) {
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
		
		//velocity += _kbVelocity;
		
		// move when all the calcs are done
		Velocity = velocity;
		MoveAndSlide();
	}
	
	// here to be called elsewhere on hit
	public void ApplyHit(int damage, float force, float angle, float duration) {
		// don't do anything with damage yet 
		_kbForce = force;
		_kbAngle = angle;
		_kbTimer = duration;
		if (_kbForce > 0) {
			_inKnockback = true;
			BuildKBVelocity();
		}
		
	}
	
	private void BuildKBVelocity() {
			float rad = Mathf.DegToRad(_kbAngle);
			_kbVelocity.X = Mathf.Cos(rad);
			_kbVelocity.Y = -Mathf.Sin(rad); // have to invert bc godot is backwards
			GD.Print("normalized vector:   " + _kbVelocity);
			_kbVelocity *= _kbForce;
			
			GD.Print("after applying force: " + _kbVelocity);
	}
	
	private void HandleGravity (ref Vector2 velocity, double delta) {
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
	}
	
	private void Reorient() {
		if (Player != null) {
			if (Player.GlobalPosition.X > GlobalPosition.X) {
				_animatedSprite2D.SetFlipH(true);
			} else {
				_animatedSprite2D.SetFlipH(false);
			}
		} else {
			GD.PrintErr("No player found");
		}
	}
}
