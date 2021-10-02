using Microsoft.Data.SqlClient;
using System;
using System.Text;
using System.Configuration;

namespace CQRSPattern
{

    /// <summary>
    /// Retry guidance: 
    /// https://docs.microsoft.com/en-us/azure/architecture/best-practices/retry-service-specific 
    /// </summary>
    public class StorageService
    {
        private string connectionString;
        private static SqlConnection _remoteSQLServerAccount;
        private static SqlRetryLogicOption options;
        private static SqlRetryLogicBaseProvider provider;

        public StorageService()
        {
            // Define the retry logic parameters
            options = new SqlRetryLogicOption()
            {
                // Tries 5 times before throwing an exception
                NumberOfTries = 5,
                // Preferred gap time to delay before retry
                DeltaTime = TimeSpan.FromSeconds(1),
                // Maximum gap time for each delay time before retry
                MaxTimeInterval = TimeSpan.FromSeconds(20),
                // SqlException retriable error numbers
                TransientErrors = new int[] { 4060, 1024, 1025 }
            };

            // Create a retry logic provider
            provider = SqlConfigurableRetryFactory.CreateExponentialRetryProvider(options);

            connectionString = ConfigurationManager.ConnectionStrings["RemoteSQLStorageConnectionString"].ConnectionString;
            _remoteSQLServerAccount = new SqlConnection(connectionString);
            
            // Assumes that connection is a valid SqlConnection object 
            // Set the retry logic provider on the connection instance
            _remoteSQLServerAccount.RetryLogicProvider = provider;
            
            using (_remoteSQLServerAccount)
            {
                _remoteSQLServerAccount.Open();
                Console.WriteLine("Connected!");
                Console.WriteLine("State: {0}", _remoteSQLServerAccount.State);
            }
        }

        public void DisconnectStorageService()
        {
            _remoteSQLServerAccount.Close();
            Console.WriteLine("Disconnected!");
        }

        public Int32 ReadWriteToRemoteStorage(int counter)
        {
            Int32 value = 0;
            if (_remoteSQLServerAccount.State == System.Data.ConnectionState.Closed)
            {
                _remoteSQLServerAccount = new SqlConnection(connectionString);

                // Assumes that connection is a valid SqlConnection object 
                // Set the retry logic provider on the connection instance
                _remoteSQLServerAccount.RetryLogicProvider = provider;
                
                _remoteSQLServerAccount.Open();
                using (_remoteSQLServerAccount)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.
                        Append("INSERT INTO dbo.mydata (DataCol, BlobCol) VALUES ").
                        Append($"('Data Column Data {counter}', CONVERT(VARBINARY(MAX), N'Blob Column {counter}')),").
                        Append($"(N'Data Column Data {counter + 1}', CONVERT(VARBINARY(MAX), N'Blob Column {counter + 1}')); ").
                        Append($"SELECT SCOPE_IDENTITY();");

                    using (SqlCommand command = new SqlCommand(sb.ToString(), _remoteSQLServerAccount))
                    {
                        var obj = command.ExecuteScalar();
                        value = (obj == null ? -1 : Convert.ToInt32(obj));
                        Console.WriteLine($"Done {counter}th time(s)!");
                        using (SqlCommand command2 = new SqlCommand($"SELECT * FROM dbo.mydata WHERE Id = {value}", _remoteSQLServerAccount))
                        {
                            SqlDataReader reader = command2.ExecuteReader();
                            while (reader.Read())
                            {
                                Console.WriteLine(String.Format("{0} {1} {2}", reader[0], reader[1], reader[2]));
                            }
                        }
                    }
                }
            }
            return value;
        }

        public void StoreBook(CQRSPattern.Book book)
        {
            if (_remoteSQLServerAccount.State == System.Data.ConnectionState.Closed)
            {
                _remoteSQLServerAccount = new SqlConnection(connectionString);

                // Assumes that connection is a valid SqlConnection object 
                // Set the retry logic provider on the connection instance
                _remoteSQLServerAccount.RetryLogicProvider = provider;

                _remoteSQLServerAccount.Open();
                using (_remoteSQLServerAccount)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.
                        Append($"SET IDENTITY_INSERT [dbo].[Books] ON INSERT INTO[dbo].[Books]([Id], [Title], [Year], [Price]) ").
                        Append($"VALUES({book.Id}, {book.Title}, {book.Year}, {book.Price}) ").
                        Append("SET IDENTITY_INSERT[dbo].[Books] OFF");

                    using (SqlCommand command = new SqlCommand(sb.ToString(), _remoteSQLServerAccount))
                    {
                        var obj = command.ExecuteScalar();
                    }
                }
            }
        }
    }
}
