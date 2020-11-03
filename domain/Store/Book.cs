﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Store
{
    public class Book
    {
        public int Id { get; }
        public string Isbn { get; }
        public string Author { get; }//Не нормализованные данные, надо по id
        public string Title { get; }
        public Book(int id, string title, string isbn, string author)
        {
            Id = id;
            Isbn = isbn;
            Author = author;
            Title = title;
        }
        
        internal static bool IsIsbn(string s)
        {
            if (s == null)
                return false;
            s = s.Replace("-", "")
                .Replace(" ", "")
                .ToUpper();
            return Regex.IsMatch(s, @"^ISBN\d{10}(\d{3})?$");
        }
    }
}
