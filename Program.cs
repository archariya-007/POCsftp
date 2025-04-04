using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
            try
            {
                Console.WriteLine("sftp POC app started....");
                
                // Prompt for password and private key passphrase
                Console.Write("Enter your UserName: ");
                string userName = Console.ReadLine();
                if (userName == "hulk-test")
                {
                    hostName = "sftp3.hsabank.com";
                    portNumber = 27;
                    userName = "BUR459";
                }
                else
                {
                    Console.Write("Enter your HostName [sftp only]: ");
                    hostName = Console.ReadLine();
                    Console.Write("Enter your PortNumber: ");
                    portNumber = int.Parse(Console.ReadLine());
                }
                    Console.Write("Enter your sshHostKeyFingerprint [ssh ..]: ");
                    sshHostKeyFingerprint = Console.ReadLine();
                    Console.Write("Enter your private key passphrase [not required]: ");
                    privateKeyPassphrase = Console.ReadLine();
                    Console.Write("Enter your SshPrivateKeyPath [not required]: ");
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
                    PortNumber = portNumber,
                    SshHostKeyFingerprint = sshHostKeyFingerprint?.Trim() ?? "",
                    //"ssh-rsa 4096 4f:d1:7f:7f:d9:31:22:be:ea:f0:b8:d1:8c:f8:a5:10"
                    //"ssh-rsa 2048 3a:1b:84:fd:24:9e:f1:12:51:32:7b:4e:5a:77:3c:5a"
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
