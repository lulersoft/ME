﻿using UnityEngine;
using System.Collections;

using NLua;

public class SpawnSphere : MonoBehaviour {

	string source = @"
import 'System'
import 'UnityEngine'
import 'Assembly-CSharp'	-- The user-code assembly generated by Unity

function Update()
	if Input.GetKey(KeyCode.Space) then
		GameObject.Instantiate(sphere, transform.position, Quaternion.identity)
	end
end

";

	Lua env;

	public GameObject sphere;

	void Awake() {
		env = new Lua();
		env.LoadCLRPackage();

		env["this"] = this;
		env["transform"] = transform;
		env["sphere"] = sphere; // Give the script access to the prefab.
		
		//System.Object[] result = new System.Object[0];
		try {
			//result = env.DoString(source);
			env.DoString(source);
		} catch(NLua.Exceptions.LuaException e) {
			Debug.LogError(FormatException(e), gameObject);
		}

	}

	void Start() {
		Call("Start");
	}

	void Update() {
		Call("Update");
	}

	void OnGUI() {
		Call("OnGUI");
	}

	public System.Object[] Call(string function, params System.Object[] args) {
		System.Object[] result = new System.Object[0];
		if(env == null) return result;
		LuaFunction lf = env.GetFunction(function);
		if(lf == null) return result;
		try {
			// Note: calling a function that does not
			// exist does not throw an exception.
			if(args != null) {
				result = lf.Call(args);
			} else {
				result = lf.Call();
			}
		} catch(NLua.Exceptions.LuaException e) {
			Debug.LogError(FormatException(e), gameObject);
		}
		return result;
	}

	public System.Object[] Call(string function) {
		return Call(function, null);
	}

	public static string FormatException(NLua.Exceptions.LuaException e) {
		string source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
		return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
	}
}
