using Godot;
using System;

public partial class BarrageShot : State 
{

	public override void _Ready()
	{
		base._Ready();
		_boss.FinishedBarrageShot += OnFinishedBarrageShot;
	}
	public override void Enter(string prevState)
	{
		_boss.ThrowBarrage();
	}

	public override void Update(double delta)
	{
	}

	public override void Exit()
	{
	}

	public void OnFinishedBarrageShot()
	{
		EmitSignal(SignalName.Finished, Barrage);
	}
}
