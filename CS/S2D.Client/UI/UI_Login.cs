using Godot;
using System;

public class UI_Login : CanvasLayer {

	public void OpenLink(string _url) {
		if (OS.HasFeature("JavaScript")) {
			JavaScript.Eval($"window.open('{_url}', '_blank').focus();");
			return;
		}
		OS.ShellOpen(_url);
	}
	
}
