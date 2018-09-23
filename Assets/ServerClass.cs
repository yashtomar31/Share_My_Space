using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Numerics;
using System.Globalization;
using Accord.Math;
using Accord.IO;
public class ServerClass : MonoBehaviour {

    // Public variables
    public int port;
    public Transform floor;
    public Transform corner;
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    //String Host = "127.0.0.1";
    //Int32 Port = 55000;
    string eps = "2.2204e-16";
    double epsilon;
    // Private state variables
    bool serverAlreadSetup = false;
    public Transform man;

    float pathx = 10;
    float pathz = 3.2f;
    float xlim = 10;
    float zlim = 3.3f;

    // Use this for initialization
    void Start() {
        Debug.Log("Floor Position" + floor.position);
        Debug.Log("Floor Scale" + floor.localScale);
        Debug.Log("Floor Corner" + corner.position);
        epsilon = double.Parse(eps, NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);
        //Debug.Log(epsilon);

        cgcoords_setup();
    }

    // Update is called once per frame
    void Update() {

        // Setup server if not done yet
        if (!serverAlreadSetup) {
            SetupServer();
        }
        socketReady = true;

        while (pathx < xlim || pathz < zlim)
        { 
            UnityEngine.Vector3 temp = new UnityEngine.Vector3(pathz, 3, pathx);

            float val1 = pathz / 10;
            float val2 = pathx / 10;

            Complex val = new Complex(val1, val2);
            cgcoords(val);

            int cage_length = src_cage.GetLength(1);
            mixedC = new Complex[2 * cage_length];

            for (int i = 0; i < cage_length; i++)
            {
                mixedC[i] = C[i];
                mixedC[i + cage_length] = Complex.Conjugate(C[i]);
            }

            Complex pos = mixedC[0] * f_i[0];

            for (int i = 1; i < f_iLength && 2 * cage_length == f_iLength; i++)
            {
                pos = pos + mixedC[i] * f_i[i];
            }
            temp = new UnityEngine.Vector3((float)pos.Real * 10 - corner.position.z, 3, (float)pos.Imaginary * 10 - corner.position.x);
            man.position = temp;
            man.transform.Rotate(0, 90, 0);
            if (pathx < xlim)
                pathx += 0.01f;
            if (pathz < zlim)
                pathz += 0.01f;
            Debug.Log(temp);
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
	//	Debug.Log ("Server: Values updated");
		MessageObject o = msg.ReadMessage<MessageObject>();
        //Debug.Log(o.positionx);
        UnityEngine.Vector3 temp = new UnityEngine.Vector3(pathz, o.positiony, pathx);

        float val1 = o.positionz/10;
        float val2 = o.positionx/10;

        Complex val = new Complex(val1, val2);
        //get cg coordinates
        cgcoords(val);

        int cage_length = src_cage.GetLength(1);
        mixedC = new Complex[2 * cage_length];

        for (int i = 0; i < cage_length; i++)
        {
            mixedC[i] = C[i];
            mixedC[i + cage_length] = Complex.Conjugate(C[i]);
        }

        //Debug.Log("conj "+mixedC[775] + " " + mixedC[775+915]);
        Complex pos = mixedC[0] * f_i[0];

        for (int i = 1; i < f_iLength && 2 * cage_length == f_iLength; i++)
        {
            pos = pos + mixedC[i] * f_i[i];
        }
        //Debug.Log(cage_length+" "+f_iLength+" "+ f_i[0]);

        //Debug.Log(pos + "\n" + epsilon);

        temp = new UnityEngine.Vector3((float)pos.Real*10 - corner.position.z, o.positiony, (float)pos.Imaginary*10 - corner.position.x);
        //if (socketReady)
        //{
        //    try
        //    {
        //        mySocket = new TcpClient(Host, Port);
        //        theStream = mySocket.GetStream();
        //        theWriter = new StreamWriter(theStream);
        //        socketReady = true;
        //        var Array1 = new float[] { val1, val2 };

        //        // create a byte array and copy the values into it...
        //        var byteArray = new byte[Array1.Length * 4];
        //        Buffer.BlockCopy(Array1, 0, byteArray, 0, byteArray.Length);


        //        theStream.Write(byteArray, 0, byteArray.Length);
        //        Debug.Log("Val1: " + val1 + " Val2: " + val2);
        //        Debug.Log("socket is sent");
        //    }
        //    catch (Exception e)
        //    {
        //        socketReady = false;
        //        Debug.Log("Matlab Socket error: " + e);
        //    }

        //}
        man.position = temp;
        man.rotation = o.rotation;
        man.transform.Rotate(0, 90, 0);
        Debug.Log(o.positionx+" "+o.positiony + " " + o.positionz);
        


    }


    //int[,] src_cage = new int[17, 2] { { 10, 10 }, { 410, 10 }, { 410, 125 }, { 310, 125 }, { 310, 275 }, { 410, 275 }, { 410, 415 }, { 330, 415 }, { 330, 370 }, { 240, 370 }, { 240, 415 }, { 170, 415 }, { 170, 369 }, { 20, 369 }, { 20, 415 }, { 10, 415 }, { 10, 10 } };
    double[,] src_cage;
    double[,] double_f_i;
    Complex[] cmplx_cage;
    Complex[] cagem1;
    Complex[] A;
    Complex[] Ap1;
    Complex[] B;
    Complex[] Bp1;
    Complex[] Bm1;
    Complex[] logBp1B;
    Complex[] logBBm1;
    bool[] wrongside;
    bool[] badB;
    bool[] badBp1;
    int f_iLength;
    Complex[] C;
    Complex[] mixedC;
    Complex[] f_i;
    void cgcoords_setup()
    {

        // Create a new MAT file reader
        string path_src_cage = Path.Combine("C:/Users/vrproject/Downloads/iccm_orig/iccm_v2", "src_cage.mat");
        //var reader = new MatReader(path_src_cage);
        var reader = new MatReader(new BinaryReader(new FileStream(path_src_cage, FileMode.Open)), false, false);
        //foreach (var field in reader.Fields)
        //    Debug.Log(field.Key);
        src_cage = reader.Read<double[,]>("Y");

        //Debug.Log(src_cage[0, 0]+" "+src_cage[1, 0]);
        //Debug.Log(src_cage.GetLength(0) + " " + src_cage.GetLength(1));
        int cage_length = src_cage.GetLength(1);

        cmplx_cage = new Complex[cage_length];
        cagem1 = new Complex[cage_length];
        A = new Complex[cage_length];
        Ap1 = new Complex[cage_length];
        B = new Complex[cage_length];
        Bp1 = new Complex[cage_length];
        Bm1 = new Complex[cage_length];
        logBp1B = new Complex[cage_length];
        wrongside = new bool[cage_length];
        badB = new bool[cage_length];
        badBp1 = new bool[cage_length];
        logBBm1 = new Complex[cage_length];
        C = new Complex[cage_length];

        for (int i = 0; i < cage_length; i++)
        {
            cmplx_cage[i] = new Complex(src_cage[0, i], src_cage[1, i]);
        }
        //Debug.Log(cmplx_cage[914] + " " + cmplx_cage[234]);
        //Debug.Log(cmplx_cage[775]);
        for (int i = 0; i < cage_length - 1; i++)
        {
            cagem1[i + 1] = cmplx_cage[i];
        }

        cagem1[0] = cmplx_cage[cage_length - 1];
        //Debug.Log(cagem1[775]);


        for (int i = 0; i < cage_length; i++)
        {
            A[i] = cmplx_cage[i] - cagem1[i];
        }
        //Debug.Log(A[775]);


        for (int i = 1; i < cage_length; i++)
        {
            Ap1[i - 1] = A[i];
        }

        Ap1[cage_length - 1] = A[0];
        //Debug.Log(Ap1[775]);


        path_src_cage = Path.Combine("C:/Users/vrproject/Downloads/iccm_orig/iccm_v2", "f_i.mat");
        //var reader = new MatReader(path_src_cage);
        reader = new MatReader(new BinaryReader(new FileStream(path_src_cage, FileMode.Open)), false, false);
        //foreach (var field in reader.Fields)
        //    Debug.Log(field.Key);

        double_f_i = reader.Read<double[,]>("double_f_i");
        //Debug.Log(double_f_i[0, 0] + " " + double_f_i[1, 0]);
        //Debug.Log(double_f_i.GetLength(0)+" "+double_f_i.GetLength(1));

        f_iLength = double_f_i.GetLength(1);

        f_i = new Complex[f_iLength];
        for (int i = 0; i < f_iLength && 2 * cage_length == f_iLength; i++)
        {
            f_i[i] = new Complex(double_f_i[0, i], double_f_i[1, i]);
        }

    }

    void cgcoords(Complex pts)
    {
        int cage_length = src_cage.GetLength(1);


        for (int i = 0; i < cage_length; i++)
        {
            B[i] = cmplx_cage[i] - pts;
        }
        //Debug.Log(B[775]);


        for (int i = 1; i < cage_length; i++)
        {
            Bp1[i - 1] = B[i];
        }

        Bp1[cage_length - 1] = B[0];
        //Debug.Log(Bp1[775]);


        for (int i = 0; i < cage_length - 1; i++)
        {
            Bm1[i + 1] = B[i];
        }

        Bm1[0] = B[cage_length - 1];
        //Debug.Log(Bm1[775]);


        for (int i = 0; i < cage_length; i++)
        {
            logBp1B[i] = Complex.Log(Bp1[i] / B[i]);
        }
        //Debug.Log(Bp1[775] / B[775]+" "+Complex.Log(Bp1[775]/B[775]));
        //Debug.Log(logBp1B[775]);

        for (int i = 0; i < cage_length; i++)
        {
            if ((logBp1B[i].Imaginary + Math.PI) < Math.Sqrt(epsilon))
            {
                Complex temp = new Complex(0, 2 * 3.1416);
                logBp1B[i] += temp;
                wrongside[i] = true;
            }
            else
                wrongside[i] = false;
        }
        //Debug.Log(wrongside[775]);
        //Debug.Log(logBp1B[775]);

        for (int i = 0; i < cage_length; i++)
        {
            if (Complex.Abs(B[i]) < Math.Sqrt(epsilon))
                badB[i] = true;
            else
                badB[i] = false;
        }
        //Debug.Log(badB[775]);



        for (int i = 1; i < cage_length; i++)
        {
            badBp1[i - 1] = badB[i];
        }

        badBp1[cage_length - 1] = badB[0];
        //Debug.Log(badBp1[775]);

        for (int i = 0; i < cage_length; i++)
        {
            if (badBp1[i] == true)
            {
                logBp1B[i] = Complex.Log(7 / B[i]);
            }
        }
        //Debug.Log(logBp1B[775]);

        for (int i = 0; i < cage_length; i++)
        {
            if (badB[i] == true)
            {
                logBp1B[i] = Complex.Log(Bp1[i] / 7);
            }
        }
        //Debug.Log(logBp1B[775]);

        Complex absum = logBp1B[0];
        for (int i = 1; i < cage_length; i++)
        {
            absum = absum + logBp1B[i];
        }

        //Debug.Log("abssum "+ absum);

        for (int i = 0; i < cage_length; i++)
        {
            if (Complex.Abs(absum) < 0.5)
                wrongside[i] = true;
            else
                wrongside[i] = false;
        }
        //Debug.Log(wrongside[775]);

        for (int i = 0; i < cage_length; i++)
        {
            if (badB[i] == true && wrongside[i] == true)
            {
                Complex temp = new Complex(0, 2 * 3.1416);
                logBp1B[i] += temp;
            }
        }
        //Debug.Log(logBp1B[775]);


        for (int i = 0; i < cage_length - 1; i++)
        {
            logBBm1[i + 1] = logBp1B[i];
        }

        logBBm1[0] = logBp1B[cage_length - 1];
        //Debug.Log(logBBm1[775]);

        for (int i = 0; i < cage_length; i++)
        {
            Complex temp = new Complex(0, 2 * 3.1416);
            C[i] = (1 / temp) * ((Bp1[i] / Ap1[i]) * logBp1B[i] - (Bm1[i] / A[i]) * logBBm1[i]);
        }
        //Debug.Log(C[775]);
    }



}
