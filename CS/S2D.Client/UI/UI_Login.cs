using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using S2D.Shared.Data;
using S2D.Shared.Networking;
using S2D.Shared.Util;
using Environment = System.Environment;

namespace S2DClient.CS.S2D.Client.UI {
	public class UI_Login : CanvasLayer {

		[Export] private NodePath panelLoginPath, panelStatusPath, labelStatusPath, lineEditUsernamePath, buttonReconnectPath;
	
		private WebSocketClient client;
		//private ServerList serverList;
		
		public override void _Ready() {
			//ServerList currently unused, but working. Add dropdown for manual server selection later.
			//serverList = ServerList.FromWeb("https://ships2.de/serverlist.json");
			
			client = new WebSocketClient();
			client.Connect("connection_established", this, nameof(OnConnected));
			client.Connect("connection_error", this, nameof(OnConnectionError));
			client.Connect("connection_closed", this, nameof(OnConnectionClosed));
			
			Connect();
		}

		private void Connect() {
			GetNode<Button>(buttonReconnectPath).Disabled = true;
			GetNode<Label>(labelStatusPath).Text = "Connecting...";
			ShowLogin(false);
		
			client.ConnectToUrl("ws://192.168.178.200:18020");
		}
	
		private void OnConnectionError() {
			GetNode<Label>(labelStatusPath).Text = "Connection failed!";
			GetNode<Button>(buttonReconnectPath).Disabled = false;
			ShowLogin(false);
		}

		private void OnConnectionClosed(bool _wasClean) {
			GetNode<Label>(labelStatusPath).Text = "Connection lost!";
			GetNode<Button>(buttonReconnectPath).Disabled = false;
			ShowLogin(false);
		}
	
		private void OnConnected(string _proto) {
			client.GetPeer(1).SetWriteMode(WebSocketPeer.WriteMode.Binary);
			ShowLogin(true);
		}
	
		public override void _Process(float delta) {
			base._Process(delta);
			client?.Poll();
		}

		private void ShowLogin(bool _show) {
			GetNode<Control>(panelStatusPath).Visible = !_show;
			GetNode<Control>(panelLoginPath).Visible = _show;
		}
	
		public void Button_Login() {
			ByteMessage message = new ByteMessage((ushort)Tags.LoginRequest);
			
			//Quick explanation:
			//We don't use any accounting system at the moment, so we simply use a longified version of the PHP session ID.
			//On web client it's available through a JavaScript function. If we can't get it for some reason, we just generate something random on every login.
			//ToDo: replace randomness here with a safer random provider from cryptography namespace - the current implementation isn't secure
			
			GD.Seed((ulong)((Environment.TickCount + 123) * DateTime.UtcNow.Ticks));
			string sid = CompressionUtil.EncryptStringBase64((GD.Randf() * GD.Randf() * GD.Randi() * 123.123f).ToString("0.0000000") + GD.Randi().ToString());
			if (OS.HasFeature("JavaScript")) {
				if ((bool)JavaScript.Eval("typeof parent.GetSessionId === 'function'")) {
					string jsid = JavaScript.Eval("parent.GetSessionId();").ToString();
					if (jsid.StartsWith("PHPSSID")) {
						sid = jsid;
					}
				} else {
					GD.Print("Could not get your browser session. Your login is randomized. Maybe your browser is unsupported?");
				}
			}
			
			string username = (GetNode(lineEditUsernamePath) as LineEdit).Text;
			message.WriteString(sid);
			message.WriteString(username.Length > 0 && username.Length <= 25 ? username : "Guest");

			client.GetPeer(1).PutPacket(message.GetBytes());
		}

		public void Button_Reconnect() {
			Connect();
		}
	
		public void OpenLink(string _url) {
			if (OS.HasFeature("JavaScript")) {
				JavaScript.Eval($"window.open('{_url}', '_blank').focus();");
				return;
			}
			OS.ShellOpen(_url);
		}
	}
	
	public class ServerList {
		public List<ServerInfo> Servers { get; set; }

		public static ServerList FromWeb(string _url) {
			return JsonConvert.DeserializeObject<ServerList>(HttpUtil.SendRequest(_url, ""));
		}
	}
}

