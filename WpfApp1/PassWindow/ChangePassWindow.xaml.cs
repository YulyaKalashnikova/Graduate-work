﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.PassWindow
{
    /// <summary>
    /// Interaction logic for ChangePassWindow.xaml
    /// </summary>
    public partial class ChangePassWindow : Window
    {
        public ChangePassWindow()
        {
            InitializeComponent();
        }

        int confirmationCode = 0;
        private void SendConfirmationCode()
        {
            if (txtLogin.Text == string.Empty
                || txtOldPassword.Password == string.Empty)
            {
                MessageBox.Show("Заполните логин / пароль для отправки кода!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = Helper.context.Users
               .FirstOrDefault(x => x.Login == txtLogin.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Извлекаем соль из БД
            string oldSalt = user.Salt;
            // Смотрим, какой пароль ввёл пользователь
            string enteredOldPassword = txtOldPassword.Password;
            // Соединяем введённый пароль и соль 
            string enteredOldPasswordPlusOldSalt = enteredOldPassword + oldSalt;

            // если хеш-код из БД равен введённому хеш-коду
            if (user.HashPass == HashPass.hashPassword(enteredOldPasswordPlusOldSalt))
            {
                // Генерируем код подтверждения
                Random random = new Random();
                confirmationCode = random.Next(1000, 10000);

                // Отправка сообщения на почту
                var to = user.Email;
                var body = "Код подтверждения для смены пароля: " + confirmationCode;
                var header = "Код подтверждения от ФГБОУ ВО СибГМУ Минздрава России";
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("uka2828@yandex.ru");
                mailMessage.To.Add(to);
                mailMessage.Subject = header;
                mailMessage.Body = body;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                using (var client = new System.Net.Mail.SmtpClient())
                {
                    client.Timeout = 5000;
                    client.Host = "smtp.yandex.ru";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("uka2828", "qwsdthdfgrhd");
                    client.Send(mailMessage);
                    MessageBox.Show($"Код подтверждения отправлен на вашу почту: {user.Email}", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Неверный пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void SendConfirmationCode_Click(object sender, RoutedEventArgs e)
        {
            SendConfirmationCode();
        }
        
        private void GeneratePass()
        {
            string chars = "!@#$%^&?()*_0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int defaultLengthPass = 12;
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < defaultLengthPass; i++)
            {
                int index = random.Next(chars.Length); // записываем в index псевдорандомный символ
                sb.Append(chars[index]); // при помощи Append добавляем строке подстроку в виде одного символа
            }
            string result = sb.ToString(); // записываем в переменную result всё, что у нас нагенерировалось
            // записываем в поля рандомный пароль
            txtNewPassword.Password = result;
            txtCheckNewPass.Text = result;
            MessageBox.Show("Пароль сгенерирован!");
        }

        private void GeneratePass_Click(object sender, RoutedEventArgs e)
        {
            GeneratePass();
        }

        private void CheckPassCB_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked.Value)
            {
                txtCheckNewPass.Text = txtNewPassword.Password; // копируем в TextBox из PasswordBox
                txtCheckNewPass.Visibility = Visibility.Visible;// TextBox - отобразить
                txtNewPassword.Visibility = Visibility.Hidden; // PasswordBox - скрыть
            }
            else
            {
                txtNewPassword.Password = txtCheckNewPass.Text; // копируем в PasswordBox из TextBox
                txtCheckNewPass.Visibility = Visibility.Hidden; // TextBox - скрыть
                txtNewPassword.Visibility = Visibility.Visible; // PasswordBox - отобразить
            }
        }

        private void SaveChangePass()
        {
            if (txtLogin.Text == string.Empty
                || txtOldPassword.Password == string.Empty
                || txtConfirmationCode.Text == string.Empty
                || txtNewPassword.Password == string.Empty
                || txtRepeatNewPassword.Password == string.Empty)
            {
                MessageBox.Show("Заполните пустые поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = Helper.context.Users
               .FirstOrDefault(x => x.Login == txtLogin.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Извлекаем соль из БД
            string oldSalt = user.Salt;
            // Смотрим, какой пароль ввёл пользователь
            string enteredOldPassword = txtOldPassword.Password;
            // Соединяем введённый пароль и соль 
            string enteredOldPasswordPlusOldSalt = enteredOldPassword + oldSalt;
            
            // если хеш-код из БД равен введённому хеш-коду
            if (user.HashPass == HashPass.hashPassword(enteredOldPasswordPlusOldSalt))
            {
                if (Convert.ToInt32(txtConfirmationCode.Text) != confirmationCode)
                {
                    MessageBox.Show("Неверный код подтверждения!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (txtOldPassword.Password == txtNewPassword.Password)
                {
                    MessageBox.Show("Введите новый пароль!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (txtNewPassword.Password.Length < 6)
                {
                    MessageBox.Show("Пароль слишком короткий!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (txtNewPassword.Password != txtRepeatNewPassword.Password)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    /*Генерируем соль*/
                    //Размер соли в байтах
                    int saltLength = 32;
                    // Генерируем случайную соль
                    byte[] salt = SaltGenerator.GenerateSalt(saltLength);
                    //Конвентируем соль в строку
                    string saltString = Convert.ToBase64String(salt);
                    // Записываем соль в БД
                    user.Salt = saltString;
                    // Присоединяем соль к паролю
                    string saltyPassword = txtNewPassword.Password + saltString;
                    // Записываем пароль в БД
                    user.HashPass = saltyPassword;
                    user.PasswordCountAttempt = 5;
                    user.dateCreatedPass = DateTime.Today;
                    try
                    {
                        Helper.context.SaveChanges();
                        MessageBox.Show("Данные сохранены!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            new AuthWindow().Show();
            Close();
        }

        private void SaveChangePass_Click(object sender, RoutedEventArgs e)
        {
            SaveChangePass();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().Show();
            Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState
                = WindowState.Minimized;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Полностью завершить работу программы?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
