using MathCore.IO;


var s = "1234567890";

var enumerable = s.EnumerateSegments(3);
var enumerator = enumerable.GetEnumerator();

//const string file_name = @"d:\123\test.txt";

//var watcher = new TextFileContentMonitor(file_name);

//watcher.NewContent += (s, e) =>
//{
//    Console.WriteLine("--------------");
//    Console.WriteLine(e.ToString());
//};

//watcher.Start();

Console.WriteLine("End.");
Console.ReadLine();



return;

//var pe_file = new PEFile("c:\\123\\user32.dll");

////pe_file.ReadData();

////var is_pe = pe_file.IsPE;
//var header = pe_file.GetHeader();

//var range = new Range(-10, 5);

//Console.WriteLine("End.");
//Console.ReadLine();