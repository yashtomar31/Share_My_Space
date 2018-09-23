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
public class cgtest : MonoBehaviour
{


    string eps = "2.2204e-16";
    double epsilon;

    // Use this for initialization
    void Start()
    {
        epsilon = double.Parse(eps, NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);
        //Debug.Log(epsilon);

        cgcoords_setup();

        //dummy val
        Complex val = new Complex(99, -17);

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

        for (int i = 1; i < f_iLength && 2*cage_length == f_iLength; i++)
        {
            pos = pos + mixedC[i] * f_i[i];
        }
        //Debug.Log(cage_length+" "+f_iLength+" "+ f_i[0]);
        
        Debug.Log(pos+"\n"+epsilon);

    }

    // Update is called once per frame
    void Update()
    {

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
