using Godot;
using System;

public class Test : Node {

	[Export]
	public PackedScene TestShip;

	[Export]
	public NodePath camPath;
	
	public override void _Ready() {
		
		Node ship = TestShip.Instance();
		RigidBody2D rigidBody2D = ship.GetNode("RigidBody2D") as RigidBody2D;
		rigidBody2D.SetScript(GD.Load("res://CS/TestShip.cs"));
		this.AddChild(ship);
		
		GetNode<GameCamera>(camPath).SetTarget(ship.GetNode<RigidBody2D>("RigidBody2D") as Node2D);
	}
}

