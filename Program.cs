using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;



public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("\n\n_____________________________________Hello!________________________\n");
        Thread.Sleep(2000);
        Console.WriteLine("____________________________001010__________________________________\n");
        Console.WriteLine("_____________________________________portscanner______________________\n\n");
        Console.Write("Enter IP address: ");
        string recv_ip = Console.ReadLine();

        // Connecting to socket file
        fixip myObj = new fixip(recv_ip);
    }
}



class fixip
{
    private string recv_ip;

    public fixip(string recv_ip)
    {
        this.recv_ip = recv_ip;
        DisplayIpParts();
    }

    private void DisplayIpParts()
    {
        try
        {
            string[] ip_parts = recv_ip.Split('.');
            if (ip_parts.Length != 4)
            {
                throw new Exception("Invalid IP address format.");
            }
            string ip_part1 = ip_parts[0];
            string ip_part2 = ip_parts[1];
            string ip_part3 = ip_parts[2];
            ip_parts[3] = null;

            List<Thread> threads = new List<Thread>();

            Console.WriteLine("\n\nPlease wait...");

            for (int i = 1; i < 256; i++)
            {
                string range_ips = ip_part1 + "." + ip_part2 + "." + ip_part3 + "." + i.ToString();
                Console.WriteLine(range_ips + ':');

                for (int j = 1; j < 500; j++)
                {
                    int port = j; 
                    Thread thread = new Thread(() => CheckPort(range_ips, port));
                    threads.Add(thread);
                    thread.Start();
                    Thread.Sleep(10);
                }
            }

            foreach (var thread in threads)
            {
                thread.Join(); 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private void CheckPort(string ip, int port)
    {
        
        try
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

            if (clientSocket.Connected)
            {
                Console.WriteLine("\n>>>>>>>>>>>>>> Port: "+port+" open <<<<<<<<<<<<<<\n");

                if (port == 25 || port == 23)
                {
                    // Send data to the server.
                    string data = "HELO";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    clientSocket.Send(buffer);

                    // Receive data from the server.
                    buffer = new byte[1024];
                    int bytesReceived = clientSocket.Receive(buffer);

                    // Convert the data to a string.
                    data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                    // Print the data to the console.
                    Console.WriteLine(">>>banner:>>> "+data);
                }
            }
            clientSocket.Close();
        }

        catch (Exception)
        {
            // Do nothing
        }
    }
}
