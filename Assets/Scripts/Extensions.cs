using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Extensions {

	public static IList<T> Clone<T>(this IList<T> listToClone) where T: ICloneable{
		return listToClone.Select(item => (T)item.Clone()).ToList();
	}

	public static void SetChildrenTo(this GameObject go, bool b){

		foreach(Transform child in go.transform){
			child.gameObject.SetActive(b);
		}
	}
}
