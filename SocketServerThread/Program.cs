using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace SocketServerThread
{
    public class Program
    {
        static int tries = 10;
        static int guess;
        static int high = 100;
        static int low = 1;
        static int number_to_guess;
        static List<StreamWriter> clientsStreams = new List<StreamWriter>();

        public static void Main(string[] args)
        {
            int port = 20001;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Server is startet listining on port " + port.ToString());
            Program program = new Program();
            Random rnd = new Random();
            number_to_guess = rnd.Next(1, 100);

            while (true)
            {
                Socket client = server.AcceptSocket();
                new Thread(() => Do_when_new_tread(client)).Start();

            }


        }

        public static void Do_when_new_tread(Socket client)
        {
            
            NetworkStream stream = new NetworkStream(client);
            StreamReader streamReader = new StreamReader(stream);
            StreamWriter streamWriter = new StreamWriter(stream);
            clientsStreams.Add(streamWriter);
            streamWriter.AutoFlush = true;
            streamWriter.WriteLine("Enter Name : ");
            string clientname = streamReader.ReadLine() + client.RemoteEndPoint;
            Console.WriteLine("Client connected with info:" + clientname);
            Game(client, streamReader, streamWriter);
        }



        static public void Game(Socket client, StreamReader reader, StreamWriter writer)
        {
            Random rnd = new Random();
            writer.WriteLine("Guess number between " + low + " and " + high + ", you have " + tries + " tries");
            try
            {
                while (tries != 0)
                {
                    if (tries == 0)
                    {
                        Broadcast("You Looooooooooooose !!!");
                        Thread.Sleep(1000);
                        tries = 10;
                        low = 0;
                        high = 100;
                        number_to_guess = rnd.Next(1, 100);
                        Game(client, reader, writer);
                    }

                    guess = int.Parse(reader.ReadLine());
                    if (guess == number_to_guess)
                    {
                        Broadcast("You win!");
                        Thread.Sleep(1000);
                        tries = 10;
                        low = 0;
                        high = 100;
                        number_to_guess = rnd.Next(1, 100);
                        Game(client, reader, writer);

                    }
                    else if (guess > number_to_guess)
                    {
                        tries = tries - 1;
                        high = guess;
                        Broadcast("Your guess = " + guess + "  Guess number between " + low + " and " + high + "  " + tries + " tries left");

                    }
                    else if (guess < number_to_guess)
                    {
                        tries = tries - 1;
                        low = guess;
                        Broadcast("Your guess = " + guess + "  Guess number between " + low + "and " + high + "  " + tries + " tries left");
                    }


                }
            }
            catch (Exception x)
            {

            }
        }

        static public void Broadcast(string message)
        {
            for (int i = 0; i < clientsStreams.Count; i++)
            {

                clientsStreams[i].WriteLine(message.ToString());

            }
        }
    }
}
