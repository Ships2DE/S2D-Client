using Godot;
using System;
using S2D.Shared.Constants;

public class TestShip : RigidBody2D {


	public bool EnableInput = true;
	
	public ShipDataShared Data;
	private float maxspeed = 1;
	private Vector2 curSpeed = Vector2.Zero;
	
	private bool[] wasd = new bool[4];

	public override void _Ready() {
		Data = this.GetParent().GetNode("ShipData") as ShipDataShared;
	}

	public override void _Process(float delta) {
		wasd[0] = Input.IsKeyPressed((int)KeyList.W);
		wasd[1] = Input.IsKeyPressed((int)KeyList.A);
		wasd[2] = Input.IsKeyPressed((int)KeyList.S);
		wasd[3] = Input.IsKeyPressed((int)KeyList.D);
	}

	public override void _PhysicsProcess(float delta) {
		Vector2 spd = new Vector2(wasd[0] ? 1.0f : wasd[2] ? -1.0f * Data.ReverseMulti : 0f, wasd[1] ? 1.0f : wasd[3] ? -1.0f : 0f);
		curSpeed = new Vector2(Mathf.Lerp(curSpeed.x, spd.x, delta * Data.AccelerationMulti), Mathf.Lerp(curSpeed.y, spd.y, delta * 5f));
		ApplyCentralImpulse(GetGlobalTransform().x.Normalized() * (curSpeed.x * delta * Data.Speed));
		ApplyTorqueImpulse(-(curSpeed.y * Data.TurnSpeed * ((LinearVelocity.Length() * Data.SpeedTurnInfluence) + Data.StandTurnSpeed)));
	}
}
