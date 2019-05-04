using UnityEngine;
using System.Collections;
using IDG;
public class TeamManager : IGameManager
{
    public int InitLayer
    {
        get
        {
            return 10;
        }
    }
    public int teamCount;
    public bool[,] enemyTeam;
    public void Init(FSClient client)
    {
        teamCount = 12;
        enemyTeam = new bool[teamCount, teamCount];
        SetEnemy(1, 2);
    }
    public void SetEnemy(int i,int j)
    {
        enemyTeam[i, j] = true;
        enemyTeam[j, i] = true;
    }
    public bool IsEnemy(ITeam a,ITeam b)
    {
        return enemyTeam[a.team, b.team];
    }
}
public interface ITeam
{
    int team
    {
        get;
    }
}
