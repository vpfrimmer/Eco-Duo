using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SwissArmyKnife;

public class ProgressionSaver : Singleton<ProgressionSaver> {
	
	[HideInInspector]
	public List<string> unlockedLevels = new List<string>(); // TODO: Débloquer les niveaux
	public Dictionary<string, int> levelScores = new Dictionary<string, int>();
	
	private float timer = 0.0f;
	private bool isRecording = false;
	
	private List<Room> allRooms = new List<Room>();
	
	void Awake () {
		DontDestroyOnLoad(this);
		UpdateLevelStats();
		//TODO Linker le lancement du timer avec les events de lancement de partie
	}
	
	void Update() {
		if(isRecording) {
			timer += Time.deltaTime * Time.timeScale;
		}
	}
	
	void OnLevelWasLoaded(int level) {
		if(Application.loadedLevelName == "LevelSelection") {
			UpdateLevelStats();
		}
	}
	
	void UpdateLevelStats() {
		// Ca regénère la liste des rooms à chaque fois que la méthode est appelée. C'est pas beau mais on est pressés.
		allRooms = new List<Room>(Object.FindObjectsOfType<Room>());
		
		foreach(Room r in allRooms) {
			if(levelScores.ContainsKey(r.roomLevelName)) {
				r.SetBestTime(levelScores[r.roomLevelName]);
			}
		}
	}
	
	void OnGameStart() {
		timer = 0;
		isRecording = true;
	}
	
	void OnGameEnd() {
		isRecording = false;
		SaveProgress();
	}
	
	void SaveProgress() {
		string currentLevelName = "LOL"; //TODO Récupérer le vrai nom du level (Louis?)
		SetLevelScore(currentLevelName, (int)timer);
	}
	
	void SetLevelScore(string levelName, int newScore) {
		// Si le niveau n'est pas encore repertorié, on l'ajoute au dico
		if(!levelScores.ContainsKey(levelName)) {
			levelScores.Add(levelName, newScore);
		}
		else {
			if(levelScores[levelName] < newScore) {
				levelScores[levelName] = newScore;
			}
		}
	}
}
