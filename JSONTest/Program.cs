using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topaz.Data.Results;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace JSONTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting JSON Test!");

            ///Set Variables
            var table = new Topaz.Data.Results.DynamicTable("SystemsFiles.dbo.TestingTable");
            string testFilePath = @"D:\echo\test\";
            string testFile = "TestFile.json";
            string tfile = testFilePath + testFile;

            ///DELETE current Table Results
            new Topaz.Data.Results.DynamicTable("DELETE FROM SystemsFiles.dbo.TestingTable").Query(null, null, null);

            ///Get JSON File
            var jres = new StreamReader(tfile).ReadToEnd();

            var tResults = Coral.Data.JSON.Parse(jres, false, false, false);

            //Go through each of the JSON rows from the parsed data
            foreach (var row in tResults)
            {
                //Get the row values
                var vals = row.Value;
                //Get the row field with children/grandchildren/greatgrandchildren
                var favinfo = vals["favorite_things"];
                //Convert the child entries into a string
                var fvnfo = favinfo.ToString();
                //Parse child entries into JSON object to get level 1 objects
                var lvl1 = Coral.Data.JSON.Parse(fvnfo, false, false, false);
                //Go through each of the level 1 rows
                foreach (var l1 in lvl1)
                {
                    //Get the level 1 row values
                    var lv1vals = l1.Value;
                    //Convert the level 1 rows to strings
                    string l1vals = lv1vals.ToString();
                    //Parse level 1 entries into JSON object to get level 2 objects
                    var lvl2 = Coral.Data.JSON.Parse(l1vals, false, false, false);
                    //Go through each of the level 2 rows
                    foreach (var l2 in lvl2)
                    {
                        //Get the level 2 row values
                        var lv2vals = l2.Value;
                        //Convert the level 2 rows to strings
                        string l2vals = lv2vals.ToString();
                        //Get the array inside of the level2 value
                        JArray arr = JArray.Parse(l2vals);
                        //Go through each object inside of the array we just grabbed
                        foreach (JObject o in arr.Children<JObject>())
                        {
                            //Convert array object to string
                            var a = o.ToString();
                            //Parse that string into json object
                            var lv3 = Coral.Data.JSON.Parse(a, false, false, false);

                            //Get the JSON values that we want to plug into the test table.
                            int id = vals["level1id"];
                            string title = vals["title"].Value.ToString();
                            //If any of the names on the list have an apostrophe, we need to convert it to be SQL friendly so '' instead of '
                            var nm = Topaz.Safe.ToString(vals["name"].Value).Replace("'", "''");
                            string name = nm.ToString();
                            string nickname = vals["nickname"].Value.ToString();
                            string gender = vals["gender"].Value.ToString();
                            string yrs = vals["years_at_scorpion"].Value.ToString();
                            string branch = vals["branch"].Value.ToString();
                            string email = vals["email"].Value.ToString();
                            string movcode = lv3["movie_code"].Value.ToString();
                            var mv = Topaz.Safe.ToString(lv3["movie"].Value).Replace("'", "''");
                            string movie = mv.ToString();
                            string genre = lv3["genre"].Value.ToString();
                            string rating = lv3["rated"].Value.ToString();
                            
                            //Write it out on the console for testing.
                            Console.WriteLine(id + " " + title + " " + name + " " + nickname + " " + gender + " " + yrs + " " + branch + " " + movcode + " " + movie + " " + genre + " " + rating + " " + email);

                            //Insert string/int data into the custom table
                            var query =
                            @"INSERT INTO SystemsFiles.dbo.TestingTable
                            (ID, Name, Title, Nickname, Gender, YearsAtScorpion, Branch, Email, MovieID, MovieName, MovieGenre, MovieRating) VALUES('" +
                            id + "','" + name + "','" + title + "','" + nickname + "','" + gender + "','" + yrs + "','" + branch + "','" + email + "','" + movcode + "','" + movie + "','" + genre + "','" + rating + "')";

                            new Topaz.Data.Results.DynamicTable(query).Query(null, null, null);

                        }

                    }


                }

            }
        }
    }
}
