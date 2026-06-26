using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CYBER_WATCH_AI_POE_PART_2
{
    public class nlp_processor
    {
        private MLContext mlContext = new MLContext();
        private ITransformer model;
        private PredictionEngine<TrainingData, AnswerPrediction> predictionEngine;

        private List<string> reply = new List<string>();
        private List<string> ignore = new List<string>() { "the", "a", "an", "is", "are", "to", "do" };

        public class TrainingData
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }

        public class AnswerPrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedAnswer { get; set; }
            public float[] Score { get; set; }
        }

        public void Train()
        {
            // Seed base knowledge data answers if empty
            if (reply.Count == 0)
            {
                reply.Add("Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.");
                reply.Add("Phishing is a method of trying to gather personal information using deceptive emails and websites.");
                reply.Add("Always use strong passwords containing letters, symbols, and unique numbers.");
            }

            var trainingData = new List<TrainingData>();
            for (int i = 0; i < reply.Count; i++)
            {
                string clean = CleanText(reply[i], ignore);
                trainingData.Add(new TrainingData { Question = clean, Answer = reply[i] });
            }

            var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Answer")
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Question"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

            model = pipeline.Fit(dataView);
            mlContext.Model.Save(model, dataView.Schema, "nlp_model.zip");
            predictionEngine = mlContext.Model.CreatePredictionEngine<TrainingData, AnswerPrediction>(model);
        }

        public void LoadModel()
        {
            if (File.Exists("nlp_model.zip"))
            {
                DataViewSchema schema;
                model = mlContext.Model.Load("nlp_model.zip", out schema);
                predictionEngine = mlContext.Model.CreatePredictionEngine<TrainingData, AnswerPrediction>(model);
            }
            else
            {
                Train(); // Auto fallback train if schema file does not exist locally
            }
        }

        private string CleanText(string text, List<string> ignoreWords)
        {
            string clean = text.ToLower();
            foreach (string word in ignoreWords)
                clean = clean.Replace(" " + word + " ", " ");

            clean = new string(clean.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
            return Regex.Replace(clean, @"\s+", " ").Trim();
        }

        public string CheckAndLearn(string userInput, string username = "user")
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Please say something, I am here to help!";

            string loweredInput = userInput.ToLower();

            // SYSTEM COM-ROUTING HOOKS: Direct commands mapped dynamically
            if (loweredInput.Contains("quiz") || loweredInput.Contains("test me"))
            {
                return "[SYSTEM_ACTION:START_QUIZ]";
            }
            if (loweredInput.Contains("add task") || loweredInput.Contains("remind me"))
            {
                try
                {
                    tasks taskManager = new tasks();
                    taskManager.insert_task("AI Assigned Task", userInput, DateTime.Now.AddDays(2).ToShortDateString(), "Pending");
                    return "Database Hook: I have processed that statement and registered it into your 'demo_tasks' database collection!";
                }
                catch (Exception dbEx)
                {
                    return "I understood you wanted to record a task, but I couldn't write it out to the DB: " + dbEx.Message;
                }
            }

            bool found = false;
            List<string> found_answers = new List<string>();

            // 1. ML Engine Processing Strategy
            if (predictionEngine != null)
            {
                var prediction = predictionEngine.Predict(new TrainingData { Question = userInput });
                if (!string.IsNullOrEmpty(prediction.PredictedAnswer))
                {
                    found_answers.Add(prediction.PredictedAnswer);
                    found = true;
                }
            }

            // 2. Keyword fallback approach if ML model scores are blank
            if (!found)
            {
                string[] find_words = loweredInput.Split(new char[] { ' ', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in find_words)
                {
                    if (!ignore.Contains(word) && word.Length > 3)
                    {
                        foreach (string answer in reply)
                        {
                            if (answer.ToLower().Contains(word))
                            {
                                if (!found_answers.Contains(answer))
                                    found_answers.Add(answer);
                                found = true;
                            }
                        }
                    }
                }
            }

            // 3. Unrecognized input automated logging pipeline (Learning Loop)
            if (!found || found_answers.Count == 0)
            {
                string unknownFile = "unknown_questions.txt";
                File.AppendAllText(unknownFile, $"{userInput}|{username}|{DateTime.Now}\n");
                return "I didn't quite catch that. Could you rephrase your question, or ask about cybersecurity topics?";
            }

            return string.Join("\n and ", found_answers);
        }
    }
}