using System.Net;
using System.Net.Sockets;

public class RelayProxy
{
    private int relay_client, relay_server, ack_client, ack_server, s_reset;
    private byte[] beat_ack = { 0xCB, 0xBD, 0xFD, 0x20, 0xF5 };
    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    UdpClient client = new UdpClient(MainModule.DST_PORT);
    private byte[] RELIEVE = { 0x12, 0x23, 0x34, 0xAB };
    public void MainLoop()
    {
        EndPoint serverEP = new IPEndPoint(IPAddress.Parse(MainModule.DST_IP), MainModule.DST_PORT);
        while (true)
        {
            // receive client data
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            byte[]? data = null;
            try
            {
                data = client.Receive(ref clientEP);
                MainModule.DebugPrint($"Received from client: " + Hexify(data));
            }
            catch (Exception e)
            {
                MainModule.DebugPrint(e.ToString());
            }
            if (data is null || data.Length == 0)
                continue;

            // relieve previous server loop
            RelievePrevThread();

            // relay everything to server
            server.SendTo(data, serverEP);
            MainModule.DebugPrint($"Relay client to server at " + serverEP.ToString());
            Thread t = new Thread(ReceiveServerLoop);
            t.Start(clientEP);

            // heartbeat check
            if (Enumerable.SequenceEqual(beat_ack, data))
            {
                // client sent a heartbeat packet, do immediate reply
                client.Send(data, clientEP);
                ack_client++;
            }
            else
            {
                MainModule.DebugPrint($"Beat_ack=" + Hexify(beat_ack));
                MainModule.DebugPrint($"data    =" + Hexify(data));
                relay_client++;
            }
            PrintStatus();
        }
    }
    private void RelievePrevThread()
    {
        int? p = (server?.LocalEndPoint as IPEndPoint)?.Port;
        if (p is null)
            MainModule.DebugPrint("First relieve!");
        else
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, (int)p);
            MainModule.DebugPrint($"Sending relieve signal to " + ep.ToString());
            client.Send(RELIEVE, ep);
        }
    }
    private void ReceiveServerLoop(object? clientep)
    {
        IPEndPoint? clientEP = clientep as IPEndPoint;

        var tid = Thread.CurrentThread.ManagedThreadId.ToString();
        MainModule.DebugPrint($"    [{tid}] New thread listening on " + server.LocalEndPoint?.ToString());

        while (true)
        {
            byte[] data = new byte[4096];
            int len;
            EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                len = server.ReceiveFrom(data, ref sender);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                s_reset++;
                return;
            }
            if (Enumerable.SequenceEqual(data.Take(len), RELIEVE))
            {
                MainModule.DebugPrint($"    [{tid}] Relieved!");
                return;
            }
            else
            {
                MainModule.DebugPrint($"    [{tid}] received from " + sender.ToString() + " | " + Hexify(data).Substring(0, 20));
            }

            // heartbeat renewal
            // var data = result.Buffer;
            data = data.Take(len).ToArray();
            if (len > 1 && data[0] == 0xCF && data[1] == 0)
            {
                beat_ack = new byte[5];
                beat_ack[0] = 0xCB;
                for (int i = 1; i < 5; i++)
                    beat_ack[i] = data[i + 1];
            }

            if (Enumerable.SequenceEqual(beat_ack, data))
            {
                ack_server += 1;
            }
            else
            {
                client.Send(data, clientEP);
                relay_server += 1;
            }
            // PrintStatus();
        }
    }
    public static string Hexify(byte[] b)
    {
        return string.Join(" ", b.Select(x => x.ToString("X2")));
    }
    private void PrintStatus()
    {
        Console.WriteLine(
            DateTime.Now.ToLongTimeString() + " | "
            + "relay_c: " + relay_client
            + "\trelay_s: " + relay_server
            + "\tack_c: " + ack_client
            + "\tack_s: " + ack_server
            + "\ts_reset: " + s_reset
        );
    }
}