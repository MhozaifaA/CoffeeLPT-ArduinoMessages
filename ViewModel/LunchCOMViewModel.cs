using CoffeeLPT.Uitl.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CoffeeLPT.ViewModel
{
    public class LunchCOMViewModel:ViewModelBase
    {
        #region Init

        bool IsConnected = false;
        string[] Ports;
        SerialPort Port;
        string[] CommandLeds;

        public LunchCOMViewModel()
        {

            _TextConnection = "Connection";
            OnPropertyChanged(nameof(TextConnection));

            CommandLeds = new string[]{ ">ON LED1", ">OFF LED1",
                                        ">ON LED2" , ">OFF LED2",
                                        ">ON LED3" ,  ">OFF LED3"
                                        };

            GetAllPort();
        }


        #endregion

        #region Command


        private ICommand _CommandCloseWindow;
        public ICommand CommandCloseWindow
        {
            get { return _CommandCloseWindow=new CommandT<Window>(window=> {
                window.Close();
            }); }
            set { _CommandCloseWindow = value; }
        }


        private ICommand _CommandConnection;
        public ICommand CommandConnection
        {
            get { return _CommandConnection=new Command(()=> {

                if (!IsConnected && !string.IsNullOrEmpty(SelectCOM))
                {
                    IsConnected = true;
                    Port = new SerialPort(SelectCOM, 9600, Parity.None, 8, StopBits.One);
                    Port.Open();
                    Port.Write("#START\n");

                    _TextConnection = "DisConnection";
                    OnPropertyChanged(nameof(TextConnection));

                    OnPropertyChanged(nameof(CommandSend));

                }
                else
                {
                    IsConnected = false;
                    Port.Write("#STOP\n");
                    Port.Close();

                    _TextConnection = "Connection";
                    OnPropertyChanged(nameof(TextConnection));

                    OnPropertyChanged(nameof(CommandSend));

                }

            },()=> { return !String.IsNullOrEmpty(_SelectCOM); }); }
            set { _CommandConnection = value; }
        }

        private ICommand _CommandSend;
        public ICommand CommandSend
        {
            get { return _CommandSend=new Command(()=> {




                if (String.IsNullOrEmpty(_TextMessage))
                {
                    Port.Write("#LED1OF\n");
                    Port.Write("#LED2OF\n");
                    Port.Write("#LED3OF\n");
                    _ListBot.Clear();
                    Port.Write("#TEXT" + "Clear.." + "#\n");
                    return;
                }

                switch (Array.IndexOf(CommandLeds, _TextMessage.ToUpper()))
                {
                    case 0:
                        Port.Write("#LED1ON\n");
                        Port.Write("#TEXT" + _TextMessage.ToUpper() + "#\n");
                        break;
                    case 1:
                        Port.Write("#LED1OF\n");
                        Port.Write("#TEXT" + _TextMessage.ToUpper() + "#\n");
                        break;


                    case 2:
                        Port.Write("#LED2ON\n");
                        Port.Write("#TEXT" + _TextMessage.ToUpper() + "#\n");
                        break;
                    case 3:
                        Port.Write("#LED2OF\n");
                        Port.Write("#TEXT" + _TextMessage.ToUpper() + "#\n");
                        break;

                    case 4:
                        Port.Write("#LED3ON\n");
                        Port.Write("#TEXT" + _TextMessage.ToUpper() + "#\n");
                        break;
                    case 5:
                        Port.Write("#LED3OF\n");
                        Port.Write("#TEXT" + _TextMessage.ToUpper() + "#\n");
                        break;

                    default:
                        Port.Write("#TEXT" + _TextMessage + "#\n");
                        break;
                }

                _ListBot.Add(_TextMessage);

            }); }
            set { _CommandSend = value; }
        }


        #endregion

        #region Method

        private void GetAllPort()
        {
            Ports = SerialPort.GetPortNames();

           _ListCOM=Ports.ToList();
           OnPropertyChanged(nameof(ListCOM));
        }

        #endregion


        #region List

        private ICollection<string> _ListCOM;
        public ICollection<string> ListCOM
        {
            get { return _ListCOM; }
            set { _ListCOM = value; }
        }


        private ObservableCollection<string> _ListBot=new ObservableCollection<string>();
        public ObservableCollection<string> ListBot
        {
            get { return _ListBot; }
            set { _ListBot = value; }
        }


        #endregion


        #region 

        private string _TextMessage;
        public string TextMessage
        {
            get { return _TextMessage; }
            set { _TextMessage = value;
                OnPropertyChanged(nameof(CommandSend));
            }
        }


        private string _SelectCOM;
        public string SelectCOM
        {
            get { return _SelectCOM; }
            set { _SelectCOM = value;
                OnPropertyChanged(nameof(CommandConnection));
            }
        }

        private string _TextConnection;
        public string TextConnection
        {
            get { return _TextConnection; }
            set { _TextConnection = value; }
        }



        #endregion


    }
}
