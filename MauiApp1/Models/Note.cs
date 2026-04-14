using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    internal class Note
    {
        public int ID { get; set;}
    public string FileName { get; set;} = string.Empty;
    public string Text { get; set;} = string.Empty;
        public DateTime Date { get; set;}
    }
}
