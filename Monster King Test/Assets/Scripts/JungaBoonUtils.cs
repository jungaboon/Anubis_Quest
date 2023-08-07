using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungaBoonUtils
{
	public static Transform GetClosestInAngle(Vector3 startPos, Vector3 fwdDir, float radius, float angle, LayerMask mask)
	{
		Collider[] coll = Physics.OverlapSphere(startPos, radius, mask);
		if(coll.Length == 0) return null;
		List<Transform> transformsInAngle = new List<Transform>();
		List<float> distances = new List<float>();
		
		for (int i = 0; i < coll.Length; i++) {
			float angleToColl = Vector3.Angle(fwdDir, coll[i].transform.position - startPos);
			if(angleToColl <= angle) transformsInAngle.Add(coll[i].transform);
		}
		
		if(transformsInAngle.Count == 0) return null;
		for (int i = 0; i < transformsInAngle.Count; i++) {
			distances.Add(Vector3.Distance(startPos, transformsInAngle[i].position));
		}
		
		float smallestDist = distances.Min();
		int index = distances.IndexOf(smallestDist);
		
		return transformsInAngle[index];
	}
}
