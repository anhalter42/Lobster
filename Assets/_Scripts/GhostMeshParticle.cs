using UnityEngine;
using System.Collections;

public class GhostMeshParticle : MonoBehaviour
{
	public GameObject m_Mesh;
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
		int neededParticles = CountPoints (m_Mesh.transform);
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
		m_Particles [aPos].position = aTransform.position;
		aPos++;
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
