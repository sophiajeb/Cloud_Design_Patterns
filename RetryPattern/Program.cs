
using System;

namespace RetryPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true);
            int counter = 10;
            StorageService storageService = new StorageService();

            while(counter > 0) {
                int result = storageService.ReadWriteToRemoteStorage(counter);
                Console.WriteLine(result);
                counter--;
            }

            storageService.DisconnectStorageService();
            Console.ReadLine();
        }
    }
}
