using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class ServerClass : MonoBehaviour {

	// Public variables
	public int port;
	
	// Private state variables
	bool serverAlreadSetup = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// Setup server if not done yet
		if (!serverAlreadSetup) {
			SetupServer ();
		}

	}

	// Setup server
	void SetupServer(){

		// Start listening and set flag
		NetworkServer.Listen (port);
		Debug.Log ("Server: Listening on port " + port + "...");
		NetworkServer.RegisterHandler (MsgType.Connect,OnConnected);
		NetworkServer.RegisterHandler (888,OnUpdateValues);
		serverAlreadSetup = true;
	}

	// On connected values
	void OnConnected(NetworkMessage msg){

		// When connected to the server
		Debug.Log("Server: Connected to client...");

	}

	// On updated values
	void OnUpdateValues(NetworkMessage msg){

		// When values updated
		Debug.Log ("Server: Values updated");
		MessageObject o = msg.ReadMessage<MessageObject>();

		

	}


//	//Detect when a client connects to the Server
//    public override void OnClientConnect(NetworkConnection connection)
//    {
//        //Change the text to show the connection on the client side
//        //m_ClientText.text =  " " + connection.connectionId + " Connected!";
//    }
//
//    //Detect when a client connects to the Server
//    public override void OnClientDisconnect(NetworkConnection connection)
//    {
//        //Change the text to show the connection loss on the client side
//        //m_ClientText.text = "Connection" + connection.connectionId + " Lost!";
//    }




}
