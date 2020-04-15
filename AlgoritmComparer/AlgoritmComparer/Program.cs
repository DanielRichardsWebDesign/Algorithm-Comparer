using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlgoritmComparer.Classes;
using CsvHelper;

namespace AlgoritmComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            //List of recommendations for each algorithm
            List<CommonFriends> commonFriendsRecommendations = new List<CommonFriends>();
            List<InfluenceScore> influenceScoreRecommendations = new List<InfluenceScore>();

            //Temporary lists for storing individual recommendations, before adding them to the recommendations object lists
            List<int> commonFriendIndividualColl = new List<int>();
            List<int> influenceScoreIndividualColl = new List<int>();


            //Read common friends csv file
            using (var reader = new StreamReader(@"recommend_by_number_of_common_friends.csv"))
            using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CommonFriends>();                

                //Console.WriteLine("Number of Common Friends");

                foreach(var record in records)
                {                                       

                    //Console.WriteLine(record.user + ": " + record.top_10_friend_recommendations);

                    //Use Regex to extract individual numbers from the friend recommendations column
                    string[] digits = Regex.Split(record.top_10_friend_recommendations, @"\D+");

                    //Get each individual friend from top_10_friend_recommendations
                    foreach (string value in digits)
                    {
                        int number;
                        if (int.TryParse(value, out number))
                        {
                            //Console.WriteLine(value);

                            commonFriendIndividualColl.Add(number);
                        }

                        //Initiate CommonFriends recommendations list
                        if (record.userFriendsList == null)
                        {
                            record.userFriendsList = new List<int>();
                        }

                        //Add each number from temporary list to CommonFriends list
                        foreach (var item in commonFriendIndividualColl)
                        {                           
                            record.userFriendsList.Add(item);
                        }
                        //Refresh list for new items
                        commonFriendIndividualColl.Clear();                                                                                              
                    }
                    //Add record to CommonFriends list of objects
                    commonFriendsRecommendations.Add(record);                    
                }
            }

            //Console.WriteLine("");

            //Read influence score csv file
            using (var reader = new StreamReader(@"recommend_by_influence_score.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<InfluenceScore>();

                //Console.WriteLine("Influence Score");

                foreach (var record in records)
                {

                    //Console.WriteLine(record.user + ": " + record.top_10_friend_recommendations);

                    string[] digits = Regex.Split(record.top_10_friend_recommendations, @"\D+");

                    //Get each individual friend from top_10_friend_recommendations
                    foreach (string value in digits)
                    {
                        int number;
                        if (int.TryParse(value, out number))
                        {
                            //Console.WriteLine(value);

                            influenceScoreIndividualColl.Add(number);
                        }
                        
                        //Initiate InfluenceScore recommendations list
                        if(record.userFriendsList == null)
                        {
                            record.userFriendsList = new List<int>();
                        }

                        //Add each number from temporary list to InfluenceScore list
                        foreach(var item in influenceScoreIndividualColl)
                        {
                            record.userFriendsList.Add(item);
                        }
                        //Refresh list for new items
                        influenceScoreIndividualColl.Clear();                                            
                    }
                    //Add record to InfluenceScore list of objects
                    influenceScoreRecommendations.Add(record);
                }
            }

            //Display Common Friends Recommendations
            Console.WriteLine("");
            Console.WriteLine("Common Friends Recommendations");

            foreach (var item in commonFriendsRecommendations)
            {
                Console.WriteLine(item.user + ": " + "[{0}]", string.Join(", ", item.userFriendsList.ToArray()));                
            }

            //Display Influence Score Recommendations
            Console.WriteLine("");
            Console.WriteLine("Influence Score Recommendations");

            foreach (var item in influenceScoreRecommendations)
            {
                Console.WriteLine(item.user + ": " + "[{0}]", string.Join(", ", item.userFriendsList.ToArray()));
            }

            List<int> areEqual = new List<int>();
            List<int> notEqual = new List<int>();

            //Compare results and organise users into different categories depending on if the recommendations are the same or not
            foreach(var itemCommon in commonFriendsRecommendations)
            {
                foreach(var itemInfluence in influenceScoreRecommendations)
                {
                    var hashSet1 = new HashSet<int>(itemCommon.userFriendsList);
                    var hashSet2 = new HashSet<int>(itemInfluence.userFriendsList);

                    if(hashSet1.SetEquals(hashSet2) == true)
                    {
                        areEqual.Add(itemCommon.user);
                    }                                  
                }
            }

            //Add all users that aren't in areEqual list to notEqual list
            foreach(var item in commonFriendsRecommendations)
            {
                if(!areEqual.Contains(item.user))
                {
                    notEqual.Add(item.user);
                }
            }

            Console.WriteLine("");

            //Display all users that display the same recommendations
            Console.WriteLine("IDs of users that have the same output under both algorithms:");
            foreach(var item in areEqual)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("");

            //Display all users that display different recommendations
            Console.WriteLine("IDs of users that have different outputs under both algorithms:");
            foreach(var item in notEqual)
            {
                Console.WriteLine(item);
            }

            //Write list results to text file
            using(TextWriter writer = new StreamWriter("Results.txt"))
            {
                writer.WriteLine("IDs of users that have the same output under both algorithms:");
                foreach(var item in areEqual)
                {
                    writer.WriteLine(item.ToString());
                }
                writer.WriteLine("");

                writer.WriteLine("IDs of users that have different outputs under both algorithms:");
                foreach(var item in notEqual)
                {
                    writer.WriteLine(item.ToString());
                }
                writer.WriteLine("");
            }

            //Open results file
            Process.Start("Results.txt");

            Console.ReadKey();             
        }
    }
}
