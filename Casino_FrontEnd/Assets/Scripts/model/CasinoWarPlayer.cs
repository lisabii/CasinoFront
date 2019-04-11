using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
[Serializable]
public class CasinoWarPlayer

{
    [SerializeField]
    private int actorNumber;
    [SerializeField]
    private string userid;
    [SerializeField]
    private string username;
    [SerializeField]
    private CasinoWarCardImpl MyCard;
    [SerializeField]
    private int result;
    private int OldCredit;
    [SerializeField]
    private int Credit;
    [SerializeField]
    private int bet;
    private int WinningAmount;
    private bool Surrender = true;

    public CasinoWarPlayer() { }
    public CasinoWarPlayer(string id, int newbet, int actorID)
    {
        userid = id;
        bet = newbet;
        actorNumber = actorID;
    }

    public string getUserName()
    {
        return username;
    }

    public void SetUserName(string username)
    {
        this.username = username;
    }
    public void setMycard(CasinoWarCardImpl newcard)
    {
        MyCard = newcard;
    }

    public void setGameResult(int result)
    {
        this.result = result;
    }
    public int getGameresult()
    {
        return result;
    }

    public int getNewCredit()
    {
        return Credit;
    }

    public void GoForWar()
    {
        Surrender = false;
    }

    public bool getSurrenderState()
    {
        return Surrender;
    }

    public void SetNewandWinning(int credit, int win)
    {
        Credit = credit;
        WinningAmount = win;
    }

    public CasinoWarCardImpl getMycard()
    {
        return MyCard;
    }

    public string getID()
    {
        return userid;
    }

    public int getResult()
    {
        return result;
    }

    public int getOldCredit()
    {
        return OldCredit;
    }

    public int getWinningamount()
    {
        return WinningAmount;
    }

    public void setOldCredit(int credit)
    {
        OldCredit = credit;
    }

    public void setCredit(int credit)
    {
        Credit = credit;
    }

    public bool hasCreditToWar()
    {
        return OldCredit >= (2 * bet);
    }
    public int getActorID()
    {
        return actorNumber;
    }
    public void setActorNumber(int an)
    {
        this.actorNumber = an;
    }
    public void setBet(int bet)
    {
        this.bet = bet;
    }

    public int Getbet()
    {
        return bet;
    }

}
