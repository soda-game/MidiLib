using System;
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
            int baseScale = 10;
            string filePath = "testDo.mid";

            //midi読み込み
            MidiSystem.ReadMidi(filePath,baseScale);

            //結果を参照
            Console.WriteLine("参照＿最初のレーン番号：" + MidiSystem.a_noteDataList[0].leanNum);
            Console.WriteLine("参照＿最初の長さ：" + MidiSystem.a_noteDataList[0].Length);
            Console.WriteLine("参照＿最初のノーツの速さ：" + MidiSystem.a_tempDataList[0].speed);

            Console.ReadKey();
        }


    }
}
