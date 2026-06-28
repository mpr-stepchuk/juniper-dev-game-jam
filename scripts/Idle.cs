using Godot;
using System;

public partial class Idle : State 
{
	[Export] public float ThrowCD;
	public override void Enter(string prevState)
	{
		_boss._animatedSprite2D.Play("idle");
		_boss.ThrowTimer.Start(ThrowCD);
	}

	public override void Update(double delta)
	{
		if (_boss._stats.Guts <= _boss._barrageBreakpoint && !_boss._barrageDone)
		{
			EmitSignal(SignalName.Finished, Barrage);
		} 
		else if (_boss.DistanceToPlayer().Length() <= _boss._detectionRad)
		{
			int num = _boss._rand.Next(2);
			// DEBUG CHANGE LATER
			if (num == 0)
			{
				EmitSignal(SignalName.Finished, Teleport);
			}
			else
			{
				EmitSignal(SignalName.Finished, Striking);
			}
		}
		else if (_boss.ThrowTimer.GetTimeLeft() == 0)
		{
			GD.Print("Throw CD over, entering Throwing");
			EmitSignal(SignalName.Finished, Throwing);
		}
	}

	public override void Exit()
	{
	}
}
