using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// Oracle Autonomous Database 홈 개발 인스턴스 접속 헬퍼.
    /// wallet 경로와 접속 정보는 App.config(appSettings Oracle.*)에서 읽고,
    /// ODP.NET 관리 드라이버의 TNS/wallet 구성은 프로세스당 한 번만 수행한다.
    /// 컨트롤 계약에 따라 DB 코드는 Samples(폼 측)에만 존재하며
    /// Modern.Lab.Commons에는 어떤 통신 코드도 두지 않는다.
    /// </summary>
    internal static class OracleDb
    {
        private static readonly object initLock = new object();
        private static bool initialized;

        /// <summary>드라이버의 TNS/wallet 경로를 구성한다. 여러 번 호출해도 안전하다.</summary>
        internal static void EnsureConfigured()
        {
            if (initialized)
            {
                return;
            }

            lock (initLock)
            {
                if (initialized)
                {
                    return;
                }

                string walletDirectory = ConfigurationManager.AppSettings["Oracle.WalletDirectory"];
                if (string.IsNullOrEmpty(walletDirectory))
                {
                    throw new InvalidOperationException("App.config appSettings에 'Oracle.WalletDirectory'가 없습니다.");
                }

                OracleConfiguration.TnsAdmin = walletDirectory;
                OracleConfiguration.WalletLocation = walletDirectory;
                initialized = true;
            }
        }

        /// <summary>구성된 데이터 소스로의 미개방 연결을 만든다.</summary>
        internal static OracleConnection CreateConnection()
        {
            EnsureConfigured();

            string dataSource = ConfigurationManager.AppSettings["Oracle.DataSource"];
            string userId = ConfigurationManager.AppSettings["Oracle.UserId"];
            string password = ConfigurationManager.AppSettings["Oracle.Password"];

            OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder();
            builder.DataSource = dataSource;
            builder.UserID = userId;
            builder.Password = password;
            builder.ConnectionTimeout = 30;

            return new OracleConnection(builder.ConnectionString);
        }

        /// <summary>조회 SQL을 실행해 결과 전체를 DataTable로 반환한다.</summary>
        internal static DataTable ExecuteTable(string sql)
        {
            using (OracleConnection connection = CreateConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        /// <summary>DDL/DML 문을 실행하고 영향받은 행 수를 반환한다.</summary>
        internal static int ExecuteNonQuery(string sql)
        {
            using (OracleConnection connection = CreateConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
