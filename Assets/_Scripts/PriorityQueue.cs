using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<TKey, TElement>
{
	private SortedDictionary<TKey, Queue<TElement>> dictionary = new SortedDictionary<TKey, Queue<TElement>> ();


	public PriorityQueue ()
	{
	}

	public void Enqueue (TKey key, TElement item)
	{
		Queue<TElement> queue;
		if (!dictionary.TryGetValue (key, out queue)) {
			queue = new Queue<TElement> ();
			dictionary.Add (key, queue);
		}

		queue.Enqueue (item);
	}

	public bool isEmpty {
		get { return dictionary.Count == 0; }
	}

	public TElement Dequeue ()
	{
		foreach (TKey key in dictionary.Keys) {
			var queue = dictionary [key];
			var output = queue.Dequeue ();
			if (queue.Count == 0)
				dictionary.Remove (key);

			return output;
		}
		throw new UnityException ("No items to Dequeue:");
	}
}