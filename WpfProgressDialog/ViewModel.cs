using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfProgressDialog
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string prgTitle = "Progress";
        public string PrgTitle
        {
            get
            {
                return prgTitle;
            }
            set
            {
                prgTitle = value;
                NotifyPropertyChanged();
            }
        }

        private string prgStatus = "処理実行中";
        public string PrgStatus
        {
            get
            {
                return prgStatus;
            }
            set
            {
                prgStatus = value;
                NotifyPropertyChanged();
            }
        }

        private int prgMin = 1;
        public int PrgMin
        {
            get
            {
                return prgMin;
            }
            set
            {
                prgMin = value;
                NotifyPropertyChanged();
            }
        }

        private int prgMax = 100;
        public int PrgMax
        {
            get
            {
                return prgMax;
            }
            set
            {
                prgMax = value;
                NotifyPropertyChanged();
            }
        }

        private int prgVal = 0;
        public int PrgVal
        {
            get
            {
                return prgVal;
            }
            set
            {
                prgVal = value;
                int range = (prgMax - prgMin) + 1;
                int percent = (int)(((double)prgVal / range) * 100);
                PrgPer = percent.ToString() + "%";
                NotifyPropertyChanged();
            }
        }

        private string prgPer = "0%";
        public string PrgPer
        {
            get
            {
                return prgPer;
            }
            set
            {
                prgPer = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ExecProgress
        {
            get
            {
                return new BaseCommand(new Action(() =>
                {
                    CancellationTokenSource cancelToken = new CancellationTokenSource();
                    PrgTitle = "処理実行中";
                    PrgVal = 0;
                    PrgMin = 1;
                    PrgMax = 100;
                    ProgressDialog pd = new ProgressDialog(this, () =>
                    {
                        for(PrgVal = 0; PrgVal < PrgMax; PrgVal++)
                        {
                            if(cancelToken != null && cancelToken.IsCancellationRequested)
                            {
                                return;
                            }
                            PrgStatus = "処理" + PrgVal.ToString("000") + "を実行しています";
                            Thread.Sleep(1000);
                        }
                    }, cancelToken);

                    pd.ShowDialog();
                    if (pd.IsCanceled)
                    {
                        MessageBox.Show("キャンセルしました", "Info", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("完了しました", "Info", MessageBoxButton.OK);
                    }
                }));
            }
        }
    }
}
