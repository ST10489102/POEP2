using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PART2
{
    // =========================================================
    // MAIN WINDOW
    // =========================================================
    public partial class MainWindow : Window
    {
        private ChatEngine chatbot;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "welcome.wav");

                if (File.Exists(path))
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Load();
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio failed.\n\n" + ex.Message);
            }

            string userName = PromptForName();

            chatbot = new ChatEngine(userName);
            chatbot.OnResponseGenerated += AddBotMessage;

            AddBotMessage("Hello " + userName + "! Welcome to Violet Vault 🔐");
            AddBotMessage("Type 'menu' to start or 'exit' to quit.");
        }

        private string PromptForName()
        {
            string name = "User";

            Window prompt = new Window
            {
                Width = 300,
                Height = 160,
                Title = "Enter Name",
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            StackPanel panel = new StackPanel();

            TextBox input = new TextBox { Margin = new Thickness(10) };

            Button ok = new Button
            {
                Content = "Start",
                Margin = new Thickness(10)
            };

            ok.Click += (s, e) =>
            {
                if (input.Text != "")
                    name = input.Text;

                prompt.Close();
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Please enter your name:",
                Margin = new Thickness(10)
            });

            panel.Children.Add(input);
            panel.Children.Add(ok);

            prompt.Content = panel;
            prompt.ShowDialog();

            return name;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = InputTextBox.Text.Trim();

            if (userInput == "")
            {
                MessageBox.Show("Please enter a message.");
                return;
            }

            AddUserMessage(userInput);
            chatbot.ProcessInput(userInput);

            InputTextBox.Text = "";
        }

        private void AddUserMessage(string message)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = "You: " + message,
                Margin = new Thickness(5)
            });

            ScrollViewer.ScrollToEnd();
        }

        private void AddBotMessage(string message)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = "Bot: " + message,
                Margin = new Thickness(5)
            });

            ScrollViewer.ScrollToEnd();
        }
    }

    // =========================================================
    // CHAT ENGINE (CS8618 FIXED + MENU SYSTEM)
    // =========================================================
    public class ChatEngine
    {
        private string _userName;
        private bool inMenu = true;

        // ✅ FIX FOR CS8618 (IMPORTANT)
        public delegate void BotResponseHandler(string message);
        public event BotResponseHandler OnResponseGenerated = delegate { };

        public ChatEngine(string userName)
        {
            _userName = userName;
        }

        // =========================
        // INPUT HANDLER
        // =========================
        public void ProcessInput(string input)
        {
            string lower = input.ToLower().Trim();

            if (lower == "exit")
            {
                OnResponseGenerated("Goodbye " + _userName + "! 🔐 Stay safe.");
                Environment.Exit(0);
                return;
            }

            if (lower == "menu")
            {
                inMenu = true;
                OnResponseGenerated(GetMenu());
                return;
            }

            if (lower == "back")
            {
                inMenu = true;
                OnResponseGenerated(GetMenu());
                return;
            }

            if (inMenu)
            {
                OnResponseGenerated(HandleMenu(lower));
                return;
            }

            OnResponseGenerated("Type 'menu' to start.");
        }

        // =========================
        // MENU
        // =========================
        private string GetMenu()
        {
            return
                "CYBERSECURITY MENU 🔐\n\n" +
                "1. Password Safety\n" +
                "2. Phishing\n" +
                "3. Malware\n" +
                "4. Privacy\n" +
                "5. Wi-Fi Security\n" +
                "6. 2FA\n\n" +
                "Type 1–6, 'menu', 'back', or 'exit'.";
        }

        // =========================
        // MENU OPTIONS
        // =========================
        private string HandleMenu(string input)
        {
            if (input == "1")
                return "Password Safety: Use strong passwords with letters, numbers, symbols.";

            if (input == "2")
                return "Phishing: Never click suspicious links or emails.";

            if (input == "3")
                return "Malware: Keep antivirus updated and avoid unsafe downloads.";

            if (input == "4")
                return "Privacy: Limit what you share online.";

            if (input == "5")
                return "Wi-Fi: Avoid public Wi-Fi for sensitive tasks.";

            if (input == "6")
                return "2FA: Enable two-factor authentication for security.";

            return "Invalid option. Type 1–6 or 'menu'.";
        }
    }
}