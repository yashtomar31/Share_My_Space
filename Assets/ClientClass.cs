using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ClientClass : MonoBehaviour {

	// Public variables
	public string host;
	public int port;
    public Transform floor;
    public Transform corner;
	// Private variables
	private int currentFrame = 1;
	NetworkClient networkClient;
	private const short id = 888;
	private bool clientAlreadSetup = false;
	public bool isConnected=false;
    public Transform z;
	
	// Update is called once per frame
	void Update () {

		// Setup client
		if (!clientAlreadSetup) {
			SetupClient ();
		}
			
			if (isConnected) {
            Debug.Log("Floor center"+floor.position);
            Debug.Log("Corner Position" + corner.position);                
				MessageObject m=new MessageObject();
                m.positionx = z.position.x - corner.position.x;
                m.positiony = 1.21f;
                m.positionz = z.position.z - corner.position.z;
                Quaternion a = z.rotation; 
                a.z = 0;
                a.x = 0;
                m.height = m.positiony + 0.454f;
                m.rotation = a;
                
                Debug.Log(z.rotation.y + " "+(a.y+90));
            Debug.Log(m.positionz + " " + m.positionx);                
				SendData(m);
			}

	}

	// Setup client
	void SetupClient(){

		// Initiate a connection
		networkClient = new NetworkClient();
		networkClient.RegisterHandler (MsgType.Connect, OnConnected);
		networkClient.RegisterHandler (MsgType.Error, OnError);
		networkClient.Connect (host, port);
		clientAlreadSetup = true;

	}

	// On connected listener
	void OnConnected(NetworkMessage msg){

		// When connected to the server
		Debug.Log("Client: Connected to server at "+host+":"+port);
		isConnected = true;

	}

	// On error
	void OnError(NetworkMessage msg){

		// When connected to the server
		Debug.Log("Error: " + msg.ToString());

	}

	// Send a message
	public void SendData(MessageObject m){

		// Send 
		if (isConnected) {			
			networkClient.Send (id, m);
			Debug.Log ("Client: SEND");
		} else {
			Debug.Log ("Client: NOT_CONNECTED_YET");
		}

	}

//	//Detect when a client connects to the Server
//    public override void OnClientConnect(NetworkConnection connection)
//    {
//        //Change the text to show the connection on the client side
//        //m_ClientText.text =  " " + connection.connectionId + " Connected!";
//		Debug.Log("Connnected");
//		isConnected = true;
//    }
//
//    //Detect when a client connects to the Server
//    public override void OnClientDisconnect(NetworkConnection connection)
//    {
//        //Change the text to show the connection loss on the client side
//        //m_ClientText.text = "Connection" + connection.connectionId + " Lost!";
//		Debug.Log("Dis-Connnected");
//    }

}
