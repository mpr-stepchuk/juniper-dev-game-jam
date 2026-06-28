using Godot;
using System;

public partial class Striking : State 
{

    public override void _Ready()
    {
        base._Ready();
		_boss.FinishedStrike += OnFinishedStrike;
    }

	public override void Enter(string prevState)
	{
		_boss.HandleStrike();
	}

	public override void Update(double delta)
	{
		if (_boss.InKnockback())
		{
			EmitSignal(SignalName.Finished, Stunned);
		}

	}

	public override void Exit()
	{
	}

	public void OnFinishedStrike ()
	{
		EmitSignal(SignalName.Finished, Idle);
	}
}
