using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using ImageProcessingTest05.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using System.Windows;

namespace ImageProcessingTest05.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        private Model _model;

        public void Initialize()
        {
            _model = new Model();
            var listener = new PropertyChangedEventListener(_model, (sender, e) => UpdateBitmap(sender, e));
            _model.Initialize();

            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
        }

        public new void Dispose()
        {
            _model.Dispose();
            base.Dispose();
        }

        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window.WindowState == WindowState.Normal)
            {
                WindowMargin = 0;
            }
            else
            {
                WindowMargin = System.Windows.Forms.SystemInformation.FrameBorderSize.Width
                             + System.Windows.Forms.SystemInformation.FrameBorderSize.Height;
            }
        }


        private void UpdateBitmap(object sender, PropertyChangedEventArgs e)
        {
            var model = sender as Model;

            using (var ms = new MemoryStream())
            {
                model.OriginalBitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, 0);
                OriginalBitmap = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            using (var ms = new MemoryStream())
            {
                model.GrayScaleBitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, 0);
                GrayScaleBitmap = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            using (var ms = new MemoryStream())
            {
                model.BlurBitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, 0);
                BlurBitmap = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            using (var ms = new MemoryStream())
            {
                model.EdgeBitmap.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, 0);
                EdgeBitmap = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            
            DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => SetBigBitmap()));
        }

        private void SetBigBitmap()
        {
            switch (BigBitmapId)
            {
                case 1:
                    BigBitmap = GrayScaleBitmap;
                    break;
                case 2:
                    BigBitmap = BlurBitmap;
                    break;
                case 3:
                    BigBitmap = EdgeBitmap;
                    break;
                default:
                    BigBitmap = OriginalBitmap;
                    break;
            }
        }

        #region ChangeBitmapCommand
        private ListenerCommand<string> _ChangeBitmapCommand;

        public ListenerCommand<string> ChangeBitmapCommand
        {
            get
            {
                if (_ChangeBitmapCommand == null)
                {
                    _ChangeBitmapCommand = new ListenerCommand<string>(ChangeBitmap);
                }
                return _ChangeBitmapCommand;
            }
        }

        public void ChangeBitmap(string parameter)
        {
            BigBitmapId = int.Parse(parameter);
        }
        #endregion ChangeBitmapCommand

        #region BigBitmapId変更通知プロパティ
        private int _BigBitmapId;

        public int BigBitmapId
        {
            get
            { return _BigBitmapId; }
            set
            { 
                if (_BigBitmapId == value)
                    return;
                _BigBitmapId = value;
                RaisePropertyChanged();
            }
        }
        #endregion BigBitmapId変更通知プロパティ

        #region BigBitmap変更通知プロパティ
        private BitmapFrame _BigBitmap;

        public BitmapFrame BigBitmap
        {
            get
            { return _BigBitmap; }
            set
            { 
                if (_BigBitmap == value)
                    return;
                _BigBitmap = value;
                RaisePropertyChanged();
            }
        }
        #endregion BigBitmap変更通知プロパティ

        #region OriginalBitmap変更通知プロパティ
        private BitmapFrame _OriginalBitmap;

        public BitmapFrame OriginalBitmap
        {
            get
            { return _OriginalBitmap; }
            set
            { 
                if (_OriginalBitmap == value)
                    return;
                _OriginalBitmap = value;
                RaisePropertyChanged();
            }
        }
        #endregion OriginalBitmap変更通知プロパティ

        #region GrayScaleBitmap変更通知プロパティ
        private BitmapFrame _GrayScaleBitmap;

        public BitmapFrame GrayScaleBitmap
        {
            get
            { return _GrayScaleBitmap; }
            set
            { 
                if (_GrayScaleBitmap == value)
                    return;
                _GrayScaleBitmap = value;
                RaisePropertyChanged();
            }
        }
        #endregion GrayScaleBitmap変更通知プロパティ

        #region BluredBitmap変更通知プロパティ
        private BitmapFrame _BlurBitmap;

        public BitmapFrame BlurBitmap
        {
            get
            { return _BlurBitmap; }
            set
            { 
                if (_BlurBitmap == value)
                    return;
                _BlurBitmap = value;
                RaisePropertyChanged();
            }
        }
        #endregion BluredBitmap変更通知プロパティ

        #region EdgeBitmap変更通知プロパティ
        private BitmapFrame _EdgeBitmap;

        public BitmapFrame EdgeBitmap
        {
            get
            { return _EdgeBitmap; }
            set
            { 
                if (_EdgeBitmap == value)
                    return;
                _EdgeBitmap = value;
                RaisePropertyChanged();
            }
        }
        #endregion EdgeBitmap変更通知プロパティ

        #region WindowMargin変更通知プロパティ
        private int _WindowMargin;

        public int WindowMargin
        {
            get
            { return _WindowMargin; }
            set
            { 
                if (_WindowMargin == value)
                    return;
                _WindowMargin = value;
                RaisePropertyChanged();
            }
        }
        #endregion WindowMargin変更通知プロパティ

    }
}
