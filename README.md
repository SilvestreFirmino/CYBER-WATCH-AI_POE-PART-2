# CYBER-WATCH-AI_POE-PART-2

# CYBER-WATCH-AI — POE Part 2
Cybersecurity Awareness Chatbot (PROG6221 POE Part 2)

A C# WPF Desktop Application that builds on Part 1 by moving from a console interface to a fully styled graphical chatbot. The application greets users with a voice message, remembers returning users, tracks their cybersecurity interests across sessions, and provides keyword-based responses with emotional sentiment detection.

## Features

Voice Greeting on Launch: Plays an audio welcome (greett.wav) automatically every time the application starts using System.Media.SoundPlayer.

Returning User Recognition: Saves usernames to a local user_names.txt file. Returning users receive a personalised "Welcome back" message while new users receive a fresh welcome.

Keyword-Based AI Responses: The chatbot scans each message for known cybersecurity keywords and returns one or more relevant answers from a pre-loaded response library. Topics covered include cybersecurity, phishing, passwords, firewalls, VPNs, hacked accounts, fraud, and malicious chatbots.

Sentiment Detection: Recognises emotional words such as frustrated, confused, worried, happy, sad, and angry, and responds with empathy before providing an answer.

Interest Tracking: Users can tell the bot what topics they are interested in, for example "I am interested in phishing and passwords". Interests are saved to interested_topic.txt and the bot reminds users of their saved interests every 3 messages.

Styled Dark-Theme Chat UI: Chat messages are displayed as styled bubbles inside a WPF ListView. Bot messages have a dark red border and user messages have a red border, all on a dark background.

Input Sanitisation: Special characters are stripped and whitespace is normalised before processing to prevent unexpected errors.

## Installation & Setup

Prerequisites: You will need Visual Studio 2019 or later with the ".NET desktop development" workload installed, .NET Framework 4.7.2 (included in that workload), and Windows 10 or later.

To get started, clone the repository using the command below, then open the project by double-clicking CYBER-WATCH-AI_POE-PART-2.slnx in Visual Studio.

```bash
git clone https://github.com/SilvestreFirmino/CYBER-WATCH-AI_POE-PART-2.git
```

If Visual Studio prompts you to restore NuGet packages, go to Tools -> NuGet Package Manager -> Restore NuGet Packages, or run dotnet restore in the terminal. Once packages are restored, press F5 to build and run the application.

## Usage

When the application launches, an audio greeting plays automatically. On the home screen, click the Proceed button to continue. You will then be asked to enter your name and click Submit. If you are a new user the bot will welcome you, and if you have used the app before it will greet you by name and welcome you back.

Once in the chat, type any cybersecurity question in the input box and click Send. You can also tell the bot what topics you are interested in, for example "I am interested in phishing and passwords", and it will save those interests and remind you of them every 3 messages.

## Project Structure

Program.cs is the main entry point that sets up the application. App.xaml and App.xaml.cs handle the application startup. MainWindow.xaml defines the visual layout of the main window including the buttons, text boxes, and chat list. MainWindow.xaml.cs contains all the button click logic, AI response processing, and interest tracking. respond.cs stores the pre-written answer library and the list of common words the bot ignores when matching keywords. user_name.cs handles saving usernames to a text file and recognising returning users. voice_greeting.cs plays greett.wav on startup using SoundPlayer. The greett.wav file is the audio greeting, and eyelogo.png, eyelogo2.png, and pattern.png are the image assets used in the UI.

## Technologies Used

The application is built using C# as the core programming language and WPF (Windows Presentation Foundation) as the desktop UI framework, running on .NET Framework 4.7.2. It uses System.Media.SoundPlayer for audio playback and System.IO for reading and writing user data files. GitHub Actions is used as a CI/CD pipeline to automatically build the project on every push.

## CI/CD Pipeline

This project uses GitHub Actions to automatically build the application whenever code is pushed or a pull request is opened on the main or master branch. The workflow file is located at .github/workflows/main.yml. It checks out the code, sets up MSBuild and NuGet, restores packages, builds the project in Release mode, and uploads the compiled output as a downloadable artifact kept for 30 days. Build results can be viewed under the Actions tab on GitHub.

## Related

Part 1 of this project (console version): https://github.com/st10453122/prog6221_poe_part1
