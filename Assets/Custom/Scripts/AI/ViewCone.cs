using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Written by Sebastian Lague for his Field of View visualisation series, minor tweaks by me
/// https://www.youtube.com/watch?v=rQG9aUWarwE
/// 
/// This code was published under the MIT License
/// https://github.com/SebLague/Field-of-View/
/// </summary>
public delegate void TargetFoundEventHandler(TargetFoundEvent _event);
public delegate void TargetLostEventHandler();

public class ViewCone : MonoBehaviour
{
    [Min(0.1f)]public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
	[Range(.0f, 2.0f)] public float TargetLostDelay = 1.0f;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector]public List<Transform> visibleTargets = new List<Transform>();
	private Transform currentTarget;
	public event TargetFoundEventHandler OnTargetFound;
	public event TargetLostEventHandler OnTargetLost;

	private bool hasTarget;
	private Coroutine findTargets;
	private Coroutine targetLostDelay;
	
	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public float maskCutawayDst = .1f;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;
	private int stepCount;
	private Vector3[] vertices;
	private int[] triangles;
	[SerializeField] private AudioClip[] guardSoundClips;

	private void Start() {
		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;

		
		stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		
		vertices = new Vector3[stepCount];
		triangles = new int[(stepCount-2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertices.Length - 1; i++) {
			if (i < stepCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
	}
	void LateUpdate() {
		DrawFieldOfView ();
	}

	private void OnEnable()
	{
		findTargets = StartCoroutine ("FindTargetsWithDelay", .2f);
	}

	private void OnDisable()
	{

		if (findTargets != null)
		{
			StopCoroutine(findTargets);
		}
		if (targetLostDelay != null)
		{
			StopCoroutine(targetLostDelay);
			targetLostDelay = null;
		}

		OnTargetLost?.Invoke();
		hasTarget = false;
	}

	#region FIND_TARGET
	private IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}
	
	private void FindVisibleTargets() {
		visibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);
		
		
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2 && Physics.Raycast(transform.position, new Vector3(dirToTarget.x, .0f, dirToTarget.z), viewRadius, targetMask)) {
				
				float dstToTarget = Vector3.Distance (transform.position, target.position);
				if (!Physics.Raycast (transform.position, new Vector3(dirToTarget.x, .0f, dirToTarget.z), dstToTarget, obstacleMask)) {
					visibleTargets.Add (target);
				}
			}
		}

		if (visibleTargets.Count > 0 && !hasTarget)
		{
			if (targetLostDelay != null)
			{
				StopCoroutine(targetLostDelay);
				targetLostDelay = null;
			}
			SoundManager.instance.PlayRandomSoundClip(guardSoundClips, transform, 1f);
			OnTargetFound?.Invoke(new TargetFoundEvent(visibleTargets[0]));
		}
		else if (visibleTargets.Count == 0 && hasTarget)
		{
			targetLostDelay = StartCoroutine(TargetLostDelayedInvoke(1.5f));
		}

		hasTarget = visibleTargets.Count > 0;
	}

	private IEnumerator TargetLostDelayedInvoke(float _time)
	{
		yield return new WaitForSeconds(_time);
		OnTargetLost?.Invoke();
	}
#endregion

	private void DrawFieldOfView() {
		
		float stepAngleSize = viewAngle / stepCount;
		for (int i = 1; i < vertices.Length; i++)
		{
			float angle = -viewAngle / 2 + stepAngleSize * i;
			//float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			vertices[i] = DirFromAngle(angle, true) * viewRadius;
			Vector3 dir = transform.TransformDirection(vertices[i]);
			dir.y = transform.position.y;
			if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius, obstacleMask))
			{
				vertices[i] = transform.InverseTransformPoint(hit.point);
			}
		}
		viewMeshFilter.transform.rotation = transform.rotation;

		viewMesh.vertices = vertices;
		viewMesh.RecalculateBounds();
	}


	private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDstThresholdExceeded = Mathf.Abs (minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}


	private ViewCastInfo ViewCast(float globalAngle) {
		Vector3 dir = DirFromAngle (globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast (transform.position, dir, out hit, viewRadius, obstacleMask)) {
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {
			return new ViewCastInfo (false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ViewCastInfo {
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle) {
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}
	}

	public struct EdgeInfo {
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
			pointA = _pointA;
			pointB = _pointB;
		}
	}
}
