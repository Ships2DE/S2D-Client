using Godot;
using System;
using S2D.Shared.Constants;

public class TestShip : RigidBody2D {

	public ShipDataShared Data;
	
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

		Vector2 spd = Vector2.Zero;
		if (wasd[0]) {
			spd.x += 1.0f;
		}
		if (wasd[1]) {
			spd.y -= 1.0f;
		}
		if (wasd[2]) {
			spd.x -= 1.0f;
		}
		if (wasd[3]) {
			spd.y += 1.0f;
		}

		Vector2 speed = (Transform.x).Normalized() * (spd.x * delta * Data.Speed * (float)GameConstants.PIXELS_PER_METER);
		this.ApplyCentralImpulse(speed);
		this.ApplyTorqueImpulse(spd.y * delta * Data.TurnSpeed * 360f);
	}
}
