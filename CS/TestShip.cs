using Godot;
using System;
using S2D.Shared.Constants;

public class TestShip : RigidBody2D {


	public bool EnableInput = true;
	
	public ShipDataShared Data;
	private float maxspeed = 1;
	private Vector2 curSpeed = Vector2.Zero;
	private float collisionForce = 0f;
	
	private bool[] wasd = new bool[4];

	public override void _Ready() {
		Data = this.GetParent().GetNode("ShipData") as ShipDataShared;
	}

	public override void _Process(float _delta) {
		wasd[0] = Input.IsKeyPressed((int)KeyList.W);
		wasd[1] = Input.IsKeyPressed((int)KeyList.A);
		wasd[2] = Input.IsKeyPressed((int)KeyList.S);
		wasd[3] = Input.IsKeyPressed((int)KeyList.D);
	}

	public override void _PhysicsProcess(float _delta) {
		//Input which is read every frame is used here
		Vector2 spd = new Vector2(wasd[0] ? 1.0f : wasd[2] ? -1.0f * Data.ReverseMulti : 0f, wasd[1] ? 1.0f : wasd[3] ? -1.0f : 0f);
		
		//Fake acceleration by lerping
		curSpeed = new Vector2(Mathf.Lerp(curSpeed.x, spd.x, _delta * Data.AccelerationMulti), Mathf.Lerp(curSpeed.y, spd.y, _delta * 5f));
		
		//Remove speed when colliding - not so much on steering, never remove 100%
		curSpeed = new Vector2(curSpeed.x * Mathf.Clamp((1f - (collisionForce / Data.Speed)), 0.1f, 1f), curSpeed.y * Mathf.Clamp((1f - (collisionForce / Data.Speed)), 0.5f, 1f));
		
		//Apply forces, done
		ApplyCentralImpulse(GetGlobalTransform().x.Normalized() * (curSpeed.x * _delta * Data.Speed));
		ApplyTorqueImpulse(-(curSpeed.y * Data.TurnSpeed * ((LinearVelocity.Length() * Data.SpeedTurnInfluence) * _delta + Data.StandTurnSpeed)));
		collisionForce = 0f;
	}

	private Vector2 linearVelocityOld;
	public override void _IntegrateForces(Physics2DDirectBodyState _state) {
		if (_state.GetContactCount() > 0) {
			//Get last collision force in _IntegrateForces for safe state access
			collisionForce = ((_state.LinearVelocity - linearVelocityOld) / (_state.InverseMass * _state.Step)).Length();
		}
		linearVelocityOld = _state.LinearVelocity;
	}
}
