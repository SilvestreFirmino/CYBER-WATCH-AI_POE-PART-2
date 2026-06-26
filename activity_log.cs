using System;
using System.Collections.Generic;
using System.IO;

namespace CYBER_WATCH_AI_POE_PART_2
{
    //this class keeps a record of everything the chatbot has done
    //it keeps a short list in memory (so we can quickly show a summary in chat)
    //and also writes every entry to a text file so there is a permanent record
    public class activity_log
    {//start of class

        //holds the action descriptions for the CURRENT session
        //this is what gets shown when the user asks "what have you done?"
        private List<string> entries;

        //the file where every action gets permanently saved, with a timestamp
        private string filename = "activity_log.txt";


        //constructor - runs once when the activity_log object is first created.
        public activity_log()
        {
            entries = new List<string>();
        }


        //call this any time the bot does something worth recording
        //example: my_log.add_entry(username, "Task added: 'Enable two-factor authentication'");
        public void add_entry(string username, string action)
        {//start

            //keep it in memory for quick display later
            entries.Add(action);

            //also save it permanently to the text file, with a timestamp and the username
            string timestamp = DateTime.Now.ToString("MMM dd yyyy HH:mm");
            string file_line = timestamp + " | " + username + " | " + action;

            File.AppendAllText(filename, file_line + "\n");

        }//end


        //builds a numbered summary of the most recent actions, ready to drop straight into a chat bubble
        //count is how many of the most recent entries to include, e.g. 5
        public string get_recent_entries(int count)
        {//start

            if (entries.Count == 0)
            {
                return "I haven't taken any actions yet this session.";
            }

            //work out where to start counting from so we only grab the LAST few entries
            int start_index = entries.Count - count;
            if (start_index < 0)
            {
                start_index = 0;
            }

            string summary = "Here's a summary of recent actions:\n";
            int number = 1;

            for (int i = start_index; i < entries.Count; i++)
            {
                summary += number + ". " + entries[i] + "\n";
                number++;
            }

            //trim the trailing newline so it doesn't leave an empty line in the chat bubble
            return summary.TrimEnd('\n');

        }//end


        //returns every entry recorded this session, useful if you want to display the full history rather than just recent ones
        public List<string> get_all_entries()
        {
            return entries;
        }

    }//end of class
}