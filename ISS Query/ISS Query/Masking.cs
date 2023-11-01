using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ISS_Client
{
    internal static class Masking
    {
        static readonly DependencyPropertyKey _maskExpressionPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("MaskExpression", typeof(Regex), typeof(Masking), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.RegisterAttached("Mask", typeof(string), typeof(Masking), new FrameworkPropertyMetadata(OnMaskChanged));

        public static readonly DependencyProperty MaskExpressionProperty = _maskExpressionPropertyKey.DependencyProperty;

        public static Regex GetMaskExpression(TextBox textBox)
        {
            if (textBox == null) throw new ArgumentException("textBox");

            return textBox.GetValue(MaskExpressionProperty) as Regex;
        }

        static void SetMaskExpression(TextBox textBox, Regex regex)
        {
            textBox.SetValue(_maskExpressionPropertyKey, regex);
        }

        static void OnMaskChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var textBox = dependencyObject as TextBox;
            var mask = e.NewValue as string;
            textBox.PreviewTextInput -= textBox_PreviewTextInput;
            textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
            DataObject.RemovePastingHandler(textBox, Pasting);

            if (mask == null)
            {
                textBox.ClearValue(MaskProperty);
                textBox.ClearValue(MaskExpressionProperty);
            }
            else
            {
                textBox.SetValue(MaskProperty, mask);
                SetMaskExpression(textBox, textBox.CharacterCasing != CharacterCasing.Normal ? 
                    new Regex(mask, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase) :
                    new Regex(mask, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline));
                textBox.PreviewTextInput += textBox_PreviewTextInput;
                textBox.PreviewKeyDown += textBox_PreviewKeyDown;
                DataObject.AddPastingHandler(textBox, Pasting);
            }
        }


        static void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = sender as TextBox;
            var maskExpression = GetMaskExpression(textBox);

            if (maskExpression == null) return;

            if(e.DataObject.GetDataPresent(typeof(string)))
            {
                var dataFormat = "UnicodeText";
                var pastedText = (e.DataObject.GetData(dataFormat) as string).Trim();
                var proposedText = GetProposedText(textBox, pastedText);

                if (!(maskExpression.Matches(proposedText).Count == proposedText.LongCount(x => x == '\n') + 1))
                {
                    e.CancelCommand();
                }                   
                else
                {
                    var dataObject = new DataObject();
                    dataObject.SetText(proposedText, TextDataFormat.Text);
                    e.DataObject = dataObject;
                }
            }
            else 
            { 
                e.CancelCommand(); 
            }
        }

        static void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var maskExpression = GetMaskExpression(textBox);

            if (maskExpression == null) return;

            var proposedText = GetProposedText(textBox, e.Text);

            e.Handled = !(maskExpression.Matches(proposedText).Count == proposedText.LongCount(x => x == '\n') + 1);
        }

        static void textBox_PreviewKeyDown(object sender, KeyEventArgs e) 
        {
            var textBox = sender as TextBox;
            var maskExpression = GetMaskExpression(textBox);

            if (maskExpression == null) return;

            if (e.Key == Key.Space)
            {
                var proposedText = GetProposedText(textBox, " ");

                e.Handled = !(maskExpression.Matches(proposedText).Count == proposedText.LongCount(x => x == '\n') + 1);
            }
        }

        static string GetProposedText(TextBox textBox, string newText)
        {
            var text = textBox.Text;

            if (textBox.SelectionStart != -1)
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);

            return text.Insert(textBox.CaretIndex, newText);
        }

        public static string GetMask(TextBox textBox)
        {
            if (textBox == null) throw new ArgumentException("textbox");

            return textBox.GetValue(MaskProperty) as string;
        }

        public static void SetMask (TextBox textBox, string mask)
        {
            if (textBox == null) throw new ArgumentException("textbox");

            textBox.SetValue(MaskProperty, mask);
        }
    }
}
