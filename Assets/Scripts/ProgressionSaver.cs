using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SwissArmyKnife;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

[System.Serializable]
public class Jesus // saves us all
{
	
	public List<string> unlockedLevels = new List<string>();	// Les niveaux déverouillés
	
	public List<string> levels = new List<string>();			// Les deux bouts du dictionnaire de score, celui ci c'est les noms de levels..
	public List<int> levelListScores = new List<int>();			//.. Celui ci, c'est les scores
	
	public void SaveJesus()
	{
		string path = Path.Combine(Application.persistentDataPath, "save.dat");
		
		try
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			}
			
			Debug.Log("Creating save at : " + path);
			
			using (FileStream file = File.Open(path, FileMode.OpenOrCreate))
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(file, this);
			}
			
			Debug.Log("Save created");
			
		}
			catch (System.Exception e)
			{
				Debug.LogWarning("Couldn't save. Error : " + e.ToString());
			}
	}
	
	public static Jesus LoadJesus()
	{
		try
		{
			string path = Path.Combine(Application.persistentDataPath, "save.dat");
			
			Debug.Log("Loading save at : " + path);
			
			using (FileStream file = File.Open(path, FileMode.Open))
			{
				BinaryFormatter bf = new BinaryFormatter();
				return bf.Deserialize(file) as Jesus;
			}  
		}
			catch (System.Exception e)
			{
				Debug.Log("Couldn't load. Error : " + e.ToString());
				return null;
			}      
	}
}


public class ProgressionSaver : SingletonPersistent<ProgressionSaver> {
	
	[HideInInspector]
	public List<string> unlockedLevels = new List<string>(); // TODO: Débloquer les niveaux
	public Dictionary<string, int> levelScores = new Dictionary<string, int>();
	
	public float timer = 0.0f;
	private bool isRecording = false;
	
	private List<Room> allRooms = new List<Room>();
	
	void Awake () {
		
		Jesus save = Jesus.LoadJesus();
		
		if(save != null) {
			Dictionary<string, int> newLevelScores = new Dictionary<string, int>();
			for (int i = 0; i < save.levels.Count; i++) {
				newLevelScores.Add(save.levels[i], save.levelListScores[i]);
			}
			
			unlockedLevels = save.unlockedLevels;
			levelScores = newLevelScores;
		
		}
		
		DontDestroyOnLoad(this);
		UpdateLevelStats();
		
		// Link le lancement du timer avec les events de lancement de partie
		SceneController.OnGameStart += OnGameStart;
		SceneController.OnGameEnd += OnGameEnd;
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
				r.SetBestScore(levelScores[r.roomLevelName]);
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
		string currentLevelName = Application.loadedLevelName;
		SetLevelScore(currentLevelName, (int)timer);
		
		Jesus save = new Jesus();
		
		List<string> levels = new List<string>();
		List<int> scores = new List<int>();
		
		foreach(KeyValuePair<string, int> kvp in levelScores) {
			levels.Add(kvp.Key);
			scores.Add(kvp.Value);
		}
		
		save.unlockedLevels = unlockedLevels;
		save.levelListScores = scores;
		save.levels = levels;
		
		save.SaveJesus();
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
