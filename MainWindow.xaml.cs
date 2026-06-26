using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace CYBER_WATCH_AI_POE_PART_2
{

    public partial class MainWindow : Window
    {//start of class



        //creating an instance for the class Array
        public ArrayList reply = new ArrayList();
        public ArrayList ignore = new ArrayList();
        user_name check_name = new user_name();

        // variables
        string username = string.Empty;
        string pre_question = string.Empty;
        int counting = 0;


        //creating a class training with an object name train_ai
        nlp_processor train_ai = new nlp_processor();


        //for the tasks class with database to add task for reminder
        //create an instance for the class tasks
        //with an object name manage_tasks
        tasks manage_tasks = new tasks();
        activity_log my_log = new activity_log();
       
        // Call this method whenever you navigate to the Log Page or want to refresh it
        //global variable to hold the task details
        string task_name, task_description, task_dueDate, task_status = string.Empty;

    
            public void RefreshLogUI()
        {
            // Clear the old binding and load the fresh list of entries
            log_append.ItemsSource = null;
            log_append.ItemsSource = my_log.get_all_entries();
        }// Refresh the list view so the user sees the change instantly
        





        // Counter for tracking how many times the user has interacted (used for reminders)
        int count_interest = 0;

        // Tracks the current quiz question index
        int currentQuestionIndex = 0;

        // Holds the user’s current quiz score
        int score = 0;

        // Stores the user’s currently selected answer
        string selectedAnswer = null;

        // Stores the list of answer buttons for the current quiz question
        List<Button> answerButtons;

        // Holds the entire list of quiz questions
        List<quiz_question> questions;

        // Handles loading quiz questions from storage or file
        quiz_game gameLoad = new quiz_game();



        public MainWindow()
        {
            InitializeComponent();

            // Add an initial log entry when the app opens
            my_log.add_entry("System", "Cyber Watch AI initialized successfully.");

            // Show it on the UI
            RefreshLogUI();

            my_log.add_entry(username, "User asked a question.");
            RefreshLogUI();


            //store answers 
            new respond(reply, ignore) { };

            //train NLP on the run
            train_ai.Train();
            manage_tasks.CreateTableIfNotExists();
            //creating an instance for the class voice_greeting 
            //with an object name greet
            voice_greeting greet = new voice_greeting();

            //call the voice method
            greet.greet();

            // Dynamically add all buttons to the UI (custom logic)
            addAllButtons();

            // Load quiz questions automatically into the 'questions' list
            gameLoad.autoLoadQuiz(ref questions);

            // Load questions into the UI for display or interaction
            loadQuestions();

        }







        // Switch to the Chats page
        private void chatting(object sender, RoutedEventArgs e)
        {
            // Show chats page and hide all others
            chat_page.Visibility = Visibility.Visible;
            HistoryPage.Visibility = Visibility.Hidden;
            reminderPage.Visibility = Visibility.Hidden;
            LogPage.Visibility = Visibility.Hidden;
            gamePage.Visibility = Visibility.Hidden;
        }

        // Switch to the Reminders page
        private void reminders(object sender, RoutedEventArgs e)
        {
            // Show reminders page and hide all others
            chat_page.Visibility = Visibility.Hidden;
            HistoryPage.Visibility = Visibility.Collapsed;
            reminderPage.Visibility = Visibility.Visible;
            LogPage.Visibility = Visibility.Hidden;
            gamePage.Visibility = Visibility.Hidden;



            //call the auto load task method, to show all the tasks
            autoLoad_task();
        }

        // Switch to the History page
        private void history(object sender, RoutedEventArgs e)
        {
            // Show history page and hide all others
            chat_page.Visibility = Visibility.Hidden;
            HistoryPage.Visibility = Visibility.Visible;
            reminderPage.Visibility = Visibility.Hidden;
            LogPage.Visibility = Visibility.Hidden;
            gamePage.Visibility = Visibility.Hidden;


        }

        // Switch to the Activity Log page
        // Switch to the Activity Log page
        private void activity(object sender, RoutedEventArgs e)
        {
            // Show log page and hide all others
            chat_page.Visibility = Visibility.Hidden;
            HistoryPage.Visibility = Visibility.Hidden;
            reminderPage.Visibility = Visibility.Hidden;
            LogPage.Visibility = Visibility.Visible;
            gamePage.Visibility = Visibility.Hidden;

            // ─── AUTOMATION ADDITION ───────────────────────────────────
            // Refresh the UI here so the latest items show up immediately
            RefreshLogUI();

            // Optional: Auto-scroll to the last element in the log window if log_append has elements
            if (log_append.Items.Count > 0)
            {
                log_append.ScrollIntoView(log_append.Items[log_append.Items.Count - 1]);
            }
            // ──────────────────────────────────────────────────────────
        }

        // Switch to the Game page
        private void game(object sender, RoutedEventArgs e)
        {
            // Show game page and hide all others
            chat_page.Visibility = Visibility.Hidden;
            HistoryPage.Visibility = Visibility.Hidden;
            reminderPage.Visibility = Visibility.Hidden;
            LogPage.Visibility = Visibility.Hidden;
            gamePage.Visibility = Visibility.Visible;

            string currentUsr = string.IsNullOrWhiteSpace(username) ? "Guest" : username;
            my_log.add_entry(currentUsr, "Navigated to the Game page.");
            RefreshLogUI();
        }

        // Exit the application
        private void exit(object sender, RoutedEventArgs e)
        {
            // Terminate the application immediately
            System.Environment.Exit(0);
        }






        //method to add all the buttons
        private void addAllButtons()
        {

            answerButtons = new List<Button> {
                         optionButtonOne,
                         optionButtonTwo,
                        optionButtonThree,
                        optionButtonFour

        };

        }//end of code

        //reset buttons background 
        private void buttonReset()
        {
            //reseting the background color of the button when clicked
            foreach (Button clickButton in answerButtons)
            {
                //resetting the background color of the button
                clickButton.ClearValue(Button.BackgroundProperty);
                clickButton.Background = System.Windows.Media.Brushes.Gray;


            }


        }

        //creating a method to load the questions
        private void loadQuestions()
        {
            //checking if the user didnt complete the quiz
            if (currentQuestionIndex >= questions.Count)
            {
                //display message for when the quiz is not completed
                MessageBox.Show("Great job! You’ve completed the quiz with a score of " + score + ".\nThe game will now reset. Press OK to continue.");
                currentQuestionIndex = 0;
                score = 0;
                selectedAnswer = null;
                loadQuestions();
                return;
            }

            //get the question
            var foundQuestion = questions[currentQuestionIndex];
            question_asked.Text = foundQuestion.Text;

            //setting  up the score
            score_count.Text = "score.." + "\n" + score;

            //resetting the selected answer
            selectedAnswer = null;

            //resetting the buttons
            buttonReset();

            //randomizing answers
            List<string> allAnswers = new List<string>(foundQuestion.wrongAnswer);
            allAnswers.Add(foundQuestion.correctAnswer);

            //creating an instance for the randomize class
            Random getIndex = new Random();
            for (int count = 0; count < allAnswers.Count; count++)
            {
                int found_index = getIndex.Next(count, allAnswers.Count);

                (allAnswers[count], allAnswers[found_index]) = (allAnswers[found_index], allAnswers[count]);


            }

            //assigning answers to the buttons
            for (int count = 0; count < answerButtons.Count; count++)
            {
                answerButtons[count].Content = allAnswers[count];

            }



        }//end of code



        //quiz game code

        private void optionSelected(object sender, RoutedEventArgs e)
        {
            //calling the button reset method
            buttonReset();
            Button clickButton = sender as Button;
            clickButton.Background = System.Windows.Media.Brushes.Green;
            selectedAnswer = clickButton.Content.ToString();

            string currentUsr = string.IsNullOrWhiteSpace(username) ? "Guest" : username;
            my_log.add_entry(currentUsr, $"Selected quiz option: \"{selectedAnswer}\"");
            RefreshLogUI();

        }

        private void answerButton(object sender, RoutedEventArgs e)
        {
            //checking if the user selected an option first
            if (selectedAnswer == string.Empty)
            {
                //display message
                MessageBox.Show("please select an answer!");

                return;
            }
            if (selectedAnswer == questions[currentQuestionIndex].correctAnswer)
            {
                score += 5;

            }
            currentQuestionIndex++;
            loadQuestions();





        }//end of quiz














        //proceed  event handler
        private void proceed(object sender, RoutedEventArgs e)
        {
            //Hide home page grid and set Username grid visible
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }










        //submit name  event handler
        private void submit_name(object sender, RoutedEventArgs e)
        {



            //check the user name from memory recall
            username = check_name.submit_name(usernames_input, chats);





            //Hide username page grid and set chats grid visible
            username_grid.Visibility = Visibility.Hidden;
            MainPage.Visibility = Visibility.Visible;

            string finalizedName = string.IsNullOrWhiteSpace(username) ? "Guest" : username;
            my_log.add_entry("System", $"User signed in as: {finalizedName}");
            RefreshLogUI();
        }





        // Handles double-clicking on an item in the reminder list
        private void remind_append_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string get_selected_value = view_tasks.SelectedValue.ToString();
            string get_id = get_selected_value.Substring(0, 1);
            int id = int.Parse(get_id);

            string currentUsr = string.IsNullOrWhiteSpace(username) ? "Guest" : username;

            if (get_selected_value.ToLower().EndsWith("done"))
            {
                manage_tasks.delete_task(id);

                // ─── AUTOMATION ADDITION ───────────────────────────────────
                my_log.add_entry(currentUsr, $"Deleted completed Task ID: {id}");
                // ───────────────────────────────────────────────────────────
            }
            else
            {
                manage_tasks.update_taskStatus(id);

                // ─── AUTOMATION ADDITION ───────────────────────────────────
                my_log.add_entry(currentUsr, $"Marked Task ID: {id} as DONE");
                // ───────────────────────────────────────────────────────────
            }

            RefreshLogUI();
            autoLoad_task();
        }




        //method to auto load the Tasks of a user
        private void autoLoad_task()
        {


            //clear the ListView first
            view_tasks.Items.Clear();

            //use the object name manage_task
            manage_tasks.load_tasks(view_tasks);

        }



        //send event handler
        
        private void send(object sender, RoutedEventArgs e)
        {
            // Get the question from the design 
            string rawQuestion = question.Text.ToString();

            if (string.IsNullOrWhiteSpace(rawQuestion))
            {
                error_method("ChatBot", "Please enter a question.");
                return;
            }

            // Remove special characters and clean the question
            string questions = RemoveSpecialCharacters(rawQuestion);

            // Show what the user typed 
            error_method(username, rawQuestion);

            // ─── AUTOMATION ADDITION ───────────────────────────────────
            // Automatically log that the user sent a chat message
            string currentUsr = string.IsNullOrWhiteSpace(username) ? "Guest" : username;
            my_log.add_entry(currentUsr, $"Asked: \"{rawQuestion}\"");
            RefreshLogUI();
            // ───────────────────────────────────────────────────────────

            //ai chats and auto_show_interest
            auto_show_interest();
            ai_check(questions);
        }

        //end for the username submit



        //start of ai_chat method
        private void ai_check(string questions)
        {


            // Check if user entered anything meaningful
            if (string.IsNullOrWhiteSpace(questions))
            {
                error_method("ChatBot", "Please enter a valid question.");
                question.Clear();
                return;
            }



            // Check if the question contains only special characters or empty after cleaning
            if (questions.Length == 0 || string.IsNullOrWhiteSpace(questions))
            {
                error_method("ChatBot", "I couldn't understand that.");
                question.Clear();
                return;
            }




            // Variables for processing
            string[] words = questions.ToLower().Split(new char[] { ' ', ',', '.', '?', '!', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            bool found = false;
            string message = string.Empty;
            Random indexer = new Random();
            List<string> per_word = new List<string>();
            List<string> answers_found = new List<string>();





            if (questions.ToLower().StartsWith("add task"))
            {//start of if add task

                //add the task to the listview as part of the chats
                message += "Great your task is added, would you like a reminder?\n and also ";

                //then filter to get task name
                task_name = questions.Replace("add task", "");
                my_log.add_entry(username, "Task added: '" + task_name + "'");


            }//end of if checking task add or add task


            //check if the user want to set a reminder
            if (questions.ToLower().StartsWith("yes, remind me in") || questions.ToLower().StartsWith("yes remind me in"))
            {//start of the reminder if

                //replace the yes, remind me in
                string reminder = questions.Replace("yes, remind me in", "");

                //then get the number inside the reminder variable
                string days_number = Regex.Replace(reminder, @"[^0-9]", "");

                //cast the days_number to an int
                int days = int.Parse(days_number);

                //add the days the user chose to do the task to the current date
                DateTime user_reminder = DateTime.Now.AddDays(days);



                //format the date how it should be
                //Like  06-07-2026 or Jun 15 2026
                string format_date = user_reminder.ToString("MMMM dd yyyy");
                //assign the format date with task_dueDate and status
                task_dueDate = format_date;
                task_status = "pending";

                //call the insert method to store the task
                error_method("ChatBot: ", "good , i will remind you in " + days + " days, on the " + format_date + "\n, to view your task click on view tasks");
                manage_tasks.insert_task(task_name, task_description, task_dueDate, task_status);
                my_log.add_entry(username, "Reminder set for '" + task_name + "' on " + format_date);
                return;

            }//end of the reminder if


            // Process each word
            foreach (string word in words)
            {
                // Skip very short words or ignored words
                if (word.Length < 3 || ignore.Contains(word.ToLower()))
                    continue;

                per_word.Clear();





                //start of interests




                if (word.Contains("interested"))
                {
                    string store_interests = string.Empty;
                    bool found_interest = false;

                    HashSet<string> currentInterests = new HashSet<string>();

                    foreach (string interest in words)
                    {
                        // CLEAN INPUT
                        string clean = interest.ToLower().Trim();
                        clean = Regex.Replace(clean, @"[^a-zA-Z0-9\s]", "");

                        // FILTER NOISE WORDS
                        if (!ignore.Contains(clean) && clean != "interested" && clean != "and" && clean != "in" && clean.Length >= 3)
                        {
                            found_interest = true;
                            currentInterests.Add(clean);
                        }
                    }


                    // prepare interests
                    store_interests = string.Join(", ", currentInterests);

                    if (found_interest && !string.IsNullOrWhiteSpace(store_interests))
                    {
                        string filename = "interested_topic.txt";
                        bool userFound = false;

                        if (File.Exists(filename))
                        {
                            string[] lines = File.ReadAllLines(filename);

                            for (int i = 0; i < lines.Length; i++)
                            {
                                if (lines[i].StartsWith(username))
                                {
                                    userFound = true;

                                    //get all the interests
                                    string existing = lines[i].Replace(username + " interested in:", "").ToLower();

                                    HashSet<string> existingSet = new HashSet<string>(existing.Split(',').Select(x => x.Trim()).Where(x => x != ""));

                                    // remove dumplicates
                                    foreach (string item in currentInterests)
                                    {
                                        existingSet.Add(item);
                                    }

                                    string finalList = string.Join(", ", existingSet);

                                    lines[i] = username + " interested in: " + finalList;
                                    File.WriteAllLines(filename, lines);

                                    message += "great, i added " + store_interests + " to your interests and ";
                                    break;
                                }
                            }
                        }

                        if (!userFound)
                        {
                            File.AppendAllText(
                                filename,
                                username + " interested in: " + store_interests + "\n"
                            );

                            message += "great, i will remember that you are interested in " + store_interests + " and ";
                        }
                    }
                    else
                    {
                        message += "Please specify what you're interested in (e.g., 'I am interested in cybersecurity')";
                    }
                }



                //end of interests




                // Search for matching answers
                bool wordFound = false;
                foreach (string answer in reply)
                {
                    if (answer.ToLower().Contains(word))
                    {
                        wordFound = true;
                        per_word.Add(answer);
                    }
                }

                if (wordFound && per_word.Count > 0)
                {
                    found = true;
                    int indexing = indexer.Next(0, per_word.Count);
                    answers_found.Add(per_word[indexing]);
                }
            }

            // Show responses or error message
            if (found && answers_found.Count > 0)
            {
                // Remove duplicate answers
                answers_found = answers_found.Distinct().ToList();

                foreach (string per_answer in answers_found)
                {
                    message += per_answer + "\n";
                    task_description += per_answer + " ";
                }
                //task_description = message.TrimEnd('\n');
                error_method("ChatBot", message.TrimEnd('\n'));


                chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
            }
            else
            {


                string response = train_ai.CheckAndLearn(questions);


                // It will learn from unknown questions
                string unknownResponse = train_ai.CheckAndLearn(questions);
                //display error message for message not found


                error_method("ChatBot", unknownResponse);
            }

            // Clear the input box
            question.Clear();


        }

        //end of ai_chat method




        //method to remove special characters
        private string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            StringBuilder sanitized = new StringBuilder();

            foreach (char c in input)
            {
                // Keep letters, numbers, spaces, and basic punctuation
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '\'' || c == '-')
                {
                    sanitized.Append(c);
                }
                else
                {
                    // Replace other special characters with space
                    sanitized.Append(' ');
                }
            }

            // Clean up extra spaces and trim
            string result = sanitized.ToString();
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ").Trim();

            return result;
        }


        //end of method to remove special characters





        //method count to show interests randomly
        private void auto_show_interest()
        {
            //check if three times
            if (counting == 3)
            {
                //read the user's interests from file
                string filename = "interested_topic.txt";

                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);

                    //find the user's line
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(username))
                        {
                            //get the interests part
                            int colonIndex = line.IndexOf("interested in:");
                            if (colonIndex >= 0)
                            {
                                string interests = line.Substring(colonIndex + 14).Trim();

                                //show reminder of interests
                                error_method("ChatBot", "Just a reminder, you are interested in " + interests + " and ");
                                ai_check(interests);
                                break;
                            }
                        }
                    }
                }

                //reset counting
                counting = 0;
            }
            else
            {
                //incrementing
                counting += 1;
            }
        }
        //end of count interest method






        // Updated error method with better formatting
        private void error_method(string name, string message)
        {
            // Create a border for chats
            Border messageBorder = new Border
            {
                Margin = new Thickness(0, 2, 0, 2),
                Padding = new Thickness(5, 3, 5, 3),
                CornerRadius = new CornerRadius(5)
            };

            bool isBot = name.ToLower().Contains("chatbot") ||
                     name.ToLower().Contains("chat");

            // BOT STYLE
            if (isBot)
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                messageBorder.BorderBrush = Brushes.DarkRed;
            }
            // USER STYLE
            else
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(35, 35, 35));
                messageBorder.BorderBrush = Brushes.Red;
            }

            // Message text
            TextBlock messageText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(2),
                FontFamily = new FontFamily("Trebuchet MS"),
                FontSize = 14
            };

            // Username colors
            Brush nameColor = isBot ? Brushes.Red : Brushes.White;

            // Message colors
            Brush messageColor = Brushes.White;

            // Username
            messageText.Inlines.Add(new Run
            {
                Text = name + ": ",
                Foreground = nameColor,
                FontWeight = FontWeights.Bold
            });

            // Actual message
            messageText.Inlines.Add(new Run
            {
                Text = message,
                Foreground = messageColor
            });

            // Add text into border
            messageBorder.Child = messageText;

            // Add to chat
            chats.Items.Add(messageBorder);

            // Auto-scroll
            chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }

    }

}//end of namespace
