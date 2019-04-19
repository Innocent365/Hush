using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace Hush
{
    public partial class MainWindow 
    {

        #region ContentPanel Event

        //检查数据库连接
        private void TestConn_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectionState conn = DBUtil.GetConn().State;
            MessageBox.Show(conn.ToString());
        }

        //保存按钮
        private void AddEntityBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (RequireInvalid() == false) return;
            var effectRows = 0;

            var action = "";
            if (_entity == null)
            {
                action = "新增";
                _entity = new Entity();
                //effectRows = new EntityService().AddEntity(_entity);
            }
            else //if (TitleBox.IsReadOnly)
            {
                action = "保存";
            }

            _entity.Title = TitleBox.Text;
            _entity.Text = TextBox.Text;
            _entity.UserName = UserNameBox.Text;
            _entity.Password = PasswordBox.Text;
            _entity.Url = UrlBox.Text;
            _entity.Note = NoteBox.Text;

            _entity.Email = Email.Text;
            _entity.PhoneNum = PhoneNum.Text;
            _entity.Question1 = Question1.Text;
            _entity.Question2 = Question2.Text;
            _entity.Question3 = Question3.Text;
            _entity.Answer1 = Answer1.Text;
            _entity.Answer2 = Answer2.Text;
            _entity.Answer3 = Answer3.Text;

            _entity.IsDelete = CheckBox.IsChecked == true;
            _entity.AppendixName = AppendixName.Text;
            //_entity.Appendix = AppendixName.DataContext as byte[];
            _entity.Note = NoteBox.Text;

            var service = new EntityService();

            if (action == "保存")
                effectRows = service.UpdateEntity(_entity);
            else if (action == "新增")
                effectRows = service.AddEntity(_entity);

            if (effectRows == 1)
            {
                MessageBox.Show(this,string.Format("{0}成功！", action));                
                LockEntity();
            }
            else MessageBox.Show(string.Format("{0}失败！", action));

            if (!string.IsNullOrEmpty(AppendixName.DataContext as string))
            {
                effectRows = service.UploadAppendix(_entity.Id, (string)AppendixName.DataContext);
                MessageBox.Show(string.Format("附件上传{0}！", effectRows == 1 ? "成功" : "失败"));
            }
        }

        private bool RequireInvalid()
        {
            if (string.IsNullOrEmpty(TitleBox.Text)) return false;
            if (string.IsNullOrEmpty(UserNameBox.Text)) return false;
            if (string.IsNullOrEmpty(PasswordBox.Text)) return false;
            //if (string.IsNullOrEmpty(UrlBox.Text)) return false;
            //if (string.IsNullOrEmpty(NoteBox.Text)) return false;
            return true;
        }

        private void HintPanelShow()
        {
            HintPanel.Visibility = Visibility.Visible;

            Point box = InputBox.TranslatePoint(new Point(0, 0), MainPanel);

            Canvas.SetLeft(HintPanel, box.X);
            Canvas.SetTop(HintPanel, box.Y + InputBox.ActualHeight + 3);
        }

        private void HintPanelHide()
        {
            HintPanel.Visibility = Visibility.Hidden;
            HintPanel.RenderResultList(null);
        }


        #region InputBox Event
        private void InputBox_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            HintPanelShow();
        }
        private void InputBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("InputBox_OnTextChanged:{0}", e.Changes);
            var keyStr = InputBox.Text;
            if (string.IsNullOrEmpty(keyStr))
            {
                HintPanel.Visibility = Visibility.Hidden;
                return;
            }
            HintPanel.RenderResultList(keyStr);
        }

        private void TextInput_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
//            if (HintPanel.Visibility == Visibility.Hidden) return;
            Console.WriteLine("TextInput_OnPreviewKeyUp:{0}", e.Key);
            var key = e.Key;
            switch (key)
            {
                case Key.Up:
                    HintPanel.LastLine();
                    goto default;
                case Key.Down:
                    HintPanel.NextLine();
                    goto default;
                case Key.Escape:
                    HintPanel.Visibility = Visibility.Hidden;
                    break;
                case Key.Enter:                    
                    PitchOn(HintPanel.Choice());
                    goto default;
                default:
                    if (HintPanel.Visibility != Visibility.Visible)
                    {
                        if (string.IsNullOrEmpty(InputBox.Text) == false)
                            HintPanelShow();
                    }
                    break;
            }
        }

        private void InputBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _entity = null;
            var textBoxList = ContentPanel.Children.OfType<TextBox>();
            foreach (var tb in textBoxList)
            {                
                tb.Text = null;
            }            
        }

        #endregion
        #endregion

        private void AttachPanel_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MorePanel.Visibility = MorePanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void MainPanel_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var source = e.Source as MyButton;
            if (source != null) PitchOn(source);

        }

        private void MainPanel_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = e.Source as MyButton;
            if (source != null) PitchOn(source);
            else HintPanelHide();
        }

        private void PitchOn(MyButton source)
        {
            if(source == null) return;
            _entity = source.Entity;            
            LockEntity();
            RenderEntity();
            HintPanelHide();
        }

        private Entity _entity;

        private void RenderEntity()
        {
            var properties = typeof(Entity).GetProperties();

            var textBoxList = ContentPanel.Children.OfType<TextBox>().ToList();
            textBoxList.AddRange(MorePanel.Children.OfType<TextBox>());
            foreach (var tb in textBoxList)
            {
                var firstOrDefault = properties.FirstOrDefault(p => tb.Name.StartsWith(p.Name));
                if (firstOrDefault != null)
                    tb.Text = (string) firstOrDefault.GetValue(_entity);
            }
            CheckBox.IsChecked = _entity.IsDelete;

            AppendixName.DataContext = _entity.Appendix;
        }

        private void TitleBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TitleBox.IsReadOnly = !TitleBox.IsReadOnly;
            if (TitleBox.IsReadOnly) LockEntity();
            else UnLockEntity();
        }

        private void LockEntity()
        {
            TitleBox.IsReadOnly = true;
            TitleBox.Background = new SolidColorBrush(Color.FromRgb(223,255,245));
        }

        private void UnLockEntity()
        {
            TitleBox.IsReadOnly = false;
            TitleBox.Background = new SolidColorBrush(Colors.White);
        }

        private void Select_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog(this) == true)
            {
                System.IO.FileInfo file = new FileInfo(fileDialog.FileName);
                if (file.Length > 100 * 1024 * 1024)
                {
                    MessageBox.Show("文件超过100M, 禁止上传");
                    //return;
                }
                if (file.Length > 10 * 1024 * 1024)
                {
                    var sure = MessageBox.Show(this, "文件超出10M", "提示！", MessageBoxButton.YesNo);
                    if (sure != MessageBoxResult.Yes)
                        return;
                }
                AppendixName.DataContext = fileDialog.FileName;
                AppendixName.Text = fileDialog.SafeFileName;
            }
        }

        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = AppendixName.Text;
            if (fileDialog.ShowDialog(this) == true)
            {

                using (FileStream fileStream = new FileStream(fileDialog.FileName, FileMode.Create))
                {
                    new EntityService().DownLoadAppendix(_entity.Id, fileStream);
                    fileStream.Close();
                }
                Process.Start("explorer.exe", fileDialog.FileName.TrimEnd(fileDialog.SafeFileName.ToCharArray()));
            }
        }


        private void AppendixName_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("是否删除","确认删除", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            if (_entity != null)
            {
                var effect = new EntityService().DeleteEntityAttach(_entity);
                if (effect > 0)
                {
                    MessageBox.Show("删除成功！");
                }
                AppendixName.Text = null;

            }
                
        }
    }
}
