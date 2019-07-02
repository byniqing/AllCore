using All.Domain.BaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace All.Domain.Model
{
    public class Users : AggregateRoot
    {
        //public int id { get => id; set => id = value; }

        public int id { get; set; }
        public int age { get; set; }
        public string address { get; set; }
    }
}
