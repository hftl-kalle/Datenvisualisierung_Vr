using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

public static class CSVParser {

    public static CSVDataObject loadCsv(string file) {

        if (File.Exists(file)) {

            //sanitizer to escape semicolons within " "
            string SEMICOLON_SANATIZER = "/u003B/";

            string content = System.IO.File.ReadAllText(file);
            //regex to find content inside " "
            string regex = "\"(.*?)\"";

            //replace all semicilons inside " " with the sanitizer
            foreach (Match match in Regex.Matches(content, regex))
                content = content.Replace(match.Value, match.Value.Replace(";", SEMICOLON_SANATIZER));

            //split on linebreak
            string[] lines = content.Replace('\r', '\n').Replace("\r\n", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            List<MultidimensionalObject> dataList = new List<MultidimensionalObject>();

            //Go through all lines, split lines on seicolons that are not escaped, replace the semicolons back and remove " ", return value and store it in multidim object
            for (int l = 0; l < lines.Length; l++) {
                object[] temp = new object[4];
                if (lines[l].Split(';').Length < 2 && lines[l].Length > 0) {
                    return null;
                }
                for (int i = 0; i < lines[l].Split(';').Length; i++) {
                    if (i >= 4) break;
                    float f;
                    if (Regex.IsMatch(lines[l].Split(';')[i], regex)) {

                        temp[i] = lines[l].Split(';')[i].Replace(SEMICOLON_SANATIZER, ";").Replace("\"", "");
                        continue;
                    }
                    if (float.TryParse(lines[l].Split(';')[i].Replace(SEMICOLON_SANATIZER, ";"), out f)) { 
                    temp[i] = f; }
                    else
                        temp[i] = lines[l].Split(';')[i].Replace(SEMICOLON_SANATIZER, ";");
                }
                dataList.Add(new MultidimensionalObject(temp[0],temp[1],temp[2],temp[3]));
            }

            //get headline and remove it from content list
            MultidimensionalObject headlines = dataList[0];
            dataList.Remove(dataList[0]);

            // if one value of the column is a string make all values in this columns strings
            if (dataList.Exists(x => x.getX() is string)) dataList.ForEach(x => x.setX(x.getX().ToString()));
            if (dataList.Exists(x => x.getY() is string)) dataList.ForEach(x => x.setY(x.getY().ToString()));
            if (dataList.Exists(x => x.getZ() is string)) dataList.ForEach(x => x.setZ(x.getZ().ToString()));

            //create csv data object and return it
            CSVDataObject obj = new CSVDataObject(file, dataList, (string) headlines.getX(), (string) headlines.getY(), (string) headlines.getZ(), (string) headlines.getW());
            return obj;
        }
        return null;
    }
   
}