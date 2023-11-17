using Godot;
using System;

public class Test : Node {

	[Export]
	public PackedScene TestShip;

	public override void _Ready() {
		
		Node ship = TestShip.Instance();
		RigidBody2D rigidBody2D = ship.GetNode("RigidBody2D") as RigidBody2D;
		rigidBody2D.SetScript(GD.Load("res://CS/TestShip.cs"));
		this.AddChild(ship);
	}
}

