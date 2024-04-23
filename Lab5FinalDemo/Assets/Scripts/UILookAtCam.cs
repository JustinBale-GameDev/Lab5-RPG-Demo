using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UILookAtCam : MonoBehaviour
{
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
			Vector3 targetPosition = cameraTransform.position;
			targetPosition.y = transform.position.y;
			transform.LookAt(targetPosition);
		}
	}
}
