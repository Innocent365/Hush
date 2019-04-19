using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Hush
{
    /// <summary>
    /// Message.xaml 的交互逻辑
    /// </summary>
    public partial class Message
    {
        public Message()
        {
            InitializeComponent();
        }

        private const int MaxCount = 5;
        private const int ItemHeight = 25;

        private int? _selectItemIndex;
        public int? SelectItemIndex
        {
            get { return _selectItemIndex; }
            set
            {
                _selectItemIndex = value;
                if(value!=null && Children.Count > 0)
                    ((MyButton)Children[value.Value]).Activied();
            }
        }      
        public void RenderResultList(string keyStr)
        {
            SelectItemIndex = null;
            Children.Clear();
            if(string.IsNullOrEmpty(keyStr)) return;
            var list = new EntityService().GetEntityList(keyStr);
            for (var i = 0; i < list.Count; i++)
            {
                if (i < ItemHeight)
                {
                    var label = new MyButton(list[i]);                    
                    Children.Add(label);
                }
            }
            MaxHeight = MaxCount * ItemHeight;
        }


        public void NextLine()
        {
            if (SelectItemIndex == null)
                SelectItemIndex = 0;
            else if(SelectItemIndex < Children.Count - 1)
            {
                SelectItemIndex += 1;
            }
        }

        public void LastLine()
        {
            if (SelectItemIndex >= 1)
            {
                SelectItemIndex -= 1;
            }
        }

        public MyButton Choice()
        {
            if (SelectItemIndex != null)
                return ((MyButton) Children[SelectItemIndex.Value]);
            return null;
        }
    }

    public class MyButton : Label
    {
        public MyButton(Entity entity)
        {
            InitStyle();
            Entity = entity;
            var contentPanel = new StackPanel() {Orientation = Orientation.Horizontal,IsHitTestVisible = false};

            var labelTitle = new Label() {Content = Entity.Title, FontSize = 12, Padding = new Thickness(0)};
            var labelText = new Label() {Content = Entity.Text};
            var labelUserName = new Label() {Content = Entity.UserName, FontSize = 10};
            contentPanel.Children.Add(labelTitle);
            contentPanel.Children.Add(labelText);
            contentPanel.Children.Add(labelUserName);
            this.Content = contentPanel;
        }

        private SolidColorBrush _normalStyle = new SolidColorBrush(Colors.Transparent);
        private SolidColorBrush _activeStyle = new SolidColorBrush(Colors.Coral);

        private void InitStyle()
        {
            //Width = 100;
            Height = 25;

            BorderBrush = new SolidColorBrush(Color.FromRgb(254, 244, 180));
            BorderThickness = new Thickness(1);
            Padding = new Thickness(0);
            HorizontalContentAlignment = HorizontalAlignment.Left;
        }

        public void Activied()
        {
            var panel = Parent as Panel;
            if (panel == null) return;
            panel.Children.OfType<MyButton>().ToList().ForEach(p => p.Background = _normalStyle);
            Background = _activeStyle;
        }

        public void Normally()
        {
            Background = _normalStyle;
        }

        public readonly Entity Entity;


        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Activied();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {            
            Normally();
        }
    }
}
