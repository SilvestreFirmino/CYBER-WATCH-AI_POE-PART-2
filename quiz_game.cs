using System;
using System.Collections.Generic;

namespace CYBER_WATCH_AI_POE_PART_2
{
    public class quiz_game
    {
        public void autoLoadQuiz(ref List<quiz_question> questions)
        {// questions
            questions = new List<quiz_question>()
            {
                new quiz_question
                {
                    Text = "What is phishing?",
                    correctAnswer = "Tricking to steal data",
                    wrongAnswer = new List<string>{ "Data backup", "Safe login", "Password tips" }
                },
                new quiz_question
                {
                    Text = "What is password safety?",
                    correctAnswer = "Unique & strong passwords",
                    wrongAnswer = new List<string>{ "Share with friends", "Short passwords", "Common words" }
                },
                new quiz_question
                {
                    Text = "What is safe browsing?",
                    correctAnswer = "Use trusted sites",
                    wrongAnswer = new List<string>{ "Click all links", "Visit unknown pages", "Enable pop-ups" }
                },
                new quiz_question
                {
                    Text = "Phishing email sign?",
                    correctAnswer = "Urgent or strange links",
                    wrongAnswer = new List<string>{ "Good grammar", "Known sender", "Unsubscribe button" }
                },
                new quiz_question
                {
                    Text = "Strong password?",
                    correctAnswer = "P@55w0rD!#987",
                    wrongAnswer = new List<string>{ "Password123", "qwerty2024", "123456789" }
                },
                new quiz_question
                {
                    Text = "When to update password?",
                    correctAnswer = "Every 3–6 months",
                    wrongAnswer = new List<string>{ "Yearly", "Never", "Only if hacked" }
                },
                new quiz_question
                {
                    Text = "Risk of reused passwords?",
                    correctAnswer = "One hack = all at risk",
                    wrongAnswer = new List<string>{ "Typing delay", "Site error", "No effect" }
                },
                new quiz_question
                {
                    Text = "Unsafe site sign?",
                    correctAnswer = "Typos & pop-ups",
                    wrongAnswer = new List<string>{ "HTTPS shown", "Fast load", "No ads" }
                },
                new quiz_question
                {
                    Text = "Safe on public Wi-Fi?",
                    correctAnswer = "Use VPN / avoid private info",
                    wrongAnswer = new List<string>{ "Bank online", "File share", "Shop online" }
                },
                new quiz_question
                {
                    Text = "Flagged site action?",
                    correctAnswer = "Leave right away",
                    wrongAnswer = new List<string>{ "Ignore it", "Refresh page", "Click through" }
                }
            };
        }
    }
}