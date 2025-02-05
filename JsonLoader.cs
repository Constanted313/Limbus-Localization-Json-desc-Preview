﻿using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Limbus_Localization_UI
{
    //чёто
    public class JsonData
    {
        public List<Data> DataList { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<SimpleDesc> SimpleDesc { get; set; }
    }

    public class SimpleDesc
    {
        public int abilityID { get; set; }
        public string simpleDesc { get; set; }
    }
    ///чёто


    class JsonLoader
    {
        private static List<string> ReadAllLines(string file)
        {
            List<string> ReadedLines = new List<string>();

            using (var fileStream = File.OpenRead(file))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    ReadedLines.Add(line);
                }
            }
            return ReadedLines;
        }

        public static Dictionary<int, Dictionary<string, object>> Json_GetAdressBook(string Json_Файл)
        {
            List<string> JsonLines = ReadAllLines(Json_Файл);

            int ID;
            int ID_JSON;
            string Name_JSON; int LineIndex_Name;
            string Desc_JSON; int LineIndex_Desc;
            string SimpleDesc1_JSON; int LineIndex_SimpleDesc1;
            string SimpleDesc2_JSON; int LineIndex_SimpleDesc2;
            string SimpleDesc3_JSON; int LineIndex_SimpleDesc3;
            string SimpleDesc4_JSON; int LineIndex_SimpleDesc4;
            string SimpleDesc5_JSON; int LineIndex_SimpleDesc5;

            Dictionary<int, Dictionary<string, object>> Json_Dictionary = new();

            // получение содержимого json
            var jsonData = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(Json_Файл));
            foreach (var data in jsonData.DataList)
            {
                ID_JSON = data.Id;
                Name_JSON = data.Name.Replace("\"", "\\\"").Replace("\n", "\\n");
                Desc_JSON = data.Desc.Replace("\"", "\\\"").Replace("\n", "\\n");
                SimpleDesc1_JSON = "{none}";
                SimpleDesc2_JSON = "{none}";
                SimpleDesc3_JSON = "{none}";
                SimpleDesc4_JSON = "{none}";
                SimpleDesc5_JSON = "{none}";
                List<string> SimpleDescs_JSON = new() { };
                foreach (var i in data.SimpleDesc) SimpleDescs_JSON.Add(i.simpleDesc.Replace("\"", "\\\"").Replace("\n", "\\n"));
                Console.WriteLine(SimpleDescs_JSON.Count);
                for (int i = 0; i <= 10 - SimpleDescs_JSON.Count; i++) SimpleDescs_JSON.Add("{none}");
                Console.WriteLine(Name_JSON);
                foreach (var i in SimpleDescs_JSON) {
                    Console.WriteLine(i);
                }

                SimpleDesc1_JSON = SimpleDescs_JSON[0];
                SimpleDesc2_JSON = SimpleDescs_JSON[1];
                SimpleDesc3_JSON = SimpleDescs_JSON[2];
                SimpleDesc4_JSON = SimpleDescs_JSON[3];
                SimpleDesc5_JSON = SimpleDescs_JSON[4];

                Json_Dictionary[ID_JSON] = new Dictionary<string, object>
                {
                    ["Name"] = Name_JSON,
                    ["Desc"] = Desc_JSON,
                    ["SimpleDesc1"] = SimpleDesc1_JSON,
                    ["SimpleDesc2"] = SimpleDesc2_JSON,
                    ["SimpleDesc3"] = SimpleDesc3_JSON,
                    ["SimpleDesc4"] = SimpleDesc4_JSON,
                    ["SimpleDesc5"] = SimpleDesc5_JSON,
                };
            }




            // получение номеров линий desc name и simpledesc в файле
            for (int i = 0; i < JsonLines.Count; i++)
            {
                if (JsonLines[i].Trim().StartsWith("\"id\": "))
                {
                    LineIndex_Name = -1;
                    LineIndex_Desc = -1;
                    LineIndex_SimpleDesc1 = -1;
                    LineIndex_SimpleDesc2 = -1;
                    LineIndex_SimpleDesc3 = -1;
                    LineIndex_SimpleDesc4 = -1;
                    LineIndex_SimpleDesc5 = -1;

                    ID = Convert.ToInt32(JsonLines[i].Trim().Split("\"id\": ")[1].Split(",")[0]);
                    LineIndex_Name = i + 2;
                    LineIndex_Desc = i + 3;
                    try
                    {
                        if (JsonLines[i + 5].Trim().StartsWith($"\"abilityID\": {ID}")) LineIndex_SimpleDesc1 = i + 7;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 9].Trim().StartsWith($"\"abilityID\": {ID}")) LineIndex_SimpleDesc2 = i + 11;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 13].Trim().StartsWith($"\"abilityID\": {ID}")) LineIndex_SimpleDesc3 = i + 15;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 17].Trim().StartsWith($"\"abilityID\": {ID}")) LineIndex_SimpleDesc4 = i + 19;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 21].Trim().StartsWith($"\"abilityID\": {ID}")) LineIndex_SimpleDesc5 = i + 22;
                    }
                    catch { }

                    Json_Dictionary[ID]["LineIndex_Name"] = LineIndex_Name;
                    Json_Dictionary[ID]["LineIndex_Desc"] = LineIndex_Desc;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc1"] = LineIndex_SimpleDesc1;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc2"] = LineIndex_SimpleDesc2;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc3"] = LineIndex_SimpleDesc3;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc4"] = LineIndex_SimpleDesc4;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc5"] = LineIndex_SimpleDesc5;
                    Console.WriteLine(Json_Dictionary[ID]["Name"]);
                    Console.WriteLine($"LineIndex_Name: {LineIndex_Name}\n" +
                        $"LineIndex_Desc: {LineIndex_Desc}\n" +
                        $"LineIndex_SimpleDesc1: {LineIndex_SimpleDesc1}\n" +
                        $"LineIndex_SimpleDesc2: {LineIndex_SimpleDesc2}\n" +
                        $"LineIndex_SimpleDesc3: {LineIndex_SimpleDesc3}\n" +
                        $"LineIndex_SimpleDesc4: {LineIndex_SimpleDesc4}\n" +
                        $"LineIndex_SimpleDesc5: {LineIndex_SimpleDesc5}\n\n\n");
                   // };
                }
            }
            Console.WriteLine($"Прочитано {Json_Dictionary.Keys.Count} объектов из {Json_Файл}");
            return Json_Dictionary;
        }

        public static (string, string) GetUnsavedContent(Dictionary<int, Dictionary<string, object>> CheckDictionary)
        {
            string exp = "";
            string exp_add1 = "";
            bool IsEditedID = false;
            int EditsCount = 0;
            string Element;
            foreach(var ID in CheckDictionary)
            {
                IsEditedID = false;
                exp_add1 = "";
                foreach (var Descriptor in ID.Value)
                {
                    if (Descriptor.Value != "{unedited}")
                    {
                        Element = Descriptor.Key switch
                        {
                            "Desc" => "Описание",
                            _ => $"Простое описание {Descriptor.Key[^1]}",
                        };
                        IsEditedID = true;
                        exp_add1 += $"  - {Element}\n";
                        EditsCount++;
                    }
                }
                if (IsEditedID)
                {
                    exp += $"ID {ID.Key}:\n{exp_add1}\n";
                }
            }

            return (exp, Convert.ToString(EditsCount));
        }


        private static void ConsoleManager(string Json_Файл, Dictionary<int, Dictionary<string, object>> Json_Dictionary)
        {
            // Для консольного отображения
            Console.WriteLine($"Json файл: \x1b[38;5;214m{Json_Файл}\x1b[0m, {Json_Dictionary.Keys.Count} ID");
            while (true)
            {
                Console.Write("\nID ЭГО Дара: \x1b[38;5;214m");
                try
                {
                    int index = Convert.ToInt32(Console.ReadLine()); Console.Write("\x1b[0m");
                    Console.WriteLine($" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_Name"]}) Name\x1b[0m: \"{Json_Dictionary[index]["Name"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_Desc"]}) Desc\x1b[0m: \"{Json_Dictionary[index]["Desc"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc1"]}) SimpleDesc1\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc1"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc2"]}) SimpleDesc2\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc2"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc3"]}) SimpleDesc3\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc3"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc4"]}) SimpleDesc4\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc4"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc5"]}) SimpleDesc5\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc5"]}\"\n\n");
                }
                catch { Console.WriteLine("Нет такого ID"); }
            }
        }
    }
}
