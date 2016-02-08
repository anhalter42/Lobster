using UnityEngine;
using System.Collections;

public class MultiActivator : MonoBehaviour
{

	public enum When
	{
		OnTriggerEnter,
		OnTriggerExit,
		OnTriggerEnterExit
	}

	public enum Action
	{
		Activate,
		Deactivate,
		Toggle
	}

	[System.Serializable]
	public class InventoryItem
	{
		public string type = string.Empty;
		public int neededAmount = 1;
		public int costAmount = 1;
		public int getAmount = 0;

		public bool ReadLine(string aLine)
		{
			string[] lVs = aLine.Split(new char[] {':'});
			if (lVs.Length > 0) {
				type = lVs[0];
			}
			if (lVs.Length > 1) {
				neededAmount = int.Parse(lVs[1]);
			}
			if (lVs.Length > 2) {
				costAmount = int.Parse(lVs[2]);
			}
			if (lVs.Length > 3) {
				getAmount = int.Parse(lVs[3]);
			}
			return !string.IsNullOrEmpty(type);
		}
	}

	public class LateExecute
	{
		public GameObject gameObject;
		public Behaviour behaviour;
		public string argument;
		public float time;
		public bool value;

		public LateExecute (GameObject aObject, Behaviour aBehaviour, float aTime, bool aValue, string aArgument)
		{
			gameObject = aObject;
			behaviour = aBehaviour;
			time = aTime;
			value = aValue;
			argument = aArgument;
		}

		public override string ToString ()
		{
			return string.Format ("{0} {1} {2} {3}", gameObject ? gameObject.ToString () : behaviour.ToString (), time, value, argument);
		}
	}

	[System.Serializable]
	public class ControlledObject
	{
		public When when = When.OnTriggerEnter;
		public Action action = Action.Toggle;
		public GameObject gameObject;
		public float delay = 0f;
		public string[] tags = { "Player" };
		public bool isRepeatable = true;
		public bool isDone = false;
		public int triggerCount = 0;

		public void DoIt (MultiActivator aActivator)
		{
			bool aValue = true;
			if (!isRepeatable) {
				isDone = true;
			}
			switch (action) {
			case Action.Activate:
				aValue = true;
				break;
			case Action.Deactivate:
				aValue = false;
				break;
			case Action.Toggle:
				aValue = !gameObject.activeSelf;
				break;
			}
			if (delay <= 0f) {
				gameObject.SetActive (aValue);
			} else {
				aActivator.lateExecutes.Add (new LateExecute (gameObject, null, Time.realtimeSinceStartup + delay, aValue, null));
			}
		}

		public void RestoreIt ()
		{
		}
	}

	[System.Serializable]
	public class ControlledBehavior
	{
		public When when = When.OnTriggerEnter;
		public Action action = Action.Toggle;
		public Behaviour behaviour;
		public float delay = 0f;
		public string argument;
		public string[] tags = { "Player" };
		public bool isRepeatable = true;
		public bool isDone = false;
		public int triggerCount = 0;

		public void DoIt (MultiActivator aActivator)
		{
			if (!isRepeatable) {
				isDone = true;
			}
			bool aValue = SwitchIt ();
			if (delay <= 0f) {
				behaviour.enabled = aValue;
				if (aValue) {
					aActivator.SpecialOn (behaviour, argument);
				} else {
					aActivator.SpecialOff (behaviour, argument);
				}
			} else {
				aActivator.lateExecutes.Add (new LateExecute (null, behaviour, Time.realtimeSinceStartup + delay, aValue, argument));
			}
		}

		public bool SwitchIt ()
		{
			switch (action) {
			case Action.Activate:
				return true;
			case Action.Deactivate:
				return false;
			case Action.Toggle:
				return !behaviour.enabled;
			}
			return true;
		}

		public void RestoreIt ()
		{
			//SwitchIt ();
		}
	}

	[System.Serializable]
	public class ControlledMethod
	{
		public When when = When.OnTriggerEnter;
		public string method = string.Empty;
		public float delay = 0f;
		public string[] tags = { "Player" };
		public bool isRepeatable = true;
		public bool isDone = false;
		public int triggerCount = 0;

		public void DoIt ()
		{
			if (!isRepeatable) {
				isDone = true;
			}
			//Toast;<Splash_Text>;<Title>;<tandingTime>
			if (method.StartsWith ("Toast;", System.StringComparison.OrdinalIgnoreCase)) {
				string[] lParts = method.Split (new char[] { ';' });
				string lTitle = lParts.Length > 2 ? lParts [2] : AllLevels.Get ().levelController.settings.name;
				string lText = lParts.Length > 1 ? lParts [1] : "What?";
				float lTime = lParts.Length > 3 ? float.Parse (lParts [3]) : 2f;
				AllLevels.Get ().levelController.ShowToast (lTitle, lText, lTime);
			} else if (method.StartsWith ("Exit", System.StringComparison.OrdinalIgnoreCase)) {
				string[] lParts = method.Split (new char[] { ';' });
				string lLevelName = lParts.Length > 1 ? (string.IsNullOrEmpty (lParts [1]) ? "NEXT" : lParts [1]) : "NEXT";
				AllLevels.Get ().levelController.PlayerHasExitReached (lLevelName);
			} else {
				AllLevels.Get ().levelController.Invoke (method, delay);
			}
		}
	}

	public ControlledObject[] controlledObjects = { };
	public ControlledBehavior[] controlledBehaviors = { };
	public ControlledMethod[] controlledMethods = { };
	public InventoryItem[] inventoryItems = { };

	bool inventoryItemsPayed = false;

	System.Collections.Generic.List<LateExecute> lateExecutes = new System.Collections.Generic.List<LateExecute> ();

	void FixedUpdate ()
	{
		if (lateExecutes.Count > 0) {
			System.Collections.Generic.List<LateExecute> lDones = new System.Collections.Generic.List<LateExecute> ();
			foreach (LateExecute lE in lateExecutes) {
				if (Time.realtimeSinceStartup >= lE.time) {
					if (lE.gameObject != null) {
						lE.gameObject.SetActive (lE.value);
					} else {
						lE.behaviour.enabled = lE.value;
						if (lE.value) {
							SpecialOn (lE.behaviour, lE.argument);
						} else {
							SpecialOff (lE.behaviour, lE.argument);
						}
					}
					lDones.Add (lE);
				}
			}
			foreach (LateExecute lE in lDones) {
				Debug.Log (lE.ToString ());
				lateExecutes.Remove (lE);
			}
		}
		bool lTriggered = false;
		foreach (ControlledObject lO in controlledObjects) {
			if (lO.triggerCount > 0) {
				lO.triggerCount = 0;
				lO.DoIt (this);
				lTriggered = true;
			}
		}
		foreach (ControlledBehavior lB in controlledBehaviors) {
			if (lB.triggerCount > 0) {
				lB.triggerCount = 0;
				lB.DoIt (this);
				lTriggered = true;
			}
		}
		foreach (ControlledMethod lM in controlledMethods) {
			if (lM.triggerCount > 0) {
				lM.triggerCount = 0;
				lM.DoIt ();
				lTriggered = true;
			}
		}
		if (lTriggered && !inventoryItemsPayed) {
			inventoryItemsPayed = true;
			foreach (InventoryItem lI in inventoryItems) {
				if (lI.getAmount > 0) {
					AllLevels.Get ().levelController.AddInventoryItem (new PlayerInventory.InventoryItem (lI.type, lI.getAmount));
				}
				if (lI.costAmount > 0) {
					AllLevels.Get ().levelController.SubInventoryItem (new PlayerInventory.InventoryItem (lI.type, lI.costAmount));
				}
			}
		}
	}

	void OnEnable ()
	{
		foreach (ControlledBehavior lB in controlledBehaviors) {
			if (lB.isDone) {
				lB.RestoreIt ();
			}
		}
		foreach (ControlledObject lO in controlledObjects) {
			if (lO.isDone) {
				lO.RestoreIt ();
			}
		}
	}

	bool HasInventoryItems ()
	{
		if (inventoryItems.Length > 0) {
			foreach (InventoryItem lI in inventoryItems) {
				if (!AllLevels.Get ().levelController.playerInventory.HasItem (lI.type, lI.neededAmount)) {
					return false;
				}
			}
			return true;
		} else {
			return true;
		}
	}

	void OnTriggerEnter (Collider aOther)
	{
		if (HasInventoryItems ()) {
			DoControlIt (When.OnTriggerEnter, aOther.tag);
		}
	}

	void OnTriggerExit (Collider aOther)
	{
		if (HasInventoryItems ()) {
			DoControlIt (When.OnTriggerExit, aOther.tag);
		}
	}

	bool TagMatched (string[] aTags, string aTag)
	{
		if (aTags.Length == 0) {
			return true;
		} else {
			foreach (string lTag in aTags) {
				if (lTag.Equals (aTag)) {
					return true;
				}
			}
			return false;
		}
	}

	bool WhenMatched (When aWhen1, When aWhen2)
	{
		if (aWhen1 == aWhen2) {
			return true;
		} else if (aWhen1 == When.OnTriggerEnterExit && (aWhen2 == When.OnTriggerEnter || aWhen2 == When.OnTriggerExit)) {
			return true;
		}
		return false;
	}

	void DoControlIt (When aWhen, string aTag)
	{
		foreach (ControlledBehavior lB in controlledBehaviors) {
			if (!lB.isDone && TagMatched (lB.tags, aTag)) {
				lB.triggerCount += WhenMatched (lB.when, aWhen) ? 1 : -1;
				if (lB.triggerCount < 0)
					lB.triggerCount = 0;
			}
		}
		foreach (ControlledObject lO in controlledObjects) {
			if (!lO.isDone && TagMatched (lO.tags, aTag)) {
				lO.triggerCount += WhenMatched (lO.when, aWhen) ? 1 : -1;
				if (lO.triggerCount < 0)
					lO.triggerCount = 0;
			}
		}
		foreach (ControlledMethod lM in controlledMethods) {
			if (!lM.isDone && TagMatched (lM.tags, aTag)) {
				lM.triggerCount += WhenMatched (lM.when, aWhen) ? 1 : -1;
				if (lM.triggerCount < 0)
					lM.triggerCount = 0;
			}
		}
	}

	void SpecialOn (Behaviour behaviour, string argument)
	{
		if (behaviour is AudioSource) {
			((AudioSource)behaviour).Play ();
		} else if (behaviour is Animation) {
			((Animation)behaviour).Play ();
		} else if (behaviour is Animator) {
			((Animator)behaviour).SetBool (argument, true);
		}
	}

	void SpecialOff (Behaviour behaviour, string argument)
	{
		if (behaviour is AudioSource) {
			((AudioSource)behaviour).Stop ();
		} else if (behaviour is Animation) {
			((Animation)behaviour).Stop ();
		} else if (behaviour is Animator) {
			((Animator)behaviour).SetBool (argument, false);
		}
	}

}
