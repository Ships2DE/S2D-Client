using Godot;
using System;

public class GameCamera : Camera2D {
	
	[Export]
	private NodePath targetPath;
	private Node2D target;

	public void SetTarget(Node2D _target) {
		target = _target;
	}
	
	public override void _Process(float _delta) {
		if (target == null) {
			return;
		}
		this.GlobalPosition = target.GlobalPosition;
	}
	
}
