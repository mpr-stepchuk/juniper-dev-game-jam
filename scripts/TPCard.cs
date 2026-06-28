using Godot;
using System;

public partial class TPCard : Node2D
{
	private AnimatedSprite2D _anim;
	[Export] public bool _source = true;
	private int _lastFrame;
	// Called when the node enters the scene tree for the first time.
	

	public void Init(bool source, Vector2 pos)
	{
		_source = source;
		GlobalTransform = new Transform2D(0, pos);
	}

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_anim.AnimationFinished += OnAnimationFinished;
		if (_source)
		{
			_anim.Play("flip");
		} else
		{
			_anim.PlayBackwards("flip");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAnimationFinished()
	{
		QueueFree();
	}
}
