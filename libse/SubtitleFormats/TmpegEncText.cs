using System;
using System.Collections.Generic;
using System.Text;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class TmpegEncText : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".subtitle"; }
        }

        public override string Name
        {
            get { return "Tmpeg Encoder Text"; }
        }

        public override bool IsTimeBased
        {
            get { return true; }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            var subtitle = new Subtitle();
            LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > _errorCount;
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            //1,1,"00:01:57,269","00:01:59,169",0,"These hills here are full of Apaches."

            StringBuilder temp = new StringBuilder();
            foreach (string l in lines)
                temp.Append(l);
            string all = temp.ToString();
            if (!all.Contains("[ItemData]"))
                return;

            _errorCount = 0;
            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                var arr = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length >= 8 && Utilities.IsInteger(arr[0]) && Utilities.IsInteger(arr[1]))
                {
                    Paragraph p = new Paragraph();
                    try
                    {
                        p.StartTime = GetTimeCode(arr[2] + "," + arr[3]);
                        p.EndTime = GetTimeCode(arr[4] + "," + arr[5]);
                        p.Text = line.Trim().TrimEnd('"');
                        p.Text = p.Text.Substring(p.Text.LastIndexOf('"')).TrimStart('"');
                        p.Text = p.Text.Replace("\\n", Environment.NewLine);
                        subtitle.Paragraphs.Add(p);
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(exception.Message);
                        _errorCount++;
                    }
                }
            }
            subtitle.Renumber();
        }

        private static TimeCode GetTimeCode(string code)
        {
            code = code.Trim().Trim('"');
            var arr = code.Split(new[] { ':', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
            int h = int.Parse(arr[0]);
            int m = int.Parse(arr[1]);
            int s = int.Parse(arr[2]);
            int ms = int.Parse(arr[3]);
            return new TimeCode(h, m, s, ms);
        }

    }
}
