using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ArduinoDataReceiver
{
    public partial class MainWindow1 : Window
    {
        private SerialPort serialPort;
        private string receivedData = "";

        public MainWindow1()
        {
            InitializeComponent();
            Closing += Window_Closing;
            UpdateStatus("Готов к мониторингу");
        }

        private void InitializeSerialPort()
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort("COM3", 9600); // Укажите порт и скорость, на которых работает Arduino
                serialPort.DataReceived += SerialPort_DataReceived;
            }

            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    UpdateStatus("Мониторинг запущен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при открытии COM порта: " + ex.Message);
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string rawData = serialPort.ReadLine(); // Чтение строки данных

            // Выводим полученные данные в консоль для отладки
            Console.WriteLine("Полученные данные: " + rawData);

            // Обновляем интерфейс в основном потоке
            Dispatcher.Invoke(() =>
            {
                // Сконкатенируем новые данные с уже существующим текстом
                txtReceivedData.Text += rawData + Environment.NewLine;

                // Прокручиваем текстовое поле к новым данным
                txtReceivedData.ScrollToEnd();
            });
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            InitializeSerialPort();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                await CloseSerialPort();
            }
        }

        private Task CloseSerialPort()
        {
            return Task.Run(() =>
            {
                serialPort.Close();
                serialPort.Dispose(); // Освобождаем ресурсы
                UpdateStatus("Мониторинг остановлен");
            });
        }

        private void UpdateStatus(string status)
        {
            Dispatcher.Invoke(() =>
            {
                statusTextBlock.Text = status;
            });
        }

        private void UpdateReceivedData(string data)
        {
            receivedData += data + Environment.NewLine;
        }

        private void Calibrate_Click(object sender, RoutedEventArgs e)
        {
            SendCalibrationCommand();
        }

        private void SendCalibrationCommand()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    // Отправляем символ 'k' для запуска калибровки на Arduino
                    serialPort.Write("k");
                    UpdateStatus("Отправлена команда на калибровку");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отправке команды: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Сначала запустите мониторинг");
            }
        }
    }
}




















