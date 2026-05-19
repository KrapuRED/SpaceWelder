using UnityEngine;
using System.Collections.Generic;

public enum PerformanceRating
{
    None,
    Excellent,
    BitGood,
    NotOkey,
    Bad

}

public class PerfomanceRatingCalculate : MonoBehaviour
{
    public PerformanceRating GetPerfomanceRatingByTime(List<MissionSuccesData> missionSuccesDatas, int totalMission)
    {
        int onTime = 0;
        int delay = 0;
        int late = 0;

        foreach (var mission in missionSuccesDatas)
        {
            switch (mission.MissionSuccesType)
            {
                case MissionSuccesType.OnTime:
                    onTime++;
                    break;

                case MissionSuccesType.Delay:
                    delay++;
                    break;

                case MissionSuccesType.Late:
                    late++; 
                    break;
            }
        }

       // ===== BAD =====
        if (late >= 2)
        {
            return PerformanceRating.Bad;
        }

        // ===== NOT OKEY =====
        if (late >= 1 || delay >= 2)
        {
            return PerformanceRating.NotOkey;
        }

        // ===== BIT GOOD =====
        if (delay == 1)
        {
            return PerformanceRating.BitGood;
        }

        // ===== EXCELLENT =====
        if (onTime == totalMission)
        {
            return PerformanceRating.Excellent;
        }

        // fallback
        return PerformanceRating.None;
    }
}
