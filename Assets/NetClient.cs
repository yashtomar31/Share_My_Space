

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class NetClient: NetworkDiscovery {
	public override void OnReceivedBroadcast(string fromAddress, string data)
	{
		Debug.Log("Received broadcast from: " + fromAddress+ " with the data: " + data);
	}
}