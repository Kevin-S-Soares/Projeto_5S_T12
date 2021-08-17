using System;
using System.Collections.Generic;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        public Dictionary<Dentist, LinkedList<AppointmentList>> Book { get; private set; } = new Dictionary<Dentist, LinkedList<AppointmentList>>();
        public AppointmentBook() { }
        public void Verify(KeyValuePair<Dentist, LinkedList<AppointmentList>> kv, DateTime date, int durationInMinutes, Appointment appointment)
        {
            LinkedList<AppointmentList> ScheduledAppointments = kv.Value;
            LinkedListNode<AppointmentList> node;
            for (node = ScheduledAppointments.First; node.Next != null; node = node.Next)
            {
                if(node.Value.SameDay(date))
                {
                    node.Value.MakeAppointment(date.TimeOfDay, durationInMinutes, appointment);
                    return;
                }
                else if(node.Value.DayBefore(date))
                {
                        LinkedListNode<AppointmentList> newNode = new LinkedListNode<AppointmentList>(new AppointmentList(date, durationInMinutes, appointment));
                        ScheduledAppointments.AddBefore(node, newNode);
                        return;
                }
            }
            node = new LinkedListNode<AppointmentList>(new AppointmentList(date, durationInMinutes, appointment));
            ScheduledAppointments.AddLast(node);
        }

    }
}
