using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Media;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        // Core classes from existing console app
        private UserMemory memory;
        private ResponseManager responseManager;
        private SentimentAnalyzer sentimentAnalyzer;
        private ConversationHandler conversationHandler;
        private ActivityLogger activityLogger;
        private TaskManager taskManager;
        private QuizManager quizManager;
        private VoiceGreetingPlayer voicePlayer;
        private AsciiArtDisplayer artDisplayer;

        // UI State
        private bool isPlaceholderText = true;
        private bool waitingForReminderResponse = false;
        private string pendingTaskForReminder = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();
        }

        private void InitializeChatbot()
        {
            // Initialize existing classes
            memory = new UserMemory();
            responseManager = new ResponseManager();
            sentimentAnalyzer = new SentimentAnalyzer();
            activityLogger = new ActivityLogger();
            taskManager = new TaskManager(activityLogger);
            quizManager = new QuizManager(activityLogger);
            conversationHandler = new ConversationHandler(memory, responseManager);
            voicePlayer = new VoiceGreetingPlayer();
            artDisplayer = new AsciiArtDisplayer();

            // Play greeting sound if file exists
            try
            {
                voicePlayer.Play("greeting.wav");
            }
            catch
            {
                // Ignore if file doesn't exist
            }
        }

        // Name Input and Initialization
        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartChat_Click(sender, e);
            }
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a valid name.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            memory.StoreName(name);
            UserNameDisplay.Text = name;
            WelcomeText.Text = $"Welcome back, {name}!";
            NameInputOverlay.Visibility = Visibility.Collapsed;

            // Add welcome message with ASCII art if available
            string asciiArt = "";
            try
            {
                asciiArt = artDisplayer.GetAsciiArt("ascii_art.txt");
            }
            catch
            {
                // Ignore if file doesn't exist
            }

            // Use ResponseManager's greeting response as base and personalize it
            string welcomeMessage = responseManager.GetResponse("greeting").Replace("Hello!", $"Hello {name}!").Replace("Hi there!", $"Hi there, {name}!").Replace("Welcome!", $"Welcome, {name}!");

            if (!string.IsNullOrEmpty(asciiArt))
            {
                welcomeMessage += "\n\n" + asciiArt;
            }

            welcomeMessage += "\n\nYou can ask me about:\n" +
                            "• Password safety\n" +
                            "• Phishing\n" +
                            "• Safe browsing\n" +
                            "• Privacy\n" +
                            "• Scams\n" +
                            "• Security\n\n" +
                            "Commands you can use:\n" +
                            "• 'add task [description]' - Add a cybersecurity task\n" +
                            "• 'show tasks' - View your tasks\n" +
                            "• 'complete task [number]' - Mark a task as complete\n" +
                            "• 'delete task [number]' - Delete a task\n" +
                            "• 'update task [number] [new description]' - Update a task\n" +
                            "• 'start quiz' - Take the cybersecurity quiz\n" +
                            "• 'show activity log' - View recent activities\n" +
                            "• 'help' - Get help with available topics\n" +
                            "• 'exit' or 'quit' - End the conversation\n\n" +
                            "How can I assist you today?";

            AddBotMessage(welcomeMessage);
            activityLogger.LogActivity("User Registration", $"New user \"{name}\" joined the chatbot");
        }
        //end of initialization

        // Chat Functionality
        private void ChatInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isPlaceholderText)
            {
                ChatInput.Text = "";
                ChatInput.Foreground = new SolidColorBrush(Colors.Black);
                isPlaceholderText = false;
            }
        }

        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage_Click(sender, e);
            }
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(message) || isPlaceholderText) return;

            // Add user message
            AddUserMessage(message);

            // Process with enhanced NLP and get response
            string response = ProcessUserInput(message);

            // Add bot response
            AddBotMessage(response);

            // Clear input
            ChatInput.Text = "";
            ResetChatInputPlaceholder();
        }

        private string ProcessUserInput(string input)
        {
            // Enhanced NLP processing
            string lowerInput = input.ToLower().Trim();
            string sentiment = sentimentAnalyzer.AnalyzeSentiment(input);

            // Check for exit commands first
            if (IsExitCommand(lowerInput))
            {
                activityLogger.LogActivity("Session End", "User ended the conversation");
                string userName = memory.GetName();
                string goodbyeMessage = $"Goodbye {userName}! Stay safe online and remember to practice good cybersecurity habits. Take care!";

                // Show goodbye message and close after a delay
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                    Application.Current.Shutdown();
                }));

                return goodbyeMessage;
            }

            // Handle waiting for reminder response
            if (waitingForReminderResponse)
            {
                return HandleReminderResponse(input);
            }

            // Handle enhanced sentiment-based responses for worried/frustrated users
            if (IsNegativeSentiment(sentiment) && ContainsWorryKeywords(lowerInput))
            {
                return HandleSentimentBasedResponse(input, sentiment);
            }

            // Task-related commands
            if (ContainsKeywords(lowerInput, new[] { "add task", "create task", "new task" }) && !lowerInput.Contains("remind me"))
            {
                return HandleAddTaskCommand(input);
            }

            if (ContainsKeywords(lowerInput, new[] { "remind me" }))
            {
                return HandleRemindMeCommand(input);
            }

            if (ContainsKeywords(lowerInput, new[] { "show tasks", "view tasks", "my tasks", "list tasks" }))
            {
                return HandleShowTasks();
            }

            if (ContainsKeywords(lowerInput, new[] { "complete task", "finish task", "done task", "mark complete" }))
            {
                return HandleCompleteTask(input);
            }

            if (ContainsKeywords(lowerInput, new[] { "delete task", "remove task" }))
            {
                return HandleDeleteTask(input);
            }

            if (ContainsKeywords(lowerInput, new[] { "update task", "edit task", "modify task", "change task" }))
            {
                return HandleUpdateTask(input);
            }

            // Quiz-related commands
            if (ContainsKeywords(lowerInput, new[] { "quiz", "test", "game", "start quiz" }))
            {
                ShowQuizOverlay();
                return "Opening the cybersecurity quiz for you!";
            }

            // Activity log commands
            if (ContainsKeywords(lowerInput, new[] { "activity log", "show activity", "what have you done", "recent actions" }))
            {
                return HandleShowActivityLog();
            }

            // Regular cybersecurity conversation - let ConversationHandler handle it
            activityLogger.LogActivity("Chat Interaction", $"User asked: \"{input}\"");
            return conversationHandler.HandleConversation(input, sentiment);
        }

        private bool IsExitCommand(string input)
        {
            return input == "exit" || input == "quit" || input == "bye" || input == "goodbye";
        }

        private bool ContainsKeywords(string input, string[] keywords)
        {
            return keywords.Any(keyword => input.Contains(keyword));
        }

        private bool IsNegativeSentiment(string sentiment)
        {
            return sentiment == "worried" || sentiment == "confused" || sentiment == "frustrated" || sentiment == "negative";
        }

        private bool ContainsWorryKeywords(string input)
        {
            return ContainsKeywords(input, new[] { "worried", "frustrated", "scared", "confused", "concerned", "anxious", "helpless", "overwhelmed" });
        }

        private string HandleSentimentBasedResponse(string input, string sentiment)
        {
            string lowerInput = input.ToLower();

            // Get sentiment-aware response prefix from ResponseManager
            string sentimentPrefix = responseManager.GetSentimentResponse(sentiment);

            // Check what they're worried about and provide targeted advice
            if (ContainsKeywords(lowerInput, new[] { "password", "passwords" }))
            {
                return sentimentPrefix + "Here are some tips to ease your password security worries:\n\n" +
                       "• Use a password manager to generate and store unique passwords\n" +
                       "• Enable two-factor authentication on important accounts\n" +
                       "• Use passphrases with 4-5 random words\n" +
                       "• Never reuse passwords across multiple sites\n\n" +
                       "Would you like me to help you create a task to improve your password security?";
            }
            else if (ContainsKeywords(lowerInput, new[] { "phishing", "email", "fake email" }))
            {
                return sentimentPrefix + "Here's how to stay safe from phishing and email scams:\n\n" +
                       "• Always verify sender identity before clicking links\n" +
                       "• Look for spelling errors and urgent language\n" +
                       "• Hover over links to see the real destination\n" +
                       "• When in doubt, contact the company directly\n\n" +
                       "Remember, being cautious is a good thing! What specific email concerns do you have?";
            }
            else if (ContainsKeywords(lowerInput, new[] { "safe browsing", "browsing", "web safety", "surfing" }))
            {
                return sentimentPrefix + "Browsing safely is important to protect your data and devices:\n\n" +
                       "• Avoid clicking on suspicious links or pop-ups\n" +
                       "• Always check for HTTPS in the website URL\n" +
                       "• Use an ad blocker and a reputable browser extension\n" +
                       "• Log out of accounts when using shared computers\n\n" +
                       "Need help with a specific browser or website?";
            }
            else if (ContainsKeywords(lowerInput, new[] { "scam", "fraud", "fake website", "online scam", "scams" }))
            {
                return sentimentPrefix + "Scammers are always evolving — here's how to stay safe:\n\n" +
                       "• Never share personal or banking details via email or text\n" +
                       "• Double-check URLs and sender addresses for authenticity\n" +
                       "• Don't trust urgent messages asking for immediate action\n" +
                       "• Report suspicious messages to relevant authorities\n\n" +
                       "Did you encounter a scam you'd like me to look into?";
            }
            else if (ContainsKeywords(lowerInput, new[] { "privacy", "personal data", "tracking", "data collection" }))
            {
                return sentimentPrefix + "Protecting your privacy helps keep your information safe:\n\n" +
                       "• Review and limit app permissions regularly\n" +
                       "• Use privacy-focused browsers and search engines\n" +
                       "• Avoid oversharing on social media\n" +
                       "• Enable two-factor authentication where possible\n\n" +
                       "Is there a specific privacy concern you want to discuss?";
            }
            else if (ContainsKeywords(lowerInput, new[] { "hacked", "hack" }))
            {
                return sentimentPrefix + "Let me help you feel more secure:\n\n" +
                       "• Keep your software updated with latest patches\n" +
                       "• Use antivirus software and keep it current\n" +
                       "• Be cautious on public Wi-Fi networks\n" +
                       "• Regularly backup your important data\n\n" +
                       "What specific security issue is worrying you most?";
            }
            else if (ContainsKeywords(lowerInput, new[] { "security", "online security", "cybersecurity", "internet security" }))
            {
                return sentimentPrefix + "Staying secure online is all about being proactive:\n\n" +
                       "• Use strong, unique passwords for each account\n" +
                       "• Enable two-factor authentication (2FA) wherever possible\n" +
                       "• Be careful with email attachments and unknown links\n" +
                       "• Keep your operating system and apps up to date\n" +
                       "• Regularly monitor your accounts for suspicious activity\n\n" +
                       "Is there a specific security topic or threat you'd like to explore further?";
            }

            else
            {
                return sentimentPrefix + "Tell me what specific aspect is concerning you, and I'll provide targeted advice to help you feel more confident and secure online.";
            }
        }

        private string HandleAddTaskCommand(string input)
        {
            string lowerInput = input.ToLower();

            // Extract task description
            string taskDescription = "";
            if (lowerInput.Contains("add task"))
            {
                int index = lowerInput.IndexOf("add task") + "add task".Length;
                taskDescription = input.Substring(Math.Min(index, input.Length)).Trim();
                if (taskDescription.StartsWith("-") || taskDescription.StartsWith(":"))
                    taskDescription = taskDescription.Substring(1).Trim();
            }

            if (string.IsNullOrEmpty(taskDescription))
            {
                return "Please specify what task you'd like to add. For example: 'add task - Enable two-factor authentication'";
            }

            // Add task without reminder first
            taskManager.AddTask(taskDescription, $"Cybersecurity task: {taskDescription}", null);

            // Set up for reminder question
            waitingForReminderResponse = true;
            pendingTaskForReminder = taskDescription;

            return $"Task added: '{taskDescription}'. Would you like to set a reminder for this task?";
        }

        private string HandleRemindMeCommand(string input)
        {
            string lowerInput = input.ToLower();

            // Extract task description
            string taskDescription = "";
            if (lowerInput.Contains("remind me"))
            {
                int index = lowerInput.IndexOf("remind me") + "remind me".Length;
                taskDescription = input.Substring(Math.Min(index, input.Length)).Trim();
                if (taskDescription.StartsWith("to"))
                    taskDescription = taskDescription.Substring(2).Trim();
            }

            if (string.IsNullOrEmpty(taskDescription))
            {
                return "Please specify what you'd like to be reminded about. For example: 'remind me to update my password tomorrow'";
            }

            // Check for time keywords
            int? reminderDays = ExtractReminderDays(lowerInput);

            if (reminderDays.HasValue)
            {
                taskManager.AddTask(taskDescription, $"Reminder: {taskDescription}", reminderDays);
                string timeText = reminderDays.Value == 1 ? "tomorrow" : $"in {reminderDays.Value} days";
                return $"Reminder set for '{taskDescription}' {timeText}.";
            }
            else
            {
                // Add task and ask for reminder timing
                taskManager.AddTask(taskDescription, $"Reminder: {taskDescription}", null);
                waitingForReminderResponse = true;
                pendingTaskForReminder = taskDescription;
                return $"Task added: '{taskDescription}'. When would you like to be reminded? (e.g., 'in 3 days', 'tomorrow', 'in 1 week')";
            }
        }

        private string HandleReminderResponse(string input)
        {
            waitingForReminderResponse = false;
            string lowerInput = input.ToLower().Trim();

            if (lowerInput == "yes" || lowerInput == "y")
            {
                return "When would you like to be reminded? You can say things like 'tomorrow', 'in 3 days', 'in 1 week', etc.";
            }
            else if (lowerInput == "no" || lowerInput == "n")
            {
                pendingTaskForReminder = "";
                return "No problem! The task has been added without a reminder.";
            }
            else
            {
                // Try to extract reminder timing
                int? reminderDays = ExtractReminderDays(lowerInput);

                if (reminderDays.HasValue && !string.IsNullOrEmpty(pendingTaskForReminder))
                {
                    // Update the existing task with reminder
                    var tasks = taskManager.GetAllTasks();
                    var taskToUpdate = tasks.FirstOrDefault(t => t.Title == pendingTaskForReminder);
                    if (taskToUpdate != null)
                    {
                        taskManager.UpdateTask(taskToUpdate.Id, taskToUpdate.Title, taskToUpdate.Description, reminderDays);
                    }

                    string timeText = reminderDays.Value == 1 ? "tomorrow" : $"in {reminderDays.Value} days";
                    string taskName = pendingTaskForReminder;
                    pendingTaskForReminder = "";
                    return $"Perfect! I'll remind you about '{taskName}' {timeText}.";
                }
                else
                {
                    return "I didn't understand the timing. Please try again with something like 'tomorrow', 'in 3 days', or 'in 1 week'.";
                }
            }
        }

        private int? ExtractReminderDays(string input)
        {
            if (ContainsKeywords(input, new[] { "tomorrow", "1 day", "in a day" }))
                return 1;
            else if (ContainsKeywords(input, new[] { "2 days", "two days" }))
                return 2;
            else if (ContainsKeywords(input, new[] { "3 days", "three days" }))
                return 3;
            else if (ContainsKeywords(input, new[] { "4 days", "four days" }))
                return 4;
            else if (ContainsKeywords(input, new[] { "5 days", "five days" }))
                return 5;
            else if (ContainsKeywords(input, new[] { "6 days", "six days" }))
                return 6;
            else if (ContainsKeywords(input, new[] { "week", "7 days", "seven days", "1 week" }))
                return 7;
            else if (ContainsKeywords(input, new[] { "10 days", "ten days" }))
                return 10;
            else if (ContainsKeywords(input, new[] { "2 weeks", "14 days", "two weeks" }))
                return 14;
            else if (ContainsKeywords(input, new[] { "3 weeks", "21 days", "three weeks" }))
                return 21;
            else if (ContainsKeywords(input, new[] { "4 weeks", "28 days", "four weeks" }))
                return 28;
            else if (ContainsKeywords(input, new[] { "month", "30 days", "1 month" }))
                return 30;
            else if (ContainsKeywords(input, new[] { "45 days", "1.5 months", "six weeks", "one and a half months" }))
                return 45;
            else if (ContainsKeywords(input, new[] { "2 months", "60 days", "two months" }))
                return 60;

            return null;
        }

        private string HandleShowTasks()
        {
            var tasks = taskManager.GetAllTasks();
            if (!tasks.Any())
            {
                return "You don't have any tasks yet. You can add one by saying 'add task - [description]'.";
            }

            string response = "Here are your current tasks:\n\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                string status = task.IsCompleted ? "✅ Completed" : "⏳ Pending";
                response += $"{i + 1}. {task.Title} - {status}\n";
                if (task.ReminderDate.HasValue)
                {
                    response += $"   Reminder: {task.ReminderDate.Value:MM/dd/yyyy}\n";
                }
                response += "\n";
            }

            response += "You can:\n";
            response += "• Complete a task: 'complete task [number]'\n";
            response += "• Delete a task: 'delete task [number]'\n";
            response += "• Update a task: 'update task [number] [new description]'\n";

            return response;
        }

        private string HandleCompleteTask(string input)
        {
            var tasks = taskManager.GetAllTasks();
            if (!tasks.Any())
            {
                return "You don't have any tasks to complete.";
            }

            // Try to extract task number
            int taskNumber = ExtractTaskNumber(input);

            if (taskNumber > 0 && taskNumber <= tasks.Count)
            {
                var task = tasks[taskNumber - 1];
                if (task.IsCompleted)
                {
                    return $"Task '{task.Title}' is already completed!";
                }

                taskManager.ToggleTaskCompletion(task.Id);
                return $"Great! I've marked '{task.Title}' as completed. Keep up the good cybersecurity practices!";
            }
            else if (taskNumber == 0)
            {
                // No number specified, complete first pending task
                var pendingTasks = tasks.Where(t => !t.IsCompleted).ToList();
                if (!pendingTasks.Any())
                {
                    return "You don't have any pending tasks to complete.";
                }

                var firstTask = pendingTasks.First();
                taskManager.ToggleTaskCompletion(firstTask.Id);
                return $"Great! I've marked '{firstTask.Title}' as completed. Keep up the good cybersecurity practices!";
            }
            else
            {
                return $"Invalid task number. Please use a number between 1 and {tasks.Count}. Use 'show tasks' to see your task list.";
            }
        }

        private string HandleDeleteTask(string input)
        {
            var tasks = taskManager.GetAllTasks();
            if (!tasks.Any())
            {
                return "You don't have any tasks to delete.";
            }

            // Try to extract task number
            int taskNumber = ExtractTaskNumber(input);

            if (taskNumber > 0 && taskNumber <= tasks.Count)
            {
                var task = tasks[taskNumber - 1];
                string taskTitle = task.Title;
                taskManager.DeleteTask(task.Id);
                return $"Task '{taskTitle}' has been deleted successfully.";
            }
            else
            {
                return $"Invalid task number. Please use a number between 1 and {tasks.Count}. Use 'show tasks' to see your task list.";
            }
        }

        private string HandleUpdateTask(string input)
        {
            var tasks = taskManager.GetAllTasks();
            if (!tasks.Any())
            {
                return "You don't have any tasks to update.";
            }

            // Try to extract task number and new description
            var updateInfo = ExtractTaskUpdateInfo(input);
            int taskNumber = updateInfo.Item1;
            string newDescription = updateInfo.Item2;

            if (taskNumber > 0 && taskNumber <= tasks.Count)
            {
                if (string.IsNullOrEmpty(newDescription))
                {
                    return $"Please provide a new description. Example: 'update task {taskNumber} Enable 2FA on all accounts'";
                }

                var task = tasks[taskNumber - 1];
                string oldTitle = task.Title;
                taskManager.UpdateTask(task.Id, newDescription, task.Description, task.ReminderDate.HasValue ? (int?)(task.ReminderDate.Value - DateTime.Now).Days : null);
                return $"Task updated successfully!\nOld: '{oldTitle}'\nNew: '{newDescription}'";
            }
            else
            {
                return $"Invalid task number. Please use a number between 1 and {tasks.Count}. Use 'show tasks' to see your task list.";
            }
        }

        private int ExtractTaskNumber(string input)
        {
            // Look for numbers in the input
            var words = input.Split(' ');
            foreach (var word in words)
            {
                if (int.TryParse(word, out int number))
                {
                    return number;
                }
            }
            return 0; // No number found
        }

        private Tuple<int, string> ExtractTaskUpdateInfo(string input)
        {
            string lowerInput = input.ToLower();

            // Find the position after "update task"
            int startIndex = lowerInput.IndexOf("update task") + "update task".Length;
            if (startIndex >= input.Length) return new Tuple<int, string>(0, "");

            string remainder = input.Substring(startIndex).Trim();
            var parts = remainder.Split(' ');

            if (parts.Length < 2) return new Tuple<int, string>(0, "");

            // First part should be the task number
            if (int.TryParse(parts[0], out int taskNumber))
            {
                // Rest is the new description
                string newDescription = string.Join(" ", parts.Skip(1)).Trim();
                return new Tuple<int, string>(taskNumber, newDescription);
            }

            return new Tuple<int, string>(0, "");
        }

        private string HandleShowActivityLog()
        {
            var activities = activityLogger.GetRecentActivities(10);
            if (!activities.Any())
            {
                return "No recent activity to show.";
            }

            string response = "Here's a summary of your recent activities:\n\n";
            foreach (var activity in activities)
            {
                response += $"• {activity.Action}: {activity.Details}\n";
                response += $"  Time: {activity.Timestamp:MM/dd/yyyy HH:mm}\n\n";
            }

            return response;
        }

        private void ResetChatInputPlaceholder()
        {
            ChatInput.Text = "Type your message here...";
            ChatInput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9CA3AF"));
            isPlaceholderText = true;
        }

        private void AddUserMessage(string message)
        {
            var messagePanel = CreateMessagePanel(message, true);
            ChatMessagesPanel.Children.Add(messagePanel);
            ScrollToBottom();
        }

        private void AddBotMessage(string message)
        {
            var messagePanel = CreateMessagePanel(message, false);
            ChatMessagesPanel.Children.Add(messagePanel);
            ScrollToBottom();
        }

        private Border CreateMessagePanel(string message, bool isUser)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(isUser ? (Color)ColorConverter.ConvertFromString("#FF3B82F6") : (Color)ColorConverter.ConvertFromString("#FFF3F4F6")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(15, 10, 15, 10),
                Margin = new Thickness(isUser ? 50 : 0, 5, isUser ? 0 : 50, 5),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(isUser ? Colors.White : Colors.Black),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                FontFamily = new FontFamily("Consolas, Courier New") // Monospace for ASCII art
            };

            border.Child = textBlock;
            return border;
        }

        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ChatScrollViewer.ScrollToBottom();
            }));
        }
        //end of Chat functionality

        // Quiz Management section
        private void ShowQuizOverlay()
        {
            QuizOverlay.Visibility = Visibility.Visible;
            QuizStartPanel.Visibility = Visibility.Visible;
            QuizQuestionPanel.Visibility = Visibility.Collapsed;
            QuizResultsPanel.Visibility = Visibility.Collapsed;
        }

        private void CloseQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizOverlay.Visibility = Visibility.Collapsed;
            quizManager.ResetQuiz();
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            quizManager.StartQuiz();
            ShowCurrentQuestion();
            QuizStartPanel.Visibility = Visibility.Collapsed;
            QuizQuestionPanel.Visibility = Visibility.Visible;
            activityLogger.LogActivity("Quiz Started", "User started the cybersecurity quiz");
        }

        private void ShowCurrentQuestion()
        {
            var currentQuestion = quizManager.GetCurrentQuestion();
            if (currentQuestion == null) return;

            QuestionNumberText.Text = $"Question {quizManager.CurrentQuestionIndex + 1} of {quizManager.TotalQuestions}";
            QuizProgressBar.Value = ((double)(quizManager.CurrentQuestionIndex + 1) / quizManager.TotalQuestions) * 100;
            QuestionText.Text = currentQuestion.Question;

            AnswerOptionsPanel.Children.Clear();
            ExplanationPanel.Visibility = Visibility.Collapsed;
            NextQuestionButton.Visibility = Visibility.Collapsed;

            for (int i = 0; i < currentQuestion.Options.Length; i++)
            {
                var button = new Button
                {
                    Content = $"{(char)('A' + i)}. {currentQuestion.Options[i]}",
                    Style = (Style)FindResource("PrimaryButtonStyle"),
                    Margin = new Thickness(0, 0, 0, 8),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Padding = new Thickness(16, 12, 16, 12),
                    Tag = i
                };
                button.Click += AnswerButton_Click;
                AnswerOptionsPanel.Children.Add(button);
            }
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int selectedAnswer = (int)button.Tag;

            bool isCorrect = quizManager.SubmitAnswer(selectedAnswer);
            var currentQuestion = quizManager.GetCurrentQuestion();

            // Update button styles to show correct/incorrect
            foreach (Button btn in AnswerOptionsPanel.Children)
            {
                btn.IsEnabled = false;
                int btnIndex = (int)btn.Tag;

                if (btnIndex == currentQuestion.CorrectAnswer)
                {
                    btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF16A34A"));
                }
                else if (btnIndex == selectedAnswer && !isCorrect)
                {
                    btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF4444"));
                }
                else
                {
                    btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE5E7EB"));
                    btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B7280"));
                }
            }

            // Show explanation
            ExplanationText.Text = currentQuestion.Explanation;
            ExplanationPanel.Visibility = Visibility.Visible;
            NextQuestionButton.Visibility = Visibility.Visible;

            activityLogger.LogActivity("Quiz Answer", $"Question {quizManager.CurrentQuestionIndex + 1}: {(isCorrect ? "Correct" : "Incorrect")}");
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (quizManager.HasNextQuestion())
            {
                quizManager.NextQuestion();
                ShowCurrentQuestion();
            }
            else
            {
                ShowQuizResults();
            }
        }

        private void ShowQuizResults()
        {
            QuizQuestionPanel.Visibility = Visibility.Collapsed;
            QuizResultsPanel.Visibility = Visibility.Visible;

            int score = quizManager.GetScore();
            int total = quizManager.TotalQuestions;
            double percentage = (double)score / total * 100;

            ScoreText.Text = $"{score}/{total}";

            if (percentage >= 90)
                FeedbackText.Text = "Excellent! You're a cybersecurity pro!";
            else if (percentage >= 70)
                FeedbackText.Text = "Great job! You have solid security knowledge.";
            else if (percentage >= 50)
                FeedbackText.Text = "Good effort! Keep learning to stay secure.";
            else
                FeedbackText.Text = "Keep studying! Cybersecurity knowledge is crucial.";

            activityLogger.LogActivity("Quiz Completed", $"Final score: {score}/{total} ({percentage:F0}%)");
        }

        private void RestartQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizResultsPanel.Visibility = Visibility.Collapsed;
            QuizStartPanel.Visibility = Visibility.Visible;
            quizManager.ResetQuiz();
        }
        //end of Quiz Management section
    }
}