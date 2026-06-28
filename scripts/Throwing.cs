using Godot;
using System;

public partial class Throwing : State 
{

    public override void _Ready()
    {
        base._Ready();
		_boss.FinishedThrow += OnFinishedThrow;
    }

	public override void Enter(string prevState)
    {
        if (prevState == Idle)
		{
			_boss.ThrowStraight();
		} 
		else if (prevState == Teleport)
		{
			_boss.ThrowCone();
		}
    }

	public override void Update(double delta)
	{
		
	}

	public override void Exit()
	{
	}

	public void OnFinishedThrow()
	{
		EmitSignal(SignalName.Finished, Idle);
	}
}
