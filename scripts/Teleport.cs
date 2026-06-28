using Godot;
using System;

public partial class Teleport : State 
{
	
	[Export] public float tpDur;
	private bool _barrage;

    public override void _Ready()
    {
        base._Ready();
		_boss.FinishedTeleport += OnFinishedTeleport;
    }


	public override void Enter(string prevState)
	{
		_barrage = _boss._barrage;
		GD.Print("Start of Teleport.Enter() -> Barrage?   "+_barrage);
		if (_barrage)
		{
			_boss.TeleportBarrage();
		} else
		{ 
			_boss.TeleportEvade();
		}

		GD.Print("End of Teleport.Enter()");
	}

	public override void Update(double delta)
	{
		
	}

	public override void Exit()
	{
	}

	public void OnFinishedTeleport()
	{
		GD.Print("Teleport.OnFinishedTeleport() -> Barrage? "+_barrage);
		if (_barrage)
		{
			EmitSignal(SignalName.Finished, BarrageShot);
		}
		else
		{
			EmitSignal(SignalName.Finished, Throwing);
		}
	}
}
