using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace MAHN42
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
		[SerializeField] bool m_MAHN42 = true;
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
		private DungeonCamera m_dungeonCam;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
		public bool useAbsoluteMovement = false;
		public float inputScale = 0.5f;
		public float shiftPressMoveFactor = 1.5f;
		public float ctrlPressMoveFactor = 0.5f;
        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
				m_dungeonCam = Camera.main.GetComponent<DungeonCamera>();
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
			if (AllLevels.Get().levelController.isPause) return;
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
			if (Input.GetKeyUp (KeyCode.N)) {
				if (m_dungeonCam) {
					m_dungeonCam.offset = new Vector3 (m_dungeonCam.offset.x, m_dungeonCam.offset.y, -m_dungeonCam.offset.z);
				}
			}
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
			if (AllLevels.Get().levelController.isPause) {
				m_Character.Move(Vector3.zero, false, false);
				return;
			}
			float lScaleH = inputScale;
			float lScaleV = inputScale;
			if (m_dungeonCam) {
				if (m_dungeonCam.offset.z > 0) {
					lScaleH = -lScaleH;
					lScaleV = -lScaleV;
				}
			}
            // read inputs
			float h = CrossPlatformInputManager.GetAxis("Horizontal") * lScaleH;
			float v = CrossPlatformInputManager.GetAxis("Vertical") * lScaleV;
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
			if (m_Cam != null && !useAbsoluteMovement)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
			if (Input.GetKey(KeyCode.LeftShift)) m_Move *= shiftPressMoveFactor;
			if (Input.GetKey(KeyCode.LeftControl)) m_Move *= ctrlPressMoveFactor;
			if (Input.GetKey(KeyCode.D)) {
				m_Character.SetDeath(true);
			}
#endif

            // pass all parameters to the character control script
			m_Character.Move(m_Move, crouch, false /*m_Jump*/);
            m_Jump = false;
        }
    }
}
