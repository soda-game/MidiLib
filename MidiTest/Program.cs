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
            string filePath = "testDo.mid";

            //midi読み込み
            MidiSystem.ReadMidi(filePath);

            //結果を参照
            Console.WriteLine("参照＿分解能："+MidiSystem.headerData.timeBase);
            Console.WriteLine("参照＿最初の音階番号：" + MidiSystem.noteDataList[0].leanNum);

            Console.ReadKey();
        }


    }
}
