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
            Console.WriteLine(MidiSystem.noteDataList[0].leanNum);

            Console.ReadKey();
        }


    }
}
