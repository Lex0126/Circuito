using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization; // Para parsear floats correctamente

[RequireComponent(typeof(ArcadeKartController))]
public class HandInputReceiver : MonoBehaviour
{
    private ArcadeKartController kart; // Referencia al script del kart

    // Configuracion UDP
    private int port = 5005; // Debe coincidir con Python
    private UdpClient udpClient;
    private Thread udpThread; // Hilo para escuchar UDP
    private volatile string lastCommand = "0.0,0.0"; // Ultimo comando recibido

    void Start()
    {
        kart = GetComponent<ArcadeKartController>();

        // Inicia el hilo de escucha
        udpClient = new UdpClient(port);
        udpThread = new Thread(new ThreadStart(ReceiveData));
        udpThread.IsBackground = true; // El hilo muere si el juego cierra
        udpThread.Start();
    }

    // Esta funcion se ejecuta en el hilo secundario
    private void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        while (true) // Bucle de escucha
        {
            try
            {
                byte[] data = udpClient.Receive(ref anyIP); // Espera datos
                string command = Encoding.UTF8.GetString(data);
                lastCommand = command; // Guarda el ultimo comando
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error al recibir UDP: " + e.Message);
            }
        }
    }

    // Update se ejecuta en el hilo principal del juego
    void Update()
    {
        if (kart == null) return; // Chequeo de seguridad

        string command = lastCommand;

        // Caso 1: "parar" es para derrapar (drift)
        if (command == "parar")
        {
            kart.moveInput = 0f;
            kart.turnInput = 0f;
            kart.isDrifting = true;
        }
        // Caso 2: Comando analogico (ej: "0.5,1.0")
        else
        {
            string[] values = command.Split(','); // Divide el string

            if (values.Length == 2) // Revisa si el mensaje es valido
            {
                float turnValue;
                float moveValue;

                // Convierte el texto a numeros (float)
                // InvariantCulture asegura que "0.5" (con punto) funcione
                bool turnParsed = float.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out turnValue);
                bool moveParsed = float.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out moveValue);

                if (turnParsed && moveParsed)
                {
                    // Aplica los valores al script del kart
                    kart.turnInput = turnValue;
                    kart.moveInput = moveValue;
                    kart.isDrifting = false;
                }
            }
        }
    }

    void OnDestroy()
    {
        // Limpiar al salir del juego
        if (udpThread != null) udpThread.Abort(); 
        if (udpClient != null) udpClient.Close(); 
    }
}