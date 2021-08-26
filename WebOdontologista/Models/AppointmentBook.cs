using System;
using System.Collections.Generic;
using WebOdontologista.Models.Exceptions;

namespace WebOdontologista.Models
{
    public class AppointmentBook
    {
        public Dictionary<int, LinkedList<AppointmentList>> Book { get; private set; } = new Dictionary<int, LinkedList<AppointmentList>>();
        public AppointmentBook() { }
        public void AddDentist(int id)
        {
            Book.Add(id, new LinkedList<AppointmentList>());
        }
        public void AddAppointment(Appointment appointment)
        {
            LinkedListNode<AppointmentList> node;
            for (node = Book[appointment.DentistId].First; node != null; node = node.Next)
            {
                if (node.Value.SameDay(appointment.Date))
                {
                    node.Value.MakeAppointment(appointment);
                    return;
                }
                else if (node.Value.DayBefore(appointment.Date))
                {
                    LinkedListNode<AppointmentList> newNode = new LinkedListNode<AppointmentList>(new AppointmentList(appointment));
                    Book[appointment.DentistId].AddBefore(node, newNode);
                    return;
                }
            }
            node = new LinkedListNode<AppointmentList>(new AppointmentList(appointment));
            Book[appointment.DentistId].AddLast(node);
            return;
        }
        /*
        public Dictionary<TimeSpan, string> FindAvailableTime(int id, DateTime date)
        {
            LinkedListNode<AppointmentList> node;
            for (node = Book[id].First; node != null; node = node.Next)
            {
                if (node.Value.SameDay(date))
                {
                    return node.Value.AvailableTime();
                }
            }
            Dictionary<TimeSpan, string> result = new Dictionary<TimeSpan, string>();
            for (int i = 0; i < 12; i++)
            {
                TimeSpan workTime = new TimeSpan(9, 15 * i, 0);
                result.Add(workTime, null);
            }
            for (int i = 16; i < 36; i++)
            {
                TimeSpan workTime = new TimeSpan(9, 15 * i, 0);
                result.Add(workTime, null);
            }
            return result;

        }
        */

    }
}
