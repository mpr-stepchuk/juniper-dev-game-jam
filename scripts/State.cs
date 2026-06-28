using System;
using Godot;

public abstract partial class State : Node

{   
    [Signal] public delegate void FinishedEventHandler(string NextStatePath);
    public Boss _boss;

    public const string Idle = "Idle";
    public const string Stunned = "Stunned";
    public const string Teleport = "Teleport";
    public const string Striking = "Striking";
    public const string Throwing = "Throwing";
    public const string Barrage = "Barrage";
    public const string BarrageShot = "BarrageShot";

    public override void _Ready()
    {
        _boss = GetTree().GetFirstNodeInGroup("Boss") as Boss;
    }
    public abstract void Update(double delta);

    public abstract void Enter(string prevState);

    public abstract void Exit();
    
}