using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Hush
{
    public class Entity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public string Question1 { get; set; }
        public string Answer1 { get; set; }
        public string Question2 { get; set; }
        public string Answer2 { get; set; }
        public string Question3 { get; set; }
        public string Answer3 { get; set; }
        public DateTime CreteTime { get; set; }
        public string CreateUser { get; set; }
        public string CreateIp { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateIp { get; set; }
        public bool IsDelete { get; set; }
        public string AppendixName { get; set; }
        public object Appendix { get; set; }
        public string Creator { get; set; }
        public string Note { get; set; }

    }


    public class EntityService
    {
        public readonly DbConnection Conn;
        public readonly DbCommand Cmd;
        public readonly string Ip;
        public readonly string ComputerName;

        public EntityService()
        {
            Conn = DBUtil.GetConn();
            Cmd = Conn.CreateCommand();
            Ip = PCUtil.GetUserIp();
            ComputerName = PCUtil.GetUserName();
        }

        public List<Entity> GetEntityList(string keyStr)
        {
            Cmd.CommandText = string.Format("SELECT id, title, text, url, userName, Password, Email, phoneNumber, " +
                                            "Question1, Answer1, Question2, Answer2, Question3, Answer3, " +
                                            "IsDelete, appendixName, Note " + //, appendix 
                                            "FROM Sheet WHERE title like '%{0}%'", keyStr);
            DbDataReader result = Cmd.ExecuteReader();

            var resultList = new List<Entity>();
            while (result.Read())
            {
                var entity = new Entity();
                entity.Id = result["id"].ToString();
                entity.Title = result["title"].ToString();
                entity.Text = result["text"].ToString();
                entity.Url = result["url"].ToString();
                entity.UserName = result["userName"].ToString();
                entity.Password = result["password"].ToString();
                entity.Email = result["email"].ToString();
                entity.PhoneNum = result["phoneNumber"].ToString();

                entity.Question1 = result["question1"].ToString();
                entity.Answer1 = result["answer1"].ToString();
                entity.Question2 = result["question2"].ToString();
                entity.Answer2 = result["answer2"].ToString();
                entity.Question3 = result["question3"].ToString();
                entity.Answer3 = result["answer3"].ToString();

                entity.IsDelete = (bool)result["isDelete"];
                entity.AppendixName = result["AppendixName"].ToString();
                //entity.Appendix = result["Appenddix"];
                entity.Note = result["note"].ToString();

                //entity.CreteTime = DateTime.Parse(result[0].ToString());
                //entity.CreateUser = result[0].ToString();
                //entity.CreateIp = result[0].ToString();
                //entity.UpdateTime = (DateTime)result[0];
                //entity.UpdateIp = result[0].ToString();
                //entity.appndixName = (byte[]) result[0];
                //entity.Appendix = (byte[])result[0];

                resultList.Add(entity);
            }
            return resultList;
        }

        public void DownLoadAppendix(string id, FileStream fileStream)
        {
            string str = string.Format("select appendix from sheet where id='{0}'", id);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(str, Conn.ConnectionString);
            DataSet myds = new DataSet();           
            sda.Fill(myds);
            byte[] files = (Byte[])myds.Tables[0].Rows[0]["appendix"];

            BinaryWriter bw = new BinaryWriter(fileStream);
            bw.Write(files);
            bw.Close();
        }

        public int UploadAppendix(string id, string fileName)
        {
            string str = string.Format("UPDATE sheet SET appendix = @appendix where id='{0}'", id);

            SQLiteCommand sqLiteCommand = Cmd as SQLiteCommand;

            if (sqLiteCommand == null) return 0;
            sqLiteCommand.CommandText = str;
            using (FileStream filsStream = new FileStream(fileName, FileMode.Open))
            {
                BinaryReader br = new BinaryReader(filsStream);
                Byte[] byData = br.ReadBytes((int) filsStream.Length);

                sqLiteCommand.Parameters.Add("@appendix", DbType.Binary, byData.Length);
                sqLiteCommand.Parameters["@appendix"].Value = byData;
            }
            return Cmd.ExecuteNonQuery();
        }

        public int AddEntity(Entity e)
        {
            var now = DateTime.Now.ToString(@"s");//yyyy-MM-dd hh:mm:ss

            string sql =  string.Format("INSERT INTO sheet (id, title, text, url, userName, password, " +
                                        "Email, phonenumber, question1, answer1, question2, answer2," +
                                        "question3, answer3, " +
                                        "createTime, createUser, createIP, " +
                                        "updateTime, updateUser, updateIP, isDelete, appendixName, appendix, note) " +
                                        "VALUES("+"" +
                                        "'{0}','{1}','{2}','{3}','{4}','{5}'," +
                                        "'{6}','{7}','{8}','{9}','{10}','{11}'," +
                                        "'{12}','{13}', " +
                                        "'{14}', '{15}','{16}'," +
                                        "'{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}'" +
                                        ");",
                                        Guid.NewGuid(), e.Title, e.Text, e.Url, e.UserName, e.Password,
                                        e.Email, e.PhoneNum, e.Question1, e.Answer1, e.Question2, e.Answer2,
                                        e.Question3, e.Answer3, 
                                        now, ComputerName, Ip,
                                        now, ComputerName, Ip, false, e.AppendixName, e.Appendix, e.Note);
            Cmd.CommandText = sql;
            Cmd.Prepare();
            return Cmd.ExecuteNonQuery();
        }

        public int UpdateEntity(Entity e)
        {
            var now = DateTime.Now.ToString(@"s");
            String sql = string.Format("UPDATE Sheet SET title = '{0}', text = '{1}', url = '{2}', " +
                                           "userName = '{3}', password = '{4}', Email = '{5}', phonenumber = '{6}', " +
                                           "question1 = '{7}', answer1 = '{8}', question2 = '{9}', answer2 = '{10}'," +
                                           "question3 = '{11}', answer3 = '{12}', " +
                                           "updateTime = '{13}', updateUser = '{14}', updateIP = '{15}', " +
                                           "isDelete = '{16}', appendixName = '{17}', appendix = '{18}', note = '{19}' " +
                                       "WHERE id = '{20}'",
                                        e.Title, e.Text, e.Url, 
                                        e.UserName, e.Password, e.Email, e.PhoneNum, 
                                        e.Question1, e.Answer1, e.Question2, e.Answer2,
                                        e.Question3, e.Answer3, 
                                        now, ComputerName, Ip,
                                        e.IsDelete, e.AppendixName, e.Appendix, e.Note,
                                        e.Id);
            Cmd.CommandText = sql;
            return Cmd.ExecuteNonQuery();
        }

        public int DeleteEntityAttach(Entity e)
        {
            var now = DateTime.Now.ToString(@"s");
            String sql = string.Format("UPDATE Sheet SET appendix = '', appendixName = ''" +
                                       "updateTime = '{0}', updateUser = '{1}', updateIP = '{2}', " +
                                       " WHERE id = '{3}'", now, ComputerName, Ip, e.Id);
            Cmd.CommandText = sql;
            var effect = Cmd.ExecuteNonQuery();
            Cmd.CommandText = "VACUUM";
            Cmd.ExecuteNonQuery();
            return effect;
        }
    }
}
