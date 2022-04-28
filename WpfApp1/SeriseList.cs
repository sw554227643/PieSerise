using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class SeriseList<T> : List<T>
    {
        public delegate void ListChangedEventHandler<T>(object sender);

        private int _count;

        public String Action { get; set; }




        public int MCount
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    DoListChangedEvent();
                }
            }
        }

        public event ListChangedEventHandler<T> ListChanged;

        private void DoListChangedEvent()
        {
            if (this.ListChanged != null)
                this.ListChanged(this);
        }

        public new void Add(T t)
        {
            base.Add(t);
            this.Action = "Add";
            MCount++;
        }

        public new void Remove(T t)
        {
            base.Remove(t);
            this.Action = "Remove";
            MCount--;
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            this.Action = "RemoveRange";
            MCount--;
        }
    }
}
