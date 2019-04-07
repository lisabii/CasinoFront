﻿
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.UI;
using Assets.TestApp.TestApp.Scripts;

public class Launcher : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // Start is called before the first frame update
    [SerializeField]
    Text InfoText;

    [SerializeField]
    Text UserInfoText;

    [SerializeField]
    Text PhaseText;

    [SerializeField]
    Text DealerCardText;

    [SerializeField]
    GameObject Seats;
    void Start()
    {
        var userid = PlayerPrefs.GetString("userid");
        PhotonNetwork.AuthValues = new AuthenticationValues(userid);
        PhotonNetwork.LocalPlayer.NickName = userid;

        UpdateUserInfoText();
        PhotonNetwork.ConnectUsingSettings();

        //  PhotonNetwork.JoinLobby("casinowarlobby", new TypedLobby { });
    }

    void UpdateUserInfoText()
    {
        CasinoWarPlayer player = PlayerSingleton.GetPlayer();
        UserInfoText.text = "User ID: " + player.getID() +
            "\nUser Name: " + player.getUserName() +
            "\nCredit: " + player.getNewCredit() ;
    }
    
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("Connected");
        InfoText.text = "Connected";
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Seats.SetActive(true);
        for (int i = 1; i <= 5; i++)
        {
            Button button = Seats.transform.GetChild(i).gameObject.GetComponent<Button>();
            int localIndex = i;
            button.onClick.AddListener(() => { ChangeSeatOnClick(localIndex); });
        }
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "bet", 0 } });
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "credit", PlayerSingleton.GetPlayer().getNewCredit()} });
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            UpdatePlayerGui(player);
        }
    }


    public override void OnLeftRoom()
    {
        //  ResetPlayerGui(PhotonNetwork.LocalPlayer);

        ResetAllSeats();
        Seats.SetActive(false);
        base.OnLeftRoom();
        //PhotonNetwork.SetPlayerCustomProperties(new Hashtable { { "seat", 0 } });
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("OnJoinRommFailed " + message);
    }
   
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("connect to master");
        //  PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("joinrandom failed " + message);


        PhotonNetwork.CreateRoom(null, new RoomOptions {PublishUserId = true, PlayerTtl = 0, MaxPlayers = 5, Plugins = new string[] { "CasinoWarMultiplePlugin" } });
        //, Plugins = new string[] { "CustomPlugin" }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if(propertiesThatChanged.ContainsKey("timer"))
            this.InfoText.text = PhotonNetwork.CurrentRoom.CustomProperties["timer"].ToString();
        if (propertiesThatChanged.ContainsKey("phase"))
        {
            //leave when didn't bet on times
            if((int)propertiesThatChanged["phase"] == 2)
            {
                PhaseText.color = Color.yellow;
            }else if((int)propertiesThatChanged["phase"] == 0)
            {
                PhaseText.color = Color.white;
                ResetAllResults();
                DealerCardText.text = "";
            }else if ((int)propertiesThatChanged["phase"] == 1)
            {
                PhaseText.color = Color.red;
            }
            if ((int)propertiesThatChanged["phase"] == 2 && (int)PhotonNetwork.LocalPlayer.CustomProperties["bet"] == 0)
            {
                //kicked by server
            }
            this.PhaseText.text = "Phase" + PhotonNetwork.CurrentRoom.CustomProperties["phase"].ToString();
        }
        if (propertiesThatChanged.ContainsKey("resultTable"))
        {
            Hashtable resultTable = (Hashtable)propertiesThatChanged["resultTable"];
            
            CasinoWarCardImpl card = JsonUtility.FromJson<CasinoWarCardImpl>(resultTable["dealercard"].ToString());
            DealerCardText.text = "Dealer's Card: " + card.ToString();

            foreach(Player player in PhotonNetwork.PlayerList)
            {
                if (resultTable.ContainsKey(player.ActorNumber))
                {
                    CasinoWarPlayer casinoPlayer = JsonUtility.FromJson<CasinoWarPlayer>(resultTable[player.ActorNumber].ToString());
                    UpdateResultGui(casinoPlayer);
                }
            }
        }
        //   this.InfoText.text = propertiesThatChanged["timer"].ToString();
    }

    public void JoinOnClick()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveOnClick()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    public void ChangeSeatOnClick(int seat)
    {
        Hashtable ht = new Hashtable();
        
        int oldseat = FindSeatByActor(PhotonNetwork.LocalPlayer.ActorNumber);
        if(oldseat == 0)
        {
            return;
        }
        ht["oldseat"] = oldseat;
        ht["seat"] = seat;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(55, ht, raiseEventOptions, sendOptions);
        Debug.Log("change seat to " + seat);
    }

    int FindSeatByActor(int actorNumber)
    {
        Hashtable table = (Hashtable)PhotonNetwork.CurrentRoom.CustomProperties["table"];
        if(table == null)
        {
            return 100;
        }
        for (int seat = 1; seat <= 5; seat++)
        {
            if ((int)table[seat] == actorNumber)
            {
                return (int)seat;
            }
        }
        return 0;
    }

    public void ResetAllSeats()
    {
        for(int i = 1; i<=5; i++)
        {
            ResetSeat(i);
        }
    }
    public void ResetAllResults()
    {
        for (int i = 1; i <= 5; i++)
        {
            ResetResult(i);
        }
    }

    public void ResetSeat(int seat)
    {
        Text actorField = Seats.transform.GetChild(seat).GetChild(0).gameObject.GetComponent<Text>();
        actorField.text = "Empty";
    }

    public void ResetResult(int seat)
    {
        Text actorField = Seats.transform.GetChild(seat).GetChild(1).gameObject.GetComponent<Text>();
        actorField.text = "";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        UpdatePlayerGui(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //  PhotonView photonView = PhotonView.Get(this);
        UpdatePlayerGui(otherPlayer);
        base.OnPlayerLeftRoom(otherPlayer);
    }
    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        UpdatePlayerGui(target);
        base.OnPlayerPropertiesUpdate(target, changedProps);
    }

    public void ReJoinClick()
    {
        PhotonNetwork.Reconnect();
    }
    public void DoBet()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["bet"] != 0 || (int)PhotonNetwork.CurrentRoom.CustomProperties["phase"] == 2)
            return;
        byte evCode = 66;

        CasinoWarPlayer player = PlayerSingleton.GetPlayer();
        player.setBet(100);
        player.setActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "bet", 100 } });

        string playerString = JsonUtility.ToJson(player);
        UpdatePlayerGui(PhotonNetwork.LocalPlayer);

        Debug.Log(playerString);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, playerString, raiseEventOptions, sendOptions);
    }

    //[PunRPC]
    void UpdatePlayerGui(Player player)
    {
        if (!player.CustomProperties.ContainsKey("seat"))
        {
            return;
        }
        Hashtable table = (Hashtable)PhotonNetwork.CurrentRoom.CustomProperties["table"];
        Text actorField;
        for (int seat = 1; seat <= 5; seat++)
        {
            if ((int)table[seat] == 0)
            {
                ResetSeat(seat);
                continue;
            } else if ((int)table[seat] == player.ActorNumber)
            {
                actorField = Seats.transform.GetChild(seat).GetChild(0).gameObject.GetComponent<Text>();
                actorField.text = "Actor Number: " + player.ActorNumber +
                    "\nSeat: " + seat +
                    "\nBet: " + player.CustomProperties["bet"].ToString() +
                    "\nCredit: " + player.CustomProperties["credit"].ToString();
            }
        }
    }

    void UpdateResultGui(CasinoWarPlayer player)
    {
        Hashtable table = (Hashtable)PhotonNetwork.CurrentRoom.CustomProperties["table"];
        Text actorField;
        for (int seat = 1; seat <= 5; seat++)
        {
            if ((int)table[seat] == player.getActorID())
            {
                actorField = Seats.transform.GetChild(seat).GetChild(1).gameObject.GetComponent<Text>();
                actorField.text = "Card: " + player.getMycard().ToString() + 
                    "\nWin Result: " + player.getResult();
            }
        }
        if(player.getActorID() == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { "credit", player.getNewCredit() } });
            PlayerSingleton.GetPlayer().setCredit(player.getNewCredit());
            UpdateUserInfoText();
            UpdatePlayerGui(PhotonNetwork.LocalPlayer);
        }
    }

    //void ResetPlayerGui(Player player)
    //{

    //    int seat = (int)player.CustomProperties["seat"];

    //    Text actorField = Seats.transform.GetChild(seat).gameObject.GetComponent<Text>();

    //    actorField.text = "Empty";
    //}

    void ReloadEmptySeats()
    {
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("create room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("create room failed: " + message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
        InfoText.text = "Disconnected";
        base.OnDisconnected(cause);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        // Debug.Log("EVENT!");
        if (eventCode == 66)
        {
            //Hashtable ht = (Hashtable)photonEvent.CustomData;
            //Card c = (Card)ht["card1"];
            Hashtable json = (Hashtable)photonEvent.CustomData;


            JsonUtility.ToJson(new Hashtable());
            CasinoWarCardImpl card1 = JsonUtility.FromJson<CasinoWarCardImpl>((string)json["playercard"]);
            CasinoWarCardImpl card2 = JsonUtility.FromJson<CasinoWarCardImpl>((string)json["dealercard"]);


            //  Debug.Log("player: "+card1.ToString() + " dealer: " + card2.ToString() + " credit: "+ json["credit"]);
        }
        else if (eventCode == 16)
        {
            Debug.Log("Recieved" + ((Hashtable)photonEvent.CustomData)["error"]);
        }
        //}
        else if (eventCode == 55)
        {
            int seat = (int)((Hashtable)photonEvent.CustomData)["seat"];
            Debug.Log("Recieved" + seat);
            //if the switch seat request is from localplayer, reload the scene. Otherwise, just change it's location.
            //because the local player should always be in the midlle, for instance it's seat is 1, the layout of seats should be:  4 5 (1) 2 3 
            //if (photonEvent.Sender == PhotonNetwork.LocalPlayer.ActorNumber)
            //{
            //    ResetSeats();
            //    foreach (Player player in PhotonNetwork.PlayerList)
            //    {
            //        UpdatePlayerGui(player);
            //    }
            //}
            //else
            //{
            //        int oldseat = (int)((Hashtable)photonEvent.CustomData)["oldseat"];
            //        Text actorField = Seats.transform.GetChild(oldseat).gameObject.GetComponent<Text>();

            //        actorField.text = "Empty";
            //}
        }

    }
}