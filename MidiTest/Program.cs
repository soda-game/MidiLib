﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//らいぶらりっぽい
using MidiLib;

namespace MidiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "C:/Users/K019G1032/Desktop/testvs/New Unity Project/Assets/sound/testDo.mid";

            //midi読み込み
            MidiSystem.ReadMidi(filePath);

            Console.WriteLine(MidiSystem.noteDataList[0].leanNum);

            Console.ReadKey();
        }


    }
}