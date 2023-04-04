using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace AutoMouseWPF.Behaviors
{
    public class ShortcutTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty ShortcutProperty =
            DependencyProperty.Register("Shortcut", typeof(List<Key>), typeof(ShortcutTextBoxBehavior), new PropertyMetadata(null));
        public List<Key> Shortcut
        {
            get { return (List<Key>)GetValue(ShortcutProperty); }
            set { SetValue(ShortcutProperty, value); }
        }

        public static readonly DependencyProperty ShortcutChangedCommandProperty =
            DependencyProperty.Register("ShortcutChangedCommand", typeof(ICommand), typeof(ShortcutTextBoxBehavior));

        public ICommand ShortcutChangedCommand
        {
            get { return (ICommand)GetValue(ShortcutChangedCommandProperty); }
            set { SetValue(ShortcutChangedCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            Shortcut = new List<Key>();
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            AssociatedObject.KeyUp += AssociatedObject_KeyUp;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
        }

        private List<Key> _pressedKeys = new List<Key>();

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            Shortcut.Clear();
            // Add the pressed key to the list of currently pressed keys
            if (!_pressedKeys.Contains(e.Key))
                _pressedKeys.Add(e.Key);


        }

        private void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
        {
            // Check if the pressed keys match the shortcut keys
            if (_pressedKeys.Count > 0 )
            {
                _pressedKeys.ForEach(x => Shortcut.Add(x));
                // Execute the shortcut command if it is set
                if (ShortcutChangedCommand.CanExecute(null))
                    ShortcutChangedCommand?.Execute(Shortcut);
            }
            _pressedKeys.Clear();
        }
    }

    public class IntegerOnlyTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int?), typeof(IntegerOnlyTextBoxBehavior), new PropertyMetadata(null));

        public int? Value
        {
            get { return (int?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueChangedCommandProperty =
            DependencyProperty.Register("ValueChangedCommand", typeof(ICommand), typeof(IntegerOnlyTextBoxBehavior));

        public ICommand ValueChangedCommand
        {
            get { return (ICommand)GetValue(ValueChangedCommandProperty); }
            set { SetValue(ValueChangedCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }
        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            string inputText = AssociatedObject.Text;
            // 입력된 텍스트에서 숫자 이외의 문자를 제거함
            string filteredText = new string(inputText.Where(char.IsDigit).ToArray());
            var currentCaretIndex = AssociatedObject.CaretIndex - 1;


            // 필터링된 텍스트와 현재 텍스트가 다른 경우, 현재 텍스트를 필터링된 텍스트로 대체함
            if (filteredText != inputText)
            {
                AssociatedObject.Text = filteredText;

                // 커서 위치를 맨 뒤로 이동시킴
                AssociatedObject.CaretIndex = currentCaretIndex;
            }

            if (int.TryParse(filteredText, out int num) && num > 0)
            {
                // Value 속성에도 값을 저장함
                Value = num;

                // ValueChangedCommand가 설정되어 있으면 실행함
                if (ValueChangedCommand.CanExecute(num))
                {
                    ValueChangedCommand?.Execute(num);
                }
            }
            else
            {
                AssociatedObject.Text = "";
            }
        }
    }
}
