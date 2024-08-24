using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerMail;
    public int coinsCollected;
    public float gameTime;
}

[System.Serializable]
public class GameData
{
    public List<PlayerData> players = new List<PlayerData>();
}

public class scoreManager : MonoBehaviour
{
    public static scoreManager s_ScoreManager;

    public TMP_Text textoNombre;
    public TMP_Text textoMail;

    private string playerName;
    private string playerMail;
    private int coinsCollected;
    private float gameTime;

    private string filePath;

    private void Awake()
    {
        s_ScoreManager = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        filePath = "Assets/JsonFile" + "/Leaderboard.json";
    }

    // Actualizar datos del player actual
    public bool setPersonalData()
    {
        if (textoNombre.text != "" && textoMail.text != "")
        {
            playerName = textoNombre.text;
            playerMail = textoMail.text;
            return true;
        }
        else {
            return false;
        }
    }
    public void setData(int newCoinsCollected, float newGameTime) {
        coinsCollected = newCoinsCollected;
        gameTime = newGameTime;
    }

    public void saveGameData()
    {
        GameData gameData = new GameData();

        // Leer el archivo JSON existente
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }

        // Crear un nuevo objeto PlayerData con los datos actuales
        PlayerData newPlayerData = new PlayerData
        {
            playerName = playerName,
            playerMail = playerMail,
            coinsCollected = coinsCollected,
            gameTime = gameTime
        };

        // Añadir los datos del nuevo jugador
        gameData.players.Add(newPlayerData);

        // Convertir a JSON y guardar en el archivo
        string updatedJson = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, updatedJson);

        Debug.Log("Datos guardados en: " + filePath);
    }

    // Cargar tabla de puntuación
    private GameData loadLeaderboardData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            return gameData;
        }
        else
        {
            Debug.LogWarning("No se encontró el archivo de datos.");
            return new GameData(); // Retorna un objeto GameData vacío si no existe el archivo
        }
    }

    // Método para ordenar los jugadores por tiempo de juego en orden descendente
    private void sortLeaderboard(GameData gameData)
    {
        gameData.players.Sort((player1, player2) => player2.gameTime.CompareTo(player1.gameTime));
    }

    // Método para devolver la lista de jugadores en un string
    private string getLeaderboardAsString(GameData gameData)
    {
        string result = "";

        foreach (PlayerData player in gameData.players)
        {
            result += $"Nombre: {player.playerName}, Monedas: {player.coinsCollected}, Tiempo: {player.gameTime:F2} segundos\n";
        }

        return result;
    }

    //Método que se debe llamar al querer revisar la leaderboard
    public string checkLeaderboard() {
        GameData gameData = loadLeaderboardData();
        sortLeaderboard(gameData);
        return getLeaderboardAsString(gameData);
    }
}

