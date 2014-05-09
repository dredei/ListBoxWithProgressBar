#region Using

using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

#endregion

#region Copyright

// Пример к статье: http://www.softez.pp.ua/2014/05/10/listbox-with-progressbar-wpf-csharp/
// Автор: dredei

#endregion

namespace ListBoxWithProgressBar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _actions;
        private Thread _thread;

        private class ListBoxDataItem
        {
            public SolidColorBrush BackColor { get; set; }
            public string Title { get; set; }
            public int Progress { get; set; }

            public ListBoxDataItem( string title, Color backColor, int progress = 0 )
            {
                this.Title = title;
                this.BackColor = new SolidColorBrush( backColor );
                this.Progress = 0;
            }

            public ListBoxDataItem( ListBoxDataItem item, int progress, Color backColor )
                : this( item.Title, backColor, progress )
            {
            }

            public ListBoxDataItem( ListBoxDataItem item, Color backColor )
                : this( item.Title, backColor, item.Progress )
            {
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();
            this.FillListBox();
        }

        private void FillListBox()
        {
            this.LbActions.Items.Clear();
            this._actions = new[] { "Action 1", "Action 2", "Action 3", "Action 4", "Action 5" };
            foreach ( string action in this._actions )
            {
                this.LbActions.Items.Add( new ListBoxDataItem( action, Colors.Transparent ) );
            }
        }

        private void Work()
        {
            var random = new Random();
            for ( int i = 0; i < this.LbActions.Items.Count; i++ )
            {
                var lbItem = this.LbActions.Items[ i ] as ListBoxDataItem;
                if ( lbItem == null )
                {
                    return;
                }
                // увеличиваем прогресс
                do
                {
                    lbItem = new ListBoxDataItem( lbItem, lbItem.Progress + 1, Colors.Yellow );
                    lbItem.BackColor.Freeze();
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.DataBind,
                        new Action( () => this.LbActions.Items[ i ] = lbItem ) );
                    Thread.Sleep( random.Next( 0, 50 ) );
                }
                while ( lbItem.Progress < 100 );
                // делаем цвет текущего элемента зеленым
                lbItem = new ListBoxDataItem( lbItem, Colors.GreenYellow );
                lbItem.BackColor.Freeze();
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.DataBind,
                    new Action( () => this.LbActions.Items[ i ] = lbItem ) );
                Thread.Sleep( 50 );
            }
        }

        private void BtnStart_Click( object sender, RoutedEventArgs e )
        {
            this.FillListBox();
            this._thread = new Thread( this.Work );
            this._thread.Start();
        }

        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            // если был создан поток - закрываем
            if ( this._thread != null )
            {
                this._thread.Abort();
            }
        }
    }
}