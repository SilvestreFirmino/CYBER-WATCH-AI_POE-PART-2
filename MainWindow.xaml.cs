using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace CYBER_WATCH_AI_POE_PART_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Creating an instance for the class Array
        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();

        public MainWindow()
        {
            InitializeComponent();

            // Creating an instance for the respond class to load data
            new respond(reply, ignore) { };
        }

        private void send(object sender, RoutedEventArgs e)
        {
            // Get the question from the design user input
            string questions = question.Text.ToString();

            // If statement to check if user entered a question or not
            if (string.IsNullOrWhiteSpace(questions))
            {
                // Call the error method
                error_method();
            }
            else
            {
                // Show the user's question in the chat
                chats.Items.Add("You: " + questions);

                // Temp variables and arrays
                string[] words = questions.Split(' ');
                bool found = false;
                string message = string.Empty;
                Random indexer = new Random();

                ArrayList per_word = new ArrayList();
                ArrayList answers_found = new ArrayList();

                // Iterate per word from the words array
                foreach (String word in words)
                {
                    // Check if the word is allowed or not
                    if (!ignore.Contains(word.ToLower()))
                    {
                        per_word.Clear();
                        bool wordFoundInReply = false; // Local tracking variable for this specific word

                        // Search for the answer of the allowed word
                        foreach (string answer in reply)
                        {
                            if (answer.Contains(word.ToLower()))
                            {
                                found = true;
                                wordFoundInReply = true;
                                per_word.Add(answer); // Store all answers for the word
                            }
                        }

                        // Check if an answer was found for THIS specific word before picking a random index
                        if (wordFoundInReply && per_word.Count > 0)
                        {
                            int indexing = indexer.Next(0, per_word.Count);
                            answers_found.Add(per_word[indexing]);
                        }
                    }
                }

                // Check and show the user the answers
                if (found)
                {
                    // Get all of answers and show to the user
                    foreach (string per_answer in answers_found)
                    {
                        message += per_answer + "\n";
                    }

                    // Add visual separation
                    chats.Items.Add(new Separator());

                    // Beautifully formatted rich-text response matching your UI setup
                    chats.Items.Add(new TextBlock
                    {
                        Inlines = {
                            new Run {
                                Text = "Cyber Watch AI : \n" ,
                                Foreground = Brushes.Green,
                                FontWeight = FontWeights.Bold
                            },
                            new Run {
                                Text = message ,
                                Foreground = Brushes.Yellow
                            }
                        }
                    });

                    chats.Items.Add(new Separator());

                    // Auto scroll to the end of the list view
                    chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
                }
                else
                {
                    // Fallback message if no keywords match anything in your reply array
                    chats.Items.Add(new TextBlock
                    {
                        Inlines = {
                            new Run {
                                Text = "Cyber Watch AI : " ,
                                Foreground = Brushes.Red,
                            },
                            new Run {
                                Text = "I am unsure how to respond to that. Please rephrase your question." ,
                                Foreground = Brushes.White
                            }
                        }
                    });
                }

                // Clear the text box for the next question
                question.Clear();
            }
        }

        // Error method
        private void error_method()
        {
            // Call the chats which is a listview and add formatted text
            chats.Items.Add(
                new TextBlock
                {
                    Inlines = {
                        new Run {
                            Text = "Cyber Watch AI : " ,
                            Foreground = Brushes.Red,
                        },
                        new Run {
                            Text = "Please enter a question !! ",
                            Foreground = Brushes.Red
                        }
                    }
                }
            );

            // Auto scroll to show the error
            chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }

        private void submit_name(object sender, RoutedEventArgs e)
        {
            // Temp variables
            string filename = "user_names.txt";

            // Check if the name exists or not, then auto create
            if (!File.Exists(filename))
            {
                // Auto create the file
                File.AppendAllText(filename, "auto_create\n");
            }

            // Temp variable to store the name
            string name = user_name.Text.ToString();
            bool found = check_name(name);

            if (!found)
            {
                // Store username into the text file
                MessageBox.Show("Welcome " + name + " to Cyber Watch AI !!");
                File.AppendAllText(filename, name + "\n");

                // Hide the name input and show chat grid
                name_grid.Visibility = Visibility.Hidden;
                Chat_Grid.Visibility = Visibility.Visible;
            }
            else
            {
                // Welcome back the user
                MessageBox.Show("Welcome back " + name + " to Cyber Watch AI !!");

                name_grid.Visibility = Visibility.Hidden;
                Chat_Grid.Visibility = Visibility.Visible;
            }
        }

        // Check_name method to check if name exists or not
        private Boolean check_name(string name)
        {
            string filename = "user_names.txt";
            bool name_found = false;

            // One dimension array to read all names from the text file
            string[] names = File.ReadAllLines(filename);

            // For each to look through the array to search for current user name
            foreach (string search_name in names)
            {
                // Check if the user name is found
                if (search_name.ToLower() == name.ToLower())
                {
                    name_found = true;
                }
            }

            // Returning status if the user is found or not
            return name_found;
        }
    }
}

