/*Christers kod


//Läs Qualisys eltidsfil (.txt)
using StreamReader sr = new(strPath, Encoding.UTF8);
string strWind = "";
string? strLine = sr.ReadLine();
for (int i = 0; i < 11; i++)
{
    if (strLine != string.Empty && strLine != null)
    {
        if (strLine.Substring(0, 5) == "Wind:")
        {
            strWind = strLine.Substring(5, 4).Trim();
        }
        if (strLine.Substring(0, 4) == "Rank" || strLine.Substring(0, 4) == "Plac")
        {
            using MySqlConnection con1 = new(_configuration.GetConnectionString("ConnTfdb"));
            {
                string strSql1 = "sp_resIn_Time_import"; //Använd Stored Procedure -      
                using MySqlCommand cmd1 = new(strSql1, con1);
                cmd1.CommandType = CommandType.StoredProcedure;

                string? strResline = sr.ReadLine();
                while (strResline != null)
                {
                    string[] strData = strResline.Split('\t');
                    string strRank = strData[0].Trim();
                    string strLane = strData[1].Trim();
                    string strBib = strData[2].Trim();
                    string strTime = strData[3].Trim();

                    cmd1.Parameters.Clear();
                    cmd1.Parameters.AddWithValue("@compId", intCompId);
                    cmd1.Parameters.AddWithValue("@eventId", intEventId);
                    cmd1.Parameters.AddWithValue("@evNum", intEventNum);
                    cmd1.Parameters.AddWithValue("@roundId", intRoundId);
                    cmd1.Parameters.AddWithValue("@heatId", intHeatId);
                    cmd1.Parameters.AddWithValue("@bib", strBib);
                    cmd1.Parameters.AddWithValue("@lane", strLane);
                    cmd1.Parameters.AddWithValue("@rank", strRank);
                    cmd1.Parameters.AddWithValue("@time", strTime);
                    cmd1.Parameters.AddWithValue("@wind", strWind);

                    con1.Open();
                    cmd1.ExecuteNonQuery();
                    con1.Close();

                    strResline = sr.ReadLine();
                }
            }
        }
    }
    strLine = sr.ReadLine();
}
}
*/