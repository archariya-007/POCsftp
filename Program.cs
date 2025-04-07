using System;
using System.IO;
using WinSCP;


namespace POCsftp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string hostName;
            int portNumber;
            string sshHostKeyFingerprint;
            string privateKeyPassphrase;
            string sshPrivateKeyPath;
            string password = "";
            try
            {
                Console.WriteLine("sftp POC app started....");
                
                // Prompt for password and private key passphrase
                Console.Write("Enter your UserName: ");
                string userName = Console.ReadLine();
                if (userName?.Trim().ToLower()  == "hulk-test")
                {
                    hostName = "sftp3.hsabank.com";
                    portNumber = 27;
                    userName = "BUR459";
                }
                else
                {

                    Console.Write("Enter your Password [not required]: ");
                    password = Console.ReadLine();
                    Console.Write("Enter your HostName [sftp only]: ");
                    hostName = Console.ReadLine();
                    Console.Write("Enter your PortNumber: ");
                    portNumber = int.Parse(Console.ReadLine());
                }
                    Console.Write("Enter your sshHostKeyFingerprint [ssh ..]: ");
                    sshHostKeyFingerprint = Console.ReadLine();
                    Console.Write("Enter your private key passphrase [not required]: ");
                    privateKeyPassphrase = Console.ReadLine();
                    Console.Write("Enter your SshPrivateKeyPath [not required *.ppk]: ");
                    sshPrivateKeyPath = Console.ReadLine();

                if (string.IsNullOrEmpty(hostName) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(sshHostKeyFingerprint))
                {
                    Console.WriteLine("keys are missing");
                    // exit applicaiton
                    Environment.Exit(1);
                }


                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    Protocol = Protocol.Sftp,
                    FtpSecure = FtpSecure.None,
                    FtpMode = FtpMode.Passive,
                    HostName = hostName?.Trim() ?? "",
                    UserName = userName?.Trim().ToUpper() ?? "",
                    Password = password?.Trim() ?? "",  
                    PortNumber = portNumber,
                    SshHostKeyFingerprint = sshHostKeyFingerprint?.Trim() ?? "",
                };

                if (!string.IsNullOrEmpty(privateKeyPassphrase))
                    sessionOptions.PrivateKeyPassphrase = privateKeyPassphrase.Trim();

                if (!string.IsNullOrEmpty(sshPrivateKeyPath))
                    sessionOptions.SshPrivateKeyPath = sshPrivateKeyPath.Trim();

                using (Session session = new Session())
                {
                    // Create directory if it doesn't exist
                    string logDirectory = @"C:\POCsftp";
                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    session.SessionLogPath = Path.Combine(logDirectory, "sessionlog.log");


                    // Connect
                    session.Open(sessionOptions);
                    Console.WriteLine("Connected successfully!");

                    //// Transfer options
                    //TransferOptions transferOptions = new TransferOptions
                    //{
                    //    TransferMode = TransferMode.Binary
                    //};

                    //// Transfer file
                    //TransferResult transferResult =
                    //    session.PutFiles(@"C:\path\to\your\local_file", "/remote/path/", false, transferOptions);

                    //transferResult.Check();
                    //Console.WriteLine($"File transferred successfully to /remote/path/your_local_file");
                    session.Close();
                    Console.WriteLine("Connection CLOSED !!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex is SessionException sessionEx)
                {
                    Console.WriteLine($"WinSCP error: {sessionEx.ToString()}");
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
