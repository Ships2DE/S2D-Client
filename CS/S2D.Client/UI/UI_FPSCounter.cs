using Godot;

namespace S2DClient.CS.S2D.Client.UI {
	public partial class UI_FPSCounter : Label {
	
		public override void _Ready() { }
	
		public override void _Process(float delta) {
			this.Text = $"FPS: {(1.0 / delta):0}";
		}
	}
}