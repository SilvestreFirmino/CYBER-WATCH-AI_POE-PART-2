# CYBER-WATCH-AI_POE-PART-3
A C# WPF Desktop Application that builds on Parts 1 and 2 by turning the chatbot into a full cybersecurity assistant. Alongside the original conversational chatbot, the application now manages tasks and reminders through a SQL Server database, runs a cybersecurity knowledge quiz, learns from unanswered questions through a small ML.NET model, and organises everything behind a proper side-menu navigation system instead of a single scrolling window.

Features

Voice Greeting on Launch: Plays an audio welcome (greett.wav) automatically every time the application starts using System.Media.SoundPlayer.

Returning User Recognition: Saves usernames to a local user_names.txt file. Returning users receive a personalised "Welcome back" message while new users receive a fresh welcome.

Cyber Menu Navigation: A side panel lets the user move between five areas of the application at any time, namely Chat, Task Reminders, Activity Log, Cyber Quiz, and Exit, with each section appearing in its own page rather than everything being crammed into one chat window.

Task Assistant with Database Storage: Users can add a cybersecurity-related task by typing something like "add task enable two-factor authentication" and can attach a reminder by replying "yes remind me in 3 days". Every task is written to a demo_tasks table inside a dedicated pro_tasks SQL Server database, which the application creates automatically the first time it runs. The Task Reminders page lists every saved task, and double-clicking an item either marks it as done or removes it from the database, depending on its current status.

Cybersecurity Quiz Mini-Game: The Cyber Quiz page presents multiple-choice questions covering topics such as phishing, password safety, and social engineering. Each question shows four shuffled answer options as buttons, awards points for correct answers, and keeps a running score that resets once every question has been answered.

NLP-Based Response Engine: Questions that don't match a pre-written answer are passed through a small ML.NET text-classification model trained on the chatbot's own knowledge base, which tries to find the closest matching response. If the model can't find a confident match either, the chatbot falls back to simple keyword matching, and if that also fails the original question is logged to unknown_questions.txt so it can be reviewed and added to the knowledge base later.

Activity Log: The Activity Log page keeps a record of notable actions the chatbot has taken during the session, such as tasks being added or reminders being set, so the user can look back over what the bot has done for them.

Interest Tracking: Users can tell the bot what topics they are interested in, for example "I am interested in phishing and passwords". Interests are saved to interested_topic.txt and the bot reminds users of their saved interests every three messages.

Keyword-Based AI Responses: For everyday questions, the chatbot scans each message for known cybersecurity keywords and returns one or more relevant answers from a pre-loaded response library covering topics such as phishing, passwords, firewalls, VPNs, hacked accounts, fraud, and malicious chatbots.

Styled Dark-Theme Interface: Every page, including the chat window, task list, activity log, and quiz, shares the same dark background with red and dark-red accents. Chat messages appear as styled bubbles inside a WPF ListView, with bot messages bordered in dark red and user messages bordered in red.

Input Sanitisation: Special characters are stripped and whitespace is normalised before processing to prevent unexpected errors.

Installation & Setup

Prerequisites: You will need Visual Studio 2019 or later with the ".NET desktop development" workload installed, .NET Framework 4.7.2 (included in that workload), SQL Server Express LocalDB (installed automatically with Visual Studio's SQL Server data tools), and Windows 10 or later.

To get started, clone the repository using the command below, then open the project by double-clicking the solution file in Visual Studio.

bashgit clone https://github.com/SilvestreFirmino/CYBER-WATCH-AI_POE-PART-2.git

If Visual Studio prompts you to restore NuGet packages, go to Tools -> NuGet Package Manager -> Restore NuGet Packages, or run dotnet restore in the terminal. This will also pull in Microsoft.ML, which the NLP engine depends on. Once packages are restored, press F5 to build and run the application. On first launch the application will automatically create the pro_tasks database and its demo_tasks table if they don't already exist.

Usage

When the application launches, an audio greeting plays automatically. On the home screen, click the Proceed button to continue. You will then be asked to enter your name and click Submit. If you are a new user the bot will welcome you, and if you have used the app before it will greet you by name and welcome you back.

Once inside the application, the Cyber Menu on the right lets you move between the different sections. On the Chat page you can type any cybersecurity question and click Send, tell the bot what topics interest you, or add a task with a phrase like "add task review account privacy settings" followed by "yes remind me in 7 days" once asked about a reminder. The Task Reminders page lists everything you've added, and double-clicking a task either completes or deletes it depending on its status. The Activity Log page shows a record of what the bot has done during the session. The Cyber Quiz page lets you answer multiple-choice cybersecurity questions and keeps track of your score as you go.

Project Structure

Program.cs is the main entry point that sets up the application. App.xaml and App.xaml.cs handle the application startup. MainWindow.xaml defines the visual layout of every page, including the chat window, task list, activity log, and quiz screen, along with the Cyber Menu navigation buttons. MainWindow.xaml.cs contains all the button click logic, AI response processing, task and reminder handling, quiz logic, and interest tracking. respond.cs stores the pre-written answer library and the list of common words the bot ignores when matching keywords. nlp_processor.cs trains and runs the ML.NET model used to interpret unmatched questions, with a keyword-based fallback and a logging step for anything it still can't answer. tasks.cs manages the SQL Server connection, automatically creates the pro_tasks database and demo_tasks table, and handles inserting, loading, completing, and deleting tasks. quiz_question.cs and quiz_game.cs hold the quiz questions and the logic for loading and scoring them. user_name.cs handles saving usernames to a text file and recognising returning users. voice_greeting.cs plays greett.wav on startup using SoundPlayer. The greett.wav file is the audio greeting, and eyelogo.png, eyelogo2.png, and pattern.png are the image assets used in the UI.

Technologies Used

The application is built using C# as the core programming language and WPF (Windows Presentation Foundation) as the desktop UI framework, running on .NET Framework 4.7.2. It uses System.Media.SoundPlayer for audio playback, System.IO for reading and writing user data files, System.Data.SqlClient together with SQL Server Express LocalDB for storing tasks and reminders, and Microsoft.ML (ML.NET) for the natural language response engine. GitHub Actions is used as a CI/CD pipeline to automatically build the project on every push.

CI/CD Pipeline

This project uses GitHub Actions to automatically build the application whenever code is pushed or a pull request is opened on the main or master branch. The workflow file is located at .github/workflows/main.yml. It checks out the code, sets up MSBuild and NuGet, restores packages, builds the project in Release mode, and uploads the compiled output as a downloadable artifact kept for 30 days. Build results can be viewed under the Actions tab on GitHub.

Related

Part 1 of this project (console version): https://github.com/st10453122/prog6221_poe_part1
