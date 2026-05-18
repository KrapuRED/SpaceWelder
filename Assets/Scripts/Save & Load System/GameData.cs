using System.Collections.Generic;

[System.Serializable]
public class MissionSuccesData
{
    public string MissionName;
    public float ArriveAt;
    public MissionSuccesType MissionSuccesType;
}

[System.Serializable]
public class GameData 
{
    public int extraBoomArm;
    public float speedUpgrade;
    public float weldingAreaUpgrade;
    public List<MissionSuccesData> missionSuccesDatas;

    public GameData()
    {
        this.extraBoomArm = 0;
        this.speedUpgrade = 0;
        this.weldingAreaUpgrade = 0;
        this.missionSuccesDatas = new List<MissionSuccesData>();
    }
}

