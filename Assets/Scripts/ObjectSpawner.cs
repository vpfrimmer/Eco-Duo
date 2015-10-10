using UnityEngine;
using System.Collections;

// Classe qui spawn une liste d'objets si ils n'existent pas déjà dans la scène (par nom)
public class ObjectSpawner : MonoBehaviour {
	
	public GameObject[] spawnablePrefabs = new GameObject[0];
	
	// Use this for initialization
	void Awake () {
		foreach(GameObject go in spawnablePrefabs) {
			if(!GameObject.Find(go.name)) {
				
				GameObject newGo = Instantiate(go) as GameObject;
				
				newGo.name = go.name;
				newGo.transform.SetParent(null);
				newGo.transform.position = Vector3.zero;
			}
		}
	}
}
