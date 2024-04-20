using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UILookAtCam : MonoBehaviour
{
	//void Update()
	//{
	//	transform.LookAt(Camera.main.transform.position);
	//}

	private Transform cameraTransform;

	void Start()
	{
		// Find the CinemachineBrain in your scene
		CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();
		if (cinemachineBrain != null)
		{
			cameraTransform = cinemachineBrain.OutputCamera.transform;
		}
	}

	void Update()
	{
		if (cameraTransform != null)
		{
			// Look at the camera
			// Also, modify to not only look at the camera's position but also to correct the orientation
			Vector3 targetPosition = cameraTransform.position;
			targetPosition.y = transform.position.y; // Optionally keep the billboard effect strictly horizontal
			transform.LookAt(targetPosition);
		}
	}
}
