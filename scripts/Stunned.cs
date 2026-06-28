using Godot;
using System;

public partial class Stunned : State 
{
	public override void Enter(string prevState)
	{
		_boss._animatedSprite2D.Play("stun");
		_boss.StunTimer.Start(3.0f);
	}

	public override void Update(double delta)
	{
		if (_boss.StunTimer.GetTimeLeft() == 0)
		{
			EmitSignal(SignalName.Finished, Idle);
		}
	}

	public override void Exit()
	{
	}
}
