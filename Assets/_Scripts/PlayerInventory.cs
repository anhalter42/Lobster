using UnityEngine;
using System.Collections;

public class PlayerInventory : MonoBehaviour
{

	[System.Serializable]
	public class InventoryItem
	{
		public string type = string.Empty;
		public int count = 1;

		public InventoryItem ()
		{
		}

		public InventoryItem (string aType, int aCount)
		{
			type = aType;
			count = aCount;
		}
	}

	public InventoryItem[] m_Items = { };

	public bool HasItem (string aType, int aAmount = 1)
	{
		foreach (InventoryItem lItem in m_Items) {
			if (lItem.type.Equals (aType)) {
				return lItem.count >= aAmount;
			}
		}
		return false;
	}

	public int AddItem (InventoryItem aItem)
	{
		foreach (InventoryItem lItem in m_Items) {
			if (lItem.type.Equals (aItem.type)) {
				lItem.count += aItem.count;
				return lItem.count;
			}
		}
		InventoryItem[] lNewItems = new InventoryItem[m_Items.Length + 1];
		InventoryItem lNew = new InventoryItem (aItem.type, aItem.count);
		System.Array.Copy (m_Items, lNewItems, 0);
		lNewItems [lNewItems.Length - 1] = lNew;
		m_Items = lNewItems;
		return aItem.count;
	}

	public int SubItem (InventoryItem aItem)
	{
		foreach (InventoryItem lItem in m_Items) {
			if (lItem.type.Equals (aItem.type)) {
				lItem.count -= aItem.count;
				return lItem.count;
			}
		}
		return 0;
	}

	public string forDisplay ()
	{
		string lRes = string.Empty;
		foreach (InventoryItem lItem in m_Items) {
			if (lItem.count > 0) {
				if (lItem.count > 1) {
					lRes += string.Format ("{0}x{1} ", lItem.count, lItem.type);
				} else {
					lRes += lItem.type + " ";
				}
			}
		}
		return lRes;
	}
}
