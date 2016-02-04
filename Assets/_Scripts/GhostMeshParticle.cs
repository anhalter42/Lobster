using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

#endif

[ExecuteInEditMode]
public class GhostMeshParticle : MonoBehaviour
{
	public GameObject m_Mesh;
	public int m_Multiplicator = 1;
	public Vector3 m_RandomOffset = new Vector3 (0, 0, 0);
	ParticleSystem m_System;
	ParticleSystem.Particle[] m_Particles;

	// Use this for initialization
	void Start ()
	{
		m_System = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_Mesh == null)
			return;
		if (m_Particles == null || m_Particles.Length < m_System.maxParticles) {
			m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
//			m_System.emission.rate = CountPoints(m_Mesh.transform);
		}
//		m_System.Clear();
		int numParticlesAlive = m_System.GetParticles (m_Particles);
		int neededParticles = CountPoints (m_Mesh.transform) * m_Multiplicator;
		if (numParticlesAlive < neededParticles) {
			m_System.Emit (neededParticles - numParticlesAlive);
		}
		numParticlesAlive = m_System.GetParticles (m_Particles);
		int lPI = 0;
		SetParticle (ref lPI, m_Mesh.transform);
		m_System.SetParticles (m_Particles, lPI);
	}

	void SetParticle (ref int aPos, Transform aTransform)
	{
		for (int j = 0; j < m_Multiplicator; j++) {
			//Vector3 lPos = transform.InverseTransformPoint(aTransform.position);
			//Vector3 lPos = m_Mesh.transform.position - aTransform.position;
			Vector3 lPos = transform.InverseTransformPoint(aTransform.position) - (m_Mesh.transform.position - transform.position);
			lPos = new Vector3 (
				lPos.x + Random.Range (-m_RandomOffset.x, m_RandomOffset.x),
				lPos.y + Random.Range (-m_RandomOffset.y, m_RandomOffset.y),
				lPos.z + Random.Range (-m_RandomOffset.z, m_RandomOffset.z));
			m_Particles [aPos].position = lPos;
			aPos++;
		}
		for (int i = 0; i < aTransform.childCount; i++) {
			SetParticle (ref aPos, aTransform.GetChild (i));
		}
	}

	int CountPoints (Transform aTransform)
	{
		int lC = 1;
		for (int i = 0; i < aTransform.childCount; i++) {
			lC += CountPoints (aTransform.GetChild (i));
		}
		return lC;
	}
}
