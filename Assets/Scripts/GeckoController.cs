using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649
public class GeckoController : MonoBehaviour
{
	[SerializeField] private Transform target;

	[Header("Head tracking")]
	[SerializeField] private Transform headBone;
	[SerializeField] private float headTurningSpeed = 20;
	[SerializeField] private float headMaxTurnAngle = 45;
	[Header("Eye tracking")]
	[SerializeField] private Transform leftEye;
	[SerializeField] private Transform rightEye;
	[SerializeField] private float eyeMinYRotation;
	[SerializeField] private float eyeMaxYRotation;
	[SerializeField] private float eyeTurningSpeed = 20;
	[Header("Legs")]
	[SerializeField] private LegStepper LBStepper;
	[SerializeField] private LegStepper RBStepper;
	[SerializeField] private LegStepper LFStepper;
	[SerializeField] private LegStepper RFStepper;

	private void Awake()
	{
		StartCoroutine(LegUpdateCoroutine());
	}

	private void LateUpdate()
	{
		HeadTracking();
		EyeTracking();
	}

	private void HeadTracking()
	{
		Quaternion currLocalRotation = headBone.localRotation;
		headBone.localRotation = Quaternion.identity;


		Vector3 targetWorldLookDir = target.position - headBone.position;
		Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

		targetLocalLookDir = Vector3.RotateTowards(
			Vector3.forward,
			targetLocalLookDir,
			Mathf.Deg2Rad * headMaxTurnAngle,
			0);


		Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

		headBone.localRotation = Quaternion.Slerp(
			currLocalRotation,
			targetLocalRotation,
			1 - Mathf.Exp(-headTurningSpeed * Time.deltaTime)
		);
	}
	private void EyeTracking()
	{
		Quaternion targetEyeRotation = Quaternion.LookRotation(
			target.position - headBone.position,
			Vector3.up);
		leftEye.rotation = Quaternion.Slerp(
			leftEye.rotation,
			targetEyeRotation,
			1 - Mathf.Exp(-eyeTurningSpeed * Time.deltaTime));
		rightEye.rotation = Quaternion.Slerp(
			rightEye.rotation,
			targetEyeRotation,
			1 - Mathf.Exp(-eyeTurningSpeed * Time.deltaTime));

		float rightEyeYRotationCurrent = rightEye.localEulerAngles.y;
		float leftEyeYRotationCurrent = leftEye.localEulerAngles.y;

		// Move the rotation to a -180 ~ 180 range
		if (leftEyeYRotationCurrent > 180)
		{
			leftEyeYRotationCurrent -= 360;
		}
		if (rightEyeYRotationCurrent > 180)
		{
			rightEyeYRotationCurrent -= 360;
		}

		float rightEyeYRotationClamped = Mathf.Clamp(
			rightEyeYRotationCurrent,
			eyeMinYRotation,
			eyeMaxYRotation);
		float leftEyeYRotationClamped = Mathf.Clamp(
			leftEyeYRotationCurrent,
			eyeMinYRotation,
			eyeMaxYRotation);

		leftEye.localEulerAngles = new Vector3(
				leftEye.localEulerAngles.x,
				leftEyeYRotationClamped,
				0);
		rightEye.localEulerAngles = new Vector3(
			rightEye.localEulerAngles.x,
			rightEyeYRotationClamped,
			0
		);
		
	}
	private IEnumerator LegUpdateCoroutine()
	{
		while (true)
		{
			do
			{
				RBStepper.TryMove();
				LFStepper.TryMove();
				yield return null;

			} while (RBStepper.moving || LFStepper.moving);

			do
			{
				LBStepper.TryMove();
				RFStepper.TryMove();
				yield return null;

			} while (LBStepper.moving || RFStepper.moving);
		}
	}
}
