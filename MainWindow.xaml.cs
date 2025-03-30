using System;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Runtime;
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

namespace AutoTradeLauncher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //Хэндл подключения
        private IntPtr handle;

        public MainWindow()
        {
            InitializeComponent();
            LotSizeTB.PreviewTextInput += NumericTextBox_PreviewTextInput;
            TPTB.PreviewTextInput += IntTextBox_PreviewTextInput;
            SLTB.PreviewTextInput += IntTextBox_PreviewTextInput;
            CountTB.PreviewTextInput += IntTextBox_PreviewTextInput;
            DropTB.PreviewTextInput += IntTextBox_PreviewTextInput;

            LotSizeTB.Text = Properties.Settings.Default.LotSize;
            TPTB.Text = Properties.Settings.Default.TP;
            SLTB.Text = Properties.Settings.Default.SL;
            CountTB.Text = Properties.Settings.Default.Count;
            DropTB.Text = Properties.Settings.Default.Drop;

            ConsoleLog.AppendText(CreateLogMessage("----------Лаунчер робота запущен, удачной торговли!----------"));

            ValidateUser();

            CreateDefaultFile();
        }

        private void CreateDefaultFile()
        {
            try
            {
                // Получаем путь к папке с exe-файлом
                string exePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string filePath = System.IO.Path.Combine(exePath, "data.txt");
                if (File.Exists(filePath)) return;

                // 5 значений для записи
                string[] values = {
                    "0.01",
                    "48",
                    "24",
                    "1",
                    "3"
                };

                // Записываем значения в файл
                File.WriteAllLines(filePath, values);
                ConsoleLog.AppendText(CreateLogMessage("Создан файл настроек data.txt"));

            }
            catch (Exception ex)
            {
                ConsoleLog.AppendText(CreateLogMessage($"Ошибка при создании файла настроек: {ex.Message}"));
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string msg = ValidateDatas();
            if (ValidateDatas() != "")
            {
                MessageBox.Show(msg, "Неправильно заданы данные", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //Валидацию прошли, сохраняем данные
            Properties.Settings.Default.LotSize = LotSizeTB.Text;
            Properties.Settings.Default.TP = TPTB.Text;
            Properties.Settings.Default.SL = SLTB.Text;
            Properties.Settings.Default.Count = CountTB.Text;
            Properties.Settings.Default.Drop = DropTB.Text;
            Properties.Settings.Default.Save();
        }

        private string ValidateDatas()
        {
            string msg = "";
            if(!double.TryParse(LotSizeTB.Text, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out _))
            {
                msg = "Значение объем лота должно быть дробным.";
            }
            else if (!int.TryParse(TPTB.Text, out _))
            {
                msg = "Значение TP должно быть целым числом.";
            }
            else if (!int.TryParse(SLTB.Text, out _))
            {
                msg = "Значение SL должно быть целым числом.";
            }
            else if (!int.TryParse(CountTB.Text, out _))
            {
                msg = "Значение максимального числа сделок должно быть целым числом.";
            }
            else if (!int.TryParse(DropTB.Text, out _))
            {
                msg = "Значение объем неудачных сделок должно быть целым числом.";
            }

            return msg;
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // Проверяем, что введённый текст — число с одной точкой/запятой
            bool isNumeric = double.TryParse(newText,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out _);

            // Разрешаем только цифры и точку для дабла
            bool isAllowedChar = e.Text.All(c => char.IsDigit(c) || c == '.');

            e.Handled = !isNumeric || !isAllowedChar;
        }

        private void IntTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // Проверяем, что введённый текст — число
            bool isNumeric = int.TryParse(newText, out _);

            // Разрешаем только цифры и точку для дабла
            bool isAllowedChar = e.Text.All(c => char.IsDigit(c));

            e.Handled = !isNumeric || !isAllowedChar;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LotSize = LotSizeTB.Text;
            Properties.Settings.Default.TP = TPTB.Text;
            Properties.Settings.Default.SL = SLTB.Text;
            Properties.Settings.Default.Count = CountTB.Text;
            Properties.Settings.Default.Drop = DropTB.Text;
            Properties.Settings.Default.Save();
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateDefaultFile();
            string filename = "data.txt";
            string exePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(exePath, filename);
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length != 5)
            {
                return;
            }
            LotSizeTB.Text = lines[0];
            TPTB.Text = lines[1];
            SLTB.Text = lines[2];
            CountTB.Text = lines[3];
            DropTB.Text = lines[4];

            ConsoleLog.AppendText(CreateLogMessage("Настройки из файла data.txt загружены"));

            Properties.Settings.Default.LotSize = LotSizeTB.Text;
            Properties.Settings.Default.TP = TPTB.Text;
            Properties.Settings.Default.SL = SLTB.Text;
            Properties.Settings.Default.Count = CountTB.Text;
            Properties.Settings.Default.Drop = DropTB.Text;
            Properties.Settings.Default.Save();
        }

        private string CreateLogMessage(string text)
        {
            DateTime now = DateTime.Now;
            return "[" + now.ToString("HH:mm:ss") + "]: " + text + "\n";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateDefaultFile();
            try
            {
                string exePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string filePath = System.IO.Path.Combine(exePath, "data.txt");
                // 5 значений для записи
                string[] values = {
                    LotSizeTB.Text,
                    TPTB.Text,
                    SLTB.Text,
                    CountTB.Text,
                    DropTB.Text
                };

                // Записываем значения в файл
                File.WriteAllLines(filePath, values);

                Properties.Settings.Default.LotSize = LotSizeTB.Text;
                Properties.Settings.Default.TP = TPTB.Text;
                Properties.Settings.Default.SL = SLTB.Text;
                Properties.Settings.Default.Count = CountTB.Text;
                Properties.Settings.Default.Drop = DropTB.Text;
                Properties.Settings.Default.Save();

                ConsoleLog.AppendText(CreateLogMessage("Настройки сохранены в файл data.txt"));
            }
            catch (Exception ex)
            {
                ConsoleLog.AppendText(CreateLogMessage($"Ошибка при сохранении настроек: {ex.Message}"));
            }
        }

        private void ValidateUser()
        {
            string exePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(exePath, "mt5user.txt");
            if (!File.Exists(filePath))
            {
                MessageBox.Show(
                    "Отсутствует файл подключения mt5user.txt, лаунчер будет немедленно закрыт.", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }

            string[] lines = File.ReadAllLines(filePath);

            if(lines.Length != 3)
            {
                MessageBox.Show(
                    "Ожидалось 3 строки данных в mt5useer.txt, "+"но найдено "+lines.Length.ToString() +", лаунчер будет немедленно закрыт",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }



        }

    }
}
