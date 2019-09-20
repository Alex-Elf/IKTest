using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoController : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private Transform headBone;

	private void LateUpdate()
	{
		Vector3 towardObjectFromHead = target.position - headBone.position;
		headBone.rotation = Quaternion.LookRotation(towardObjectFromHead, transform.up);
	}
}
