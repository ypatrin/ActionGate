using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace action_gw
{
    class DB
    {
        protected MySqlConnection MySQL;
        protected Log Log = Log.getInstance();

        public bool connect()
        {
            String ConnectionString = String.Format("Server={1}; Database={0}; Uid={2}; Pwd={3}; CharSet=utf8;",
                Properties.Settings.Default.MysqlDB, 
                Properties.Settings.Default.MysqlHost, 
                Properties.Settings.Default.MysqlUser, 
                Properties.Settings.Default.MysqlPass);
            try
            {
                Log.add("Подключаемся к БД...");   
                MySQL = new MySqlConnection(ConnectionString);
                MySQL.Open();
                Log.add("Подключились к БД успешно.");

                return true;
            }
            catch(Exception e)
            {
                Log.add("Ошибка подключения к БД: " + e.Message);
                return false;
            }
        }

        public bool disconnect()
        {
            MySQL.Close();
            return true;
        }
    }
}
