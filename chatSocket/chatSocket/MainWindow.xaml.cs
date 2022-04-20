using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;

namespace chatSocket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket = null;
        DispatcherTimer dTimer = null;
        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);   //addressFamily(enum) specifica quali sono le modalità di lavoro la nostra
                                                                                                   //è internetwork(ipv4), SocketType(enum) noi usaimo Dgram per lavorare in udp,
                                                                                                   //ProtocolType.Udp per specificare il protocollo che usiamo

            IPAddress local_address = IPAddress.Any;    //alla variabile viene assegnato un indirizzo ip in modo automatico
            IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 65000);  //creo un oggetto endpoint e gli dico chi è che invia e specifico manualmente la porta


            socket.Bind(local_endpoint);    //unisce socket(canale) e endpoint(mittente)


            dTimer = new DispatcherTimer();     //creazione oggetto

            dTimer.Tick += new EventHandler(aggiornamento_dTimer);  //cosa fare quando scatta il timer (aggiornamento_dTimer)
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);    //ogni quanto fa scattare il tick, quindi ogni quanto va a fa aggiornamento_dTimer (250 ms)
            dTimer.Start();
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = IPAddress.Parse(txtIp.Text);     //creo il destinatario mettendo il contenuto del txtIp in cui c'è l'ip del destinatorio

            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, int.Parse(txtPorta.Text));  //prendo la porta dal txtPorta in cui c'è la porta del mittente

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);   //converto il messaggio preso dalla txtMessaggio in byte

            socket.SendTo(messaggio, remote_endpoint);  //invio il messaggio specificando il messaggio da inviare e a chi inviarlo
        }

        private void aggiornamento_dTimer(object sender, EventArgs e)   //metodo per l'aggiornamento del dTimer
        {
            int nBytes = 0;

            if((nBytes = socket.Available) > 0) //verifico se qualcuno mi ha mandato qualcosa, se si entro nell'if
            {
                byte[] buffer = new byte[nBytes];   //quanti byte devo leggere

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                //ricezione:
                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);

                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                lstChat.Items.Add(from + ": " + messaggio);

            }
        }
    }
}
