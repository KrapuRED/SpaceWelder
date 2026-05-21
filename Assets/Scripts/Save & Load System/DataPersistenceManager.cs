using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private FileDataHandler dataHandler;
    private List<IDataPersistence> _dataPersistences = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene!");
        }

        Instance = this;
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

    }

    private void Start()
    {
        this._dataPersistences = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        dataHandler.Save(gameData);

        foreach (IDataPersistence dataPersistenceObj in _dataPersistences)
            dataPersistenceObj.LoadData(gameData);
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No Game Data was found. Initializing data to defaults!");
            NewGame();
        }

        //Load to all script who need it
        foreach (IDataPersistence dataPersistenceObj in _dataPersistences)
            dataPersistenceObj.LoadData(gameData);

        Debug.Log("Loaded extra boom Arm : " + gameData.extraBoomArm);
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in _dataPersistences)
            dataPersistenceObj.SaveData(ref gameData);

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
