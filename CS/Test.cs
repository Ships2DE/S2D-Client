using Godot;
using System;
using S2D.Shared.Constants;

public class Test : Node {

	[Export]
	public PackedScene TestShip;

	[Export]
	public NodePath camPath, speedPath;

	private RigidBody2D rbody;
	
	public override void _Ready() {
		
		Node ship = TestShip.Instance();
		RigidBody2D rigidBody2D = ship.GetNode("RigidBody2D") as RigidBody2D;
		rigidBody2D.SetScript(GD.Load("res://CS/TestShip.cs"));
		this.AddChild(ship);

		rbody = ship.GetNode<RigidBody2D>("RigidBody2D");
		GetNode<GameCamera>(camPath).SetTarget(rbody);
	}

	public override void _Process(float delta) {
		if (rbody == null) {
			return;
		}
		GetNode<Label>(speedPath).Text = (rbody.LinearVelocity.Length() / GameConstants.PIXELS_PER_METER * 1.9f).ToString("0.0") + " kn";
	}
}

