using Godot;
using System;

public partial class Barrage : State 
{
	[Export] public float BarrageStartup;
	[Export] public int NumShots;
	
	[Export] public Vector2 BarrageTransform;


    public override void _Ready()
    {
        base._Ready();
		_boss.FinishedBarrage += OnFinishedBarrage;
    }

	public override void Enter(string prevState)
	{
		_boss.BarrageTimer.Start(BarrageStartup);
		_boss._barrage = true;
		_boss.PosBarrageL = _boss._player.GlobalPosition + (BarrageTransform * (Vector2.Left + Vector2.Down));	
		_boss.PosBarrageR = _boss._player.GlobalPosition + BarrageTransform;	
		GD.Print("BarrageL: "+_boss.PosBarrageL+"   BarrageR: "+_boss.PosBarrageR);
		
		if (prevState != BarrageShot)
		{
			NumShots = 7;
			_boss._barrage = true;
		}
		
		if (NumShots > 0)
		{
			NumShots--;
			GD.Print($"FIRING BARRAGE SHOT - {NumShots} shots remaining");
			EmitSignal(SignalName.Finished, Teleport);
		} 
		else
		{
			_boss._barrage = false;
			_boss._barrageDone = true;
			EmitSignal(SignalName.Finished, Idle);
		}
		GD.Print("End of Barrage.Enter() -> Barrage? "+_boss._barrage);
	}

	public override void Update(double delta)
	{
		
	}

	public override void Exit()
	{
	}

	public void OnFinishedBarrage()
	{
		EmitSignal(SignalName.Finished, Idle);
	}
}
