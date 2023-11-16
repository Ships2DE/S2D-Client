using Godot;
using System;
using S2D.Shared.Networking;
using S2D.Shared.Util;

public class UI_Login : CanvasLayer {

	[Export] private NodePath panelLoginPath, panelStatusPath, labelStatusPath, lineEditUsername;
	
	private WebSocketClient client;

	public override void _Ready() {
		client = new WebSocketClient();
		client.ConnectToUrl("ws://192.168.178.200:18020");
		client.Connect("connection_established", this, nameof(OnConnected));
	}

	private void OnConnected(string _proto) {
		GD.Print("Connected!");
		client.GetPeer(1).SetWriteMode(WebSocketPeer.WriteMode.Binary);
	}
	
	public override void _Process(float delta) {
		base._Process(delta);
		client?.Poll();
	}

	public void Button_Login() {
		ByteMessage message = new ByteMessage((ushort)Tags.LoginRequest);
		string sid = CompressionUtil.EncryptStringBase64(GD.Randf().ToString() + GD.Randi().ToString());
		if (OS.HasFeature("JavaScript")) {
			string jsid = JavaScript.Eval("getSessionId();").ToString();
			if (jsid.StartsWith("PHPSSID")) {
				sid = jsid;
			}
		}
		GD.Print("Session ID: " + sid);
		string username = (GetNode(lineEditUsername) as LineEdit).Text;
		
		message.WriteString(sid);
		message.WriteString(username.Length > 0 && username.Length <= 25 ? username : "Guest");

		client.GetPeer(1).PutPacket(message.GetBytes());
	}

	public void Button_Reconnect() {
		
	}
	
	public void OpenLink(string _url) {
		if (OS.HasFeature("JavaScript")) {
			JavaScript.Eval($"window.open('{_url}', '_blank').focus();");
			return;
		}
		OS.ShellOpen(_url);
	}
}
