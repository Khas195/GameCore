using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	// Use this for initialization
	[SerializeField]
	int expandAmount;
	Camera cam;
	float originalSize = 5;
	void Start () {
		
		cam = this.GetComponent<Camera>();
		originalSize = cam.orthographicSize;
	}
	public void OnRestart(){
		cam.orthographicSize = originalSize;
	}
	public void OnBoardExpand(int size){
		cam.orthographicSize = size + expandAmount;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
