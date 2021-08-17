using System;
using System.Collections.Generic;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        public LinkedList<UniqueDate> ScheduledAppointments { get; private set; } = new LinkedList<UniqueDate>();
        public AppointmentBook() { }
        public void Verify(DateTime date, int duration, string appointment)
        {
            LinkedListNode<UniqueDate> node;
            for (node = ScheduledAppointments.First; node.Next != null; node = node.Next)
            {
                if(node.Value.SameDay(date))
                {
                    node.Value.MakeAppointment(date.TimeOfDay, duration, appointment);
                    return;
                }
                else if(node.Value.DayBefore(date))
                {
                        LinkedListNode<UniqueDate> newNode = new LinkedListNode<UniqueDate>(new UniqueDate(date, duration, appointment));
                        ScheduledAppointments.AddBefore(node, newNode);
                        return;
                }
            }
            node = new LinkedListNode<UniqueDate>(new UniqueDate(date, duration, appointment));
            ScheduledAppointments.AddLast(node);
        }

    }
}
