﻿using System;

namespace Media_Player.Structures
{
    public class Song
    {
        public string Title { get; set; }
        public uint Number { get; set; }
        public TimeSpan Length { get; set; }
        public string [] Genres { get; set; }
        public int TimesPlayed { get; set; }
        public string Url { get; set; }

        //  public enum Rating { get; set; }

        public int Duration => Length.Minutes * 60 + Length.Seconds;

        public Song(string url, string title, string[] genres, uint trackNumber, TimeSpan length)
        {
            Url = url;
            Title = title;
            Genres = genres;
            Number = trackNumber;
            Length = length;
        }

        public Song(string url, string title, uint number, TimeSpan length)
        {
            Url = url;
            TimesPlayed = 0;
            Title = title;
            Number = number;
            Length = length;
        }

        public override string ToString()
        {
            string formatedName;
            var duration = Length.Minutes + ":" + Length.Seconds;
            if(Title.Length>=30)
            {
                formatedName = Title.Substring(0, 27) + "...";
            } else
            {
                formatedName = Title;
            }
            return $"{Number,2}.{formatedName,-30}{duration,30}"; 
        }

        public override bool Equals(object obj)
        {
            Song o = (Song)obj;
            return o != null && Title == o.Title;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
