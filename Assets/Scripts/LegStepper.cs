using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class LegStepper : MonoBehaviour
{
	[SerializeField] private Transform homeTransform;
	[SerializeField] private float stepDistance;
	[SerializeField] private float stepDuration;
	/// <summary>
	/// Fraction of the max distance from home we want to overshoot by
	/// </summary>
	[SerializeField] float stepOvershootFraction;


	public bool moving;

	public void TryMove()
	{
		if (moving) return;

		float distFromHome = Vector3.SqrMagnitude(transform.position - homeTransform.position);

		if (distFromHome > stepDistance)
		{
			StartCoroutine(MoveToHome());
		}

	}


	private IEnumerator MoveToHome()
	{
		moving = true;

		Quaternion startRot = transform.rotation;
		Vector3 startPos = transform.position;

		Quaternion endRot = homeTransform.rotation;

		Vector3 towardHome = (homeTransform.position - transform.position);
		float overshootDistance = stepDistance * stepOvershootFraction;
		Vector3 overshootVector = towardHome * overshootDistance;
		overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

		Vector3 endPos = homeTransform.position + overshootVector;

		Vector3 centerPoint = (startPos + endPos) / 2;
		centerPoint += homeTransform.up * Vector3.Distance(startPos, endPos) / 2f;

		float timeElapsed = 0;

		do
		{
			timeElapsed += Time.deltaTime;

			float normalizedTime = timeElapsed / stepDuration;
			normalizedTime = Easing.Cubic.InOut(normalizedTime);

			transform.position = Vector3.Lerp(
				Vector3.Lerp(startPos, centerPoint, normalizedTime),
				Vector3.Lerp(centerPoint, endPos, normalizedTime),
				normalizedTime);


			transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

			yield return null;
		}
		while (timeElapsed < stepDuration);

		moving = false;
	}
}
