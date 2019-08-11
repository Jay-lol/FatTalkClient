using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; //Icommand ����� ����

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using WpfApp1.Models;
using WpfApp1.Modules;
using WpfApp1.Service;

namespace WpfApp1.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        public MessengerClient messenger { get; set; }
        public MessangerService messangerService { get; set; }
        private int fcnt = 0;
        private string usernickname = string.Empty;
        private ObservableCollection<Frienddata> _Friendlist;
        private ObservableCollection<Frienddata> _Selectlist;
        private Imessanger _imessanger;
        public MainViewModel(Imessanger imessanger)
        {
            messenger = imessanger.GetMessenger(ResponseMessage);
            usernickname = messenger.userdata.nickname;
            _Selectlist = new ObservableCollection<Frienddata>();
            _Friendlist = new ObservableCollection<Frienddata>();
            fcnt = 0;
            _imessanger = imessanger;
        }
        public ObservableCollection<Frienddata> Selectlist
        {
            get
            {
                return this._Selectlist;
            }
            set
            {
                if (this._Selectlist != value)
                {
                    this._Selectlist = value;
                    this.RaisePropertyChanged(() => this._Selectlist);
                }
            }
        }
        public ObservableCollection<Frienddata> Friendlist
        {
            get
            {
                return _Friendlist;
            }
            set
            {
                _Friendlist = value;
                RaisePropertyChanged("Friendlist");
            }
        }
        public System.Collections.IList SelectFriendlist
        {
            get
            {
                return Selectlist;
            }
            set
            {
                Selectlist.Clear();
                foreach(Frienddata s in value){
                    Selectlist.Add(s);
                }
            }
        }
        public string NICKNAME
        {
            get { return messenger.userdata.nickname; }
            set { usernickname = messenger.userdata.nickname; RaisePropertyChanged("NICKNAME"); }
        }
        public int Fcnt
        {
            get { return fcnt; }
            set { fcnt = value; RaisePropertyChanged("Fcnt"); }
        }
        public void closeWindow()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                    }
                }
            });
        }
        public void ResponseMessage(TCPmessage tcpmessage)
        {
            switch (tcpmessage.Command)
            {
                case Command.logout:
                    Validlogout(tcpmessage.check);
                    break;
                case Command.Removefriend:
                    ValidRemove(tcpmessage.check);
                    break;
                case Command.Refresh:
                    ValidFresh(tcpmessage.Friendcount,tcpmessage.message);
                    break;
                case Command.Makechat:
                    ValidMakechat(tcpmessage.message, tcpmessage.check,tcpmessage.Chatnumber);
                    break;
                case Command.ReceiveJoinchat:
                    Validreceivejoinchat(tcpmessage.check, tcpmessage.Chatnumber, tcpmessage.message);
                    break;
            }

        }

        public void Validreceivejoinchat(int check, int Chatnumber, string message)
        {
            switch (check)
            {
                case 1:
                    MessageBox.Show("�濡 �ʴ�Ǿ����ϴ�.");
                    ChatViewModel chatViewModel = new ChatViewModel(_imessanger);
                    chatViewModel.Chatnumber = Chatnumber;
                    chatViewModel.NICKNAME = message;
                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ChatView chatView = new ChatView(chatViewModel);
                        chatView.Show();
                    });

                    break;
            }

        }

        public void ValidMakechat(string message,int check,int chatnumber)
        {
            switch (check)
            {
                case 0:
                    MessageBox.Show("���� ģ���� ��ϵ� ģ���� �ƴմϴ�. �ٽ� �������ּ���");
                    Selectlist.Clear();
                    break;
                case 1:
                    MessageBox.Show("�α׾ƿ��� �г����� �ֽ��ϴ�. �ٽ� �������ּ���");
                    Selectlist.Clear();
                    break;
                case 2:
                    MessageBox.Show("�̹� ���� ����� ������ ���� �ֽ��ϴ�. �ٽ� �������ּ���");
                    Selectlist.Clear();
                    break;
                case 3:
                    MessageBox.Show("ä�ù��� �����Ǿ����ϴ�.");
                    Selectlist.Clear();
                    ChatViewModel chatViewModel = new ChatViewModel(_imessanger);
                    chatViewModel.Chatnumber = chatnumber;
                    chatViewModel.Usernickname = NICKNAME;
                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ChatView chatView = new ChatView(chatViewModel);
                        chatView.Show();
                    });
                    break;
                case 4:
                    MessageBox.Show("�ʴ��Ϸ��� ģ���� �߿� ģ�� ������ �Ѱ�찡 �ֽ��ϴ�. �ٽ� �������ּ���");
                    Selectlist.Clear();
                    break;
            }
        }
        public void ValidRemove(int check)
        {
            if (check == 1)
            {
                MessageBox.Show("ģ�� ������ �Ϸ�Ǿ����ϴ�. ���ΰ�ħ�� �����ּ���");
            }
        }

        public void Validlogout(int check)
        {
            

            messenger.userdata.Reset();
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Fcnt = 0;
                Friendlist.Clear();
                NICKNAME = string.Empty;
                MainWindow login = new MainWindow();
                login.Show();
            });
            closeWindow();
        }
        public void ValidFresh(int friendcnt,string message)
        {
            JsonHelp jsonHelp = new JsonHelp();
            string[] refresharray = jsonHelp.getRefreshnickarray(message);
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Fcnt = friendcnt;
                Friendlist.Clear();
                //���⼭ ���� �����͸� �޾Ƹ� �ü� �ִٸ�, �׳� add���ָ� ��
                for(int i=0;i<refresharray.Length;i++){
                    Friendlist.Add(new Frienddata(refresharray[i]));
                }
            });
        }
        public ICommand Freshcommand
        {
            get
            {
                RelayCommand command = new RelayCommand(Executefresh);
                return command;
            }
        }
        public void Executefresh()
        {
            if (!messenger.requestFreshcommand(messenger.userdata.nickname))
            {
                MessageBox.Show("������ ������ ������ϴ�.");
            }
        }
        public ICommand Logoutcommand
        {
            get
            {
                RelayCommand command = new RelayCommand(Executelogout);
                return command;
            }
        }
        public void Executelogout()
        {
            if (messenger.requestLogout(messenger.userdata.nickname))
            {
                MessageBox.Show("�α׾ƿ��Ǿ����ϴ�.");
            }
        }
        public ICommand Plusfriendcommand
        {
            get
            {
                RelayCommand command = new RelayCommand(Executeplusfriend);
                return command;
            }
        }
        public void Executeplusfriend()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                PlusfriendView plusfriendView = new PlusfriendView();
                plusfriendView.ShowDialog();
            });

        }
        public ICommand Deletefriendcommand
        {
            get
            {
                RelayCommand command = new RelayCommand(Executedeletefriend);
                return command;
            }
        }
        public void Executedeletefriend()
        {
            string[] sendarray = null;
            sendarray = new string[Selectlist.Count];
            for(int i = 0; i < Selectlist.Count; i++)
            {
                sendarray[i] = Selectlist[i].Fnickname.ToString();
            }
            if (!messenger.requestDeletefriendcommand(sendarray,messenger.userdata.nickname))
            {
                MessageBox.Show("������ ������ ������ϴ�.");
            }
        }
        public ICommand Chatfriendcommand
        {
            get
            {
                RelayCommand command = new RelayCommand(ExecuteChat);
                return command;
            }
        }
        public void ExecuteChat()
        {
            string[] sendarray2 = null;
            sendarray2 = new string[Selectlist.Count];
            for (int i = 0; i < Selectlist.Count; i++)
            {
                sendarray2[i] = Selectlist[i].Fnickname.ToString();
            }
            if (!messenger.requestMakechatcommand(sendarray2,messenger.userdata.nickname))
            {
                MessageBox.Show("������ ������ ������ϴ�.");
            }
        }

        public ICommand Blockfriendcommand
        {
            get
            {
                RelayCommand command = new RelayCommand(Executeblockfriend);
                return command;
            }
        }
        public void Executeblockfriend()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                BlockfriendView blockfriendView = new BlockfriendView();
                blockfriendView.Show();
            });

        }
    }
}